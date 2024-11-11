using AndroidX.Activity.Result;
using System;

namespace com.companyname.navigationgraph9net9
{
    public class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
    {

        public EventHandler<ActivityResult>? OnActivityResultCalled;
        public EventHandler<Java.Lang.Object>? OnJavaObjectResultCalled;

        public void OnActivityResult(Java.Lang.Object? result)
        {

            if (result is ActivityResult activityResult)
                OnActivityResultCalled?.Invoke(this, activityResult);
            else
                OnJavaObjectResultCalled?.Invoke(this, result!);
        }
    }
}