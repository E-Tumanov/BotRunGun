using UnityEngine;
using System.Threading.Tasks;

/*
=====================================================
Copyright 2020 Evgeny Tumanov

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

tumanfunc@gmail.com
2020 Moscow 
=====================================================
*/

/// <summary>
/// Тривиальный менеджер задачек
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
        System.Func<float, bool> action = null,
        System.Func<bool> onFinalAction = null)
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

        var etm = Time.time + periodTime;
        float currValue = 0;
        bool continueLoopResult = true;

        while (linkedObject && (periodCount-- != 0) && continueLoopResult)
        {
            while (etm > Time.time && continueLoopResult)
            {
                /*
                Пока убрал цикл-блок. Сейчас только ТИК
                try
                {
                    if (linkedObject && action != null)
                    {
                        continueLoopResult = action (Mathf.Clamp01 (currValue / periodTime));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError ("MTaskRun.Loop> " + e.ToString ());
                    return;
                }
                */
                currValue += Time.deltaTime;
                await Task.Yield ();
            }


            // reset
            etm = Time.time + periodTime;
            currValue = 0;

            if (!onPerionDoneAction ())
                return;

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
        var dtm = Time.time + delayTime;
        while (dtm > Time.time) 
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
            Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
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
        var dtm = Time.time + delayTime;
        while (dtm > Time.time)
            await Task.Yield ();

        try
        {
            action?.Invoke ();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
            return;
        }
    }


    /// <summary>
    /// Выполнять в течении времени, начиная с задержкой. Линк к объекту.
    /// </summary>
    public static async void Run (
        UnityEngine.Object linkedObject,
        float delayTime,
        float elapsedTime,
        System.Action<float> action,
        System.Action onComplete = null)
    {
        var dtm = Time.time + delayTime;
        while (dtm > Time.time)
            await Task.Yield ();

        var etm = Time.time + elapsedTime;
        float currValue = 0;
        if (elapsedTime == 0)
        {
            currValue = 1;
            elapsedTime = 1;
            etm = Time.time + 1;
        }

        while (etm > Time.time)
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
                Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
                return;
            }
            currValue += Time.deltaTime;
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
            Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
            return;
        }
    }

    /// <summary>
    /// Выполнять в течении времени, начиная с задержкой. Без линковки к объекту.
    /// </summary>
    public static async void Run (
        float delayTime,
        float elapsedTime,
        System.Action<float> action,
        System.Action onComplete = null)
    {
        var dtm = Time.time + delayTime;
        while (dtm > Time.time)
            await Task.Yield ();

        var etm = Time.time + elapsedTime;
        float currValue = 0;
        if (elapsedTime == 0)
        {
            currValue = 1;
            elapsedTime = 1;
            etm = Time.time + 1;
        }

        while (etm > Time.time)
        {
            try
            {
                action?.Invoke (Mathf.Clamp01 (currValue / elapsedTime));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
                return;
            }
            currValue += Time.deltaTime;
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
            Debug.LogWarning ("MTaskRun.Run> " + e.ToString ());
            return;
        }
    }
}