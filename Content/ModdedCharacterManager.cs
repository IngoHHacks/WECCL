using Newtonsoft.Json;
using System.IO;

namespace WECCL.Content;

public class ModdedCharacterManager
{
    public static void SaveAllCharacters()
    {
        var exportPath = Path.Combine(Plugin.PluginPath, "Export");
        var characters = Characters.c;
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }
        Directory.CreateDirectory(exportPath);
        foreach (var character in characters)
        {
            if (character.id == 0)
            {
                continue;
            }
            var moddedCharacter = new CharacterWithModdedData(character);
            var json = JsonConvert.SerializeObject(moddedCharacter, Formatting.Indented);
            var path = Path.Combine(exportPath, $"{character.id}_{character.name}.json");
            File.WriteAllText(path, json);
        }
    }
    
    public static Character ImportCharacter(string path)
    {
        var json = File.ReadAllText(path);
        var character = JsonConvert.DeserializeObject<CharacterWithModdedData>(json);
        if (character != null)
        {
            var internalCharacter = character.ModdedToCharacter();
            return internalCharacter;
        }
        return null;
    }
}