using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

/// <summary>
/// Простые объекты на карте. Ящики, бочки, монеты, алмазы и т.д.
/// </summary>
public class ItemSimple : Item
{
    public float extX = 0.5f;
    public float extY = 0.25f;

    [Header ("Если не 0, то считает только радиус")]
    public float extRad = 0;

    public AudioSource get_sound;
    public GameObject FX;
    public GameObject impactFx;

    [SerializeField] List<Color32> groupColor;
    [SerializeField] List<Material> groupMaterial;

    //public virtual Vector4 BoundSphere(){ }

    public override void SetGroupNumber (int num)
    {
        if (itemType == MAP_ITEM_TYPE.COIN)
        {
            var coin = transform.GetChild (0).GetComponent<MeshRenderer> ();
            //var mat = Instantiate (coin.material);
            //mat.color = groupColor[num % 4];
            transform.GetChild (0).GetComponent<MeshRenderer> ().sharedMaterial = groupMaterial[num % 4];
        }
    }

    public override void RemoveFromScene (bool isCollected)
    {

        if (isCollected && FX)
        {
            var p = transform.position;
            p.y = transform.position.y + 0.5f;
            Instantiate (FX, p, Quaternion.identity);

        }

        base.RemoveFromScene (isCollected);
    }


    public override void Init (JSONNode param)
    {

        float zpos = param[1].AsFloat;
        float ypos = 0;

        if (G.isRoundStarted)
            ypos = di.GetUpOffset (zpos);

        transform.position = new Vector3 (param[0].AsFloat, ypos, zpos);
    }



    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Player"))
        {
            di.TripManager.CollisionItemTest (this);
            if (impactFx)
            {
                Instantiate (impactFx, null, false).transform.position = di.PlayerModel.position + Vector3.up * 1;
                //other.con
                //var pos = transform.position + other.contactOffset;
            }
        }
    }

    public override bool XCollision (UpdateData data)
    {
        return false;
        /*
        if (Mathf.Abs (transform.position.z - data.playerPos.y) > 2)
            return false;

        Vector2 ppt = new Vector2 (transform.position.x, transform.position.z);

        Vector2 cnr0 = new Vector2 (-extX, -extY) + ppt;
        Vector2 cnr1 = new Vector2 (-extX, extY) + ppt;
        Vector2 cnr2 = new Vector2 (extX, -extY) + ppt;
        Vector2 cnr3 = new Vector2 (extX, extY) + ppt;

        Vector2 ppos = data.playerPos;

        // 2MIND сделать уменьшение радиуса от скорости. угар

        // Это радиус игрока
        float prad = data.playerRad;

        //  Нужно Физику ударов об ящики проработать чётко.
        // Нужны будут градации столкновений. "ВЛОБ", "ЧИРКОМ/УГОЛОК", "ВБОЧИНУ"

        if (extRad == 0)
        {
            return ((cnr0 - ppos).magnitude < prad ||
                    (cnr1 - ppos).magnitude < prad ||
                    (cnr2 - ppos).magnitude < prad ||
                    (cnr3 - ppos).magnitude < prad ||
                    (new Vector2 (transform.position.x, transform.position.z) - ppos).magnitude < 0.85f);
        }
        else
        {
            return (new Vector2 (transform.position.x, transform.position.z) - ppos).magnitude < (extRad + prad);
        }*/
    }

    public override void XUpdate (float dt, UpdateData data)
    {
    }
}
