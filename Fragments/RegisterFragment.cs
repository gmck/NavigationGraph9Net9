using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    // The comments here also apply equally to the LeaderboardFragment 
    // By default because the LeaderboardFragment and the RegisterFragment are not top level fragments, they will default to showing an up button or left arrow on the toolbar plus the title of the fragment
    // If you don't want the up button, it can be removed in MainActivity's OnDestinationChange - see example there.
    // This also means that the additional code in OnSupportNavigationUp can be removed. The code in OnSupportNavigationUp is there for when you do want for whatever reason want to retain the Up buttom.
    // All it does is to ensure that a fragment containing an Up button can still be directed away from the standard action of an up button, which is to always return when closing to the StartDestination Fragment.


    // OnCreateOptionsMenu, SetHasOptionsMenu (i.e. C# HasOptionsMenu) and OnOptionsItemSelected have been deprecated with the release of Xamarin.AndroidX.Navigation.Fragment 2.5.1
    // New with this release are the new IMenuProvider and IMenuHost and replacement methods OnCreateMenu and OnMenuItemSelected
    // Therefore this requires the removal of OnCreateOptionsMenu and OnOptionsItemSelected from the MainActivity in your MainActivity if your fragments require different menus.
    // If retained, then every fragment will have the same menu.
    // You can no longer remove a menu from a fragment which doesn't require a menu by setting HasOptionsMenu = true and then doing a menu.Clear in OnCreateOptionsMenu.
    // In other words if you do have OnCreateOptionsMenu and OnOptionsItemSelected (until next version of AppCompatActivity) then you should move those menuitems to
    // the StartDestinationFragment = e.g. as in this example the HomeFragment.
    // AddMenuProvider is based on LifeCycle therefore it is only applicable while this fragment is visible. 
    // Any fragment that doesn't require a menu then doesn't implement the IMenuProvider

    public class RegisterFragment : Fragment, IMenuProvider
    {
        public RegisterFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_register, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_register);
            textView!.Text = "This is the Register fragment";

            // New with release of Xamarin.AndroidX.Navigation.Fragment 2.5.1
            //IMenuHost menuHost = RequireActivity();
            //menuHost.AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed);

            // More concise than the above 
            (RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);
            return view;
        }
        #endregion

        #region OnCreateMenu
        public void OnCreateMenu(IMenu? menu, MenuInflater? menuInflater)
        {
            menuInflater!.Inflate(Resource.Menu.menu_register_fragment, menu);
        }
        #endregion

        #region OnMenuItemSelected
        public bool OnMenuItemSelected(IMenuItem? menuItem)
        {
            switch (menuItem!.ItemId)
            {
                case Resource.Id.action_register_fragment:

                    // Various attempts and then found the correct way - see below 04/03/2022
                    // Either of these work - here I prefer to use the pragma, both the following lines look like overkill to me.
                    //Toast? toast = Toast.MakeText(Activity!, Resources.GetString(Resource.String.toast_message), ToastLength.Long);toast!.Show();
                    //Toast? toast = Toast.MakeText(Activity ?? throw new InvalidOperationException("Activity is null!"),Resources.GetString(Resource.String.toast_message),ToastLength.Long) ?? throw new InvalidOperationException("Toast.MakeText returned null!");toast.Show();

                    Toast.MakeText(Activity, Resources.GetString(Resource.String.toast_message), ToastLength.Long)?.Show();
                    return true;

                // Must have this default condition - otherwise we lose the ability to open the NavigationMenu in the MainActivity via the hamburger icon 
                default:
                    return false;
                    //return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(Activity!, Resource.Id.nav_host));
            }
        }
        #endregion
    }
}