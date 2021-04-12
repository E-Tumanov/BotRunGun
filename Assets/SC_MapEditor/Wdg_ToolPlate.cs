using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IWdg_ToolPlate
{
    void SetIco(Sprite ico);
    void SelectIt();
    void UnSelectIt();
}

public class Wdg_ToolPlate : MonoBehaviour, IWdg_ToolPlate
{
    [SerializeField] Transform selectBoard;

    public void SetIco(Sprite ico)
    {
        GetComponent<Image>().sprite = ico;
    }

    public void SelectIt()
    {
        selectBoard.gameObject.SetActive(true);
    }

    public void UnSelectIt()
    {
        selectBoard.gameObject.SetActive(false);
    }
}
