using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class SlideshowFragment : Fragment
    {
        public SlideshowFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            
            View? view = inflater.Inflate(Resource.Layout.fragment_slideshow, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_slideshow);
            textView!.Text = "This is Slideshow fragment";
            return view;
        }
        #endregion

    }
}