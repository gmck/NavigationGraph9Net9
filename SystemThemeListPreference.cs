using Android.Content;
using Android.Util;
using AndroidX.Preference;

namespace com.companyname.navigationgraph9net9
{
    public class SystemThemeListPreference : ListPreference
    {
        
        internal readonly string DefaultSystemThemeValue = "Android 10, 11 - use Quick Settings Theme button";
        internal string[] themeEntries = [ "System Default", "Light", "Dark" ];
        internal string[] themeValues = [ "1", "2", "3" ];  // equivelent to UiNightMode.Auto, No and Yes, we manipulate it and subtract 1 to match 0,1,2 instead of 1,2,3
        
        #region Ctors
        public SystemThemeListPreference(Context context) : base(context, null)
        {

        }
        public SystemThemeListPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        #endregion

        #region Init
        public void Init()
        {
            // This is called from the SettingsFragment, just to update the Summary. It wont be called again each time OnCreateDialogview is called.
            // Therefore we need to call FindIndexByValue again in OnCreateDialogView
            SetEntries(themeEntries);
            SetEntryValues(themeValues);

            if (!string.IsNullOrEmpty(Value))
                Summary = themeEntries[FindIndexOfValue(Value)];  // Get the current theme
            else
                Summary = DefaultSystemThemeValue;
        }
        #endregion
    }
}