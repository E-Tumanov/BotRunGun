using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


/// <summary>
/// Тулкит. Установка простого объекта itemID
/// </summary>
public class EdTkSimple : IEdToolKit
{
    ITkSimpleHUD vcontrol;
    Stage stage;
    Vector3 cursorPos;
    string itemID;

    public EdTkSimple(string itemID, ITkSimpleHUD vcontrol, Stage stage)
    {
        this.vcontrol = vcontrol;
        this.itemID = itemID;
        this.stage = stage;
        
        vcontrol.OnChangeState += Action;
    }

    public void DestroyIt() 
    {
        vcontrol.DestroyIt();
    }

    public void SetCursorPos(Vector3 pos)
    {
        cursorPos = pos;
        Action();
    }

    public Transform GetVControl()
    {
        return vcontrol.GetGameObject().transform;
    }

    private void Action()
    {
        if (vcontrol == null || stage == null)
            return;

        if (vcontrol.isDrawMode)
        {
            if (!stage.IsPlaceBusy(cursorPos))
            {
                var data = JSONNode.Parse("[]");
                data[0].AsFloat = cursorPos.x;
                data[1].AsFloat = cursorPos.z;
                stage.AddItem(itemID, data);               
            }
        }
    }
}
