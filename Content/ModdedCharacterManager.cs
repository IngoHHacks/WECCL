using Newtonsoft.Json;

namespace WECCL.Content;

public class ModdedCharacterManager
{
    public static void SaveAllCharacters()
    {
        string exportPath = Plugin.ExportDir.FullName;
        Character[] characters = Characters.c;
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }

        Directory.CreateDirectory(exportPath);
        foreach (Character character in characters)
        {
            if (character.id == 0)
            {
                continue;
            }

            CharacterWithModdedData moddedCharacter = new(character);
            string json = JsonConvert.SerializeObject(moddedCharacter, Formatting.Indented);
            string path = Path.Combine(exportPath, $"{character.id}_{character.name}.json");
            File.WriteAllText(path, json);
        }
    }

    public static Character ImportCharacter(string path, out string overrideMode)
    {
        string json = File.ReadAllText(path);
        CharacterWithModdedData character = JsonConvert.DeserializeObject<CharacterWithModdedData>(json);
        if (character != null)
        {
            Character internalCharacter = character.ModdedToCharacter();
            overrideMode = character.OverrideMode;
            return internalCharacter;
        }

        overrideMode = null;
        return null;
    }
}