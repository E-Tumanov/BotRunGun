using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Фабрика. Инструменты создания объектов
/// </summary>
public interface IEdToolKit
{
    Transform GetVControl();
    void SetCursorPos(Vector3 pos);
    void DestroyIt();
}

public class EdTkFactory : MonoBehaviour
{
    [System.Serializable]
    public class ToolKitDescription
    {
        public string tkName;
        public GameObject prefabHUD;
        public GameObject prefabTk;
    }

    [SerializeField] List<ToolKitDescription> tkList;

    public IEdToolKit Create(string itemID, Stage stage)
    {
        var item = ConfDB.item[itemID];
        
        /*

            Тут проблема, что объекты это GameObject, т.е. тип теряется.
            Нужно думать над общим интерфейсом
            для тулкитов и оконо(HUD) чтоб их линковать


        var tk = tkList.Find(rez => rez.tkName.Equals(itemID));
        var vcontrol = GameObject.Instantiate(tk.prefabHUD);// Resources.Load<EdTkHMoveHUD>("HUD_hmove"));
        return new EdTkHMove(itemID, vcontrol, stage); // link

        пока оставлю как было
        */
        
        if (item.tkit == "hor_mover")
        {
            var vcontrol = GameObject.Instantiate(Resources.Load<EdTkHMoveHUD>("HUD_hmove"));
            return new EdTkHMove(itemID, vcontrol, stage); // link
        }
        else if (item.tkit == "simp")
        {
            var vcontrol = GameObject.Instantiate(Resources.Load<EdTkSimpleHUD>("HUD_simple"));
            return new EdTkSimple(itemID, vcontrol, stage); // link
        }
        else if (item.tkit == "lazer_2p")
        {
            var vcontrol = GameObject.Instantiate(Resources.Load<EdTkLazerHUD>("HUD_2point_lazer")); // proxi view
            var viz = GameObject.Instantiate<ItemLazer>(Resources.Load<ItemLazer>("lazer_edtool"));
            return new EdTkLazer(itemID, vcontrol, stage, viz); // link
        }
        else if (item.tkit == "walk_ball")
        {
            var vcontrol = GameObject.Instantiate(Resources.Load<EdTkWalkBallHUD>("HUD_walk_ball")); // proxi view
            var viz = GameObject.Instantiate<ItemWalkBall>(Resources.Load<ItemWalkBall>("walk_ball_editor"));
            return new EdTkWalBall(itemID, vcontrol, stage, viz); // link
        }
        throw new System.Exception("ToolKit not found. " + itemID);
    }
}