using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Держит группу объектов монет
/// 
/// Группа - набор "монет" без разрыва по Z
/// Если новая "монета" не попадает ни в одну группу, то создаём новую
/// Если группа меньше N элементов, то можно не давать бонус
/// </summary>
public class StageGroupInfo
{
    public int MaxCount;
    public int CurrCount;
    public int GroupNum;
    public event System.Action<StageGroupInfo> OnFullCollected;

    List<ItemHandler> coinList = new List<ItemHandler> ();

    public StageGroupInfo ()
    {
        GroupNum = acnum++;
    }

    public void DropHandlers ()
    {
        OnFullCollected = null;
    }

    private void Add (ItemHandler item)
    {
        MaxCount++;
        CurrCount++;
        item.group = this;
        item.OnCollected = () => // не "+="
        {
            CurrCount--;
            if (CurrCount == 0)
            {
                CurrCount = MaxCount;
                OnFullCollected?.Invoke (this);
            }
        };
        coinList.Add (item);
    }

    /// <summary>
    /// false - если не подходит в эту группу 
    /// </summary>
    public bool TryToAppend (ItemHandler item)
    {
        if (coinList.Count == 0)
        {
            Add (item);
            return true;
        }
        
        bool isAppend = false;
        foreach (var e in coinList)
        {
            if ((e.pos - item.pos).magnitude < 1.5f)
            {
                isAppend = true;
                break;
            }
        }

        if (isAppend)
        {
            Add (item);
        }

        return isAppend;
    }

    // сквозной номер для новых "ГРУПП". Не обнуляется (нет смысла)
    public static int acnum; 
}