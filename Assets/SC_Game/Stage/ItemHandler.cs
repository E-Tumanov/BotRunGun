using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


/// <summary>
/// Объект трассы.
/// Препятствия, монеты, алмазы и т.д.
/// </summary>
public class ItemHandler
{
    public MAP_ITEM_TYPE itemType { get; set; }
    public Vector3 pos;

    // конфиг объекта. ID идет в экспорт
    public ItemInfo cfg;

    // параметры создания объекта. JSON из ItemInfo. Используем при импорте
    public JSONNode initData;

    // если есть в сцене, то ссылка.
    public IMapItem go;

    StageGroupInfo _group;
    public StageGroupInfo group
    {
        get => _group;
        set
        {
            _group = value;
            if (go != null && value != null)
            {
                go.SetGroupNumber (_group.GroupNum);
            }
        }
    }
    public System.Action OnCollected = delegate { };

    public static ItemHandler Build (string itemID, JSONNode data)
    {
        // У любого объекта всегда есть PIVOT. Его координаты - это 1-ое и 2-ое число в массиве.
        // Доп. параметры конструктора будут третьим объектом
        // т.е. {objects:{ lazer:[ [0,0,{tpos:[0,2]}]  ]}} - лазер с пивотом(первой точкой) [0,0] и второй точкой [0,2]
        // т.е. {objects:{ mover:[ [0,0,{amp:4,start_dir:-1,speed:2}]  ]}} - мувер. пивот 0,0. амплитуда:4, начальное направление:"лево", скорость:2
        var pos = new Vector3 (data[0].AsFloat, 0, data[1].AsFloat);

        var itemHdr = new ItemHandler ();
        itemHdr.cfg = ConfDB.item[itemID];
        itemHdr.initData = data;
        itemHdr.pos = new Vector3 (data[0].AsFloat, 0, data[1].AsFloat);
        itemHdr.itemType = (MAP_ITEM_TYPE)System.Enum.Parse (typeof (MAP_ITEM_TYPE), itemHdr.cfg.item_type);

        return itemHdr;
    }
}