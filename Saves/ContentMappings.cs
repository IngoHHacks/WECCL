using Newtonsoft.Json;

namespace WECCL.Saves;

internal class ContentMappings
{
    private static ContentMappings _instance;

    internal ContentMappings()
    {
        for (int i = 0; i < 40; i++)
        {
            this.MaterialNameMap.Add(new List<string>());
            this.FleshNameMap.Add(new List<string>());
            this.ShapeNameMap.Add(new List<string>());
        }
    }

    internal static ContentMappings ContentMap => _instance ??= new ContentMappings();

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
    
    public List<string> PreviouslyImportedCharacters { get; set; } = new();
    
    public List<int> PreviouslyImportedCharacterIds { get; set; } = new();

    public void Save()
    {
        string path = Plugin.ContentMappingsPath;
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
        Plugin.Log.LogDebug($"Saved custom content map to {path}.");
    }

    public static ContentMappings Load()
    {
        string path = Plugin.ContentMappingsPath;
        if (!File.Exists(path))
        {
            return new ContentMappings();
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<ContentMappings>(json,
            new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
    }
    
    public void AddPreviouslyImportedCharacter(string name, int id)
    {
        if (this.PreviouslyImportedCharacters.Contains(name))
        {
            return;
        }
        
        this.PreviouslyImportedCharacters.Add(name);
        this.PreviouslyImportedCharacterIds.Add(id);
    }
}