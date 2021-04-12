using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class G
{
    public static float time { get; private set; }
    public static int   tick { get; private set; }

    public static float deltaTime => GSV.DTIME;

    public static bool isTutorEnable = false;
    public static bool isEditMode = true;

    public static bool isRoundStarted;
    public static bool isPause;

    public static string stageFileName = string.Empty; // имя файла загружаемого уровня

    public static void ResetStaticData()
    {
        time = 0;
        tick = 0;
        isRoundStarted = false;
        isPause = false;
    }

    public static void ResetTime()
    {
        time = 0;
        tick = 0; 
    }

    public static void SolveTime()
    {
        time += G.deltaTime;
        tick++;
    }


    // Поставить игру на паузу
    public static void SetPause(bool pause)
    {
        isPause = pause;

        // сообщим подписчикам
        // Сейчас нет ни одного подписчика, т.к. по задумке ПАУЗЫ в игре нет
        //cx.EVENT.Send(new PauseEvent { isPause = G.isPause });
    }
}
