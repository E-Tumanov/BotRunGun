using UnityEngine;
using SimpleJSON;
using System.IO;

public class FilesTool : MonoBehaviour
{
    void Awake()
    {
        //  Не затирает кэш. Есть проверки на exist файлов и директорий
        LoadFiles2Cache();

        //  А теперь с WEB
        DownloadNewCache(GSV.APP_VER.ToString());
    }


    /// <summary>
    /// Проверка на существование
    /// </summary>
    /// <param name="fname"></param>
    /// <returns></returns>
    public static bool FileExist(string fname)
    {
        return File.Exists(GetDataPath(fname));
    }

    /// <summary>
    /// Загрузить текстовый файл.
    /// 
    /// Внимание! JsonUtility.FromJson использует строку. Чтоб два раза
    /// не гонять конвертацию. База конфигов использует этот функционал
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string LoadFile(string fileName)
    {
        var fname = GetDataPath(fileName);
        try
        {
            return File.ReadAllText(fname);//, System.Text.Encoding.UTF8);
        }
        catch (System.Exception e)
        {
            Debug.LogError("FileTool.LoadFile> " + e.ToString());
        }
        return "";
    }

    /// <summary>
    /// Загрузить JSON файл
    /// </summary>
    /// <param name="jsonFileName"></param>
    /// <returns></returns>
    public static JSONNode LoadJson(string jsonFileName)
    {
        var fname = GetDataPath(jsonFileName);
        try
        {
            return JSON.Parse(File.ReadAllText(fname));//, System.Text.Encoding.UTF8));
        }
        catch (System.Exception e)
        {
            Debug.LogAssertion(jsonFileName + " :: FileTool.LoadFile> " + e.ToString());
        }
        return JSONNode.Parse("{}");
    }

    /// <summary>
    /// Сформировать имя файла+путь в кэше (на телефоне)
    /// </summary>
    /// <param name="fname"></param>
    /// <returns></returns>
    public static string GetDataPath(string fname)
    {
#if (UNITY_EDITOR)
        return Path.Combine(Application.streamingAssetsPath, fname);
#elif (UNITY_IPHONE) || (UNITY_ANDROID)
        return Path.Combine(Application.persistentDataPath, fname);
#elif (UNITY_STANDALONE_WIN)
        return GSV.devDataPath;
#endif
    }


    /// <summary>
    /// Сформировать имя файла+путь до файла в APK(архиве)
    /// </summary>
    /// <param name="fname"></param>
    /// <returns></returns>
    public static string GetApkPath(string fname)
    {
        return Path.Combine(Application.streamingAssetsPath, fname);
    }


    /// <summary>
    /// Скачать свежий кэш с сервера
    /// </summary>
    /// <param name="getPrefixAPPVER"></param>
    public static void DownloadNewCache(string getPrefixAPPVER)
    {
        var xn = new XNetClient();
        xn.GET_FILE(getPrefixAPPVER, "file_list.json", file_list =>
        {
            SaveFile("file_list.json", file_list.ToString(3));
            foreach (var e in file_list.Values)
            {
                DirTest(e);
                var fname = e;
                xn.GET_FILE(getPrefixAPPVER, fname, data =>
                {
                    SaveFile(fname, data.ToString(3));
                });
            }
        });
    }


    /// <summary>
    /// Проверка на директорию. Если есть слеши, то нужно создать если нету
    /// </summary>
    /// <param name="fname"></param>
    public static void DirTest(string fname)
    {
        if (fname.Contains("/"))
        {
            var spos = fname.LastIndexOf("/");
            var dirName = GetDataPath(fname.Substring(0, spos));
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
        }
    }


    /// <summary>
    /// Скачать файл из APK в кэш
    /// </summary>
    /// <param name="fname"></param>
    static void CopyFileFromAPK(string fname)
    {
#if (UNITY_ANDROID)
#pragma warning disable 0618
        using (WWW www = new WWW(GetApkPath(fname)))
        {
            while (!www.isDone) { }
            try
            {
                File.WriteAllBytes(GetDataPath(fname), www.bytes);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
#elif (UNITY_IPHONE)
        var bytes = System.IO.File.ReadAllBytes(GetApkPath(fname));
        File.WriteAllBytes(GetDataPath(fname), bytes);
#endif
    }


    /// <summary>
    /// Скачать все файлы из APK в кэш
    /// Если файл file_list.json уже есть, то начит уже запускали. Можно ничего не копировать
    /// </summary>
    public static void LoadFiles2Cache()
    {
        if (FileExist(GetDataPath("file_list.json")))
            return;

        Debug.Log("LoadDLLFiles> start load files to cache");

        CopyFileFromAPK("file_list.json"); // CONSTANT

        var files = LoadJson("file_list.json");
        foreach (var e in files.Values)
        {
            DirTest(e);
            if (!FileExist(GetDataPath(e)))
            {
                CopyFileFromAPK(e);
            }
        }
    }


    /// <summary>
    /// Сохранить строковый файл
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    public static void SaveFile(string fileName, string data)
    {
        var fname = GetDataPath(fileName);
        try
        {
            File.WriteAllText(fname, data);//, System.Text.Encoding.UTF8);
            //Debug.Log("SaveFile> " + fileName);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("FileTool.SaveFile> " + e.ToString());
        }
    }
}
