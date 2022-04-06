using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerHLoopMove : MonoBehaviour
{
    void Update()
    {
        var rt = GetComponent<RectTransform>();//  rect.position;
        var t = rt.anchoredPosition;
        t.x = Mathf.Sin(4 * Time.time) * 40;
        rt.anchoredPosition = t;
        //transform.position = t;
    }
}
