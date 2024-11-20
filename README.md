# NavigationGraph9Net9 net9.0-android35
Nov 21, 2024

Just an update of the NuGet packages.

Nov 19, 2024

Fixed an error in the MainActivity's SetShortEdgesIfRequired(NavDestination navDestination);

Nov 12, 2024

**Android 15 Version of NavigationGraph9Net9.**

The contents of the project (.csprog) file have not been changed. All code changes and additions demonstrate the changes necessary to enable the edge-to-edge requirement for Android 15 devices. We have also enabled edge-to-edge for devices running Android 10 through Android 14. However, we have deliberately disabled edge-to-edge for those devices. Therefore, to see edge-to-edge on those devices, you will need to enable it via the SettingsFragment and the preference ***Devices with Notches/Cutouts allow full-screen display***. This is purely for development purposes so that you can easily observe the differences. Whether you enable this preference option for your app builds on those devices is entirely optional.

***The following is a requirement of Android 15 as Android 15 enforces edge-to-edge - see article the 1st link below***

_Before target SDK 35 (Android 15), your app does not draw edge-to-edge without explicit code changes to intentionally go edge-to-edge. After setting targetSdk = 35 or higher, the system will draw your app edge-to-edge by default on Android 15 and later devices. While this change can make it easier for many apps to go edge-to-edge, critical UI elements may also be inaccessible to users. Your app must handle insets to ensure critical UI elements remain accessible._


This is a significant change because both SetStatusBarColor and SetNavigationBarColor have also been deprecated for Android 15. Not just deprecated, but removed. Therefore, existing Material3 themes will require modifications.

To demonstrate edge-to-edge in NavigationGraph9Net9, we added a BooksFragment to the project containing a RecyclerView. The RecyclerView is a simple list of books containing enough items to ensure it can scroll. Each RecyclerView item consists of Book Title, Author Name and Release Date. To add a little more interest, the 3-dot menu of the BooksFragment contains menu items to allow different sort orders of the RecyclerView. The main feature to observe is that the RecylerView items are visible when scrolling through the NavigationBar for both 3-button and gesture navigation. Notably, the last RecyclerView item should be positioned just above the NavigationBar.

However, before getting to the BooksFragment and the edge-to-edge requirements, we first need to modify the BaseActivity.cs. Probably the easiest way to track the changes is to also open the NavigationGraph8Net8 project and compare the two BasicActivity.cs. Alternately, I've left the old code commented out at the end of the new BasicActivity.cs.

**BasicActivity.cs**

The first change I made was the following, but after some testing, I think it is safe only to use EdgeToEdge.Enable() as far as being backwards compatible.

```
if (OperatingSystem.IsAndroidVersionAtLeast(35))
    EdgeToEdge.Enable(this);
else
    WindowCompat.SetDecorFitsSystemWindows(Window!, false);
```
 
 I’ll leave it as it is for now and wait for feedback as I’m unsure if EdgeToEdge.Enable() is safe to use on devices lower than Android 10.

The preference useTransparentStatusBar has also been removed because we can't use the SetStatusBarColor or SetNavigationBarColor with Android 15. However, since both the StatusBar and NavigationBar are transparent by default in Android 15, we default to only use SetStatusBarColor and SetNavigationBarColor for all devices less than Android 15 and now use Resource.Attribute.colorSurface instead of the previous Resource.Attribute.colorSecondary. This results in a white StatusBar, which is a significant change from the prior colorSecondary and the overall look of your application. The two-tone colour combination of StatusBar and Toolbar has been around virtually forever, so this change will likely induce a reaction from your users, favourable or not!!


Consequently, the method SetStatusBarColor() of the old SetAppTheme() has been replaced with ```SetSystemBarsAppearance().``` This new method uses the two new ```windowInsetsController.AppearanceLightStatusBars, windowInsetsController.AppearanceLightNavigationBars``` to control the StatusBar and NavigationBar for both light and dark themes.

**MainActivty.cs**

Next, we have to move to the MainActivity.cs and modify ```OnDestinationChanged()```, actually not OnDestinationChanged itself, but the method it calls ```SetShortEdgesIfRequired(navDestination).``` This introduces the new Android 15 ```Window.Attributes.LayoutInDisplayCutoutMode LayoutInDisplayCutoutMode.Always.``` Please recall the earlier reference to the preference setting ***Devices with Notches/Cutouts allow full-screen display*** above.

Also, the OnApplyWindowInsets method in the MainActivity has been changed to use the following.
```AndroidX.Core.Graphics.Insets windowInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());```
Note it is now Type.SystemBars(), which includes insets for both bars, StatusBar and NavigationBar

**BooksFragment**

There is nothing special about this fragment besides the fact that it is the first time I've ever included a RecyclerView in one of these NavigationGraph projects.

I needed something to demonstrate going edge-to-edge, and a RecylerView can readily demonstrate going edge-to-edge.

As stated earlier, this RecyclerView contains a simple list of books, so I don't think it is necessary to explain how to build a RecyclerView, as most developers have already been using them for years.

