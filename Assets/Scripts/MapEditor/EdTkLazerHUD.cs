using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD. Установка ЛАЗЕР
/// </summary>
public interface ILazerHUD
{
    event System.Action OnCreate;
    event System.Action OnPlaceAPoint;
    event System.Action OnPlaceBPoint;
    event System.Action<int> OnChangeOnTime;
    event System.Action<int> OnChangeOffTime;
    void SetOnTime(int val);
    void SetOffTime(int val);
    GameObject GetGameObject();
    void DestroyIt();
}

public class EdTkLazerHUD : MonoBehaviour, ILazerHUD
{
    [SerializeField] Button btAPoint;
    [SerializeField] Button btBPoint;
    [SerializeField] Button btCreate;

    public event System.Action<int> OnChangeOnTime = delegate { };
    public event System.Action<int> OnChangeOffTime = delegate { };
    public event System.Action OnPlaceAPoint = delegate { };
    public event System.Action OnPlaceBPoint = delegate { };
    public event System.Action OnCreate = delegate { };

    public void DestroyIt()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        btAPoint.onClick.AddListener(()=> OnPlaceAPoint());
        btBPoint.onClick.AddListener(() => OnPlaceBPoint());
        btCreate.onClick.AddListener(() => OnCreate());
    }

    public void SetOnTime(int val)
    {
        //sw_on_text.text = "sw_on: " + val;
    }

    public void SetOffTime(int val)
    {
        //sw_off_text.text = "sw_off: " + val;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
