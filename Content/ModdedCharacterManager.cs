using Newtonsoft.Json;
using WECCL.Saves;
using WECCL.Utils;

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
        int i = 0;
        foreach (Character character in characters)
        {
            i++;
            if (i == 1)
            {
                continue;
            }

            BetterCharacterData moddedCharacter = BetterCharacterData.FromRegularCharacter(character, characters);
            BetterCharacterDataFile file = new()
            {
                characterData = moddedCharacter,
                overrideMode = "append"
            };
            string json = JsonConvert.SerializeObject(file, Formatting.Indented);
            string path = Path.Combine(exportPath, $"{character.id}_{FileNameUtils.Escape(character.name)}.json");
            File.WriteAllText(path, json);
        }
    }

    public static Character ImportCharacter(string path, out string overrideMode)
    {
        GameSaveFile.GPFFEHKLNLD.savedChars[0] = null;
        string json = File.ReadAllText(path);
        BetterCharacterDataFile character = JsonConvert.DeserializeObject<BetterCharacterDataFile>(json);
        if (character != null)
        {
            Character internalCharacter = character.CharacterData.ToRegularCharacter(GameSaveFile.GPFFEHKLNLD.savedChars);
            overrideMode = character.OverrideMode;
            if (character.OverrideMode != "append")
            {
                overrideMode += $"-{character.FindMode}";
            }
            return internalCharacter;
        }
        overrideMode = null;
        return null;
    }
}