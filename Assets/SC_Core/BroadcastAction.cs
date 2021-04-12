using System.Collections.Generic;
using System;


/// <summary>
/// 
/// </summary>
public interface Command
{
    //void Exec();
}


public class EventHandler<T> where T : Command
{
    class SubsriberInfo
    {
        public UnityEngine.Object carryOject;
        public System.Action<T> action;
    }

    List<SubsriberInfo> actList = new List<SubsriberInfo>();

    public void AddListener(UnityEngine.Object linkedObject, System.Action<T> action)
    {
        actList.Add(new SubsriberInfo { carryOject = linkedObject, action = action });
    }

    public void FireEvent(T data)
    {
        actList.RemoveAll(x => !x.carryOject);
        foreach (var e in actList)
        {
            if (e.carryOject) // всё верно. при обработке могут быть Destory
                e.action?.Invoke(data);
        }
    }
}



/// <summary>
/// 
/// </summary>
public class GameCommandSystem
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