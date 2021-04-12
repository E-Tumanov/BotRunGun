using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GSV
{
    public static int APP_VER = 540;
    public static int ACCESS_VER = 0x0;

    public static string GOOGLE_ID = "none";
    public static string GOOGLE_DISPLAY_NAME = "none";
    public static string IOS_ID = "";
    public static long USER_ID = 0;


#if (UNITY_EDITOR) || (UNITY_IPHONE)
    public static float DTIME = 0.02f;
#elif (UNITY_ANDROID)
    public static float DTIME = 0.04f;
#endif

    const string RCA = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgywSqWClKtq01O18xaL+jYRmgTrmLIxTRkVLcWLLuzKrNBafRCay/QnZuld13bUCCAyjKSBhatCUQ/EpdDy5YfI/qOr0cQmBSJKfKuMZ+neyn/79DUV9iCKMhwXZneVfyBzdGrWCkWHWXxk1rHD70NH89xLTs/uh7bWmII9VNwiDma7qGQ8j1EiG8y2odUMfVjFOJKFdS6eDuygKpC46woU+ieWGjl6Af030p7h68nf3iwUVzGYzucig8k8j5mf6Se7tXR3tM/KdW4U1ZIzIXx7QKBnxXQhJG1mBtcgJyxGpRTCyp5nWbFgpyn53oByyntLOXZ3dV7i0hYkb/HzPvwIDAQAB";
    public const string AMP_ID = "3095e123ea66ed5c5dc0d34cd342eb99";
}

