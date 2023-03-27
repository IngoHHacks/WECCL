using Newtonsoft.Json;

namespace WECCL.Saves;

public class CustomConfigsSaveFile
{
    private static CustomConfigsSaveFile _instance;

    internal static CustomConfigsSaveFile Config => _instance ??= Load();
    
    public List<string> PrefixPriorityOrder { get; set; } = new();
    public bool HideNextTime { get; set; } = false;

    public void Save()
    {
        string path = Plugin.CustomConfigsSavePath;
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
        Plugin.Log.LogDebug($"Saved custom config file to {path}.");
    }

    public static CustomConfigsSaveFile Load()
    {
        string path = Plugin.CustomConfigsSavePath;
        if (!File.Exists(path))
        {
            return new CustomConfigsSaveFile();
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CustomConfigsSaveFile>(json,
            new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
    }
}