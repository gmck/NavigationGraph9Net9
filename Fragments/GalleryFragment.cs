using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class GalleryFragment : Fragment
    {
        // This fragment doesn't require a menu
        
        public GalleryFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_gallery, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_gallery);
            textView!.Text = "This is gallery fragment";
            return view;
        }
        #endregion

    }
}