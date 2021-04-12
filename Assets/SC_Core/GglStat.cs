using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if (FALSE)
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public static class GglStat
{
    //static Firebase.FirebaseApp app;

    public static bool initDone;

    public static void Init(System.Action onDone)
    {
        initDone = false;
        
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    //app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.

                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }

                initDone = true;
                onDone();
                //DOTween.Sequence().AppendInterval(0.01f).AppendCallback(() => onDone?.Invoke());
            });
        }
        catch 
        {
            onDone();
            //DOTween.Sequence().AppendInterval(0.01f).AppendCallback(() => onDone?.Invoke());
        }
    }

    static void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //Debug.Log("Set user properties.");
        // Set the user's sign up method.
        /*
        FirebaseAnalytics.SetUserProperty(
          FirebaseAnalytics.UserPropertySignUpMethod,
          "Google");
        */

        // Set the user ID.
        FirebaseAnalytics.SetUserId(GS.USER_ID.ToString());

        // Set default session duration values.
        //FirebaseAnalytics.SetMinimumSessionDuration(new System.TimeSpan(0, 0, 10));
        FirebaseAnalytics.SetSessionTimeoutDuration(new System.TimeSpan(0, 30, 0));

        Debug.Log("GglStat> initialize done");
    }

    public static void EvConversionLevelDone()
    {
        if (!initDone)
            return;
        FirebaseAnalytics.LogEvent("LevelComplete");
    }

    public static void EvLevelDone(int levelNum, bool isDone, bool killed)
    {
        if (!initDone)
            return;

        string v = "done";
        if (!isDone)
            v = "fail";
        if (killed)
            v = "killed";

        // Log an event with multiple parameters.
        Debug.Log("Logging a Level Done event.");
        FirebaseAnalytics.LogEvent(
          FirebaseAnalytics.EventLevelEnd,
          new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum),
          new Parameter(FirebaseAnalytics.ParameterSuccess, v));
    }

    public static void EvLevelUp()
    {
        if (!initDone)
            return;

        // Log an event with multiple parameters.
        Debug.Log("Logging a level up event.");
        FirebaseAnalytics.LogEvent(
          FirebaseAnalytics.EventLevelUp,
          new Parameter(FirebaseAnalytics.ParameterLevel, 5),
          new Parameter(FirebaseAnalytics.ParameterCharacter, "mrspoon"),
          new Parameter("hit_accuracy", 3.14f));
    }

    public static void EvLogin()
    {
        if (!initDone)
            return;

        // Log an event with no parameters.
        Debug.Log("Logging a login event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }
}

#else

public static class GglStat
{
    public static void Init(System.Action onDone)
    {
        onDone?.Invoke();
    }
    
    public static void EvConversionLevelDone()
    {
    }

    public static void EvLevelDone(int levelNum, bool isDone, bool killed)
    {
    }
    public static void EvLevelUp()
    {
    }

    public static void EvLogin()
    { }
}

#endif

