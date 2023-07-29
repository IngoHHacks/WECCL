using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;

namespace WECCL.Utils;

public static class DataSharing
{
    private const string URL = "https://ingoh.net/weccl/data.php";

    private static string _uuid = null;
    
    private static string DataUUID => _uuid ??= LoadUUID() ?? GenerateUUID();

    private static string GenerateUUID()
    {
        string uuid = Guid.NewGuid().ToString();
        Plugin.Log.LogDebug("Generated Data UUID: " + uuid);
        File.WriteAllText(Path.Combine(Plugin.PersistentDataPath, "uuid.txt"), uuid, Encoding.UTF8);
        return uuid;
    }

    private static string LoadUUID()
    {
        if (!File.Exists(Path.Combine(Plugin.PersistentDataPath, "uuid.txt"))) return null;
        string uuid = File.ReadAllText(Path.Combine(Plugin.PersistentDataPath, "uuid.txt"), Encoding.UTF8);
        Plugin.Log.LogDebug("Loaded Data UUID: " + uuid);
        return uuid;
    }

    public static void SendExceptionToServer(string message, string stackTrace, string dataSharingLevel)
    {
        if (dataSharingLevel == "None") return;
        Dictionary<string, string> data;
        if (dataSharingLevel == "Full")
        {
            data = new Dictionary<string, string>
            {
                { "_uuid", DataUUID },
                { "type", "exception" },
                { "message", message },
                { "stackTrace", stackTrace },
                { "wecclVersion", Plugin.PluginVersion.ToString(CultureInfo.CurrentCulture) },
                { "installedMods", string.Join(",", BepInEx.Bootstrap.Chainloader.PluginInfos.Select(x => x.Key)) },
                { "prefixes", string.Join(",", Prefixes) },
                { "nonDefaultConfigValues", Plugin.GetConfigValues() },
                { "currentScene", SceneManager.GetActiveScene().name },
                { "numberOfCharacters", Characters.c.Length.ToString() }
            };

        }
        else
        {
            data = new Dictionary<string, string>
            {
                { "type", "exception" },
                { "message", message },
                { "stackTrace", stackTrace },
                { "wecclVersion", Plugin.PluginVersion.ToString(CultureInfo.CurrentCulture) }
            };
        }
        Thread thread = new(() =>
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";
               
                using (StreamWriter writer = new(request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(data);
                    writer.Write(json);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Plugin.Log.LogDebug("Data sent! Server response: " + response.StatusCode);
            } catch (Exception e)
            {
                Plugin.Log.LogDebug("Error sending data to server: " + e);
            }
        });
        thread.Start();

    }
}