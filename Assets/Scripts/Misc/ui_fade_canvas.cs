using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (CanvasGroup))]
public class ui_fade_canvas : MonoBehaviour
{
    [SerializeField] AnimationCurve cv;
    [SerializeField] float fadeTime;
    [SerializeField] bool autoDestroy;

    void Start ()
    {
        var canvas = GetComponent<CanvasGroup> ();
        
        MTask.Run (this, 0, fadeTime, t =>
        {
            canvas.alpha = cv.Evaluate (t);
        },
        () => { if (autoDestroy) Destroy (gameObject); });
    }
}