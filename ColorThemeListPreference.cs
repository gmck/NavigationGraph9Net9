using Android.Content;
using Android.Util;
using AndroidX.Preference;

namespace com.companyname.navigationgraph9net9
{
    public class ColorThemeListPreference : ListPreference
    {
        internal readonly string DefaultThemeValue = "Select Color Theme";
        internal string[] themeEntries = { "Red Bmw", "Blue Audi", "Green Bmw" };
        internal string[] themeValues = { "1", "2", "3"  };

        #region Ctors
        public ColorThemeListPreference(Context context) : base(context, null)
        {

        }
        public ColorThemeListPreference(Context context, IAttributeSet attrs) : base(context, attrs)
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
                Summary = DefaultThemeValue;
        }
        #endregion
    }
}