using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using RBGame;

public enum MAP_ITEM_TYPE
{
    JEWEL = 1,
    BATA = 30,
    COIN = 40,
    BULLET = 50,
    BLOCK = 100, // etalon box
    __end = 255
}

public class UpdateData
{
    public Vector2 playerPos;
    public float playerRad;
}


public interface IMapItem
{
    string          itemID { get; }
    MAP_ITEM_TYPE   itemType { get; }
    Vector3         pos     { get; }
    GameObject      sceneObject { get; }
    void            RemoveFromScene(bool isCollected);
    void            Init(JSONNode param);

    void XUpdate(float dt, UpdateData data);
    bool XCollision(UpdateData data);
    bool IsUpdated();
    void SetGroupNumber (int num);
}


public abstract class Item : GModel, IMapItem
{
    public MAP_ITEM_TYPE itemType { get; set; }
    public string itemID { get; set; }
    
    public abstract void Init(JSONNode param);
    public abstract void XUpdate(float dt, UpdateData data);
    public abstract bool XCollision(UpdateData data);
    public virtual Vector4 BoundSphere() => Vector4.zero;
    public virtual bool IsUpdated() => false; // чтоб монетки не "обновлять"
    public virtual void RemoveFromScene(bool isCollected) => Destroy(base.gameObject);
    public virtual void SetGroupNumber (int num) { }

    public virtual Vector3 pos => transform.position;
    public GameObject sceneObject => base.gameObject;
}
