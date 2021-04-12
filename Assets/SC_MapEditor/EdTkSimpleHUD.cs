using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HUD. Установка простого объекта itemID
/// </summary>
public interface ITkSimpleHUD
{
    bool isDrawMode { get; }
    event System.Action OnChangeState;
    GameObject GetGameObject();
    
    void DestroyIt();
}


public class EdTkSimpleHUD : MonoBehaviour, ITkSimpleHUD
{
    public bool isDrawMode { get; private set; }
    public event System.Action OnChangeState = delegate { };

    public void DestroyIt()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void CMD_AppendItem(bool active)
    {
        isDrawMode = active;
        OnChangeState();
    }
    
    private void Update()
    {
#if (UNITY_EDITOR) || (UNITY_WIN)
        isDrawMode = Input.GetKey(KeyCode.W);
        if (Input.GetKeyDown(KeyCode.W))
            OnChangeState();
#endif
    }
}
