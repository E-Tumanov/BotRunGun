using UnityEngine;
using System.Threading.Tasks;


/// <summary>
/// 
/// </summary>
public static class MTask
{
    /// <summary>
    /// Повторять петлю. Линк к объекту. 
    /// 
    /// LoopCount. 0, -1 = бесконечно
    /// OnPeriodComplete
    /// </summary>
    public static async void Loop (
        UnityEngine.Object linkedObject,
        float periodTime,
        int periodCount,
        System.Func<bool> onPerionDoneAction,
        System.Action<float> actionDuringLoop = null,
        System.Action<bool> onFinalAction = null)
    {
        if (periodTime == 0)
        {
            Debug.LogWarning ("MTaskRun.Loop> elapsedTime == 0");
            return;
        }

        if (periodCount == 0)
        {
            Debug.LogWarning ("MTaskRun.Loop> periodCount == 0");
            return;
        }

        var etm = Time.unscaledTime + periodTime;
        float currValue = 0;
        bool continueLoopResult = true;

        while (linkedObject && (periodCount-- != 0) && continueLoopResult)
        {
            while (etm > Time.unscaledTime && continueLoopResult)
            {

                try
                {
                    if (linkedObject && actionDuringLoop != null)
                    {
                        actionDuringLoop (Mathf.Clamp01 (currValue / periodTime));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError ("MTaskRun.Loop> " + e.ToString ());
                    return;
                }

                currValue += Time.unscaledDeltaTime;
                await Task.Yield ();
            }

            // reset
            etm = Time.unscaledTime + periodTime;
            currValue = 0;

            if (onPerionDoneAction != null)
                onPerionDoneAction ();

            await Task.Yield ();

            /*
            else if (continueLoopResult)
                await Task.Yield ();
            */
        }
    }


    /// <summary>
    /// Выполнить с задержкой. Линк к объекту. 
    /// </summary>
    public static async void Run (
        UnityEngine.Object linkedObject,
        float delayTime,
        System.Action action = null)
    {
        var dtm = Time.unscaledTime + delayTime;
        while (dtm > Time.unscaledTime) 
            await Task.Yield ();

        try
        {
            if (linkedObject)
            {
                action?.Invoke ();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError ("MTaskRun> " + e.ToString ());
            return;
        }
    }

    /// <summary>
    /// Выполнить с задержкой. Без линковки к объекту. 
    /// </summary>
    public static async void Run (
        float delayTime,
        System.Action action = null)
    {
        var dtm = Time.unscaledTime + delayTime;
        while (dtm > Time.unscaledTime)
            await Task.Yield ();


        try
        {
            action?.Invoke ();
        }
        catch (System.Exception e)
        {
            Debug.LogError ($"MTaskRun> {e.ToString ()} tm: {delayTime}");
            return;
        }
    }


    /// <summary>
    /// Выполнять в течении времени, начиная с задержкой. Линк к объекту.
    /// action<Timer01:float> - часто нужно нормализованное время в обработчик
    /// </summary>
    public static async void Run (
        UnityEngine.Object linkedObject,
        float delayTime,
        float elapsedTime,
        System.Action<float> action,
        System.Action onComplete = null)
    {
        var dtm = Time.unscaledTime + delayTime;
        while (dtm > Time.unscaledTime)
            await Task.Yield ();

        var etm = Time.unscaledTime + elapsedTime;
        float currValue = 0;
        if (elapsedTime == 0)
        {
            currValue = 1;
            elapsedTime = 1;
            etm = Time.unscaledTime + 1;
        }

        while (etm > Time.unscaledTime)
        {
            try
            {
                if (linkedObject)
                {
                    action?.Invoke (Mathf.Clamp01 (currValue / elapsedTime));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError ("MTaskRun> " + e.ToString ());
                return;
            }
            currValue += Time.unscaledDeltaTime;
            await Task.Yield ();
        }

        try
        {
            if (linkedObject)
            {
                onComplete?.Invoke ();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError ("MTaskRun> " + e.ToString ());
            return;
        }
    }

    /// <summary>
    /// Выполнять в течении времени, начиная с задержкой. Без линковки к объекту.
    /// action<Timer01:float> - часто нужно нормализованное время в обработчик
    /// </summary>
    public static async void Run (
        float delayTime,
        float elapsedTime,
        System.Action<float> action,
        System.Action onComplete = null)
    {
        var dtm = Time.unscaledTime + delayTime;
        while (dtm > Time.unscaledTime)
            await Task.Yield ();

        var etm = Time.unscaledTime + elapsedTime;
        float currValue = 0;
        if (elapsedTime == 0)
        {
            currValue = 1;
            elapsedTime = 1;
            etm = Time.unscaledTime + 1;
        }

        while (etm > Time.unscaledTime)
        {
            try
            {
                action?.Invoke (Mathf.Clamp01 (currValue / elapsedTime));
            }
            catch (System.Exception e)
            {
                Debug.LogError ("MTaskRun> " + e.ToString ());
                return;
            }
            currValue += Time.unscaledDeltaTime;
            await Task.Yield ();
        }

        /*
        { // Всегда есть строго 1 или нет???????

            try
            {
                action?.Invoke(1);
            }
            catch (System.Exception e)
            {
                Debug.LogError("MTaskRun.Run> " + e.ToString());
                return;
            }
        }
        */

        try
        {
            onComplete?.Invoke ();
        }
        catch (System.Exception e)
        {
            Debug.LogError ("MTaskRun> " + e.ToString ());
            return;
        }
    }
}