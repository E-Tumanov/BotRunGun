using System;
using System.Collections.Generic;

/*
    Можно сделать еще DELAY для запуска. Время и кадры
*/

public class MEventHandler<T>
{
    class SubsriberInfo
    {
        public UnityEngine.Object carryOject;
        public System.Action<T> action;
    }

    private List<SubsriberInfo> actList = new List<SubsriberInfo>();
    //private List<System.Action<T>> rawActionList = new List<System.Action<T>> ();

    public void AddListener(UnityEngine.Object linkedObject, System.Action<T> action)
    {
        foreach (var e in actList)
        {
            if (e.carryOject == linkedObject && e.action == action)
               return; // Уже подписан
        }

        actList.Add(new SubsriberInfo { carryOject = linkedObject, action = action });
    }

    /*
    public void AddListener (System.Action<T> action)
    {
        rawActionList.Add (action);
    }
    */

    public void RemoveListener (UnityEngine.Object linkedObject, System.Action<T> action)
    {
        foreach (var e in actList)
        {
            if (e.carryOject == linkedObject && e.action == action)
                e.carryOject = null;
        }
        //actList.RemoveAll (x => (x.carryOject == linkedObject && x.action == action));
    }

    public void RemoveListener (UnityEngine.Object linkedObject)
    {
        foreach (var e in actList)
        {
            if (e.carryOject == linkedObject)
                e.carryOject = null;
        }
        //   actList.RemoveAll (x => (x.carryOject == linkedObject));
    }

    /*
    public void RemoveListener (System.Action<T> action)
    {
        rawActionList.RemoveAll (x => x == action);
    }
    **/

    public void FireEvent(T data)
    {
        actList.RemoveAll(x => !x.carryOject);
        
        var tempListenerList = new List<SubsriberInfo> ();
        foreach (var e in actList)
        {
            var __subInfo = new SubsriberInfo ();
            __subInfo.carryOject = e.carryOject;
            var __delegate = e.action as Delegate;
            foreach (var mtd in __delegate.GetInvocationList())
                __subInfo.action += mtd as System.Action<T>;
            tempListenerList.Add (__subInfo);
        }

        foreach (var e in tempListenerList)
        {
            if (e.carryOject) // всё верно. при обработке могут быть Destory
                e.action?.Invoke (data);
        }
    }
}



/// <summary>
/// 
/// </summary>
public class Cmd
{
    List<System.Action> cmdList = new List<System.Action>();

    public void Add(System.Action cmd)
    {
        cmdList.Add(cmd);
    }

    public void RunList()
    {
        var tempCmdList = new List<System.Action>(cmdList);
        cmdList.Clear();
        tempCmdList.ForEach(x => x());
    }
}