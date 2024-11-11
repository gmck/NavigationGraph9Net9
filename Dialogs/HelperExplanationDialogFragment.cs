using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.MaterialSwitch;

namespace com.companyname.navigationgraph9net9.Dialogs
{
    /*
        Using a BottomSheetDialogFragment instead of using MaterialAlertDialogBuilder to build a dialog for use within an AppCompatDialogFragment to show a series of helper screens for first time users of the app. 
        
        These dialogs give the new user information about what is available at a particular point in the app, explaining features that are available, that might not be obvious to a new user. Especially to those users 
        whom are unlikely to ever read your carefully written 100+ page user manual. 
    
        Once the user is familiar with the features then these helper screens would become tiresome and therefore there is no need to keep displaying that information. A MaterialSwitch on the dialog that when turned on 
        will save that choice so that the helper screen will no longer be displayed, each time the app reaches that same point. 
    
        Recommmended. This also requires a setting in Settings that can reverse the saved preference values, so that the user can return to the original state if they need to recheck the features available and have prematurely turned
        the help screen off. 

        BottomSheetDialogFragments have a distinct advantage over AppCompatDialogFragments in that the dialog built is displayed at full screen width and therefore more information can be displayed without the need of 
        making the layout scrollable as you would with a conventional dialog. Conventional Modal dialogs are abrupt and intrusive by their nature. They are after all modal, they have to be addressed by the user and therefor dismissed 
        before the app can continue. You can easily make a scrollable model dialog, but if there is a lot of content the user may miss the fact that it is scrollable and so not even be able to see the OK button at the bottom of the 
        dialog when the dialog first appears.
    
        BottomSheetDialog are also modal, but by their design are far less abrupt and therefore less intrusive. They gently slide up from the bottom of the screen with a subtle animation, which is more likely to 
        draw the attention of the user in a more favourable way. They subtely indicate that they are model by overlaying the screen with a transparent scrim. Full screen width so more information at first glance without scrolling. 
        MateralSwitch immediately visible.
    
        Behaviour notes for BottomSheetDialogFragment
        If screen is rotated - events called - only OnDismiss called - therefore preference can be saved if MaterialSwitch is checked
        If back key or dialog dragged down or tapped outside the dialog - events called - OnCancel then OnDismiss. Therefore preference can be saved if MaterialSwitch checked with any of those actions.
        We don't really need OnCancel for this situation, since we don't have a separate requirement for processing onCancel. If the MaterialSwitch is not checked (switch on) then these HelperExplanations aren't going away.
    
        Note Cancelable must be set true, otherwise both these events are ignored.

        Note we have two xml layouts generic_dialog_switch and generic_dialog_switch_scrollable. The scrollable version is to accomodate messages that contain large amounts of text, that are not totally visible in landscape mode 
        when fully extended.
        Wrapping the constraintLayout inside a NestedScrollView allows the text to scroll so that the MaterialSwitch becomes visible. Note that the NestedScrollView requires android:fillViewport="true" and the ConstraintLayout's 
        android:layout_height="match_parent" changed to be android:layout_height="wrap_content".
        
        If you want to include an OK button and then to keep it totally modal you would need to make Cancelable = false. Then act on the button.Click event calling OnDismiss directly
        e.g. view.FindViewById<Button>(Resource.Id.button_ok).Click += (sender, e) => { OnDismiss(); };
        
        We also don't need to inherit the interfaces IDialogInterfaceOnDismissListener, IDialogInterfaceOnCancelListener as Android has already built them into the BottomSheetDialogFragment. 
        We therefore just need to override OnDismiss and OnCancel to use them.

        
    */


    /*
        Notes: Re Rounded corners becoming flat corners when fully extended

        It seems to be an expected behavour.

        Check this comment in PR: #437

        Bad news: Our design team is strongly opinionated that rounded corners indicate scrollable content while flat corners indicate that there is no additional content. As such, they do no want us to add this change with fitToContents.

        Also in the design doc there is indication to use flat corners when the bottomsheet is expanded..

        See https://github.com/material-components/material-components-android/issues/1278

        and another interesting article
        https://medium.com/halcyon-mobile/implementing-googles-refreshed-modal-bottom-sheet-4e76cb5de65b

        and another
        https://bryanherbst.com/2020/05/04/bottom-sheet-corners/
     */

