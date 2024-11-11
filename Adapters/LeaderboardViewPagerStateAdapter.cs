using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using com.companyname.navigationgraph9net9.Fragments;
using System;

namespace com.companyname.navigationgraph9net9.Adapters
{
    public class LeaderboardViewPagerStateAdapter : FragmentStateAdapter
    {
        private readonly int itemCount;

        // This is the constructor we are using in this app.
        // Refer to https://stackoverflow.com/questions/61779776/leak-canary-detects-memory-leaks-for-tablayout-with-viewpager2/62184494#62184494 

        public LeaderboardViewPagerStateAdapter(FragmentManager fragmentManager, Lifecycle lifecylce, int itemCount) : base(fragmentManager, lifecylce)
        {
            this.itemCount = itemCount;
        }

        // implement inherited abstract member ItemCount
        public override int ItemCount => itemCount;

        // implement inherited abstract member
        public override Fragment CreateFragment(int position) =>

            // Could just replace with new SampleFragmant1() etc rather than using GetInstance. Using GetInstance allows you to optionally attach a bundle and supply the parameter int position if required to the fragment.
            // All fragment constructors are required to be parameterless, as the adapter can call the constructor any time while paging during the cycle of creating and destroying the fragment.
            // Note we are always creating a new fragment. Do not pass a fragment list of previously created fragments and just return fragmentList[position] as was done with GetItem in ViewPager.
            // The FragmentManager is responsible for looking after the creating and destroying of the fragments as the user swipes through the pages.
            position switch
            {
                0 => new SampleFragment1(),
                1 => new SampleFragment2(),
                2 => new SampleFragment3(),
                _ => throw new InvalidOperationException("ItemCount doesn't match number of fragments")
            };
    }
}