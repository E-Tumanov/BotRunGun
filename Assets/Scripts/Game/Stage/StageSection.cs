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

        //  TODO:

        Assert.IsTrue (false);
        return null;
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


    public void InstanceAllItems ()
    {
        if (ItemFactory == null)
            return;

        if (IsLoaded)
            return;

        foreach (var e in ItemList)
        {
            InstanceItem (e);
        }
        IsLoaded = true;
    }


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