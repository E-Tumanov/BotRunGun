using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


/// <summary>
/// ТулКит для установки лазера через 2 точки
/// </summary>
public class EdTkLazer : IEdToolKit
{
    string itemID;
    ILazerHUD hud;
    ILazer viz;

    Stage stage;

    Vector3 cursorPos;
    Vector3 pointA;
    Vector3 pointB;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hud">HUD панель</param>
    /// <param name="itemID">ID предмета из конфигов</param>
    /// <param name="stage">Текущий уровень</param>
    /// <param name="viz">Префаб для визуализации расположения/поведения</param>
    public EdTkLazer(string itemID, ILazerHUD hud, Stage stage, ILazer viz)
    {
        this.itemID = itemID;
        this.stage = stage;

        this.hud = hud;
        hud.OnChangeOnTime += MControl_OnChangeOnTime;
        hud.OnChangeOffTime += MControl_OnChangeOffTime;
        hud.OnPlaceAPoint += MControl_OnPlaceAPoint;
        hud.OnPlaceBPoint += MControl_OnPlaceBPoint;
        hud.OnCreate += MControl_OnCreate;

        this.viz = viz;
        __reset();
    }

    void __reset()
    {
        viz.SetAPos(-10 * Vector3.forward);
        viz.SetBPos(-10 * Vector3.forward);
        pointA = -10 * Vector3.forward;
        pointB = -10 * Vector3.forward;
    }

    public void DestroyIt() 
    {
        viz.Destroy();
        hud.DestroyIt();
    }

    public void SetCursorPos(Vector3 pos)
    {
        cursorPos = pos;
    }

    //  вернёт HUD(набор кнопок) для текущего инструмента
    public Transform GetVControl()
    {
        return hud.GetGameObject().transform;
    }

    void MControl_OnPlaceAPoint()
    {
        pointA = cursorPos;
        viz.SetAPos(cursorPos);

        //  TODO: случай когда тумбы совпадают
        //isAPointPlaced = true;
    }

    void MControl_OnPlaceBPoint()
    {
        pointB = cursorPos;
        viz.SetBPos(cursorPos);
        
        //MControl_OnCreate();
    }

    void MControl_OnChangeOffTime(int val)
    {
    }

    void MControl_OnChangeOnTime(int val)
    {
    }

    void MControl_OnCreate()
    {
        if (pointA.z < 1 || pointB.z < 1) // заставит нормаьно разместить тумбы
            return;

        var data = JSONNode.Parse("[]");

        if (pointA.z < pointB.z)
        {
            data[0].AsFloat = pointA.x;
            data[1].AsFloat = pointA.z;
            data[2].AsFloat = pointB.x - pointA.x;
            data[3].AsFloat = pointB.z - pointA.z;
        }
        else
        {
            data[0].AsFloat = pointB.x;
            data[1].AsFloat = pointB.z;
            data[2].AsFloat = pointA.x - pointB.x;
            data[3].AsFloat = pointA.z - pointB.z;
        }
        stage.AddItem(itemID, data);
    }
}
