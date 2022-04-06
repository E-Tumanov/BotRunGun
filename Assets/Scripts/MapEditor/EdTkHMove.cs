using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

/// <summary>
/// #toolkit #editor #model
/// Тулкит. Создать препятствие. Горизонтальное движение. От стены к стене
/// </summary>
public class EdTkHMove : IEdToolKit
{
    // вот это, конечно, не верно. Нужно за интерфейс его держать
    // Вот как только модель перестанет контрол создавать, тогда жизнь наладится.
    // + т.е. сейчас связка модели и контрола(кнопок) зашита здесь(в модели). под вопросом?
    
        //  + Всё! бл. ПОФИКСИЛ

    IHMoveHUD hud;
    Stage stage;
    Vector3 cursorPos;
    string itemID;
    float exportSpeed;

    public EdTkHMove(string itemID, IHMoveHUD hud, Stage stage)
    {
        exportSpeed = -1;

        this.itemID = itemID;
        this.stage = stage;

        this.hud = hud;
        hud.OnChangeDir += MControl_OnChangeDir;
        hud.OnChangeSpeed += MControl_OnChangeSpeed;
        hud.OnCreate += Action;
    }
    
    public void DestroyIt() 
    {
        hud.DestroyIt();
    }

    public void SetCursorPos(Vector3 pos)
    {
        cursorPos = pos;
    }

    //  создаст набор кнопок для текущего TK (полиморф)
    public Transform GetVControl()
    {
        return hud.GetGameObject().transform;
    }

    private void MControl_OnChangeSpeed(int v)
    {
        var saveSign = Mathf.Sign(exportSpeed);
        exportSpeed = Mathf.Abs(exportSpeed) + v;
        exportSpeed = saveSign * Mathf.Clamp(exportSpeed, 1, 3);
        hud.SetSpeed((int)exportSpeed);
    }

    private void MControl_OnChangeDir(int v)
    {
        exportSpeed = (int)(Mathf.Sign(v) * Mathf.Abs(exportSpeed));
        hud.SetSpeed((int)exportSpeed);
    }

    private void Action()
    {
        if (hud == null || stage == null)
            return;

        if (!stage.IsPlaceBusy(cursorPos))
        {
            var data = JSONNode.Parse("[]");
            data[0].AsFloat = cursorPos.x;
            data[1].AsFloat = cursorPos.z;
            data[2].AsFloat = exportSpeed;
            stage.AddItem(itemID, data);
        }
    }
}
