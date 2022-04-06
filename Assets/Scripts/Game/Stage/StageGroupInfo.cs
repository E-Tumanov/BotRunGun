using System.Collections.Generic;
using System.Linq;


/// <summary>
/// ������ ������ �������� �����
/// 
/// ������ - ����� "�����" ��� ������� �� Z
/// ���� ����� "������" �� �������� �� � ���� ������, �� ������ �����
/// ���� ������ ������ N ���������, �� ����� �� ������ �����
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
        item.OnCollected = () => // �� "+="
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
    /// false - ���� �� �������� � ��� ������ 
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

    // �������� ����� ��� ����� "�����". �� ���������� (��� ������)
    public static int acnum; 
}