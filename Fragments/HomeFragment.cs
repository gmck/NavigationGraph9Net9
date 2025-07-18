using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Preference;
using com.companyname.navigationgraph9net9.Dialogs;
using System;
using System.Linq;

namespace com.companyname.navigationgraph9net9.Fragments
{
    // OnCreateOptionsMenu, SetHasOptionsMenu (or when using C# HasOptionsMenu) and OnOptionsItemSelected have been deprecated with the release of Xamarin.AndroidX.Navigation.Fragment 2.5.1
    // New with this release are the new IMenuProvider and IMenuHost and replacement methods OnCreateMenu and OnMenuItemSelected
    // Therefore this requires the removal of OnCreateOptionsMenu and OnOptionsItemSelected from the MainActivity in your MainActivity if your fragments require different menus.
    // If retained, then every fragment will have the same menu.
    // You can no longer remove a menu from a fragment which doesn't require a menu by setting HasOptionsMenu = true and then doing a menu.Clear in OnCreateOptionsMenu.
    // In other words if you do have OnCreateOptionsMenu and OnOptionsItemSelected then you should move those menuitems to
    // the StartDestinationFragment = e.g. as in this example the HomeFragment.
    // AddMenuProvider is based on LifeCycle therefore it is only applicable while this fragment is visible. The other ctor requires you to use RemoveMenuProvider.
    // Any fragment that doesn't require a menu then doesn't implement the IMenuProvider

    // OnPrepareMenu and onMenuClosed are missing from IMenuProvider interface
    // See Menu Deprecations when upgrading AndroidX.Navigation.Fragment to 2.5.1 #611 Sept 2nd 2022 - Fixed May 25, 2023
    public class HomeFragment : Fragment, IMenuProvider
    {
        private const int BLUETOOTH_PERMISSIONS_REQUEST_CODE = 9999;
        private readonly string logTag = "Nav-HomeFragment";
        private bool animateFragments;
        private bool enableSubscriptionInfoMenuItem;

        public HomeFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View? view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_home);
            textView!.Text = "This is home fragment";
            
            return view;
        }
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Check for any changed values
            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            animateFragments = sharedPreferences!.GetBoolean("use_animations", false);
            enableSubscriptionInfoMenuItem = sharedPreferences.GetBoolean("showSubscriptionInfo", false);

            // New with release of Xamarin.AndroidX.Navigation.Fragment 2.5.1 or more accurately AndroidX.Core.View
            // see https://medium.com/tech-takeaways/how-to-migrate-the-deprecated-oncreateoptionsmenu-b59635d9fe10
            //IMenuHost menuHost = RequireActivity();
            //menuHost.AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);

            // A more concise version of the above 
            (RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);
        }
        #endregion

        #region OnCreateMenu
        public void OnCreateMenu(IMenu? menu, MenuInflater? menuInflater)
        {
            menuInflater!.Inflate(Resource.Menu.menu_home_fragment, menu);
        }
        #endregion

        #region OnPrepareMenu
        public void OnPrepareMenu(IMenu? menu)
        {
            // OnPrepareMenu as well as OnMenuClosed (which is not used here ) was missing from IMenuProvider. Fixed May 25, 2023 
            IMenuItem? menuItemSettings = menu!.FindItem(Resource.Id.action_subscription_info);
            menuItemSettings!.SetEnabled(enableSubscriptionInfoMenuItem);
        }
        #endregion

        #region OnMenuItemSelected
        public bool OnMenuItemSelected(IMenuItem? menuItem)
        {
            if (!animateFragments)
                AnimationResource.Fader3();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .SetPopUpTo(Resource.Id.home_fragment, false)     // Inclusive false, saveState true.
                    .Build();

            switch (menuItem!.ItemId)
            {
                case Resource.Id.action_settings:

                    //Requirement for Android 12+ before Navigatating to the SettingsFragment from the HomeFragment, we need the following Permissions Manifest.Permission.BluetoothConnect and Manifest.Permission.BluetoothScan. Therefore we need an something like activityResultLauncher = RegisterForActivityResult(new ActivityResultContracts.RequestMultiplePermissions())
                    //we need something like the followowing

                    //if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                    //{
                    //    if (BluetoothPermissionsGranted())
                    //        Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment, null, navOptions);
                    //    else
                    //        bluetoothPermissionLauncher!.Launch(new string[] { Android.Manifest.Permission.BluetoothConnect, Android.Manifest.Permission.BluetoothScan });
                    //}
                    //else
                    //    Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment, null, navOptions);


                    // This works for now but it is not the correct way to do it. The correct way is to use the activityResultLauncher = RegisterForActivityResult(new ActivityResultContracts.RequestMultiplePermissions())
                    // and then use the bluetoothPermissionLauncher.Launch(new string[] { Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan });

                    // When the app starts for the very first time, the user wont have already given the Bluetooth permissions, therefore permissionsGranted will be false and we will have to request permissions from here.
                    // When the request Permissions dialog appears, the user answer is received (collected) in the MainActivity's OnRequestPermissionsResult method. From there if the user has granted permissions
                    // the code there will navigate directly to the SettingsFragment. In all other instances permissionsGranted will be true and therefore we will navigate to the SettingsFragment from here. 

                    if (OperatingSystem.IsAndroidVersionAtLeast(31))
                    {
                        if (BluetoothPermissionsGranted())
                            Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment, null, navOptions);
                        else
                            ActivityCompat.RequestPermissions(Activity!, [Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan], BLUETOOTH_PERMISSIONS_REQUEST_CODE);
                    }
                    else
                        Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment, null, navOptions);

                    return true;


                case Resource.Id.action_logfiles:
                    Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.maintenance_file_selection_fragment, null, navOptions);
                    return true;

                case Resource.Id.action_subscription_info:
                    ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
                    return true;

                case Resource.Id.action_about_info:
                    ShowAboutDialog();
                    return true;
                
                default:
                    return false;
            }
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(logTag, logTag + " OnDestroy");
        }
        #endregion

        #region BluetoothPermissionsGranted
        private bool BluetoothPermissionsGranted()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                string[] permissions = [Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan];
                return permissions.All(permission => AndroidX.Core.Content.ContextCompat.CheckSelfPermission(Context!, permission) == Permission.Granted);
            }
            return false;
        }
        #endregion

        #region ShowSubscriptionInfoDialog - Moved from the MainActivity
        private void ShowSubscriptionInfoDialog(string title, string explanation)
        {
            string tag = "SubscriptionInfoDialogFragment";
            FragmentManager fm = Activity!.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
            }
        }
        #endregion

        #region ShowAboutDialog
        internal void ShowAboutDialog()
        {
            string tag = "AboutDialogFragment";
            AndroidX.Fragment.App.FragmentManager fm = Activity!.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    AboutDialogFragment.NewInstance().Show(fm, tag);
            }
        }
        #endregion

        #region BackStackCount - not used, keep as a reference
        public int BackStackCount() 
        {
            // if backStackCount is 0, which it is here in the HomeFragment then we are at the HomeFragment and navHostFragment.ChildFragmentManager.Fragments.Count is 1.
            // PopBackStack() will attempt to go back one step in your backstack, and will not do anything if there is no backstack entry. Therefore no point in calling it if backStackCount is 0.
            // Also popBackStack(null, FragmentManager.PopBackStackInclusive) will clear the entire backstack, and will not do anything if there is no backstack entry.
            NavHostFragment? navHostFragment = Activity!.SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
            int backStackCount = navHostFragment!.ChildFragmentManager.BackStackEntryCount;
            return backStackCount;
        }
        #endregion
    }


}