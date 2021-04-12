using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


/// <summary>
/// Стадия. Stage
/// В обоих случаях.Редактор и Рантайм
/// </summary>
public class Stage
{
    // ===================== Serialized fields =====================
    public int ver;
    public int sid;

    public int coinPreReward; // халявные монеты
    public int bulletPreReward; // халявные ящики с патронами

    public BossInfo bossInfo;
    public StageColorInfo colorInfo;
    public List<StageMsgInfo> msgList = new List<StageMsgInfo> ();

    public string stageFileName; // имя файла
    public Vector3 cursorPos;

    //===========================================================
#if (UNITY_EDITOR)
    public const int CHANK_SECTION_LEN = 60; // Размер чанка для предметов
#else
    public const int CHANK_SECTION_LEN = 20; // Размер чанка для предметов/ Дальность видимости
#endif
    const int MAX_SECTION = 1024;

    //  Список инстанцированных объектов
    public List<ItemHandler> instancedItemList { get; private set; } = new List<ItemHandler> ();

    public float finishDist { get => maxItemZ + 20; }

    [System.NonSerialized]
    public float LeftBorder = -3.0f; // ширина трассы

    [System.NonSerialized]
    public float RightBorder = 3.0f;

    int maxItemZ = 0;
    int maxSectionNum = 0;
    int lastSnapZ = -1000; // смена обзора

    List<ItemHandler> coinList = new List<ItemHandler> ();

    StageSection[] sectionList = new StageSection[MAX_SECTION];
    static StageSection clipboardSection;

    IItemFactory _itemFactory;
    public IItemFactory itemFactory
    {
        get => _itemFactory;
        set
        {
            _itemFactory = value;
            for (int i = 0; i < MAX_SECTION; i++)
                sectionList[i].ItemFactory = value;
        }
    }

    public event System.Action<ItemHandler> OnItemAdd = delegate { };

    public Stage ()
    {
        for (int i = 0; i < MAX_SECTION; i++)
            sectionList[i] = new StageSection (i);
    }
    

    // Сюда передают JSON объект. Так будто это загрузка из файла.
    // Т.е. редактор(тулкит объекта) должен сам уметь создавать JSON для ,например, лазеров и пр.
    public void AddItem (string itemID, JSONNode data)
    {
        var itemHdr = ItemHandler.Build (itemID, data);

        // Если в разработке, то не ставим
        if (itemHdr.cfg.UNDER_CONSTRUCT == 1)
            return;

        var sectionNum = Mathf.FloorToInt (itemHdr.pos.z / CHANK_SECTION_LEN);
        if (sectionNum >= 1024)
            return;

        // Обновить количество секций. Где будет конец раунда
        maxSectionNum = Mathf.Max (maxSectionNum, sectionNum);

        // Координата самого удалённого объекта
        maxItemZ = (int) Mathf.Max (itemHdr.pos.z, maxItemZ);

        sectionList[sectionNum].AppendItem (itemHdr);

        OnItemAdd (itemHdr);
    }

    /// <summary>
    /// Это билдит группы. В бою группы билдятся один раз(!) при старте 
    /// )) хихи даже формат не пришлось менять
    /// </summary>
    public List<ItemHandler> RebuildCoinList ()
    {
        coinList.Clear ();
        for (int i = 0; i < MAX_SECTION; i++)
        {
            foreach (var e in sectionList[i].ItemList)
            {
                if (e.itemType == MAP_ITEM_TYPE.COIN)
                {
                    coinList.Add (e);
                }
            }
        }
        return coinList;
        //  Передать манагеру список объектов. Сбилдить группы.
        //GroupManager.GroupList (coinList);
    }



    /// <summary>
    /// 
    /// </summary>
    public void SetViewPoint (Vector3 viewPos)
    {
        cursorPos = viewPos;

        //  snap on 20 meters
        // НАдо убрать отсюда.
        //  Сделать внешний источник вызова
        if (Mathf.Abs (viewPos.z - lastSnapZ) < 20)
            return;

        lastSnapZ = (int)cursorPos.z;
        int num = lastSnapZ / CHANK_SECTION_LEN;

        instancedItemList.Clear ();

        // Расчёт видимых секций.
        int mulByEditor = G.isRoundStarted ? 1 : 100;
        for (int i = 0; i < MAX_SECTION; i++)
        {
            var section = sectionList[i];
            if (i > num - 2 * mulByEditor && i < num + 4 * mulByEditor)
            {
                section.InstanceAllItems ();

                //  список объектов трассы
                foreach (var e in section.ItemList)
                {
                    //if (e.go.IsUpdated())
                    instancedItemList.Add (e);
                }
            }
            else
            {
                section.DestroyAllItems ();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void CopySection (Vector3 pos)
    {
        int sectionNum = Mathf.FloorToInt (pos.z / CHANK_SECTION_LEN);
        if (sectionNum >= 1024)
            return;
        clipboardSection = sectionList[sectionNum].Clone ();
    }


    /// <summary>
    /// 
    /// </summary>
    public void PasteSection (Vector3 pos)
    {
        if (clipboardSection == null)
            return;

        int sectionNum = Mathf.FloorToInt (pos.z / CHANK_SECTION_LEN);
        if (sectionNum >= 1024)
            return;

        foreach (var e in clipboardSection.ItemList)
        {
            var initData = JSON.Parse (e.initData.ToString ());
            initData[1].AsFloat += pos.z;

            if (!IsPlaceBusy (new Vector3 (initData[0].AsFloat, 0, initData[1].AsFloat)))
                AddItem (e.cfg.id, initData);
        }
    }


    public void ClearSection (Vector3 pos)
    {
    }


    public bool IsPlaceBusy (Vector3 pos)
    {
        if (pos.z < 0)
        {
            Debug.LogWarning ("IsPlaceBusy> " + pos.z);
            return true;
        }
        int sectionNum = Mathf.FloorToInt (pos.z / CHANK_SECTION_LEN);
        if (sectionNum >= MAX_SECTION)
            return true;
        return sectionList[sectionNum].IsPlaceBusy (pos);
    }


    // Удалить объект в позиции
    public void DeleteItemOnPos (Vector3 pos, bool isCollected)
    {
        int sectionNum = Mathf.FloorToInt (pos.z / CHANK_SECTION_LEN);
        if (sectionNum >= MAX_SECTION)
            return;
        if (sectionList[sectionNum] == null)
            return;
        
        sectionList[sectionNum].RemoveAndDestroyItem (pos, isCollected); 
    }


    /// <summary>
    /// формировать JSON для экспорта/сохранения
    /// </summary>
    /// <returns>JSON представление уровня</returns>
    public JSONNode Export ()
    {
        JSONNode data = JSONNode.Parse (JsonUtility.ToJson (this, false));
        JSONNode objects = JSON.Parse ("{}");
        data.Add ("objects", objects);

        foreach (var e in sectionList)
        {
            foreach (var item in e.ItemList)
            {
                var itemID = item.cfg.id;
                // если это первое вхождение такого типа
                if (objects[itemID] == null)
                    objects.Add (itemID, JSON.Parse ("[]"));
                objects[itemID].Add (item.initData);
            }
        }
        return data;
    }


    /// <summary>
    /// Посчитать количество объектов определённного типа 
    /// </summary>
    /// <param name="item_type">Тип объекта для подсчёта</param>
    /// <returns>количество</returns>
    public int CountItemByType (string item_type)
    {
        int res = 0;
        foreach (var e in sectionList)
        {
            foreach (var item in e.ItemList)
            {
                if (item.cfg.item_type == item_type)
                    res++;
            }
        }
        return res;
    }
}



