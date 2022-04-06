using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleport
{
    /*
   public static Teleport inst;
   public static Transform place;

   public static System.Action<Vector3> beforeTP = delegate { };
   public static System.Action<Vector3> afterTP = delegate { };

   Vector3 pos;

   public static void Init()
   {
      inst = null;
      place = null;
      beforeTP = delegate { };
      afterTP = delegate { };
   }

   public static void Click()
   {
      if (inst != null)
      {
         DoIt();
         //GameObject.Destroy(place.gameObject);
         //inst = null;
      }
      else
      {
         inst = new Teleport();
         inst.pos = G.playerPos;
         place = GameObject.Instantiate<Transform>(Resources.Load<Transform>("t-port2"));
         place.position = inst.pos;
      }
   }

   public void Update()
   {
      if (Mathf.Abs(inst.pos.z - G.playerPos.z) < 7f)
         return;
      DoIt();
   }

   static void DoIt()
   {

      beforeTP(inst.pos);

      Debug.LogError("fixit");
      // control.inst.Transport(inst.pos);

      afterTP(inst.pos);
      //G.inst.mSpeedBase01 *= 0.1f;
      GameObject.Destroy(place.gameObject);
      inst = null;
   }*/
}
