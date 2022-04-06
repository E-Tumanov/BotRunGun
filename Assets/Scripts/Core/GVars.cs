using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class G
{
    public static float time => Time.unscaledTime;
    public static int tick => Time.frameCount;
    public static float deltaTime => Time.unscaledDeltaTime;

    public static bool isTutorEnable = false;
    public static bool isEditMode = true;

    public static bool isRoundStarted;
    public static bool isPause;

    public static string stageFileName = string.Empty;

    public static void ResetStaticData()
    {
        isRoundStarted = false;
        isPause = false;
    }

    public static void SetPause(bool pause)
    {
        isPause = pause;
    }
}