    public class HelperExplanationDialogFragment : BottomSheetDialogFragment, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly string logTag = "GLM - HelperExplantionDialogFragment";
        private MaterialSwitch? dontShowSwitch;
        private TextView? textViewExplantion;
        private View? view;
        
        public HelperExplanationDialogFragment() { }

        #region NewInstance
        public static HelperExplanationDialogFragment NewInstance(string title, string explanation, string preferenceName, bool switchChecked = false)
        {
            Bundle arguments = new Bundle();
            arguments.PutString("Title", title);
            arguments.PutString("Explanation", explanation);
            arguments.PutString("PreferenceName", preferenceName);
            arguments.PutBoolean("SwitchChecked", switchChecked);

            HelperExplanationDialogFragment fragment = new()
            {
                Cancelable = true,
                Arguments = arguments
            };
            return fragment;
        }
        #endregion

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.generic_dialog_switch_scrollable, container, false);

            TextView? textViewHeader = view!.FindViewById<TextView>(Resource.Id.textView_header);
            textViewExplantion = view!.FindViewById<TextView>(Resource.Id.textView_explanation);
            dontShowSwitch = view!.FindViewById<MaterialSwitch>(Resource.Id.switch_show_again);

            textViewHeader!.Text = Arguments!.GetString("Title");
            textViewExplantion!.Text = Arguments.GetString("Explanation");
            dontShowSwitch!.Checked = Arguments.GetBoolean("SwitchChecked");

            return view;
        }
        #endregion

        #region OnStart
        public override void OnStart()
        {
            base.OnStart();
            
            BottomSheetBehavior? bottomSheetBehavior = BottomSheetBehavior.From((View)view!.Parent!); //OK
            //BottomSheetBehavior? bottomSheetBehavior = BottomSheetBehavior.From(view!.Parent is View);// Crashed at runtime

            Configuration configuration = Activity!.Resources!.Configuration!;
            if (configuration.Orientation == Android.Content.Res.Orientation.Landscape && configuration.ScreenWidthDp > 450)
            {
                view!.ViewTreeObserver!.AddOnGlobalLayoutListener(this);
                bottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded; // optional
            }
        }
        #endregion

        #region OnGlobalLayout
        public void OnGlobalLayout()
        {
            BottomSheetDialog? bottomSheetDialog = Dialog as BottomSheetDialog;

            // This is required if you are displaying a bottomSheetDialog on a "letterboxed fragment" with a camera notch device in Landscape.Orientation as without it, the dialog scrim will expand to full screen width, which doesn't look good 
            // To see the effect just comment out OnStart in the class declaration

            // Could also consider increasing the width of the dialog 
            int percentageIncrease = (textViewExplantion!.MeasuredWidth * 20/100);
            bottomSheetDialog!.Window!.SetLayout(textViewExplantion.MeasuredWidth /* + percentageIncrease*/, ViewGroup.LayoutParams.WrapContent);
            view!.ViewTreeObserver!.RemoveOnGlobalLayoutListener(this);
            bottomSheetDialog.SetContentView(view);
        }
        #endregion

        #region OnDismiss
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);

            if (dontShowSwitch!.Checked)
            {
                Log.Debug(logTag, "OnDismiss - dontShowSwitch is checked Preference will be written");

                // Working code before nullable
                //ISharedPreferencesEditor editor = PreferenceManager.GetDefaultSharedPreferences(Activity).Edit();
                //editor.PutBoolean(Arguments.GetString("PreferenceName"), false);
                //editor.Commit();

                // Working nullable code
                ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
                ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
                editor!.PutBoolean(Arguments!.GetString("PreferenceName"), false);
                editor.Commit();

                

            }
        }
        #endregion

        #region OnCancel
        public override void OnCancel(IDialogInterface dialog)
        {
            Log.Debug(logTag, "OnCancel Preference can't be written if dontShowSwitch is unchecked - OnDismiss will still be called");
            base.OnCancel(dialog);
        }
        #endregion
    }
}


