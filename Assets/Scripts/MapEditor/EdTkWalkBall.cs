using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

/// <summary>
/// ТулКит для установки лазера через 2 точки
/// 
/// </summary>
public class EdTkWalBall : IEdToolKit
{
    IWalkBallHUD hud;
    IWalkBall viz;

    string itemID;

    int ballSpeed = 1;
    Vector3 cursorPos;
    Vector3 pointA;
    Vector3 pointB;

    Stage stage;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="vcontrol">HUD панель</param>
    /// <param name="itemID">ID предмета из конфигов</param>
    /// <param name="stage">Текущий уровень</param>
    /// <param name="viz">Префаб для визуализации расположения/поведения</param>
    public EdTkWalBall(string itemID, IWalkBallHUD hud, Stage stage, IWalkBall viz)
    {
        this.itemID = itemID;
        this.stage = stage;

        this.hud = hud;
        hud.OnSpeedDec += Vcontrol_OnSpeedDec;
        hud.OnSpeedInc += Vcontrol_OnSpeedInc;
        hud.OnPlaceAPoint += MControl_OnPlaceAPoint;
        hud.OnPlaceBPoint += MControl_OnPlaceBPoint;
        hud.OnCreate += MControl_OnCreate;

        this.viz = viz;
        Reset();
    }

    void Reset()
    {
        viz.SetAPos(-10 * Vector3.forward);
        viz.SetBPos(-10 * Vector3.forward);
        pointA = -10 * Vector3.forward;
        pointB = -10 * Vector3.forward;
    }

    private void Vcontrol_OnSpeedDec()
    {
        ballSpeed = Mathf.Clamp(ballSpeed - 1, 1, 3);
        hud.SetSpeed(ballSpeed);
    }

    private void Vcontrol_OnSpeedInc()
    {
        ballSpeed = Mathf.Clamp(ballSpeed + 1, 1, 3);
        hud.SetSpeed(ballSpeed);
    }

    public void DestroyIt()
    {
        viz.DestroyIt();
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
    }

    void MControl_OnCreate()
    {
        if (pointA.z < 1 || pointB.z < 1)
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
        
        data[4].AsFloat = ballSpeed;

        stage.AddItem(itemID, data);
    }
}
