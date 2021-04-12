using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ui_fade_image : MonoBehaviour
{
    [SerializeField] AnimationCurve cv;
    [SerializeField] float fadeTime;
    [SerializeField] bool autoDestroy;

    void Start()
    {
        var image = GetComponent<Image> ();
        var prevColor = image.color;
        MTask.Run (this, 0, fadeTime, t => 
        {
            prevColor.a = cv.Evaluate (t);
        },
        () => { if (autoDestroy) Destroy (gameObject); });
    }
}
