using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Preference;
using com.companyname.navigationgraph9net9.Dialogs;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class PurchaseFragment : Fragment
    {
        private bool showSubscriptionExplanationDialog;
        
        public PurchaseFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View? view = inflater.Inflate(Resource.Layout.fragment_purchase, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_purchase);
            textView!.Text = "This is purchase fragment";

            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            showSubscriptionExplanationDialog = sharedPreferences!.GetBoolean("showSubscriptionExplanationDialog", true);

            return view;
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            if (showSubscriptionExplanationDialog)
                ShowSubscriptionExplanationDialog();

        }
        #endregion

        #region ShowSubscriptionExplanationDialog
        internal void ShowSubscriptionExplanationDialog()
        {
            // Make sure the the MaterialSwitch is checked. We don't want this to pop up again after the transaction completes. Optional fourth parameter.
            string tag = "SubscriptionExplanationDialogFragment";
            string preferenceName = "showSubscriptionExplanationDialog";

            FragmentManager fm = Activity!.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    HelperExplanationDialogFragment.NewInstance(GetString(Resource.String.subscription_explanation_title),
                                                                GetString(Resource.String.subscription_explanation_text_short), preferenceName).Show(fm, tag);  
            }
        }
        #endregion
    }
}