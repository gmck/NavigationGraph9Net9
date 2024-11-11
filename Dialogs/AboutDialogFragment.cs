#nullable enable
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using com.companyname.navigationgraph9net9.Classes;
using Google.Android.Material.Dialog;
using System;
using System.Collections.Generic;

namespace com.companyname.navigationgraph9net9.Dialogs
{
    public class AboutDialogFragment  : AppCompatDialogFragment
    {
        internal TextView? textViewVersionName;
        internal TextView? textViewTargetVersionName;
        internal TextView? textViewAndroidVersionName;
        internal List<AndroidVersions>? androidVersions;
        
        #region Ctors
        public AboutDialogFragment() { } // Required Parameter less ctor
        #endregion

        #region NewInstance
        public static AboutDialogFragment NewInstance()
        {
            AboutDialogFragment fragment = new()
            {
                Cancelable = false,
            };
            return fragment;
        }
        #endregion

        #region OnCreate
        public override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // We need to create a new instance of androidVersions here because it is null when we come back from a rotation. 27/01/1924 Moved from the ctor to here.
            androidVersions =
                [
                    //                  AndroidName      AndroidBuildCode    AndroidCodeName         AndroidApiNumber
                    new AndroidVersions("Android 7.0",   "N",                "Nougat",               "Api 24"),
                    new AndroidVersions("Android 7.1",   "N_MR1",            "Nougat",               "Api 25"),
                    new AndroidVersions("Android 8.0",   "O",                "Oreo",                 "Api 26"),
                    new AndroidVersions("Android 8.1",   "O_MRI",            "Oreo",                 "Api 27"),
                    new AndroidVersions("Android 9",     "P",                "Pie",                  "Api 28"),
                    new AndroidVersions("Android 10",    "Q",                "Quince Tart",          "Api 29"),
                    new AndroidVersions("Android 11",    "R",                "Red Valvet Cake",      "Api 30"),
                    new AndroidVersions("Android 12",    "S",                "Snow Cone",            "Api 31"),
                    new AndroidVersions("Android 12L",   "S_V2",             "Snow Cone",            "Api 32"),
                    new AndroidVersions("Android 13",    "TIRAMISU",         "TiraMisu",             "Api 33"),
                    new AndroidVersions("Android 14",    "UPSIDE_DOWN_CAKE", "Upside Down Cake",     "Api 34"),
                    new AndroidVersions("Android 15",    "VANILA_ICE_CREAM", "Vanila Ice Cream",     "Api 35")
                ];
        }
        #endregion

        #region OnCreateDialog
        public override Dialog OnCreateDialog(Bundle? savedInstanceState)
        {
            PackageInfo packageInfo;
            PackageManager packageManager = Activity!.PackageManager!;
             
            if (OperatingSystem.IsAndroidVersionAtLeast(33))  // Api 13
                packageInfo = packageManager.GetPackageInfo(Activity!.PackageName!, PackageManager.PackageInfoFlags.Of(PackageInfoFlagsLong.None));
            else
#pragma warning disable CS0618 // Type or member is obsolete
                packageInfo = packageManager.GetPackageInfo(Activity!.PackageName!, 0)!;
#pragma warning restore CS0618 // Type or member is obsolete

            string? build = Build.VERSION.Release;
            AndroidVersions? androidVersion = androidVersions!.Find(x => x.AndroidName == "Android " + build);

            LayoutInflater? inflater = LayoutInflater.From(Activity);
            View? view = inflater!.Inflate(Resource.Layout.about_dialog, null);

            textViewVersionName = view!.FindViewById<TextView>(Resource.Id.textview_versionName);
            string buildDate = GetString(Resource.String.build_date); 
            textViewVersionName!.Text = packageInfo.VersionName +" - " + buildDate;

            textViewTargetVersionName = view.FindViewById<TextView>(Resource.Id.textview_targetVersionName);
            textViewTargetVersionName!.Text = ((int)packageInfo!.ApplicationInfo!.TargetSdkVersion).ToString();
            textViewAndroidVersionName = view.FindViewById<TextView>(Resource.Id.textview_androidVersionName);

            // Make sure we did obtain a legit androidVersion from AndroidVersions
            if (androidVersion != null)
            {
                string androidName = !string.IsNullOrEmpty(androidVersion.AndroidName) ? " - " +androidVersion.AndroidName : string.Empty;
                textViewAndroidVersionName!.Text = androidVersion.AndroidCodeName + " - " + androidVersion.AndroidApiNumber + androidName;
            }
            else
                textViewAndroidVersionName!.Text = build;

            MaterialAlertDialogBuilder builder = new(Activity); 
            builder.SetTitle(Resource.String.about_dialog_title);
            builder.SetView(view);
            builder.SetPositiveButton(Android.Resource.String.Ok, delegate (object? o, DialogClickEventArgs e)
            {
                Dismiss();
            });
            return builder.Create();

        }
        #endregion
    }
}