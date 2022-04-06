using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleUI : MonoBehaviour
{
    [SerializeField] string locale_id = "";
 
    void Start()
    {
        var t = GetComponent<Text>();
        if (t != null)
        {
            if (locale_id != "")
            {
                t.text = Locale.Get(locale_id);
            }
        }
    }
}
























