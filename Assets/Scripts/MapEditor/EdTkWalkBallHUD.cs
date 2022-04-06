using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD. "Блуждающий шар"
/// 
/// </summary>
public interface IWalkBallHUD
{
    event System.Action OnCreate;
    event System.Action OnPlaceAPoint;
    event System.Action OnPlaceBPoint;
    event System.Action OnSpeedDec;
    event System.Action OnSpeedInc;
    void SetSpeed(int speed);
    GameObject GetGameObject();
    void DestroyIt();
}

public class EdTkWalkBallHUD : MonoBehaviour, IWalkBallHUD
{
    [SerializeField] Button btSpeedDec;
    [SerializeField] Button btSpeedInc;
    [SerializeField] Text speedText;

    [SerializeField] Button btAPoint;
    [SerializeField] Button btBPoint;
    [SerializeField] Button btCreate;

    public event System.Action OnSpeedDec = delegate { };
    public event System.Action OnSpeedInc = delegate { };
    public event System.Action OnPlaceAPoint = delegate { };
    public event System.Action OnPlaceBPoint = delegate { };
    public event System.Action OnCreate = delegate { };

    void Start()
    {
        btSpeedDec.onClick.AddListener(() => OnSpeedDec());
        btSpeedInc.onClick.AddListener(() => OnSpeedInc());

        btAPoint.onClick.AddListener(() => OnPlaceAPoint());
        btBPoint.onClick.AddListener(() => OnPlaceBPoint());
        btCreate.onClick.AddListener(() => OnCreate());
    }
    
    public void SetSpeed(int speed)
    {
        switch (speed )
        {
            case 1: speedText.text = "low";  break;
            case 2: speedText.text = "mid"; break;
            case 3: speedText.text = "hi"; break;
        }
    }

    public void DestroyIt()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
