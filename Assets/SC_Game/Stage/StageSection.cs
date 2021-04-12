using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Dynamic;
using System.Threading.Tasks;
using UnityEngine.Assertions;


/// <summary>
/// Вся трасса поделена на "секции"
/// Секция - набор объектов.
/// </summary>
class StageSection
{
    public List<ItemHandler> ItemList = new List<ItemHandler> ();
    
    public bool IsLoaded { get; set; }
    public IItemFactory ItemFactory { get; set; }
    
    int sectionNum;

    public StageSection (int num)
    {
        sectionNum = num;
    }

    /// <summary>
    /// 
    /// </summary>
    public StageSection Clone ()
    {
        Assert.IsTrue (false);
        return null;
        // Добавилась группировка. В режиме редактора. Надо просто всё снести нахер
        // и заново поставить. Актуализация данных - трудно. Нужно просто пересчитывать
        /*
        var rs = new RaceSection (sectionNum);
        rs.IsLoaded = false;
        rs.ItemFactory = ItemFactory;

        foreach (var e in ItemList)
        {
            var data = JSON.Parse (e.initData.ToString ());

            //  Осторожно. Если объект содержит более одной координаты, то для
            //  поддержки простоты копирования, все остальные коорды должны быть в локальном пространстве объекта.
            //  Например. Лазер содержит 2 тумбы, пивот всегда лежит в первых двух координатах, а положение
            //  второй тумбы, это уже смещение относительно пивота.
            // + причём, сам объкт должен следить, чтоб пивот был минимальной координатой. Хотя - херня
            // ибо, подумай про карусель. У неё минимального пивота быть не может. Просто центр. 
            // + Объекты должны BoundSphere проверять

            data[1].AsInt -= sectionNum * Stage.SECTION_LEN;

            ItemHandler item = new ItemHandler ();
            item.cfg = e.cfg;
            item.initData = data;
            item.pos = new Vector3 (data[0].AsFloat, 0, data[1].AsFloat);
            rs.AppendItem (e);
        }
        return rs;*/
    }


    /// <summary>
    /// Добавить объект в секцию. И инст если видна
    /// </summary>
    public void AppendItem (ItemHandler item)
    {
        
        ItemList.Add (item);
        if (IsLoaded)
        {
            
            InstanceItem (item);
        }
    }


    /// <summary>
    /// Проверить "место занято"
    /// </summary>
    public bool IsPlaceBusy (Vector3 pos)
    {
        foreach (var item in ItemList)
        {
            if (item.go != null)
            {
                if ((item.go.pos - pos).magnitude < 0.2f)
                    return true;
            }
        }
        return false;
    }


    //  FIXIT^ фабрику юнити объектов нужно передавать (DI)
    //  тогда не будет завязки на юнити сцену
    // + Так и сделал. DONE!
    private void InstanceItem (ItemHandler info)
    {
        Assert.IsNotNull (ItemFactory);

        var item = ItemFactory.Create (info.cfg);
        item.Init (info.initData);
        info.go = item;

        info.group = info.group;
    }



    /// <summary>
    /// Удалить из секции и убрать со сцены(если инст)
    /// </summary>
    public void RemoveAndDestroyItem (Vector3 pos, bool isCollected)
    {
        List<ItemHandler> del = new List<ItemHandler> ();
        ItemList.RemoveAll (item =>
        {
            if (item.go == null)
                return false;

            bool res = (item.go.pos - pos).magnitude < 0.1f;
            if (res)
                del.Add (item);
            return res;
        });

        foreach (var e in del)
        {
            if (isCollected)
                e?.OnCollected ();

            e.go.RemoveFromScene (isCollected);
            e.go = null;
        }
    }


    /// <summary>
    /// async, проблема. мне нужен список сразу
    /// или тогда проектировать асинхронно
    /// </summary>
    public void InstanceAllItems ()
    {
        if (ItemFactory == null)
            return;

        if (IsLoaded)
            return;

        foreach (var e in ItemList)
        {
            InstanceItem (e);
            //await Task.Yield();
        }
        IsLoaded = true;
    }


    /// <summary>
    /// 
    /// </summary>
    public void DestroyAllItems ()
    {
        if (IsLoaded)
        {
            foreach (var e in ItemList)
            {
                if (e.go != null)
                    e.go.RemoveFromScene (false); // FIXIT: unity link!
                e.go = null;
            }
            IsLoaded = false;
        }
    }

}