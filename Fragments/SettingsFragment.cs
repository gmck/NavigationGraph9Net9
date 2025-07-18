using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.Preference;
using com.companyname.navigationgraph9net9.Classes;
using System;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat, IOnApplyWindowInsetsListener
    {
        private ISharedPreferences? sharedPreferences;
        private ColorThemeListPreference? colorThemeListPreference;
        private SystemThemeListPreference? systemThemeListPreference;
        private int initialPaddingBottom;

        #region OnCreatePreferences
        public override void OnCreatePreferences(Bundle? savedInstanceState, string? rootKey)
        {
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);

            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);

            if (PreferenceScreen!.FindPreference("colorThemeValue") is ColorThemeListPreference colorThemeListPreference)
            {
                colorThemeListPreference.Init();
                colorThemeListPreference.PreferenceChange += ColorThemeListPreference_PreferenceChange;
            }

            if (PreferenceScreen.FindPreference("systemThemeValue") is SystemThemeListPreference systemThemeListPreference)
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(31))    //*if (Build.VERSION.SdkInt >= BuildVersionCodes.S)*/ //Either here is ok
                {
                    systemThemeListPreference.Init();
                    systemThemeListPreference.PreferenceChange += SystemThemeListPreference_PreferenceChange;
                }
                else
                    systemThemeListPreference.Enabled = false;
            }

            if (PreferenceScreen.FindPreference("darkTheme") is CheckBoxPreference checkboxDarkThemePreference)
            {
                if (!OperatingSystem.IsAndroidVersionAtLeast(29))   //Build.VERSION.SdkInt < BuildVersionCodes.Q) // Android 10 API 29 
                    checkboxDarkThemePreference.PreferenceChange += CheckboxDarkThemePreference_PreferenceChange;
                else
                    checkboxDarkThemePreference.Enabled = false;
            }

            if (PreferenceScreen.FindPreference("use_dynamic_colors") is CheckBoxPreference checkboxDynamicColors)
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(31))    //*if (Build.VERSION.SdkInt >= BuildVersionCodes.S)*/ //Either here is ok
                    checkboxDynamicColors.PreferenceChange += CheckboxDynamicColors_PreferenceChange;
                else
                    checkboxDynamicColors.Enabled = false;
            }

            // Removed from NavigationGraph9Net9 - no longer applicable.
            //if (PreferenceScreen.FindPreference("use_transparent_statusbar") is CheckBoxPreference checkboxTransparentStausBar)
            //    if (!OperatingSystem.IsAndroidVersionAtLeast(35))
            //        checkboxTransparentStausBar.PreferenceChange += CheckboxTransparentStausBar_PreferenceChange;
            //    else
            //        checkboxTransparentStausBar.Enabled = false;

            if (PreferenceScreen.FindPreference("devicesWithNotchesAllowFullScreen") is CheckBoxPreference checkboxDevicesWithNotchesAllFullScreen)
            {
                //if (OperatingSystem.IsAndroidVersionAtLeast(29))  
                //    checkboxDevicesWithNotchesAllFullScreen.PreferenceChange += CheckboxDevicesWithNotchesAllFullScreen_PreferenceChange;
                //else
                //    checkboxDevicesWithNotchesAllFullScreen.Enabled = false;

                // Changed from above to 10/11/2024
                if (OperatingSystem.IsAndroidVersionAtLeast(35))
                    checkboxDevicesWithNotchesAllFullScreen.Enabled = false;  // No need to enable as Android 35 choice isn't required as it's allows full screen
                else if (!OperatingSystem.IsAndroidVersionAtLeast(35))
                {
                    if (OperatingSystem.IsAndroidVersionAtLeast(29)) // i.e. Q or Android 10 and above
                        checkboxDevicesWithNotchesAllFullScreen.PreferenceChange += CheckboxDevicesWithNotchesAllFullScreen_PreferenceChange;
                    else
                        checkboxDevicesWithNotchesAllFullScreen.Enabled = false;
                }
            }
        }
        #endregion

        #region OnCreateView
        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            // An androidx.preference.PreferenceScreen is not the same as a normal Resource.xml file for example like a androidx.constraintlayout.widget.ConstraintLayout.
            // It is a xml layout, but it is not created in the Resource layout folder, but in Resource xml folder, therefore it doesn't have any widget type.
            // Therefore to use it with IOnApplyWindowInsetsListener which we need if we are supporting both gesture and 3-button navigation we need to find out what type of view it contains.
            // If you put a break point on the first line here and check the view variable, you will see that it is a LinearLayout.
            // And if we check layout.GetChildAt(0) we will see that it is a FrameLayout and GetChildAt(1) is a MaterialTextView.
            // So in this case the LinearLayout is the root view and we therefore need to set the OnApplyWindowInsetsListener on it.

            View? view = base.OnCreateView(inflater, container, savedInstanceState);
            if (view is LinearLayout layout)
            {
                //view.ClipToOutline = false; Didn't work
                initialPaddingBottom = layout!.PaddingBottom;
                ViewCompat.SetOnApplyWindowInsetsListener(layout, this);
            }
            return view;
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
        {
            if (v is LinearLayout)
            {
                AndroidX.Core.Graphics.Insets? systemBarsInsets = insets!.GetInsets(WindowInsetsCompat.Type.SystemBars());
                if (NavigationMode.IsGestureNavigation(insets))
                    v.SetPadding(systemBarsInsets!.Left, v.Top, systemBarsInsets.Right, systemBarsInsets.Bottom + initialPaddingBottom);
                else
                    v.SetPadding(v.Left, v.Top, v.Right, systemBarsInsets!.Bottom + initialPaddingBottom);
            }
            return insets!;
        }
        #endregion

        #region CheckboxDarkThemePreference_PreferenceChange
        private void CheckboxDarkThemePreference_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
        {
            bool requestedNightMode = (bool)e.NewValue!;
            ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
            editor!.PutBoolean("darkTheme", requestedNightMode!)!.Apply();
            editor.Commit();

            // This is only available to devices running less than Android 10.
            SetDefaultNightMode(requestedNightMode);
        }
        #endregion

        #region CheckboxDynamicColors_PreferenceChange
        private void CheckboxDynamicColors_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
        {
            bool useDynamicColors = (bool)e.NewValue!;
            ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
            editor!.PutBoolean("use_dynamic_colors", useDynamicColors)!.Apply();
            editor.Commit();

            Activity!.Recreate();
        }
        #endregion

        #region CheckboxDevicesWithNotchesAllFullScreen_PreferenceChange
        private void CheckboxDevicesWithNotchesAllFullScreen_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
        {
            bool requestedMode = (bool)e.NewValue!;
            ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
            editor!.PutBoolean("devicesWithNotchesAllowFullScreen", requestedMode)!.Apply();
            editor.Commit();

            Activity!.Recreate();
        }
        #endregion

        #region ColorThemeListPreference_PreferenceChange
        private void ColorThemeListPreference_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
        {
            colorThemeListPreference = e.Preference as ColorThemeListPreference;

            // Working nullable code - the long way
            //ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            //ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
            //editor!.PutString("colorThemeValue", e.NewValue!.ToString());
            //editor.Apply();

            ISharedPreferencesEditor? editor = colorThemeListPreference!.SharedPreferences!.Edit();
            editor!.PutString("colorThemeValue", e.NewValue!.ToString())!.Apply();
            editor.Commit();

            int index = Convert.ToInt16(e.NewValue.ToString());
            string colorThemeValue = colorThemeListPreference!.GetEntries()![index - 1];
            colorThemeListPreference.Summary = (index != -1) ? colorThemeValue : colorThemeListPreference.DefaultThemeValue;

            // Must now force the theme to change - see BaseActivity. It's OnCreate checks the sharedPreferences, get the string currentTheme and passes that value to SetAppTheme(currentTheme)
            // which checks to see if it has changed and if so calls SetTheme which the correct Resource.Style.Theme_Name)
            Activity!.Recreate();
        }
        #endregion

        #region SystemThemeListPreference_PreferenceChange
        private void SystemThemeListPreference_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
        {
            systemThemeListPreference = e.Preference as SystemThemeListPreference;

            ISharedPreferencesEditor? editor = systemThemeListPreference!.SharedPreferences!.Edit();
            editor!.PutString("systemThemeValue", e.NewValue!.ToString())!.Apply();
            editor.Commit();

            int index = Convert.ToInt16(e.NewValue.ToString());
            string systemThemeValue = systemThemeListPreference.GetEntries()![index - 1];
            systemThemeListPreference.Summary = (index != -1) ? systemThemeValue : systemThemeListPreference.DefaultSystemThemeValue;

            // Only available on devices running Android 12+

            // Note we subtract 1 from the index - See SystemThemeListPreference
            // Equivelent to UiNightMode.Auto, No and Yes, we manipulated it by subtracting 1 to match 0,1,2 instead of 1,2,3 as in SystemThemeListPreference

            UiNightMode uiNightMode = (UiNightMode)index - 1;
            SetDefaultNightMode12(uiNightMode);
        }
        #endregion

        #region SetDefaultNightMode12
        private void SetDefaultNightMode12(UiNightMode uiNightMode)
        {
            // Sets and persists the night mode setting for this app. This allows the system to know
            // if the app wants to be displayed in dark mode before it launches so that the splash
            // screen can be displayed accordingly.
            // You don't need to do this if your app doesn't provide in-app dark mode setting. e.g. System Default, Light, Dark.
            // UiModeService
            // You could call overriding the Quick Settings Day/Night Theme button. 
            // In other words the user can select to override whatever the theme button is set to when on an Android 12+ device.

            UiModeManager? uiModeManager = Activity!.GetSystemService(Context.UiModeService) as UiModeManager;
            if (OperatingSystem.IsAndroidVersionAtLeast(31))    /* Build.VERSION.SdkInt >= BuildVersionCodes.S */   //Must use OperatingSystem.IsAndroidVersionAtLeast(xx) otherwise a warning  
                uiModeManager!.SetApplicationNightMode((int)uiNightMode);  // Only avaialable on Android 12 -API31 -S and above.

            ISharedPreferencesEditor? editor = sharedPreferences!.Edit();
            editor!.PutInt("night_mode", (int)uiNightMode)!.Apply();
            editor.Commit();
        }
        #endregion

        #region SetDefaultNightMode
        private static void SetDefaultNightMode(bool requestedNightMode)
        {
            AppCompatDelegate.DefaultNightMode = requestedNightMode ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;
        }
        #endregion

    }
}