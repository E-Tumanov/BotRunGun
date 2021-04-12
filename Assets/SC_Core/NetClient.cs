using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Threading.Tasks;

public interface IXNetClient
{
    void POST2(string cmd, JSONNode param, System.Action<JSONNode> onSuccess = null, System.Action onFailure = null);
    void GET_FILE(string prefix, string fname, System.Action<JSONNode> onSuccess = null, System.Action onFailure = null);
}


class XNetClient : IXNetClient
{
    struct CMsg
    {
        public string cmd;
        public JSONNode param;
        public System.Action<string, JSONNode> onFileDone;
        public System.Action<JSONNode> onSuccess;
        public System.Action onError;
    }

    public void GET_FILE(string prefix, string fname, System.Action<JSONNode> success = null, System.Action failure = null)
    {
        try
        {
            var Hosts = System.Net.Dns.GetHostEntry("tumangames.ru");
            var ip = Hosts.AddressList[0].ToString();
            var webClient = new System.Net.WebClient();

            ///var rest = g.DownloadString("http://185.63.191.213:8888/?data=" + data.ToString());
            // Ошибки идут в exception
            var rest = webClient.DownloadString("http://" + ip + "/" + prefix + "/" + fname);
            success?.Invoke(JSONNode.Parse(rest));
        }
        catch (System.Exception e)
        {
            Debug.LogError("XNetClient.GET_FILE [ " + fname + " ]. " + e.ToString());
            failure?.Invoke();
        }
    }

    // Всегда ждём! любой запрос блокирующий и синхронный
    //
    public void POST2 (string cmd, JSONNode param, System.Action<JSONNode> success = null, System.Action failure = null)
    {
        SendMSG(new CMsg { cmd = cmd, param = param, onSuccess = success, onError = failure });
    }

    void SendMSG(CMsg msg)
    {
        JSONNode data = JSONNode.Parse("{cmd:none, param:{},uid:0,app_ver:0}");
        
        data["cmd"] = msg.cmd;
        data["param"] = msg.param;
        data["uid"].AsLong = GSV.USER_ID;
        data["app_ver"].AsLong = GSV.APP_VER;

        try
        {
            var Hosts = System.Net.Dns.GetHostEntry("tumangames.ru");
            var ip = Hosts.AddressList[0].ToString();
            var webClient = new System.Net.WebClient();


            ///var rest = g.DownloadString("http://185.63.191.213:8888/?data=" + data.ToString());
            // Ошибки идут в exception
            webClient.DownloadStringCompleted += (s, e) => {
                msg.onSuccess?.Invoke (JSONNode.Parse (e.Result));
            };// WebClient_DownloadStringCompleted;


            //var rest = 
            var uri = new System.Uri("http://" + ip + ":8888/?data=" + data.ToString ());
            webClient.DownloadStringAsync(uri);
            
            //msg.onSuccess?.Invoke(JSONNode.Parse(rest));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("NET.POST [ " + msg.cmd + " ]. " + e.ToString());
            msg.onError?.Invoke();
        }
    }

    private void WebClient_DownloadStringCompleted (object sender, System.Net.DownloadStringCompletedEventArgs e)
    {
        
        
    }
}

