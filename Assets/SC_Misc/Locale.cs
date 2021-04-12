using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Locale 
{
    static bool isRU = false;
    static string loc = "en";
    static JSONNode data;

    public static void Init()
    {
        isRU = Application.systemLanguage == SystemLanguage.Russian;
        isRU = false;
        loc = isRU ? "ru" : "en";

        data = FilesTool.LoadJson("locale.json");
    }

    public static string Get(string id)
    {
        try
        {
            var rec = data[id][loc];

            if (data[id][loc].IsArray)
            {
                return rec[new System.Random().Next(0, rec.Count)].Value;
            }
            else
            {
                return rec.Value;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LOCALIZE> id: {id}, loc: {loc}");
        }
        return $"###{id}:{loc}";
    }
}
