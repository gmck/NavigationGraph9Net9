using AndroidX.Core.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.companyname.navigationgraph9net9.Classes
{
    public static class NavigationMode
    {
        public static bool IsGestureNavigation(WindowInsetsCompat insets)
        {
            // Determine if using Gesture navigation

            // Notes: Without this check etc - prior to API 35, we would just adjust the recyclerview with systemBarInserts.Bottom + initialPaddingBottom.
            // However, that caused bizarre behaviour when closing this fragment with a back gesture to close the fragment. When closing a fragment, OnApplyWindowsInsets is called again,
            // and this time systemBarInsets.Left and SystemBarInsets.Right have positive values, therefore without accounting for them, the back gesture was non - reversible,
            // and the recyclerview disappeared, leaving the header of the recyclerview, requiring a another swipe to close the fragment, including the header of the recyclerview.
            // Therefore, this method and the new replacement code are needed in the OnApplyWindowInsets(..).
            // Comment the if/else lines and uncomment the single v.Padding() line to see the effect. Note - this didn't affect the closing of the fragment when using 3-button navigation.

            AndroidX.Core.Graphics.Insets systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            return systemBarsInsets.Bottom != 0;
        }

        # region Alternate IsGestureNavigationMode(WindowInsetsCompat insets)
        //private static bool IsGestureNavigationMode(WindowInsetsCompat insets) // Not using, but works.
        //{
        //    // This came from https://stackoverflow.com/questions/56689210/how-to-detect-full-screen-gesture-mode-in-android-10/60733427#60733427
        //    // See the commented Android code outside of the final } Also works, but more complex. 

        //    AndroidX.Core.Graphics.Insets systemGesturesInsets = insets.GetInsetsIgnoringVisibility(WindowInsetsCompat.Type.SystemGestures());
        //    AndroidX.Core.Graphics.Insets navigationBarsInsets = insets.GetInsetsIgnoringVisibility(WindowInsetsCompat.Type.NavigationBars());

        //    bool hasSystemGestureHorizontalInset = systemGesturesInsets.Left > 0 || systemGesturesInsets.Right > 0;
        //    bool hasNavigationBarHorizontalInset = navigationBarsInsets.Left > 0 || navigationBarsInsets.Right > 0;

        //    return hasSystemGestureHorizontalInset && !hasNavigationBarHorizontalInset;
        //}
        #endregion
    }
}
