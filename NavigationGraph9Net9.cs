using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using System;

namespace com.companyname.navigationgraph9net9
{
    [Application]
    public class NavigationGraph9ApplicationNet9 : Application, Application.IActivityLifecycleCallbacks
    {
        protected readonly string logTag = "Nav-ApplicationClass";
        protected NavigationGraph9ApplicationNet9(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
        public NavigationGraph9ApplicationNet9() { }

        public override void OnCreate()
        {
            base.OnCreate();
            // This screws up when using the SplashScreen Api - looks like an Android bug. So we use DynamicColors.ApplyToActivityIfAvailable in BaseActivity since we only have one activity. - note the singular, rather than the plural as is here  
            //DynamicColors.ApplyToActivitiesIfAvailable(this);
            
            // Note that this is only called on a cold start.
            //RegisterActivityLifecycleCallbacks(this);
            //Log.Debug(logTag, logTag+" OnCreate - RegisterActivityLifecycleCallbacks");
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            //UnregisterActivityLifecycleCallbacks(this);
            //Log.Debug(logTag, logTag+ "OnTerminate");
        }

        // This fires before OnActivityDestroyed
        //public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        //{
        //    base.OnTrimMemory(level);
        //    if (level == TrimMemory.UiHidden)
        //    {
        //        // stop your service here
        //        //StopService(new Intent(this, typeof(MyService)));
        //        // ...
        //    }
        //}

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState) 
        {
            Log.Debug(logTag, logTag+" OnActivityCreated");
        }

        public void OnActivityDestroyed(Activity activity) 
        {
            // We require it because we have to implement it because of the Interface, however we don't require any code in the body because we are using OnActivitySaveInstanceState below

            // Note: This worked fine when we had Developer Settings Apps Don’t keep activities in the ON position, but doesn't fire when Don’t keep activities is OFF
            // Since our users aren't going to have that setting on, so we needed something else and that turned out to be OnActivitySaveInstanceState - see below

            //if (!activity.IsChangingConfigurations)
            //{
            //    activity.Finish();
            //    Log.Debug(logTag, logTag + " OnActivityDestroyed - only called because OnActivitySaveInstanceState called Finish() ");
            //}

            Log.Debug(logTag, logTag + " OnActivityDestroyed - only called because OnActivitySaveInstanceState called Finish() ");
        }

        public void OnActivityPaused(Activity activity) 
        {
            Log.Debug(logTag, logTag+" OnActivityPaused");
        }


        public void OnActivityResumed(Activity activity)
        {
            Log.Debug(logTag, logTag+" OnActivityResumed");
        }

        //public void OnActivityRestoreInstanceState(Activity activity, Bundle? savedInstanceState)
        //{
        //    //Log.Debug(logTag, logTag+" OnActivityRestoreInstanceState");
        //}

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            // Replaces the functionality of OnActivityDestroyed above, which only works if in Developer Options - App - Don't keep activities is in the On position, which most likely wouldn't be ON for a non developer user. 
            if (!activity.IsChangingConfigurations)
            {
                activity.Finish();
                Log.Debug(logTag, logTag + " OnActivitySaveInstanceState - Calling Activity.Finish()");
            }
        }

        public void OnActivityStarted(Activity activity) 
        {
            Log.Debug(logTag, logTag+" OnActivityStarted");
        }

        public void OnActivityStopped(Activity activity) 
        {
            Log.Debug(logTag, logTag+" OnActivtyStopped");
        }

    }
}