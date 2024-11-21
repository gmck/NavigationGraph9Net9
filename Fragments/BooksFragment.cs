using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using com.companyname.navigationgraph9net9.Adapters;
using com.companyname.navigationgraph9net9.Classes;
using Google.Android.Material.Color.Utilities;
using System;
using System.Collections.Generic;

namespace com.companyname.navigationgraph9net9.Fragments
{
    public class BooksFragment : Fragment, IMenuProvider, IOnApplyWindowInsetsListener
    {
        private RecyclerView? recyclerView;
        private BookAdapter? bookAdapter;
        private List<Book>? books;
        private int initialPaddingBottom;
        private SortOrder currentSortOrder;     // It will default to zero e.g. SortOrder.Original

        public BooksFragment() { }

        #region OnCreate
        public override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            books = GetBooks();
        }
        #endregion

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_books, container, false);

            if (savedInstanceState != null)
                currentSortOrder = (SortOrder)savedInstanceState.GetInt(GetString(Resource.String.sort_order_key), (int)SortOrder.Original);

            bookAdapter = new BookAdapter(books!);
            recyclerView = view!.FindViewById<RecyclerView>(Resource.Id.recyclerview_books);
            recyclerView!.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.HasFixedSize = true;
            recyclerView.AddItemDecoration(new SimpleItemDecoration(Activity!));
            recyclerView.SetAdapter(bookAdapter);
            initialPaddingBottom = recyclerView.PaddingBottom; //// incase not zero

            ApplySortOrder(currentSortOrder);

            // OnApplyWindowInsetsListener is needed otherwise, the last record while visible, is below the NavigationBar. With the insets listener it is visible above the NavigationBar.
            // Only really obvious when using 3 button Navigation as with Swipe navigation 
            // The requirments for EdgeToEdge are:
            // 1. The last item should be visible above the NavigationBar. 
            // 2. Each item of the recyclerview should be visible in the NavigationBar while scrolling.
            // 3. android:clipToPadding="false" should be included in the xml layout of the recyclerview. 

            // Special note: For devices below Android 15, i.e. 10-14 API 29-34 you need to check the following setting in Settings. Devices with Notches/Cutouts allow full screen display.
            // This settings option normally wouldn't be displayed. But for developers it demonstrates what happens when we don't go full screen and shows that we can make it backward compatible. 
            // Check and uncheck to see the difference.

            // Android 10 and above
            if (OperatingSystem.IsAndroidVersionAtLeast(29))    // 29 Android 10 - Build.VERSION_CODES.Q
                ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, this);

            return view;
        }
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            (RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);
        }
        #endregion

        #region OnSaveInstanceState
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(GetString(Resource.String.sort_order_key), (int)currentSortOrder);
        }
        #endregion

        #region OnCreateMenu
        public void OnCreateMenu(IMenu menu, MenuInflater menuInflater)
        {
            menuInflater.Inflate(Resource.Menu.menu_books_fragment, menu);
        }
        #endregion

        #region OnMenuItemSelected
        public bool OnMenuItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.action_sort_by_title:
                    currentSortOrder = SortOrder.Title;
                    ApplySortOrder(currentSortOrder);
                    return true;

                case Resource.Id.action_sort_by_author:
                    currentSortOrder = SortOrder.Author;
                    ApplySortOrder(currentSortOrder);
                    return true;

                case Resource.Id.action_sort_by_release_date:
                    currentSortOrder = SortOrder.ReleaseDate;
                    ApplySortOrder(currentSortOrder);
                    return true;

                case Resource.Id.action_sort_by_original_order:
                    currentSortOrder = SortOrder.Original;
                    ApplySortOrder(currentSortOrder);
                    return true;

                default:
                    return false;
            }
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            if (v is RecyclerView)
            {
                // Makes sure the last item in the recycler view is visible above the NavigationBar.
                // Really obvious when using 3-button navigation. The last item is visible, but it is not above the the NavigationBar
                // Comment out the lines above in OnCreateView - ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, this) to see the difference. 

                // Before API 35 - padding of the recyclerview.
                //AndroidX.Core.Graphics.Insets navigationBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
                //v.SetPadding(v.Left, v.Top, v.Right, navigationBarInsets.Bottom + initialPaddingBottom);


                // API 35 Requirment - now need systemBar.Insets.Left and systemBarInsets.Right to make sure it works with a backgesture when closing the fragment. See notes in IsGestureNavigation() 
                AndroidX.Core.Graphics.Insets systemBarInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
                if (NavigationMode.IsGestureNavigation(insets))
                    v.SetPadding(systemBarInsets.Left, v.Top, systemBarInsets.Right, systemBarInsets.Bottom + initialPaddingBottom);
                else
                    v.SetPadding(v.Left, v.Top, v.Right, systemBarInsets.Bottom + initialPaddingBottom);


                // WindowInsetsCompat.Type.SystemGestures() or WindowInsetsCompat.Type.SystemGestures() also fails 
                //AndroidX.Core.Graphics.Insets windowInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.MandatorySystemGestures());
                //v.SetPadding(windowInsets.Left, v.Top, windowInsets.Right, windowInsets.Bottom + initialPaddingBottom);

            }
            return insets;
        }
        #endregion

        #region IsGestureNavigation - Now has it own class NavigationMode.IsGestureNavigation
        //private static bool IsGestureNavigation(WindowInsetsCompat insets)
        //{
        //    // Determine if using Gesture navigation

        //    // Notes: Without this check etc - prior to API 35, we would just adjust the recyclerview with systemBarInserts.Bottom + initialPaddingBottom.
        //    // However, that caused bizarre behaviour when closing this fragment with a back gesture to close the fragment. When closing a fragment, OnApplyWindowsInsets is called again,
        //    // and this time systemBarInsets.Left and SystemBarInsets.Right have positive values, therefore without accounting for them, the back gesture was non - reversible,
        //    // and the recyclerview disappeared, leaving the header of the recyclerview, requiring a another swipe to close the fragment, including the header of the recyclerview.
        //    // Therefore, this method and the new replacement code are needed in the OnApplyWindowInsets(..).
        //    // Comment the if/else lines and uncomment the single v.Padding() line to see the effect. Note - this didn't affect the closing of the fragment when using 3-button navigation.

        //    AndroidX.Core.Graphics.Insets systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
        //    return systemBarsInsets.Bottom != 0;
        //}
        #endregion

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

        #region GetBooks
        private List<Book> GetBooks()
        {
            // A random assortment of books to demonstrate a RecyclerView
            return books = [
                new("The Secrets She Keeps", "Michael", "Robotham", "2017"),
                new("Say You're Sorry", "Michael", "Robotham",  "2012"),
                new("When She Was Good", "Michael", "Robotham", "2020"),
                new("Lying Beside You", "Michael", "Robotham", "2022"),
                new("When You Are Mine", "Michael", "Robotham", "2021"),
                new("Watching you", "Michael", "Robotham", "2013"),
                new("Storm Child", "Michael", "Robotham", "2024"),
                new("The Night Ferry", "Michael", "Robotham", "2007"),
                new("Shatter", "Michael", "Robotham", "2008"),
                new("Bleed For Me", "Michael", "Robotham", "2010"),
                new("The Wreckage", "Michael", "Robotham", "2011"),
                new("The Suspect", "Michael", "Robotham", "2004"),
                new("Lost", "Michael", "Robotham", "2005"),
                new("The Other Wife", "Michael", "Robotham", "2018"),
                new("Watching You", "Michael", "Robotham", "2013"),
                new("Good Girl, Bad Girl", "Michael","Robotham","2019"),
                new("The Lost Man", "Jane", "Harper", "2018"),
                new("The Dry", "Jane", "Harper", "2016"),
                new("Force of Nature", "Jane", "Harper", "2017"),
                new("No Middel Name", "Lee","Child", "2017"),
                new("The Sentinel", "Lee","Child", "2020"),
                new("Night School", "Lee","Child", "2016"),
                new("Past Tense", "Lee", "Child", "2024"),
                new("Midnight and Blue", "Ian","Rankin", "2024"),
                new("A Heart Full of Headstones","Ian","Rankin","2022"),
                new("A Song for the Dark Times ","Ian","Rankin","2020"),
                new("In a House of Lies","Ian","Rankin","2018"),
                new("Rather Be the Devil","Ian","Rankin","2016"),
                new("Even Dogs in the Wild","Ian","Rankin","2015"),
                new("Saints of the Shadow Bible","Ian","Rankin","2013")
            ];
        }
        #endregion

        #region ApplySortOrder
        private void ApplySortOrder(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Title:
                    books!.Sort((b1, b2) => string.Compare(b1.Title, b2.Title, StringComparison.Ordinal));
                    break;
                case SortOrder.Author:
                    books!.Sort((b1, b2) => string.Compare(b1.LastName, b2.LastName, StringComparison.Ordinal));
                    break;
                case SortOrder.ReleaseDate:
                    books!.Sort((b1, b2) => string.Compare(b1.ReleaseDate, b2.ReleaseDate, StringComparison.Ordinal));
                    break;
                case SortOrder.Original:
                default:
                    books = GetBooks();
                    break;
            }
            bookAdapter!.UpdateBooks(books);
        }
        #endregion

        #region SortOrder - enum
        public enum SortOrder
        {
            Original,
            Title,
            Author,
            ReleaseDate
        }
        #endregion

        #region Leave for reference
        //public override void OnSaveInstanceState(Bundle? outState)
        //{
        //    base.OnSaveInstanceState(outState!);
        //    LinearLayoutManager? layoutManager = (LinearLayoutManager)recyclerView!.GetLayoutManager()!;
        //    outState!.PutInt("scrolled_position",
        //    layoutManager.FindFirstCompletelyVisibleItemPosition());
        //}

        //public override void OnViewStateRestored(Bundle? savedInstanceState)
        //{
        //    base.OnViewStateRestored(savedInstanceState);
        //    if (savedInstanceState != null)
        //    {
        //        int scrollPosition = savedInstanceState.GetInt("scrolled_position");
        //        recyclerView!.ScrollToPosition(scrollPosition);
        //    }
        //}

        //public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        //{
        //    if (v is RecyclerView)
        //    {
        //        // Make sure the last item in the recycler view is visible. Really obvious when using 3-button navigation.
        //        // Comment out the line above in OnCreateView - ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, this) to see the difference. 
        //        AndroidX.Core.Graphics.Insets systemBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars()); 

        //        //AndroidX.Core.Graphics.Insets systemGesturesInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemGestures());
        //        //AndroidX.Core.Graphics.Insets mandatorySystemGesturesInsets = insets.GetInsets(WindowInsetsCompat.Type.MandatorySystemGestures());
        //        //AndroidX.Core.Graphics.Insets systemBarsSystemGestureInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.SystemGestures());
        //        //AndroidX.Core.Graphics.Insets navigationBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());

        //        v.SetPadding(v.Left, v.Top, v.Right, v.Bottom = systemBarInsets.Bottom + initialPaddingBottom);

        //    }
        //    return insets;
        //}
        #endregion

    }

}

// Considering converting to C# but so far don't have a need. See above IsGestureNavigation

/*
 * Check if gesture navigation mode is active.
 * In gesture navigation mode, the standard navigation bar is hidden and replaced with a gesture area with a handle.
 *
 * To detect if the device is in gesture navigation mode, both conditions should be met:
 * 1. At least one system gesture horizontal inset is greater than 0.
 * 2. Navigation bar horizontal insets are equal to 0.
 */
//fun WindowInsetsCompat.isInGestureNavigationMode(): Boolean {
//    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.Q) {
//        return false
//    }
//    val systemGesturesInsets = getInsetsIgnoringVisibility(WindowInsetsCompat.Type.systemGestures())
//    val navigationBarsInsets = getInsetsIgnoringVisibility(WindowInsetsCompat.Type.navigationBars())

//    val hasSystemGestureHorizontalInset = systemGesturesInsets.left > 0 || systemGesturesInsets.right > 0
//    val hasNavigationBarHorizontalInset = navigationBarsInsets.left > 0 || navigationBarsInsets.right > 0

//    return hasSystemGestureHorizontalInset && !hasNavigationBarHorizontalInset
//}
