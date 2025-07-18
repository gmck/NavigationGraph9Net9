using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using Google.Android.Material.CheckBox;
using Google.Android.Material.MaterialSwitch;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class MaterialWidgetsFragment : Fragment, IOnApplyWindowInsetsListener
    {
        private ConstraintLayout? widgetsConstraintLayout;
        private int initialPaddingBottom;

        public MaterialWidgetsFragment( ) { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view =inflater.Inflate(Resource.Layout.fragment_widgets, container, false);

            widgetsConstraintLayout = view!.FindViewById<ConstraintLayout>(Resource.Id.widgets_constraint);
            MaterialCheckBox? materialCheckBox = view!.FindViewById<MaterialCheckBox>(Resource.Id.checkBox1);
            MaterialSwitch? materialSwitch = view.FindViewById<MaterialSwitch>(Resource.Id.switch1);
            MaterialButton? materialButton = view.FindViewById<MaterialButton>(Resource.Id.button1);
            ViewCompat.SetOnApplyWindowInsetsListener(widgetsConstraintLayout!, this);
            initialPaddingBottom = widgetsConstraintLayout!.PaddingBottom;

            materialCheckBox!.Checked = true;
            materialSwitch!.Checked = true;

            materialButton!.Click += (sender, e) =>
                {
                    if (materialCheckBox.Checked)
                    {
                        materialCheckBox.Checked = false;
                        materialSwitch.Checked = false;
                    }
                    else
                    {
                        materialCheckBox.Checked = true;
                        materialSwitch.Checked = true;
                    }
                };

            return view;
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
        {
            // Need to keep the FloatingActionButton above the NavigationBar in Landscape Mode so we pad the view
            if (v is ConstraintLayout)
            {
                AndroidX.Core.Graphics.Insets? navigationBarsInsets = insets!.GetInsets(WindowInsetsCompat.Type.NavigationBars());
                v.SetPadding(v.PaddingLeft, v.PaddingTop, v.PaddingRight, navigationBarsInsets!.Bottom + initialPaddingBottom);
            }
            return insets!;
        }
        #endregion
    }
}