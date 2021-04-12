using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class AMPLITUDE
{
#if (USE_AMPLITUDE)
    static bool used = true;
#else
    static bool used = false;
#endif

    public static void sync_start()
    {
        if (!used || G.isEditMode)
            return;

        Amplitude.Instance.logEvent("sync_start");
    }

    public static void game_start()
    {
        if (!used || G.isEditMode)
            return;

        Amplitude.Instance.logEvent("game_start");
    }

    public static void level_start(int stageNum)
    {
        if (!used || G.isEditMode)
            return;

        Dictionary<string, object> data = new Dictionary<string, object>() {
            {"num" , stageNum.ToString() }
        };
        Amplitude.Instance.logEvent("level_start", data);
    }

    public static void level_finished(int stageNum, int percentCoinGet)
    {
        if (!used || G.isEditMode)
            return;

        Dictionary<string, object> data = new Dictionary<string, object>() {
            {"lnum", stageNum.ToString() },
            {"pval", percentCoinGet.ToString() }
        };
        Amplitude.Instance.logEvent("level_finish", data);
    }

    public static void level_end(int stageNum, bool isWin)
    {
        if (!used || G.isEditMode)
            return;

        Dictionary<string, object> data = new Dictionary<string, object>() {
            {"num", stageNum.ToString() }
        };

        if (isWin)
            Amplitude.Instance.logEvent("level_win", data);
        else
            Amplitude.Instance.logEvent("level_fail", data);
    }

    public static void main_menu()
    {
        Amplitude.Instance.logEvent("main_menu");
    }
}