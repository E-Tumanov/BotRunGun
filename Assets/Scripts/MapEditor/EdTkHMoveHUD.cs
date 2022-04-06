using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD/ Тумба с шипами.
/// </summary>
public interface IHMoveHUD
{
    event System.Action OnCreate;
    event System.Action<int> OnChangeDir;
    event System.Action<int> OnChangeSpeed;
    void SetSpeed(int speed);
    GameObject GetGameObject();
    void DestroyIt();
}

public class EdTkHMoveHUD : MonoBehaviour, IHMoveHUD
{
    [SerializeField] Button btDirLeft;
    [SerializeField] Button btDirRight;
    [SerializeField] Text textDir;

    [SerializeField] Button btSpeedInc;
    [SerializeField] Button btSpeedDec;
    [SerializeField] Text textSpeed;

    [SerializeField] Button btCreate;

    public event System.Action<int> OnChangeDir = delegate { };
    public event System.Action<int> OnChangeSpeed = delegate { };
    public event System.Action OnCreate = delegate { };

    public void DestroyIt()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        btDirLeft.onClick.AddListener(() => OnChangeDir(-1));
        btDirRight.onClick.AddListener(() => OnChangeDir(+1));
        btSpeedInc.onClick.AddListener(() => OnChangeSpeed(+1));
        btSpeedDec.onClick.AddListener(() => OnChangeSpeed(-1));
        btCreate.onClick.AddListener(() => OnCreate());
    }

    public void SetSpeed(int speed)
    {
        if (speed < 0)
            textDir.text = "left";
        else
            textDir.text = "right";
        speed = Mathf.Abs(speed);
        if (speed == 3)
            textSpeed.text = "hight";
        else if (speed == 2)
            textSpeed.text = "mid";
        else
            textSpeed.text = "low";
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
