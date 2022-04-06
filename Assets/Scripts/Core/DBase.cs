using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.Linq;


[Serializable]
public class BundleInfo
{
    [Serializable]
    public class BundleMapInfo
    {
        public int num;
        public long map_id; // ID != SID
    }

    public string desc;
    public int num;
    public List<BundleMapInfo> maps = new List<BundleMapInfo>();
}

[Serializable]
public class BundlesPackInfo
{
    public List<BundleInfo> bundles;
}

[Serializable]
public class VerInfo
{
    public int config_version;
    public string msg;
    public List<string> files = new List<string>();
}

[Serializable]
public class UserStatInfo
{
    [Serializable]
    public class SMapRes
    {
        public long id;
        public long ver;
        public int run_count;
        public int restart_count;
        public int max_coin;
        public int max_jewel;
        public int max_star;
        public int is_complete;
    }
    public long user_id;
    public int app_version; // DEP
    public int cfg_version; // DEP
    public int use_dao_control = 0;
    public int use_vibro = 1;
    public int use_sound = 1;
    public string player_name;
    public int ver;
    public int run_app;
    public int run_editor;
    public int time_app;
    public int time_editor;
    public int total_distance;
    public int total_distance_round;
    public int total_distance_editor;
    public int total_coins;
    public int total_jewels;
    public int total_box;
    public int tutor_complete;
    public List<SMapRes> map_result = new List<SMapRes>();
    public List<int> user_maps = new List<int>() { 0, 0, 0, 0, 0 };
}

[Serializable]
public class JWrapper<T>
{
    public JWrapper(List<T> val) { Items = val; }
    public List<T> Items;
}

[Serializable]
public class StageColorInfo
{ 
    public Vector3 floor;
    public Vector3 border;
    public Vector3 sky;
}

[Serializable]
public class StageMsgInfo
{
    public string text;
    public int distance;
    public float ttl;
}

[Serializable]
public class CBrona
{
    public string mtype = "nondef";
};


[Serializable]
public class BossInfo
{
    public float hp = 0;
    public float regenPeriod = 2;
    public float regenPcnt = 0.08f;
    public float regenAddon = 0;
    public List<CBrona> brona = new List<CBrona> ();
}

[Serializable]
public class BotInfo
{
    public float BaseSpeed;
    public float BaseAccel;
    public float BoostSpeed;
    public float BoostAccel;
    public float LongTapVSpeedAdd;
    public float ShiftSpeed; // базовая скорость шифта
    public float ShiftTension; // это натяжение
    public float LongTapHSpeedAdd; // добавочная скорость шифта(лонгтап)
    public float LongTapAccel; // ускорение лонгтап
    public float LongTapDeccel; // торможение  лонгтап
    public float LongTapPow; // кривая лонгтап
    public float TwistBySpeed; // при наборе скорости вилять надо меньше. т.е. влияет на ShiftTension
    public float WallFrictionPCent;


    public float ballPower;
    public float ballPowerPlayer;
    public float ballDamp;
    public float ballRandomAng;
    public float ballDirMix;
}


[Serializable]
public class ItemInfo
{
    public string id; // 
    public string prefab; // НУЖНО Unity.ResourceLoad
    public string edname; // имя для редактора
    public string item_type; // тип предмета. BLOCK/COIN/JEWEL
    public string desc; // описание для редактора
    public string ico; // НУЖНО Unity.Sprite
    public string tkit; // Имя нужного тулкита. если пусто, то дефолт
    public int UNDER_CONSTRUCT;
}

[Serializable]
public class GameInfo
{
    public float[] CameraPos;
    public float Camera_XFollowMul;
    public float Camera_XRadActivation;
    public float Camera_ZFollowMul;
    public float Stage_Time2WinScene;
    public float WinScene_DelayBeforeCanExit;
    public float Finished_FallSpeedMul;
    public float ContinueDelaySec;
    public float EditorContinueDelaySec;
    public float BulletSmallBoxCount;

    public float tm_ow_killed;
    public float tm_ow_impact;
    public float tm_ow_victory;

    public float StoppedDist;

    public float ads_inter_p_impact;
    public float ads_inter_p_killed;
    public float ads_inter_p_victory;

