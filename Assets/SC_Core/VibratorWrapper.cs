
using UnityEngine;
using System.Collections.Generic;


#if !(UNITY_EDITOR) && (UNITY_ANDROID)
public static class VibratorWrapper
{
    static AndroidJavaObject vibrator = null;

    static VibratorWrapper()
    {
        var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = unityPlayerActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }
    public static void Vibrate(long time)
    {
        if (ConfDB.stat.use_vibro > 0 && time > 0)
        {
            vibrator.Call("vibrate", time);
        }
    }
}
#else
public static class VibratorWrapper
{
    public static void Vibrate(long time) { }
}
#endif