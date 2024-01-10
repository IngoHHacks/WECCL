using Newtonsoft.Json;

namespace WECCL.Saves;

internal class CharacterMappings
{
    private static CharacterMappings _instance;
    
    internal static CharacterMappings CharacterMap {
        get
        {
            if (_instance == null)
            {
                _instance = Load();
            }
            return _instance;
        }
    }
    
    public List<string> PreviouslyImportedCharacters { get; set; } = new();
    public List<int> PreviouslyImportedCharacterIds { get; set; } = new();
    
    public void Save()
    {
        string path = Locations.CharacterMappings.FullName;
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
        LogDebug($"Saved custom content map to {path}.");
    }

    public static CharacterMappings Load()
    {
        string path = Locations.CharacterMappings.FullName;
        if (!File.Exists(path))
        {
            return new CharacterMappings();
        }
        try
        {
            string json = File.ReadAllText(path);
            CharacterMappings obj = JsonConvert.DeserializeObject<CharacterMappings>(json,
                new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            for (int i = 0; i < obj.PreviouslyImportedCharacters.Count; i++)
            {
                if (obj.PreviouslyImportedCharacters[i].EndsWith(".json"))
                {
                    obj.PreviouslyImportedCharacters[i] = obj.PreviouslyImportedCharacters[i]
                        .Substring(0, obj.PreviouslyImportedCharacters[i].Length - 5);
                }
                else if (obj.PreviouslyImportedCharacters[i].EndsWith(".character"))
                {
                    obj.PreviouslyImportedCharacters[i] = obj.PreviouslyImportedCharacters[i]
                        .Substring(0, obj.PreviouslyImportedCharacters[i].Length - 10);
                }
            }
            return obj;
        }
        catch (Exception e)
        {
            LogError($"Unable to load custom character map: {e}");
            return new CharacterMappings();
        }
    }
    
    public void AddPreviouslyImportedCharacter(string name, int id)
    {
        if (name.EndsWith(".json"))
        {
            name = name.Substring(0, name.Length - 5);
        }
        else if (name.EndsWith(".character"))
        {
            name = name.Substring(0, name.Length - 10);
        }

        if (this.PreviouslyImportedCharacters.Contains(name))
        {
            return;
        }

        this.PreviouslyImportedCharacters.Add(name);
        this.PreviouslyImportedCharacterIds.Add(id);
    }
}