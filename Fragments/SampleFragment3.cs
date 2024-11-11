using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class SampleFragment3 : Fragment
    {
        public SampleFragment3() { }

        #region NewInstance
        internal static SampleFragment3 NewInstance()
        {
            SampleFragment3 fragment = new SampleFragment3();
            return fragment;
        }
        #endregion

        #region OnCreateView
        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_sample3, container, false);
        }
        #endregion
    }
}