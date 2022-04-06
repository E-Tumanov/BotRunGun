using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingRotator : MonoBehaviour
{
    [SerializeField] float periodTime = 1;
    [SerializeField] AnimationCurve cv;
    private void Start ()
    {
        cv.postWrapMode = WrapMode.Loop;
    }
    void Update()
    {
        var p = cv.Evaluate (Time.unscaledTime / periodTime);
        transform.rotation = Quaternion.AngleAxis (p * 360.0f, Vector3.up);
    }
}
