using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Preference;
using AndroidX.ViewPager2.Widget;
using com.companyname.navigationgraph9net9.Adapters;
using Google.Android.Material.Tabs;
using System;


namespace com.companyname.navigationgraph9net9.Fragments
{
    public class LeaderboardPagerFragment : Fragment, IOnApplyWindowInsetsListener
    {
        private ViewPager2? leaderboardViewPager;
        private LeaderboardViewPagerStateAdapter? leaderboardViewPagerStateAdapter;
        private TabLayout? leaderboardTabLayout;
        private OnPageChangeCallback? onPageChangeCallback;  // Have to register and unregister it. -- Not used
        private bool displayPageIndicator;
        private bool useViewPagerAnimations;
        
        private ConstraintLayout? leaderboardConstraintLayout;
        private int initialPaddingBottom;

        public LeaderboardPagerFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View? view = inflater.Inflate(Resource.Layout.fragment_leaderboard_viewpager, container, false);
            leaderboardConstraintLayout = view!.FindViewById<ConstraintLayout>(Resource.Id.leader_board_constraint);
            leaderboardViewPager = view!.FindViewById<ViewPager2>(Resource.Id.holder_viewpager);
            leaderboardTabLayout = view!.FindViewById<TabLayout>(Resource.Id.tablayout1);

            ViewCompat.SetOnApplyWindowInsetsListener(leaderboardConstraintLayout!, this);
            initialPaddingBottom = leaderboardConstraintLayout!.PaddingBottom;

            return view;
        }
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            displayPageIndicator = sharedPreferences!.GetBoolean("displayPageIndicator", false);
            useViewPagerAnimations = sharedPreferences!.GetBoolean("useViewPagerAnimations", false);

            
            // FragmentStateAdapter - which calls CreateFragment for the number of fragments - note the extra param ViewLifecycleOwner.Lifecycle 
            // Refer to https://stackoverflow.com/questions/61779776/leak-canary-detects-memory-leaks-for-tablayout-with-viewpager2/62184494#62184494 
            // This is using the third of the three FragmentStateAdapter public constructors.
            // Note - it is also an abstract class, so we can add out own params. e.g. passing the total number of fragments that the ViewPager2 will hold.
            leaderboardViewPagerStateAdapter = new LeaderboardViewPagerStateAdapter(ChildFragmentManager, ViewLifecycleOwner.Lifecycle, 3);

            // ViewPager2
            leaderboardViewPager!.OffscreenPageLimit =  ViewPager2.OffscreenPageLimitDefault;
            leaderboardViewPager.Adapter = leaderboardViewPagerStateAdapter;
            
            if (useViewPagerAnimations)
                leaderboardViewPager.SetPageTransformer(new ZoomOutPageTransformer());      // Needs some more controls in the xml of each fragment to see the effect 

            // Create an OnPageChangeCallback and register it with the dashboardViewPager. We don't need it here - but keep as an example.
            onPageChangeCallback = new OnPageChangeCallback(Activity!);
            leaderboardViewPager.RegisterOnPageChangeCallback(onPageChangeCallback);  // Unregister it in OnDestroy

            TabLayoutMediator tabMediator = new(leaderboardTabLayout!, leaderboardViewPager, new TabConfigurationStrategy());
            tabMediator.Attach();

            if (displayPageIndicator)
            {
                // Leave this - just another way of doing it, but the easier way is just to use the Dimension in dp which automatically takes care of Density
                // No this makes the bar smaller than standard and therefore better places the two gauges.
                ViewGroup.LayoutParams layoutParams = leaderboardTabLayout!.LayoutParameters!;
                layoutParams.Height = (int)(16 * Resources!.DisplayMetrics!.Density);
                leaderboardTabLayout.LayoutParameters = layoutParams;
                TogglePageIndicatorClickable(false);
            }
            else
                leaderboardTabLayout!.Visibility = ViewStates.Gone;
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
        {
            // Need to keep the leaderboardTabLayout above the NavigationBar
            if (v is ConstraintLayout)
            {
                AndroidX.Core.Graphics.Insets? navigationBarsInsets = insets!.GetInsets(WindowInsetsCompat.Type.NavigationBars());
                v.SetPadding(v.PaddingLeft, v.PaddingTop, v.PaddingRight, navigationBarsInsets!.Bottom +initialPaddingBottom);
            }
            return insets!;
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            leaderboardViewPager!.UnregisterOnPageChangeCallback(onPageChangeCallback!);
            base.OnDestroy();
        }
        #endregion

        #region TogglePageIndicatorClickable
        private void TogglePageIndicatorClickable(bool enable = false)
        {
            // TODO: We would need a listener if we want to turn this feature on or off.
            // Ideally normally each indicator would be clickable. When the the statusBar and NavigationBar appear make them non clickable
            // These comments really only apply when using a fully immersive fragment.
            foreach (View view in leaderboardTabLayout!.Touchables!)
                view.Clickable = enable;
        }
        #endregion

        #region  TabConfigurationStrategy
        private class TabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            // No need to do anything here - empty method as the tablayout itself is supplying the tab selector - see the xml.
            public void OnConfigureTab(TabLayout.Tab tab, int position)
            {

            }
        }
        #endregion

        #region ZoomPageTransformer
        public class ZoomOutPageTransformer : Java.Lang.Object, ViewPager2.IPageTransformer
        {
            private readonly static float MinimumScale = 0.85f;
            private readonly static float MinimumAlpha = 0.5f;

            public void TransformPage(View view, float position)
            {
                int pageWidth = view.Width;
                int pageHeight = view.Height;

                // page filled position = 0
                // page when drawn just off to right position = 1
                // page when drawn just off to left position = -1

                if (position < -1)
                {   // [-Infinity,-1)
                    // This page is way off-screen to the left.
                    view.Alpha = 0f;
                }
                else if (position <= 1)
                {
                    // e.g. -1,1
                    // Modify the default slide transition to shrink the page as well
                    float scaleFactor = Math.Max(MinimumScale, 1 - Math.Abs(position));
                    float vertMargin = pageHeight * (1 - scaleFactor) / 2;
                    float horzMargin = pageWidth * (1 - scaleFactor) / 2;
                    if (position < 0)
                        view.TranslationX = (horzMargin - vertMargin / 2);
                    else
                        view.TranslationX = (-horzMargin + vertMargin / 2);

                    // Scale the page down (between MinimumScale and 1)
                    view.ScaleX = scaleFactor;
                    view.ScaleY = scaleFactor;

                    // Fade the page relative to its size.
                    view.Alpha = MinimumAlpha + (scaleFactor - MinimumScale) / (1 - MinimumScale) * (1 - MinimumAlpha);
                }
                else
                    // This page is way off-screen to the right.
                    view.Alpha = 0f;
            }
        }
        #endregion

        #region OnPageChangeCallback - Not using
        private class OnPageChangeCallback : ViewPager2.OnPageChangeCallback
        {
            // We don't really need this. But would for instance if we wanted to display a Toast
            private readonly Context context;

            public OnPageChangeCallback(Context context) => this.context = context;

            public override void OnPageSelected(int position)
            {

            }
        }
        #endregion

    }
}