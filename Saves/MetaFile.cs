using Newtonsoft.Json;

namespace WECCL.Saves;

public class MetaFile
{
    private static MetaFile _instance;

    internal static MetaFile Data => _instance ??= Load();
    
    public List<string> PrefixPriorityOrder { get; set; } = new();
    public bool HidePriorityScreenNextTime { get; set; } = false;
    
    public bool FirstLaunch { get; set; } = true;
    
    public int TimesLaunched { get; set; } = 0;
    
    public string PreviousUser { get; set; } = "";

    public void Save()
    {
        string path = Locations.Meta.FullName;
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
        Plugin.Log.LogDebug($"Saved meta file to {path}.");
    }

    public static MetaFile Load()
    {
        try
        {
            string path = Locations.Meta.FullName;
            if (!File.Exists(path))
            {
                return new MetaFile();
            }

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<MetaFile>(json,
                new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }).IncrementTimesLaunched();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Unable to load meta file: {e}");
            return new MetaFile();
        }
    }
    
    public MetaFile IncrementTimesLaunched()
    {
        if (TimesLaunched != int.MaxValue)
        {
            TimesLaunched++;
        }
        return this;
    }
}