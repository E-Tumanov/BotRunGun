using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

/// <summary>
/// Манагер загруженных уровней 
/// </summary>
public static class StageManager
{
    // мета инфа о выбранном уровне
    public static int stageNUM; // номер по порядку в BUNDLE.JSON
    public static long stageID; // ID != SID

    // Информация о прохождениях
    public static UserStatInfo.SMapRes FindStageStat (long rmapID)
    {
        UserStatInfo.SMapRes prevResult = ConfDB.stat.map_result.Find (x => x.id == rmapID);
        //if (prevResult == null)
        //  throw new System.Exception("1004");
        return prevResult;
    }

    public static string currStageFileName => stageID.ToString () + ".json";

    /// <summary>
    /// 
    /// </summary>
    public static Stage Create (string stageFileName, IItemFactory itemFactory)
    {
        var data = FilesTool.LoadJson (stageFileName);
        var stage = __parser (data, stageFileName);

        stage.itemFactory = itemFactory;
        return stage;
    }


    /// <summary>
    /// 
    /// </summary>
    public static Stage __parser (JSONNode data, string stageFileName)
    {
        Stage level = JsonUtility.FromJson<Stage> (data.ToString ());
        level.stageFileName = stageFileName;

        var objects = data["objects"];
        foreach (var itemID in objects.Keys)
            foreach (var e in objects[itemID].Values)
                level.AddItem (itemID, e); // false Группы считаем только в редакторе
        return level;
    }

    /*
    public static bool IsTutorComplete()
    {
        var firstMap = DB.mapBundles[0].maps.Find(x => x.num == 1); // ищем какая карта первая
        return StageManager.FindStageStat(firstMap.map_id).is_complete != 0; // первый уровень(ТУТОР) не пройден
    }
    */

    /// <summary>
    /// Начальная загрузка. Выбор игровой сцены (продолжение/рандом и т.д)
    /// </summary>
    public static void Init (bool isTutorEnable)
    {
        long lastMapID = 0;
        int lastMapNUM = 0;

        if (isTutorEnable) // Тутор не пройден. возможно первый запуск
        {
            var e = ConfDB.mapBundles[0].maps.Find (x => x.num == 1);
            lastMapID = e.map_id;
            lastMapNUM = e.num;
        }
        else // продолжаем игру
        {
            //  Найдём последний пройденный уровень
            bool stop = false;
            foreach (var e in ConfDB.mapBundles[0].maps)
            {
                foreach (var sm in ConfDB.stat.map_result)
                {
                    if (e.map_id == sm.id && sm.is_complete == 0)
                    {
                        stop = true;
                        lastMapID = e.map_id;
                        lastMapNUM = e.num;
                        break;
                    }
                }
                if (stop)
                    break;
            }
        }

        // Если игра пройдена (нет непройденых карт), то рандом
        if (lastMapNUM == 0)
        {
            var maps = ConfDB.mapBundles[0].maps;
            lastMapNUM = Random.Range (1, maps.Count - 1);
            lastMapID = maps[lastMapNUM].map_id;
            lastMapNUM += 1; // генерит от 0, поэтому надо добавить.
        }

        StageManager.stageNUM = lastMapNUM;
        StageManager.stageID = lastMapID;
    }
}


/// <summary>
/// Манагер пользовательских уровней
/// </summary>
public static class StageUserManager
{
    public static int currLevel = 0;

    public static void SelectNext ()
    {
        currLevel = Mathf.Clamp (currLevel + 1, 0, 4);
    }

    public static void SelectPrev ()
    {
        currLevel = Mathf.Clamp (currLevel - 1, 0, 4);
    }

    public static string currStageFileName => "ustage_" + currLevel + ".json";

    /*
    public static int GetStageVersion(int map_index)
    {
        string fname = "ustage_" + map_index + ".json";
        if (map_index < 0 || map_index > 4)
        {
            Debug.LogAssertion("StageUserManager.GetStageVersion>  map_index out range: " + map_index);
            return -1;
        }
        if (!FilesTool.FileExist(fname))
        {
            //  это не ошибка. файла может не быть. чистый кэш.
            return -1;
        }
        var data = FilesTool.LoadJson(fname);
        return data["ver"].AsInt;
    }


    public static int GetStageSID(int map_index)
    {
        string fname = "ustage_" + map_index + ".json";
        if (map_index < 0 || map_index > 4)
        {
            Debug.LogAssertion("StageUserManager.GetStageVersion>  map_index out range: " + map_index);
            return -1;
        }
        if (!FilesTool.FileExist(fname))
        {
            //  это не ошибка. файла может не быть. чистый кэш.
            return -1;
        }
        var data = FilesTool.LoadJson(fname);
        return data["sid"].AsInt;
    }*/
}