using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/*
==========================================
Aim constrain
==========================================
*/

[ExecuteInEditMode]
public class CameraAim : MonoBehaviour
{
    [SerializeField] Transform locator;
    [SerializeField] Vector3 startPos;
    [SerializeField] float XFollowWeight;
    [SerializeField] float XFollowRad = 3;

    [SerializeField] float ZFollowWeight;
    [SerializeField] float currZFollowWeight;

    Vector3 interLocator;
    float savedZ;

    void Start()
    {
        interLocator = locator.position;
        savedZ = locator.position.z;
        /*
      Teleport.beforeTP += (Vector3 pos) =>
      {
         currZFollowWeight = 0;
         savedZ = locator.position.z;
      };*/
    }


    void LateUpdate()
    {
        if (locator == null)
            return;

        if (G.isRoundStarted && G.isPause)
            return;

        float dd = Math.Abs(interLocator.x - locator.position.x) / XFollowRad;
        //dd *= dd;
        if (dd < 0.05f)
            dd = 0;


        interLocator.x = Mathf.Lerp(interLocator.x, locator.position.x, dd * XFollowWeight * G.deltaTime);
        interLocator.z = Mathf.Lerp(interLocator.z, locator.position.z, ZFollowWeight * 50 * G.deltaTime);

        //Vector3 pos = new Vector3(interLocator.x, locator.position.y, interLocator.z) - startPos;
        Vector3 pos = new Vector3(interLocator.x, locator.position.y, locator.position.z) - startPos;
        transform.position = Vector3.Lerp(transform.position, pos, 0.5f);

        //transform.position = locator.position - startPos;
    }
}
