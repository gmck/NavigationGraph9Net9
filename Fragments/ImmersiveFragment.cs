using Android.Util;
using Android.Views;
using AndroidX.Core.View;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class ImmersiveFragment : Fragment 
    {
        private readonly string logTag = "navigationGraph7";

        public ImmersiveFragment() { }

        #region OnStart
        public override void OnStart()
        {
            base.OnStart();
            
            Log.Debug(logTag, "ImmersiveFragment - OnStart");
            if (Activity is MainActivity mainActivity)
            {
                mainActivity.SupportActionBar!.Hide();
                mainActivity.DisableDrawerLayout();             // Disable the navigationDrawer of the MainActivity. We don't want a user to be able to swipe it into view while viewing any of the gauges
            }
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            HideSystemUi();
            Log.Debug(logTag, "ImmersiveFragment - OnResume - called HideSystemUI");
        }
        #endregion

        #region OnPause        
        public override void OnPause()
        {
            base.OnPause();
            ShowSystemUi();
        }
        #endregion

        #region HideSystemUi
        public void HideSystemUi()
        {
            // 17/01/2022 Added this reference as explanation of why we needed this code from Android 11 and on...
            // Don't use android:fitsSystemWindows="true" anywhere.
            // Refer to https://stackoverflow.com/questions/57293449/go-edge-to-edge-on-android-correctly-with-windowinsets/70714398#70714398 goto the bottom for this solution

            // Had to add the following line to ensure the immersiveFragment went full screen on launch, without it it left a black rectangle where the statusbar had been. 
            // Note it would display correctly after one rotation on return from the rotation was ok.
            Activity!.Window!.AddFlags(WindowManagerFlags.LayoutNoLimits);
            //WindowCompat.SetDecorFitsSystemWindows(Activity.Window, false);   // Don't need, because it is our default at startup - see BaseActivity
            WindowInsetsControllerCompat windowInsetsControllerCompat = WindowCompat.GetInsetsController(Activity.Window, Activity.Window.DecorView);
            windowInsetsControllerCompat.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            windowInsetsControllerCompat.Hide(WindowInsetsCompat.Type.StatusBars() | WindowInsetsCompat.Type.NavigationBars());

        }
        #endregion

        #region ShowSystemUi
        private void ShowSystemUi()
        {
            Activity!.Window!.ClearFlags(WindowManagerFlags.LayoutNoLimits);// We had to add, so we need to clear it.
            //WindowCompat.SetDecorFitsSystemWindows(Activity.Window, true);    // Don't need because it is false at start up      
            WindowInsetsControllerCompat windowInsetsControllerCompat = WindowCompat.GetInsetsController(Activity.Window, Activity.Window.DecorView);
            windowInsetsControllerCompat.Show(WindowInsetsCompat.Type.StatusBars() | WindowInsetsCompat.Type.NavigationBars());

            if (Activity is MainActivity mainActivity)
            {
                mainActivity.SupportActionBar!.Show();
                mainActivity.EnableDrawerLayout();
            }
        }
        #endregion
    }
}


