using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Facebook.Unity;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Работа с сетью
/// Просто команды собраны в одном месте
/// </summary>
public class NetCmd
{
    /// <summary>
    /// Проверить регистрацию по google_id 
    /// </summary>
    public static void RegistrationCheck()
    {
        var xnet = new XNetClient();
        xnet.POST2("GET_USER", JSON.Parse("{google_id:" + GSV.GOOGLE_ID + "}"), (res) =>
        {
            if (res["user_id"].AsLong == 0)
            {
                // CASE. игру установили, но не регались (нет сети или отложенная регистрация)/ Нас нет в базе
                // тогда ID для карт не сгенерены.
                Registration();
            }
            else
            {
                GSV.USER_ID = res["user_id"].AsLong;
            }
        });
    }

    /// <summary>
    /// Регистрация, если первый запуск 
    /// </summary>
    public static void Registration()
    {
        // Создать новую запись статистики
        ConfDB.stat = new UserStatInfo();
        ConfDB.stat.app_version = GSV.APP_VER;
        ConfDB.stat.player_name = GSV.GOOGLE_DISPLAY_NAME;

        //  сохранить save в кэш
        ConfDB.SaveStat();

        var data = JSONNode.Parse("{}");
        data["name"] = ConfDB.stat.player_name;
        data["google_id"] = GSV.GOOGLE_ID;

        var xnet = new XNetClient();
        xnet.POST2("REGISTER", data, (res) =>
        {
            // Обновить ID полученный с сервера
            GSV.USER_ID = ConfDB.stat.user_id = res["user_id"].AsInt;

            // Номера карт игрока
            ConfDB.stat.user_maps = new List<int>();
            foreach (var e in res["map_list"].Values)
            {
                ConfDB.stat.user_maps.Add(e.AsInt);
            }
            Debug.Log("Registration success. UID: " + GSV.USER_ID);
        });

        //  Сразу отправим на сервер SAVE_FILE
        ConfDB.SaveStat();
    }

    /*
    /// <summary>
    /// Скачать файлы перечисленные в file:ver.json (приходит с сервера) 
    /// </summary>
    public static void DownloadNewCache()
    {
        var xnet = new XNetClient();

        //  внимание. Уже приходит отформаченый JSON объект. не строка
        xnet.GET_FILE("common", "access_ver.json", (res) =>
        {
            GSV.ACCESS_VER = res["ver"].AsInt;
            Debug.Log("MIN ACCESS VER: " + GSV.ACCESS_VER);
        }, () => { });

        string fname = "ver.json";
        xnet.GET_FILE(GSV.APP_VER.ToString(), fname, (res) =>
        {
           FilesTool.SaveFile(fname, res.ToString());
           foreach (var e in res["files"].Values)
           {
               fname = e.Value;
               xnet.GET_FILE(GSV.APP_VER.ToString(), fname, (_res) =>
               {
                   FilesTool.SaveFile(fname, _res.ToString());
               });
           }
        });
    }
    */

    /// <summary>
    /// Скачать SAVE с сервера
    /// </summary>
    public static void DownloadSave()
    {
        //  TODO: Сделать проверку на версии. Серверу можно слать текущую версию
        // , и если у меня свежая, то не присылать карту.
        var data = JSON.Parse("{}");
        data["save_version"].AsInt = ConfDB.stat.ver;

        // сохранённый user_id не совпадает с выданным SID
        if (ConfDB.stat.user_id != GSV.USER_ID)
        {
            data["save_version"].AsInt = -1;
            Debug.LogWarning("DownloadSave> save.user_id not eq SID");
        }
        
        new XNetClient().POST2("DOWNLOAD_SAVE", data, (JSONNode res) =>
        {
            //  сервер проверяет версию save, и шлёт если больше локальной
            if (res["save_file"] != null)
            {
                Debug.Log("++++++ UPDATE SAVE");
                FilesTool.SaveFile("save_file.json", res["save_file"].ToString());
            }
        });
    }


    /// <summary>
    /// Скачать карты игрока 
    /// </summary>
    public static void DownloadUserMaps()
    {
        /*
        for (int i = 0; i < ConfDB.stat.user_maps.Count; i++)
        {
            var data = JSON.Parse("{map_index:0,map_version:-1}");
            data["map_index"].AsInt = i;
            data["map_version"].AsInt = StageUserManager.GetStageVersion(i);

            //  SID не совпадает с выданными номерами. Старый кэш, другой игрок
            if (StageUserManager.GetStageSID(i) != ConfDB.stat.user_maps[i])
            {
                data["map_version"].AsInt = -1;
                Debug.LogWarning("DownloadUserMaps> map_id not eq SID");
            }

            new XNetClient().POST2("DOWNLOAD_USER_MAP", data, (JSONNode res) =>
            {
                if (res["map"] != null)
                {
                    Debug.Log("++++++ UPDATE USER_MAP" + i);
                    FilesTool.SaveFile("ustage_" + i + ".json", res["map"].ToString());
                }
            });
        }
        */
    }


    /// <summary>
    /// Залить пользовательские карты на сервер
    /// </summary>
    public static void UploadUserMap(int map_sid, int map_ver, JSONNode map)
    {
        //  Никаких проверок не надо. Если карта редактируется, то надо upload
        var data = JSON.Parse("{}");
        data["map_id"].AsInt = map_sid;
        data["ver"].AsInt = map_ver;
        data["data"] = map;
        new XNetClient().POST2("UPLOAD_MAP", data);
    }


    /// <summary>
    /// Залить на сервер SAVE
    /// </summary>
    public static void UploadUserStat()
    {
        var data = JSON.Parse("{}");
        data["save_ver"].AsInt = ConfDB.stat.ver;
        data["save"] = JSON.Parse(JsonUtility.ToJson(ConfDB.stat, false));
        new XNetClient().POST2("UPLOAD_USER_STAT", data);
    }
}


/*
    public static async void DOL()
    {
        await Task.Run(() => {
            Thread.Sleep(5000);
            Debug.Log("ШЛЙМА");
        });

        Debug.Log("ШЛЙМА 1");

        await Task.Run(() => {
            Thread.Sleep(5000);
            Debug.Log("ШЛЙМА 2");
        });
    }
    {
        Debug.Log("ШЛЕМА 0");

        DOL();

        Debug.Log("ШЛЕМА 1");

        while (Time.frameCount != 500)
            await Task.Yield();

        var tt = Task.Delay(200);
        Debug.Log("Ш task del 0. " + Time.frameCount);
        await Task.Delay(100);

        Debug.Log("Ш task del 1. " + Time.frameCount);

        await tt;
        Debug.Log("Ш task del 2. "+ Time.frameCount);

        Debug.Log("ШЛЕМА 2");
    }
*/
