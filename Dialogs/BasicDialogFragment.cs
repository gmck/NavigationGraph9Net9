using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Dialog;

namespace com.companyname.navigationgraph9net9.Dialogs
{
    internal class BasicDialogFragment : AppCompatDialogFragment
    {
        public BasicDialogFragment() { } // Required Parameter less ctor

        internal static BasicDialogFragment NewInstance(string title, string message)
        {
            Bundle arguments = new();
            arguments.PutString("Title", title);
            arguments.PutString("Message", message);

            BasicDialogFragment fragment = new()
            {
                Cancelable = false,
                Arguments = arguments
            };
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle? savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            
            LayoutInflater? inflater = LayoutInflater.From(Activity!);
            View? view = inflater!.Inflate(Resource.Layout.generic_dialog, null);

            TextView? textViewExplanation = view!.FindViewById<TextView>(Resource.Id.textView_explanation);
            textViewExplanation!.Text = Arguments!.GetString("Message");

            MaterialAlertDialogBuilder builder = new(Activity!);    
            builder.SetTitle(Arguments.GetString("Title"));
            builder.SetPositiveButton(Android.Resource.String.Ok, (sender, args) => { Dismiss(); });
            builder.SetView(view);
            return builder.Create();
        }
    }
}