    public float shootPrice = 1;
}


public static class ConfDB
{
    public static bool isConfigsLoaded = false;

    public static VerInfo ver;
    public static UserStatInfo stat;
    public static GameInfo game;
    public static BotInfo bot;
    public static Dictionary<string, ItemInfo> item;
    public static List<BundleInfo> mapBundles;

    public static void LoadSave()
    {
        stat = JsonUtility.FromJson<UserStatInfo>(FilesTool.LoadFile("save_file.json"));

        //  Сформируем "результаты прохождения" для непройденных карт
        foreach (var e in ConfDB.mapBundles[0].maps)
        {
            UserStatInfo.SMapRes prevResult = ConfDB.stat.map_result.Find(x => x.id == e.map_id);
            if (prevResult == null)
            {
                prevResult = new UserStatInfo.SMapRes();
                prevResult.id = e.map_id;
                Debug.Log("LoadSave> Create MapResult. ID: " + e.map_id);
                ConfDB.stat.map_result.Add(prevResult);
            }
        }
    }

    public static void LoadConfigsAndSave()
    {
        if (isConfigsLoaded)
            return;

        //  Загрузить и поднять локализацию
        Locale.Init();

        // unity dict не умеет нормально парсить/ А можт и не надо
        InitKeyValueDict<ItemInfo>(out item, FilesTool.LoadJson("item_base.json"));

        ver = JsonUtility.FromJson<VerInfo>(FilesTool.LoadFile("ver.json"));
        game = JsonUtility.FromJson<GameInfo>(FilesTool.LoadFile("game_props.json"));

#if !(DG_BALANCE)
        bot = JsonUtility.FromJson<BotInfo>(FilesTool.LoadFile("robike_bot.json"));
#else
        Debug.LogWarning("====== [DG_BALANCE] robike_bot_dev");
        bot = JsonUtility.FromJson<BotInfo>(FilesTool.LoadFile("robike_bot_dev.json"));
#endif
        //  т.к. юнитёвый json не умеет сразу парсить массив, то делаю так.
        mapBundles = JsonUtility.FromJson<BundlesPackInfo>(FilesTool.LoadFile("bundle.json")).bundles;

        //LoadSave();

        isConfigsLoaded = true;

        LoadSave();
    }


    // ЭТО МОЖНО ЗАШАБЛОНИТЬ И ИСПОЛЬЗОВАТЬ ВЕЗДЕ где нужно KEY:VALUE
    /*
    static void InitKeyValueDict<T>(JSONNode data)
    {
        item = new Dictionary<string, ItemInfo>();
        foreach (var v in data.Values)
        {
            var e = JsonUtility.FromJson<ItemInfo>(v.ToString());
            item.Add(e.id, e);
        }

    }*/

    static void InitKeyValueDict<T> (out Dictionary<string, T> dict, JSONNode data)
    {
        dict = new Dictionary<string, T> ();
        foreach (var v in data.Values)
        {
            var e = JsonUtility.FromJson<T> (v.ToString ());
            dict.Add (v["id"].Value, e);
        }
    }

    public static void SaveStat(bool send2server = true)
    {
        ConfDB.stat.ver++;
        FilesTool.SaveFile("save_file.json", JsonUtility.ToJson(ConfDB.stat, true));

        //  Сохранит на серваке текущий прогресс
        if (GSV.USER_ID != 0 && send2server)
        {
            NetCmd.UploadUserStat();
        }
    }

    // Считаем запуск редактора
    public static void IncEnterEditor()
    {
        ConfDB.stat.run_editor++;
    }
}


public static class Load
{
    public static Sprite Sprite(string name)
    {
        string[] split = name.Split('#');
        if (split.Length > 1)
            return System.Array.Find(Resources.LoadAll<Sprite>(split[0]), t => t.name == split[1]);
        else
            return Resources.Load<Sprite>(name);
    }

    public static Sprite SpriteFromAtlas(string name, UnityEngine.U2D.SpriteAtlas sprite)
    {
        string[] split = name.Split('#');
        if (split.Length > 1)
            return sprite.GetSprite(split[1]);
        else
        {
            Debug.LogError("SpriteFromAtlas> string [#] split expected");
            return null;
        }
    }
}