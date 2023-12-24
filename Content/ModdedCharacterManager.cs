using Newtonsoft.Json;
using WECCL.Saves;

namespace WECCL.Content;

internal class ModdedCharacterManager
{
    public static void SaveAllCharacters()
    {
        string exportPath = Locations.Export.FullName;
        Character[] characters = Characters.c;
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }

        Directory.CreateDirectory(exportPath);
        int i = 0;
        foreach (Character character in characters)
        {
            i++;
            if (i == 1)
            {
                continue;
            }

            BetterCharacterData moddedCharacter = BetterCharacterData.FromRegularCharacter(character, characters);
            BetterCharacterDataFile file = new() { characterData = moddedCharacter, overrideMode = "append" };
            string json = JsonConvert.SerializeObject(file, Formatting.Indented);
            string path = Path.Combine(exportPath, $"{character.id}_{Escape(character.name)}.character");
            File.WriteAllText(path, json);
        }
    }
}