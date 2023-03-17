using Newtonsoft.Json;

namespace WECCL.Saves;

internal class CustomContentSaveFile
{
    private static CustomContentSaveFile _instance;
    
    internal static CustomContentSaveFile ContentMap { get => _instance ??= new(); }

    internal CustomContentSaveFile()
    {
        for (int i = 0; i < 40; i++)
        {
            MaterialNameMap.Add(new List<string>());
            FleshNameMap.Add(new List<string>());
            ShapeNameMap.Add(new List<string>());
        }
    }
    
    public List<List<string>> MaterialNameMap { get; set; } = new();
    public List<List<string>> FleshNameMap { get; set; } = new();
    public List<List<string>> ShapeNameMap { get; set; } = new();
    
    public List<string> FaceFemaleNameMap { get; set; } = new();
    
    public List<string> BodyFemaleNameMap { get; set; } = new();
    
    public List<string> SpecialFootwearNameMap { get; set; } = new();
    
    public List<string> TransparentHairMaterialNameMap { get; set; } = new();
    
    public List<string> TransparentHairHairstyleNameMap { get; set; } = new();
    
    public List<string> KneepadNameMap { get; set; } = new();
    
    public List<string> MusicNameMap { get; set; } = new();

    public void Save()
    {
        var path = Plugin.CustomContentSavePath;
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        System.IO.File.WriteAllText(path, json);
        Plugin.Log.LogDebug($"Saved custom content map to {path}.");
    }

    public static CustomContentSaveFile Load()
    {
        var path = Plugin.CustomContentSavePath;
        if (!System.IO.File.Exists(path))
        {
            return new CustomContentSaveFile();
        }
        var json = System.IO.File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CustomContentSaveFile>(json, new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });
    }
}