The RecyclerView is contained within a ConstraintLayout in fragment_books.xml. It includes a standard RecyclerView.Adapter, BookAdapter, and the BookViewHolder uses book.xml to inflate its view for each RecyclerView item.

**The requirements for edge-to-edge are:**

1.	The last item should be visible above the NavigationBar.
2.	Items of the recyclerview should be visible in the NavigationBar while scrolling.
3.	```android:clipToPadding="false"``` should be included in the xml layout of the recyclerview.

If you test the app with the following line in the OnCreateView commented out you will see that you do need the pad the recyclerview using NavigationBarInsets.Bottom

```
//if (OperatingSystem.IsAndroidVersionAtLeast(29))
//   ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, this);
```

The last item is above the NavigationBar when using Gesture navigation but it is only just above. Now turn on 3-button navigation, and the last item, while visible is not above the NavigationBar. So it is obvious that it it needs the padding

Before Android 15, my RecyclerView's OnApplyWindowsInsets would look like this: Now, uncomment the line and let the OnApplyWindowInsetsListener fire.

```
public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
{
    if (v is RecyclerView)
    {
        AndroidX.Core.Graphics.Insets navigationBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
        v.SetPadding(v.Left, v.Top, v.Right, navigationBarInsets.Bottom + initialPaddingBottom);
    }
    return insets;
}
```
With the above code, the last item is now above the NavigationBar using both modes of Navigation.

You'd probably think you were done if you finished your test with three-button navigation and closed the fragment with the back button. But if you finish your test with gesture navigation at first glance, you may think you are done, but if you decide to reverse the close of the fragment, you'll find you can't because the recyclerview has already disappeared, leaving behind the three headers of the recyclerview, meaning that you have to swipe again to close the fragment completely. It looks really bizarre, with the header values still visible.

Typical reaction: how did that happen? Deploy to an Android 13 device Pixel3a - doesn't do it. That's it. It's only Android 15. Deploy to a Pixel 6 running Android 14 - opps it does it too.

It then took quite abit of debugging to realise what was happening. I'd never debugged a fragment closing before and wasn't even aware that the OnApplyWindowsInsets would fire. But once I understood that systemBarInsets.Left and systemBarInsets.Right could have positive values when swiping from the left or right I finally figured out how to fix it. Of course, that wasn't the end of it. I then needed to figure out if we were using 3-button or Gesture navigation and came up with this IsGestureNavigation() method. 

Requiring a method to test the mode of navigation has never been a requirement previously, so I'm not exactly comfortable having to introduce it. I'm concerned that I've may have overlooked something. However at this point to correct the above behaviour it is required. 

The following is the **IsGestureNavigation()** method.

```
private static bool IsGestureNavigation(WindowInsetsCompat insets)
 {
    // Determine if using Gesture navigation
            
    // Notes: Without this check etc - prior to API 35, we would just adjust the recyclerview with systemBarInserts.Bottom + initialPaddingBottom.
    // However, that caused bizarre behaviour when closing this fragment with a back gesture to close the fragment. When closing a fragment, OnApplyWindowsInsets is called again,
    // and this time systemBarInsets.Left and SystemBarInsets.Right have positive values, therefore without accounting for them, the back gesture was non - reversible,
    // and the recyclerview disappeared, leaving the header of the recycler view, requiring a another swipe to close the fragment, including the header of the recyclerview.
    // Therefore, this method and the new replacement code are needed in the OnApplyWindowInsets(..).
    // Comment the if/else lines and uncomment the single v.Padding() line to see the effect. Note - this didn't affect the closing of the fragment when using 3-button navigation.
            
    AndroidX.Core.Graphics.Insets systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            return systemBarsInsets.Bottom != 0
}
```

Now the modification to the OnApplyListener

```
public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
 {
    if (v is RecyclerView)
    {
        // Makes sure the last item in the recycler view is visible above the NavigationBar.
        // Really obvious when using 3-button navigation. The last item is visible, but it is not above the the NavigationBar
        // Comment out the line above in OnCreateView - ViewCompat.SetOnApplyWindowInsetsListener(recyclerView, this) to see the difference. 

        // Before API 35 - padding of the recyclerview.
        // AndroidX.Core.Graphics.Insets navigationBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
        // v.SetPadding(v.Left, v.Top, v.Right, navigationBarInsets.Bottom + initialPaddingBottom);

        // API 35 Requirment - now need systemBar.Insets.Left systemBarInsets.Right to make sure it works with a backgesture when closing the fragment. See notes in IsGestureNavigation() 
        AndroidX.Core.Graphics.Insets systemBarInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());

        if (IsGestureNavigation(insets)) 
            v.SetPadding(systemBarInsets.Left, v.Top, systemBarInsets.Right, systemBarInsets.Bottom + initialPaddingBottom);
        else
            v.SetPadding(v.Left, v.Top, v.Right, systemBarInsets.Bottom + initialPaddingBottom);
    }
    return insets;
 }
  ```
  That just about covers all the changes. One more reminder, to see the Predictive Back Gesture on quitting the app on Android 13 and 14 devices you need to enable Predictive Back Animations in Developer Options. The same setting has been removed from Android 15 as it is now the deault.
  
  **The following is a list of devices that were tested**

  |Phone| Android Version|
  | --- | --- |
  |Pixel 8|   Android 15|
  |Pixel 7|   Android 15| 
  |Pixel 6|   Android 14|
  |Pixel 3a|  Android 13|
  |Samsung Tab S7|Android 13|
  |Samsung S20 5G| Android 13|
  |Samsung S8| Android 9|

  _Note the Samsung Tab S7, even though it is Android 13 doesn't really belong in this list because it doesn't have a display cutout. Therefore it should be considered more like the Samsung S8 which is really only tested, to make sure there are no problems running on devices less than Android 10._

  **Opting out of edge-to-edge**

