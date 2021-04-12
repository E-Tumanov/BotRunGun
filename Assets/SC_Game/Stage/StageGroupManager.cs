using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Обертка вокруг GropInfo
/// Формирует и держит список групп
/// </summary>
public class StageGroupManager
{
    List<StageGroupInfo> groupList = new List<StageGroupInfo> ();
    public event System.Action<StageGroupInfo> OnGroupCollected = delegate { };
    public int GroupCount => groupList.Count;

    public int TotalCoins;

    public void GroupList (List<ItemHandler> coinList)
    {
        StageGroupInfo.acnum = 0;
        groupList.ForEach (x => x.DropHandlers());
        groupList.Clear ();

        TotalCoins = coinList.Count;

        // Сортировка
        var ls = coinList.OrderBy (x => x.pos.z * 10);
            
        foreach (var coin in ls)
        {
            bool isCaptured = false;
            foreach (var group in groupList)
            {
                if (group.TryToAppend (coin))
                {
                    isCaptured = true;
                    break;
                }
            }

            if (!isCaptured)
            {
                var newGrop = new StageGroupInfo ();
                newGrop.OnFullCollected += Gr_OnFullCollected;
                newGrop.TryToAppend (coin);
                groupList.Add (newGrop);
            }
        }
    }

    private void Gr_OnFullCollected (StageGroupInfo grp)
    {
        OnGroupCollected (grp);
    }
}