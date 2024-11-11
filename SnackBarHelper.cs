using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.Snackbar;

namespace com.companyname.navigationgraph9net9
{
    public class SnackBarHelper
    {
        public static  void ShowSnackbar(View viewToAttachTo, string message, int lines)
        {
            //See https://m3.material.io/components/snackbar/overview. etc
            Snackbar snackbar = Snackbar.Make(viewToAttachTo, message, Snackbar.LengthIndefinite).SetAction("OK", v => { });

            //snackbar.SetDuration(duration);   // Removed the parameter in Material 3.
            
            TypedValue typedValue = new();
            viewToAttachTo.Context!.Theme!.ResolveAttribute(Resource.Attribute.colorPrimary, typedValue, true); 
            int color = ContextCompat.GetColor(viewToAttachTo.Context, typedValue.ResourceId);

            //snackbar.View.SetBackgroundColor(new Color(color));               // Doesn't work in Material 3. Replace with SetBackgroundTint 
            snackbar.SetBackgroundTint(new Color(color));                   
            
            View view = snackbar.View;
            TextView? tv = view.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv!.SetMaxLines(lines);
            snackbar.Show();
        }
    }
}