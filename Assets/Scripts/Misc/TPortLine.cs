using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TPortLine : MonoBehaviour
{
    LineRenderer lr;
    [SerializeField] Transform APoint;
    [SerializeField] Transform BPoint;
    [SerializeField] float YUp;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, APoint.position + Vector3.up * YUp);
        lr.SetPosition(1, BPoint.position + Vector3.up * YUp);
    }
}
