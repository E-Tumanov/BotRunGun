using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStolb : MonoBehaviour
{
    [SerializeField] float rad = 0.5f;

    private void LateUpdate()
    {
        //CBall.inst.AppendStolb(transform.position, rad);
    }
    void Update()
    {
        //CBall.inst.AppendStolb(transform.position, rad);   
    }
}
