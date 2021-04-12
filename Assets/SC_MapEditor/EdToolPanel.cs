using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


interface IToolPanel
{
    event System.Action<ItemInfo> changeToolKit;
}


/// <summary>
/// Панель с инструментами (toolits)
/// </summary>
public class EdToolPanel : MonoBehaviour, IToolPanel
{
    [SerializeField] Transform toolsPanel;
    [SerializeField] Button wdgToolButtonPrefab;
    [SerializeField] SpriteAtlas tkIcoAtlasPrefab;

    public event System.Action<ItemInfo> changeToolKit = delegate { };
    event System.Action unselectAll = delegate { };

    void Start()
    {
        var it = ConfDB.item.GetEnumerator();

        Button firstTool = null;
        while (it.MoveNext())
        {
            ItemInfo tkInfo = it.Current.Value; // closure

            if (tkInfo.UNDER_CONSTRUCT == 1)
                continue;

            var tk = Instantiate<Button>(wdgToolButtonPrefab, toolsPanel);
            IWdg_ToolPlate wdg = tk.GetComponent<Wdg_ToolPlate>(); // closure
            unselectAll += wdg.UnSelectIt;
            tk.onClick.AddListener(() => __onSelectTool(wdg, tkInfo));

            //  иконка. Где искать задаётся так: имя_массива#имя_спрайта. например, ToolsIco#walk_ball
            //var ico = Load.Sprite(tkInfo.ico);
            var ico = Load.SpriteFromAtlas(tkInfo.ico, tkIcoAtlasPrefab);
            if (ico)
                wdg.SetIco(ico);

            if (firstTool == null)
            {
                firstTool = tk;
            }
        }

        //  Кликнем по первому элементу в панели инструментов
        firstTool.onClick.Invoke();
    }


    //  Обработчик клика по инструменту
    void __onSelectTool(IWdg_ToolPlate wdgPlate, ItemInfo tk)
    {
        unselectAll();
        wdgPlate.SelectIt();

        // Сообщим подписчикам, что тулкит поменялся
        changeToolKit(tk);
    }
}