I should add that Google offers a way to opt out of Android 15’s requirements of going edge-to-edge. However, I’d still be wary about recommending this option, as it has been rumoured that Google intends to introduce Android 16 earlier than usual, which may be the trigger to remove this option. 

I’ve got nothing against going edge-to-edge, for I extensively use fully immersive fragments in my main app. However, I’m not so sure about SetStatusBarColor being deprecated, as that is a significant change to the design of Material3 that some of my users may object to. To counteract that, I will admit that I’m slowly becoming more comfortable with the loss of colour on the Statusbar.

I haven’t tested the following, as I've no interest in opting out, so you are on your own with these instructions. You need a new values-v35 folder with the following contents. This info comes from the Medium article below in the first link.
```
<resources>
    <style name="OptOutEdgeToEdgeEnforcement">
        <item name="android:windowOptOutEdgeToEdgeEnforcement">true</item>
    </style>
</resources>
```
Then, in the MainActivity call before SetContentView.  

```
Theme.ApplyStyle(Resource.Style.OptOutEdgeToEdgeEnforcement, false)
```
**Google Apps and edge-to-edge**

The one thing in favour of opting out is that Google appears to utilise this option on many of its apps. The criteria I’ve used for the test is based on rotation. Do they hide the cutout, or is it displayed? Obviously, there is more to edge-to-edge than that, such as data scrolling through both the StatusBar and the NavigationBar. The only one that nearly gets a pass as perfect is Google News. The Top App Bar collapses correctly, but the data is not visible when you scroll through the StatusBar.

Yes, I'm being a bit picky... However, one would expect Google's apps to at least showcase edge-to-edge correctly.

I just noticed their new update to the Weather App, is also screwed. Just rotate the screen to the right and check the cards - they need padding...

|Google App|Edge-to-Edge|  
| --- | --- |
| Mail| No| 
| Messages |Yes| 
| Play Store|No|
| Calender|Yes|
| Maps|No|
| Files|Yes|
| Photos|Yes|
| Contacts|Yes|
| Calculator|No|
| Home|No|
| You Tube|Yes|
| Play Books|No|
| Keep Notes |No|
| Google News|Yes|
| Drive |Yes|
| Google TV|No|
| YT Music|No|
| Translate |No|****
| Podcasts|No|
| Google Play Console|Yes|
| Docs|No|
| Tasks |Yes|

**Wrapup**

I would have preferred to also include a collapsing toolbar example in this project. Unfortunately, I had problems in making it work. I’d like to get it working before I decide if I want to add that feature to my published apps. My main app has many fragments using RecyclerViews, and at this stage, I am finding it difficult to decide whether I would like that data to be visible through the StatusBar. The only way to decide would be to see it in action so you can be sure I’ll be working towards a working collapsing top bar version. 

**Links to edge-to-edge articles and documentation**

https://medium.com/androiddevelopers/insets-handling-tips-for-android-15s-edge-to-edge-enforcement-872774e8839b#:~:text=Android%2015%20enforces%20edge%2Dto,Android%2015%20and%20later%20devices.
https://developer.android.com/about/versions/15/behavior-changes-15#edge-to-edge
https://developer.android.com/develop/ui/views/layout/edge-to-edge
https://developer.android.com/codelabs/edge-to-edge#3




Aug 8, 2024

This is a test project to migrate to net9.0-android35 and Android 15. Presently it is a copy of the NavigationGraph8Net8 (Android 14) which was uploaded here approximately 12 months ago. Namespaces and all references etc have been updated to com.companyname.navigationgraph9net9. 

 The intention of this project is track the changes required to migrate an the app from net8-android (Android 14) to net9.0-android35 (Android 15). The first step will be to update all the Nuget Packages to the lastest version while still a net8 project, before changing the target framework to net9.0-android35. For reference please also download the NavigationGraph8Net8 project.

 After building with ```<TargetFramework>net8-android</TargetFramework>``` with the updated Nuget Packages (all packages were updated 29 August 2024, the app runs fine on both Pixel 7 (beta Android 15) and Pixel 8 (Android 14) with no changes in functionality. Have also commented out the proguard.cfg as the two keep rules are no longer necessary.

 From here the target framework will be changed to ```<TargetFramework>net9.0-android35</TargetFramework>``` 

 

