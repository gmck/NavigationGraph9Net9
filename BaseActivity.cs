using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.Graphics;
using AndroidX.Core.View;
using AndroidX.Preference;
using Google.Android.Material.Color;
using System;

namespace com.companyname.navigationgraph9net9
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected ISharedPreferences? sharedPreferences;
        private string? requestedColorTheme;
        private bool useDynamicColors;

        #region OnCreate
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            EdgeToEdge.Enable(this);

            base.OnCreate(savedInstanceState);

            if (OperatingSystem.IsAndroidVersionAtLeast(29))
                Window!.NavigationBarContrastEnforced = false;

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

            // colorThemeValue defaults to GreenBmw
            requestedColorTheme = sharedPreferences!.GetString("colorThemeValue", "3");
            useDynamicColors = sharedPreferences.GetBoolean("use_dynamic_colors", false);

            if (OperatingSystem.IsAndroidVersionAtLeast(31) & useDynamicColors)  // DynamicColors was introduced in API 31 Android 12 
            {
                SetAppTheme(requestedColorTheme!);
                DynamicColors.ApplyToActivityIfAvailable(this);
            }
            else
                SetAppTheme(requestedColorTheme!);
        }
        #endregion

        #region SetAppTheme
        private void SetAppTheme(string requestedColorTheme)
        {
            
            if (requestedColorTheme == "1")
                SetTheme(Resource.Style.Theme_NavigationGraph_RedBmw);
            else if (requestedColorTheme == "2")
                SetTheme(Resource.Style.Theme_NavigationGraph_BlueAudi);
            else if (requestedColorTheme == "3")
                SetTheme(Resource.Style.Theme_NavigationGraph_GreenBmw);

            SetSystemBarsAppearance();
        }
        #endregion

        #region SetSystemBarsAppearance()
        private void SetSystemBarsAppearance()
        {
            WindowInsetsControllerCompat windowInsetsController = new(Window!, Window!.DecorView);

            if (!IsNightModeActive())
            {
                windowInsetsController.AppearanceLightStatusBars = true;
                windowInsetsController.AppearanceLightNavigationBars = true;
            }
            else
            {
                windowInsetsController.AppearanceLightStatusBars = false;
                windowInsetsController.AppearanceLightNavigationBars = false;
            }
        }
        #endregion

        #region IsNightModeActive
        private bool IsNightModeActive()
        {
            UiMode currentNightMode = Resources!.Configuration!.UiMode & UiMode.NightMask;
            return currentNightMode == UiMode.NightYes;
        }
        #endregion
    }
}
