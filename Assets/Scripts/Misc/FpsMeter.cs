using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsMeter : MonoBehaviour
{
    [SerializeField] Text txtFPS;

    float refresh;
    float nextTick;
    int count;

    void Start()
    {
        nextTick = Time.unscaledTime + 1;
    }
    
    void LateUpdate()
    {
        count++;
        if (nextTick < Time.unscaledTime)
        {
            txtFPS.text = count.ToString ();
            nextTick = Time.unscaledTime + 1;
            count = 0;
        }
    }
}
