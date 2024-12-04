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
            base.OnCreate(savedInstanceState);

            // Sets whether the decor view should fit root-level content views for WindowInsetsCompat.
            // I have tested without this test (i.e. EdgeToEdge only) and it was ok on a SamsungS20, but I'm not sure about older APIs.
            // The only other device I've got that is lower than Android 10 is SamsungS8 running Pie Android 9. I'll leave as is but in may not be necessay 

            //if (OperatingSystem.IsAndroidVersionAtLeast(35))
            //    EdgeToEdge.Enable(this);
            //else
            //    WindowCompat.SetDecorFitsSystemWindows(Window!, false);

            EdgeToEdge.Enable(this); // Probably ok for devices less than 10 - therfore could probably comment out the above lines
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
                if (OperatingSystem.IsAndroidVersionAtLeast(35))
                {
                    windowInsetsController.AppearanceLightStatusBars = true;
                    windowInsetsController.AppearanceLightNavigationBars = true;
                }
                else
                {
                    // Making it work the same way for non Android 15 devices 
                    TypedValue typedValue = new();
                    Theme!.ResolveAttribute(Resource.Attribute.colorSurface, typedValue, true);
                    int color = ContextCompat.GetColor(this, typedValue.ResourceId);
                    
                    int transparentColor = new Color(ColorUtils.SetAlphaComponent(color, 00));
                    Window!.SetStatusBarColor(new Color(transparentColor));
                    Window!.SetNavigationBarColor(new Color(transparentColor));

                    windowInsetsController.AppearanceLightStatusBars = true;
                    windowInsetsController.AppearanceLightNavigationBars = true;
                }
            }
            else
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(35))
                {
                    windowInsetsController.AppearanceLightStatusBars = false;
                    windowInsetsController.AppearanceLightNavigationBars = false;
                }
                else
                {
                    // Making it work the same way for non Android 15 devices 
                    TypedValue typedValue = new();
                    Theme!.ResolveAttribute(Resource.Attribute.colorSurface, typedValue, true);
                    int color = ContextCompat.GetColor(this, typedValue.ResourceId);

                    // Leave the following here incase we ever want to go back to a coloured statusBar for devices lower than API35
                    //0 - 255 e.g. 204-> 80 % transparent - What we used previously when using colorSecondary. Will work even if the alpha component of the colour is already set e.g.to opaque FF.See chart below

                    int transparentColor = new Color(ColorUtils.SetAlphaComponent(color, 00));
                    Window!.SetStatusBarColor(new Color(transparentColor));
                    Window!.SetNavigationBarColor(new Color(transparentColor));

                    windowInsetsController.AppearanceLightStatusBars = false;
                    windowInsetsController.AppearanceLightNavigationBars = false;
                }
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
#region Notes - Table for translucent colors
//100% — FF - 255
//95%  — F2 - 242
//90%  — E6 - 230
//85%  — D9 - 217
//80%  — CC - 204
//75%  — BF - 191
//70%  — B3 - 179
//65%  — A6 - 166
//60%  — 99 - 153
//55%  — 8C - 140
//50%  — 80 - 128
//45%  — 73 - 115
//40%  — 66 - 102
//35%  — 59 - 89
//30%  — 4D - 77
//25%  — 40 - 64
//20%  — 33 - 51
//15%  — 26 - 38
//10%  — 1A - 26
//5%   — 0D - 13
//0%   — 00 - 0
#endregion

#region Commented older code
#region OnCreate
//protected override void OnCreate(Bundle? savedInstanceState)
//{
//    base.OnCreate(savedInstanceState);

//    EdgeToEdge.Enable(this);

//    // Sets whether the decor view should fit root-level content views for WindowInsetsCompat.
//    //if (OperatingSystem.IsAndroidVersionAtLeast(35))
//    //EdgeToEdge.Enable(this);
//    //else
//    //WindowCompat.SetDecorFitsSystemWindows(Window!, false);

//    sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

//    // colorThemeValue defaults to RedBmw
//    requestedColorTheme = sharedPreferences!.GetString("colorThemeValue", "1");
//    useDynamicColors = sharedPreferences.GetBoolean("use_dynamic_colors", false);
//    useTransparentStatusBar = sharedPreferences.GetBoolean("use_transparent_statusbar", false);

//    //if (OperatingSystem.IsAndroidVersionAtLeast(31) & useDynamicColors)
//    //{
//    //    SetAppTheme(requestedColorTheme!);
//    //    DynamicColors.ApplyToActivityIfAvailable(this);
//    //    SetStatusBarAppearance();
//    //}
//    //else
//    //    SetAppTheme(requestedColorTheme!);

//    SetAppTheme(requestedColorTheme!);
//}
#endregion

#region SetStatusBarAppearance()
//private void SetStatusBarAppearance()
//    {

//        WindowInsetsControllerCompat windowInsetsController = new(Window!, Window!.DecorView);

//        if (!IsNightModeActive())
//        {
//            //if (OperatingSystem.IsAndroidVersionAtLeast(35))
//            //{
//            windowInsetsController.AppearanceLightStatusBars = true;
//            windowInsetsController.AppearanceLightNavigationBars = true;
//            //}
//            //else
//            //{
//            //    //TypedValue typedValue = new();
//            //    //Theme!.ResolveAttribute(Resource.Attribute.colorSecondary, typedValue, true);
//            //    //int color = ContextCompat.GetColor(this, typedValue.ResourceId);
//            //    //// 0-255 e.g. 204 -> 80% transparent - will work even if the alpha component of the colour is already set e.g. to opaque FF. See chart below
//            //    //Window!.SetStatusBarColor(new Color(useTransparentStatusBar ? ColorUtils.SetAlphaComponent(color, 204) : color));
//            //    ////Window!.SetNavigationBarColor(new Color(Color.Transparent));
//            //}
//        }
//        else
//        {
//            //if (OperatingSystem.IsAndroidVersionAtLeast(35))
//            //{
//            windowInsetsController.AppearanceLightStatusBars = false;
//            windowInsetsController.AppearanceLightNavigationBars = false;
//            //}
//            //else
//            //    Window!.SetStatusBarColor(new Color(Color.Black));   // Don't like the desaturated colour for statusbar with a Dark Theme 
//        }
//    }
//}
#endregion

#region OnCreate
//protected override void OnCreate(Bundle? savedInstanceState)
//{
//    base.OnCreate(savedInstanceState);

//    if (OperatingSystem.IsAndroidVersionAtLeast(35))
//        EdgeToEdge.Enable(this);
//    else
//        WindowCompat.SetDecorFitsSystemWindows(Window!, false);

//    sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

//    // colorThemeValue defaults to RedBmw
//    requestedColorTheme = sharedPreferences!.GetString("colorThemeValue", "1");
//    SetAppTheme(requestedColorTheme!);
//}
#endregion

//private void SetStatusBarAppearance()
//        {
//            WindowInsetsControllerCompat windowInsetsController = new(Window!, Window!.DecorView);

//            if (!IsNightModeActive())
//            {
//                windowInsetsController.AppearanceLightStatusBars = true;
//                windowInsetsController.AppearanceLightNavigationBars = true;
//            }
//            else
//            {
//                windowInsetsController.AppearanceLightStatusBars = false;
//                windowInsetsController.AppearanceLightNavigationBars = false;
//            }
//        }

//}
#endregion