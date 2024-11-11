using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class RaceResultFragment : ImmersiveFragment
    {
        public RaceResultFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_raceresult, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_race_result);
            textView!.Text = "Immersive RaceResult fragment";
            return view;
        }
        #endregion

    }
}