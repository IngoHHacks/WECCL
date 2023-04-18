using Newtonsoft.Json;

namespace WECCL.Saves;

public class MetaFile
{
    private static MetaFile _instance;

    internal static MetaFile Data => _instance ??= Load();
    
    public List<string> PrefixPriorityOrder { get; set; } = new();
    public bool HidePriorityScreenNextTime { get; set; } = false;
    
    public bool FirstLaunch { get; set; } = true;
   

    public void Save()
    {
        string path = Plugin.MetaFilePath;
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
        Plugin.Log.LogDebug($"Saved meta file to {path}.");
    }

    public static MetaFile Load()
    {
        string path = Plugin.MetaFilePath;
        if (!File.Exists(path))
        {
            return new MetaFile();
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<MetaFile>(json,
            new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
    }
}