using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class SampleFragment1 : Fragment
    {
        public SampleFragment1() { }

        #region NewInstance
        internal static SampleFragment1 NewInstance()
        {
            SampleFragment1 fragment = new();
            return fragment;
        }
        #endregion

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_sample1, container, false);
            
        }
    }
}