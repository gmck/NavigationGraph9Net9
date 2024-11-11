#nullable enable
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace com.companyname.navigationgraph9net9
{
    internal class SimpleItemDecoration : RecyclerView.ItemDecoration
    {
        internal Drawable? divider;
        internal int[] attributes = new int[] {Android.Resource.Attribute.ListDivider}; 
        
        internal SimpleItemDecoration(Context context)
        {
            TypedArray ta = context.ObtainStyledAttributes(attributes);
            divider = ta.GetDrawable(0);
            ta.Recycle();
        }

        public override void OnDraw(Android.Graphics.Canvas c, RecyclerView parent, RecyclerView.State state)
        {
 	        base.OnDraw(c, parent, state);

            int left = parent.PaddingLeft;
            int right = parent.Width - parent.PaddingRight;

            for (int i = 0; i < parent.ChildCount; i++)
            {
                View child = parent.GetChildAt(i)!;

                //ViewGroup.MarginLayoutParams parameters = (ViewGroup.MarginLayoutParams)child.LayoutParameters; This was the workaround, until usage of JavaCast
                //RecyclerView.LayoutParams parameters = (RecyclerView.LayoutParams)child.LayoutParameters; // InvalidCastException
                RecyclerView.LayoutParams? parameters = child.LayoutParameters.JavaCast<RecyclerView.LayoutParams>();
                
                
                int top = child.Bottom + parameters!.BottomMargin;
                int bottom = top + divider!.IntrinsicHeight;

                divider.SetBounds(left, top, right, bottom);
                divider.Draw(c);
                parameters.Dispose();
            }
            
        }
    }
}




