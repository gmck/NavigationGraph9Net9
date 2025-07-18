using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using com.companyname.navigationgraph9net9.Dialogs;
using System.Collections.Generic;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class MaintenanceFileSelectionFragment : Fragment, IMenuProvider
    {
        internal List<FileItem>? diagnosticFileList= null;

        internal RecyclerView? recyclerViewFileMaintenceSelection;
        internal RecyclerView.LayoutManager? layoutManager;
        internal TextView? textViewFileType;

        internal Uri? uriSelectedFileName = null;
        internal ViewAction viewAction = ViewAction.None;
        internal FileType fileType;
        internal ProgressBar? progressbar;
        internal bool showFileMaintenanceExplanationDialog;
        internal string? folderUri;

        public MaintenanceFileSelectionFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_maintenance_file_selection, container, false);
            return view!;
        }   
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View? view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view!, savedInstanceState);

            if (savedInstanceState != null)
                fileType = savedInstanceState.GetString("file_type") == "Diagnostic" ? FileType.Diagnostic : FileType.Data;

            textViewFileType = view!.FindViewById<TextView>(Resource.Id.textViewFileType);
            if (fileType == FileType.Diagnostic)
                textViewFileType!.Text = GetString(Resource.String.diagnostic_files);
            else
                textViewFileType!.Text = GetString(Resource.String.data_files);

            progressbar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressbar!.Visibility = ViewStates.Gone;

            // RecyclerView
            recyclerViewFileMaintenceSelection = View!.FindViewById<RecyclerView>(Resource.Id.file_selection_recyclerview);

            // What is now required required (since deprecation of old menu methods) for adding a menu to a fragment 
            (RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);

            // Remove the back button - Fragments that have a menu should not have a back button - See alternative method in OnDestinationChanged in the MainActivity 
            ((MainActivity)Activity!).SupportActionBar!.SetDisplayHomeAsUpEnabled(false);

            // Or
            //if (Activity is MainActivity mainActivity)
            //    mainActivity.SupportActionBar!.SetDisplayHomeAsUpEnabled(false);

            // Could also use the OnDestinationChangedListener in the MainActivity.

        }
        #endregion

        #region OnCreateMenu
        public void OnCreateMenu(IMenu? menu, MenuInflater? menuInflater)
        {
            menuInflater!.Inflate(Resource.Menu.menu_file_maintenance, menu);
        }
        #endregion

        #region OnMenuItemSelected
        public bool OnMenuItemSelected(IMenuItem? menuItem)
        {
            switch (menuItem!.ItemId)
            {
                case Resource.Id.action_show_diagnostic_files:
                    // Change this message
                    Toast.MakeText(Activity, Resources.GetString(Resource.String.toast_message), ToastLength.Long)?.Show();
                    return true;

                case Resource.Id.action_get_permission:
                    Toast.MakeText(Activity, Resources.GetString(Resource.String.permission_for_files), ToastLength.Long)?.Show();
                    return true;

                // Must have this default condition - otherwise we lose the ability to open the NavigationMenu in the MainActivity via the hamburger icon 
                default:
                    return false; 
                    //return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(Activity!, Resource.Id.nav_host));

            }
        }
        #endregion

        #region OnPrepareMenu
        public void OnPrepareMenu(IMenu? menu)
        {
            // OnPrepareMenu as well as OnMenuClosed (which is not used here ) was missing from IMenuProvider. Fixed May 25, 2023 

            // Added 14/02/2023 - Needs to be re-read again.
            // Whatever folder name is created the folder can't be deleted within Files, even if we reset it via Settings.
            // The only way to permanently remove a folder is with a file explorer app like X-plore, which we dont recommend for users.
            // If we reset the folder as above without the use of X-plore, the old folder name will appear. It can be hidden by reselecting Documents (the root} and a new folder created.

            // Note - Even though there is no code here in this example. To invalide the menu we need to call (RequireActivity() as IMenuHost).InvalidateMenu();

            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            folderUri = sharedPreferences!.GetString("FolderUri", string.Empty);

            // Disable/enable menu items as needed.
            IMenuItem? menuItemShowDiagnostic = menu!.FindItem(Resource.Id.action_show_diagnostic_files);
            IMenuItem? menuItemShowData = menu.FindItem(Resource.Id.action_show_data_files);
            IMenuItem? menuItemDelete = menu.FindItem(Resource.Id.action_delete);
            IMenuItem? menuItemViewFile = menu.FindItem(Resource.Id.action_view_file);
            IMenuItem? menuItemSendFile = menu.FindItem(Resource.Id.action_email_file);
            IMenuItem? menuItemCopyFile = menu.FindItem(Resource.Id.action_copy_file);
            IMenuItem? menuItemAskPermission = menu.FindItem(Resource.Id.action_get_permission);

            if (diagnosticFileList != null)
            {
                if (fileType == FileType.Diagnostic)
                {
                    fileType = FileType.Diagnostic;
                    menuItemShowDiagnostic!.SetChecked(true);
                    menuItemShowData!.SetChecked(false);
                }
                else if (fileType == FileType.Data)
                {
                    fileType = FileType.Data;
                    menuItemShowDiagnostic!.SetChecked(false);
                    menuItemShowData!.SetChecked(true);
                }

                if (viewAction == ViewAction.None)
                {
                    menuItemDelete!.SetEnabled(true);
                    menuItemViewFile!.SetEnabled(true);
                    menuItemSendFile!.SetEnabled(true);
                    menuItemCopyFile!.SetEnabled(true);
                }
                else if (viewAction == ViewAction.DeleteFile)
                {
                    menuItemDelete!.SetEnabled(true);
                    menuItemViewFile!.SetEnabled(false);
                    menuItemSendFile!.SetEnabled(false);
                    menuItemCopyFile!.SetEnabled(false);
                }
                else if (viewAction == ViewAction.ViewFile || viewAction == ViewAction.SendFile || viewAction == ViewAction.CopyFile) // the last two don't really matter ViewAction.ViewFile does it, so we could simplify this
                {
                    menuItemDelete!.SetEnabled(false);
                    menuItemViewFile!.SetEnabled(true);
                    menuItemSendFile!.SetEnabled(true);
                    menuItemCopyFile!.SetEnabled(true);
                }
            }
            else
            {
                menuItemShowDiagnostic!.SetEnabled(false);
                menuItemShowData!.SetEnabled(false);
                menuItemDelete!.SetEnabled(false);
                menuItemViewFile!.SetEnabled(false);
                menuItemSendFile!.SetEnabled(false);
                menuItemCopyFile!.SetEnabled(false);
            }

            // This is not correct. If we delete the only file available then it shows Copy selected file enabled as the only available option, which would have been
            // true before we deleted. Needs to be reworked.
            //if (diagnosticFileList != null & string.IsNullOrEmpty(folderUri))
            //{
            //    menuItemAskPermission!.SetEnabled(true);
            //    menuItemCopyFile!.SetEnabled(false);
            //}
            //else
            //{
            //    menuItemAskPermission!.SetEnabled(false);
            //    menuItemCopyFile!.SetEnabled(true);
            //}

            // A quick fix just for this example for demo purposes, since we don't have any data in this example in NavigationGraph7Net8
            menuItemAskPermission!.SetEnabled(true);
            menuItemCopyFile!.SetEnabled(false);
        }
        #endregion

        #region OnStart
        public override void OnStart()
        {
            base.OnStart();

            // Fixed size recyclerView
            recyclerViewFileMaintenceSelection!.HasFixedSize = true;

            // Plug in the linear layout manager:
            layoutManager = new LinearLayoutManager(this.Activity);
            recyclerViewFileMaintenceSelection.SetLayoutManager(layoutManager);

            // Add a ListDivider
            //recyclerViewFileMaintenceSelection.AddItemDecoration(new SimpleItemDecoration(this.Activity));

            // Create the adapter and two event listeners for ItemClick and ItemLongClick
            //if (fileMaintenanceItemsAdapter == null)
            //{
            //    fileMaintenanceItemsAdapter = new FileMaintenanceItemsAdapter(Activity);
            //    fileMaintenanceItemsAdapter.ItemClick += FileMaintenanceItemsAdapter_ItemClick;
            //    fileMaintenanceItemsAdapter.ItemLongClick += FileMaintenanceItemsAdapter_ItemLongClick;
            //}

            // Set the Adapter and ItemAnimator
            //recyclerViewFileMaintenceSelection.SetAdapter(fileMaintenanceItemsAdapter);
            //recyclerViewFileMaintenceSelection.SetItemAnimator(null);

            // Only read from the folder once
            //if (diagnosticFileList == null)
            //  CreateFileList();
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            // 29/04/2021 Had to move from OnViewCreated
            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            showFileMaintenanceExplanationDialog = sharedPreferences!.GetBoolean("showFileMaintenanceExplanationDialog", true);

            // Clear the adapter and then rebuild it from the global list
            //fileMaintenanceItemsAdapter.Clear();

            //if (diagnosticFileList != null)
            //{
            //    for (int i = 0; i < diagnosticFileList.Count; i++)
            //        fileMaintenanceItemsAdapter.AddItem(diagnosticFileList[i]);
            //}

            //  We only want to display the ExplanationDialog if there are files to view
            //if (diagnosticFileList != null && diagnosticFileList.Count > 0)

            if (showFileMaintenanceExplanationDialog)
                ShowFileMaintenanceExplanationDialog();
        }
        #endregion

        #region OnSaveInstanceState
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("file_type", fileType.ToString());
        }
        #endregion

        #region ShowFileMaintenanceExplanationDialog
        private void ShowFileMaintenanceExplanationDialog()
        {
            // Handle rotation otherwise we will have more than one copy to Dismiss
            string tag = "FileMaintenanceExplanationDialogFragment";
            string preferenceName = "showFileMaintenanceExplanationDialog";

            FragmentManager fm = Activity!.SupportFragmentManager;

            if (fm != null && !fm.IsDestroyed)
            {
                Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    HelperExplanationDialogFragment.NewInstance(GetString(Resource.String.file_maintenance_explanation_title),
                                                            GetString(Resource.String.file_maintenance_explanation), preferenceName).Show(fm, tag);
            }
        }
        #endregion

        #region ViewAction enums
        public enum ViewAction
        {
            None,
            DeleteFile,
            ViewFile,
            SendFile,
            CopyFile,
            AskPermission
        }
        #endregion

        #region FileType
        public enum FileType
        {
            Diagnostic,
            Data
        }
        #endregion
    }
}
