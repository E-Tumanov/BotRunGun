using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateRotator : MonoBehaviour
{
    public float rotSpeed;
    public float polarH, polarR;
    float currRot;

    void Start()
    {
        currRot = polarR;
        SolvePos ();
    }

    void SolvePos ()
    {
        var h = Quaternion.AngleAxis (currRot, Vector3.up) * Vector3.forward;
        transform.localPosition = h * polarH;
        transform.localRotation = Quaternion.FromToRotation (Vector3.forward, h);
    }

    void Update()
    {
        currRot += rotSpeed * Time.deltaTime;
        SolvePos ();
    }
}
