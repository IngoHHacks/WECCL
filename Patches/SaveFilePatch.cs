using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using WECCL.Content;
using WECCL.Saves;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SaveFilePatch
{
    private static int[] fedCharCount;

    /*
     * GameSaveFile.OKDAOPACMLB is called when the game restores the default data
     * This patch resets the character and federation counts.
     * It also resets the star (wrestler) and booker to 1 if they are greater than the new character count.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.OKDAOPACMLB))]
    [HarmonyPostfix]
    public static void SaveFile_OKDAOPACMLB()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            return;
        }
        try
        {
            Characters.no_chars = 350;
            Characters.fedLimit = Plugin.BaseFedLimit.Value;

            if (Characters.star > 350)
            {
                Characters.star = 1;
            }

            if (Characters.booker > 350)
            {
                Characters.booker = 1;
            }

            Array.Resize(ref Characters.c, Characters.no_chars + 1);
            Array.Resize(ref Progress.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.savedChars, Characters.no_chars + 1);

            CharacterMappings.CharacterMap.PreviouslyImportedCharacters.Clear();
            CharacterMappings.CharacterMap.PreviouslyImportedCharacterIds.Clear();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    /*
     * GameSaveFile.DGJGBLELPNF is called when the game loads the save file.
     * This prefix patch is used to update character counts and arrays to accommodate the custom content.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.DGJGBLELPNF))]
    [HarmonyPrefix]
    public static void GameSaveFile_DGJGBLELPNF_PRE(int EMGACFNBENO)
    {
        try
        {
            string save = Application.persistentDataPath + "/Save.bytes";
            if (!File.Exists(save))
            {
                return;
            }

            FileStream fileStream = new(save, FileMode.Open);
            SaveData data = new BinaryFormatter().Deserialize(fileStream) as SaveData;
            Characters.no_chars = data!.savedChars.Length - 1;
            fedCharCount = new int[Characters.no_feds + 1];
            foreach (Character c in data.savedChars)
            {
                if (c != null)
                {
                    fedCharCount[c.fed]++;
                }
            }

            Characters.fedLimit = Math.Max(Plugin.BaseFedLimit.Value, fedCharCount.Max() + 1);
            Array.Resize(ref Characters.c, Characters.no_chars + 1);
            Array.Resize(ref Progress.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.charUnlock, Characters.no_chars + 1);

            fileStream.Close();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    /*
     * This postfix patch is used to remap any custom content that has moved, and also add the imported characters.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.DGJGBLELPNF))]
    [HarmonyPostfix]
    public static void GameSaveFile_DGJGBLELPNF_POST(int EMGACFNBENO)
    {
        string save = Application.persistentDataPath + "/Save.bytes";
        if (!File.Exists(save))
        {
            return;
        }

        if (fedCharCount != null && GameSaveFile.GGKPJPBDFFJ.savedFeds != null)
        {
            for (int i = 1; i <= Characters.no_feds; i++)
            {
                int count = Plugin.BaseFedLimit.Value <= 48 ? fedCharCount[i] + 1 : Plugin.BaseFedLimit.Value + 1;
                if (GameSaveFile.GGKPJPBDFFJ.savedFeds[i] != null)
                {
                    GameSaveFile.GGKPJPBDFFJ.savedFeds[i].size = fedCharCount[i];
                    if (count > GameSaveFile.GGKPJPBDFFJ.savedFeds[i].roster.Length)
                    {
                        Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.savedFeds[i].roster, count);
                    }
                }

                Array.Resize(ref Characters.fedData[i].roster, count);
            }
        }

        try
        {
            SaveRemapper.PatchCustomContent(ref GameSaveFile.GGKPJPBDFFJ);
            foreach (BetterCharacterDataFile file in ImportedCharacters)
            {
                string nameWithGuid = file._guid;
                string overrideMode = file.OverrideMode + "-" + file.FindMode;
                overrideMode = overrideMode.ToLower();
                if (overrideMode.EndsWith("-"))
                {
                    overrideMode = overrideMode.Substring(0, overrideMode.Length - 1);
                }

                try
                {
                    bool previouslyImported = CheckIfPreviouslyImported(nameWithGuid);
                    if (previouslyImported)
                    {
                        Plugin.Log.LogInfo(
                            $"Character with name {file.CharacterData.name ?? "null"} was previously imported. Skipping.");
                        continue;
                    }
                    if (!overrideMode.Contains("append"))
                    {
                        Plugin.Log.LogInfo(
                            $"Importing character {file.CharacterData.name ?? "null"} with id {file.CharacterData.id.ToString() ?? "null"} using mode {overrideMode}");
                    }
                    else
                    {
                        Plugin.Log.LogInfo(
                            $"Appending character {file.CharacterData.name ?? "null"} to next available id using mode {overrideMode}");
                    }

                    Character importedCharacter = null;
                    if (!overrideMode.Contains("merge"))
                    {
                        importedCharacter = file.CharacterData.ToRegularCharacter(GameSaveFile.GGKPJPBDFFJ.savedChars);
                    }
                    switch (overrideMode)
                    {
                        case "override-id":
                        case "override-name":
                        case "override-name_then_id":
                            int id = overrideMode.Contains("id") ? importedCharacter.id : -1;
                            if (overrideMode.Contains("name"))
                            {
                                string find = file.FindName ?? importedCharacter.name;
                                try
                                {
                                    id = GameSaveFile.GGKPJPBDFFJ.savedChars
                                        .Single(c => c != null && c.name != null && c.name == find).id;
                                }
                                catch (Exception e)
                                {
                                    // ignored
                                }
                            }

                            if (id == -1)
                            {
                                Plugin.Log.LogWarning(
                                    $"Could not find character with id {importedCharacter.id} and name {importedCharacter.name} using override mode {overrideMode}. Skipping.");
                                break;
                            }

                            Character oldCharacter = GameSaveFile.GGKPJPBDFFJ.savedChars[id];
                            string name = importedCharacter.name;
                            string oldCharacterName = oldCharacter.name;
                            GameSaveFile.GGKPJPBDFFJ.savedChars[id] = importedCharacter;
                            if (importedCharacter.fed != oldCharacter.fed)
                            {
                                if (GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size + 1 ==
                                    GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster,
                                        GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size + 2);
                                    if (GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size++;
                                GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed]
                                    .roster[GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size] = id;
                                GameSaveFile.GGKPJPBDFFJ.savedFeds[oldCharacter.fed].MFDEPHCPKJC(id);
                            }

                            Plugin.Log.LogInfo(
                                $"Imported character with id {id} and name {name}, overwriting character with name {oldCharacterName}.");
                            break;
                        case "append":
                            Plugin.Log.LogInfo($"Appending character {importedCharacter.name ?? "null"} to next available id.");
                            int id2 = Characters.no_chars + 1;
                            importedCharacter.id = id2;
                            if (GameSaveFile.GGKPJPBDFFJ.savedChars.Length <= id2)
                            {
                                Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.savedChars, id2 + 1);
                                Array.Resize(ref GameSaveFile.GGKPJPBDFFJ.charUnlock, id2 + 1);
                                Array.Resize(ref Characters.c, id2 + 1);
                                Array.Resize(ref Progress.charUnlock, id2 + 1);
                                GameSaveFile.GGKPJPBDFFJ.charUnlock[id2] = 1;
                                Progress.charUnlock[id2] = 1;
                            }
                            else
                            {
                                Plugin.Log.LogWarning(
                                    $"The array of characters is larger than the number of characters. This should not happen. The character {GameSaveFile.GGKPJPBDFFJ.savedChars[id2].name} will be overwritten.");
                            }

                            GameSaveFile.GGKPJPBDFFJ.savedChars[id2] = importedCharacter;
                            if (importedCharacter.fed != 0)
                            {
                                if (GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size + 1 ==
                                    GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(
                                        ref GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster,
                                        GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size + 2);
                                    if (GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size++;
                                GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed]
                                    .roster[GameSaveFile.GGKPJPBDFFJ.savedFeds[importedCharacter.fed].size] = id2;
                            }

                            Characters.no_chars++;
                            Plugin.Log.LogInfo(
                                $"Imported character with id {id2} and name {importedCharacter.name}. Incremented number of characters to {Characters.no_chars}.");
                            break;
                        case "merge-id":
                        case "merge-name":
                        case "merge-name_then_id":
                            int id3 = overrideMode.Contains("id") ? file.CharacterData.id ?? -1 : -1;
                            if (overrideMode.Contains("name"))
                            {
                                string find = file.FindName ?? file.CharacterData.name ??
                                    throw new Exception($"No name found for file {nameWithGuid}");
                                try
                                {
                                    id3 = GameSaveFile.GGKPJPBDFFJ.savedChars
                                        .Single(c => c != null && c.name != null && c.name == find).id;
                                }
                                catch (Exception e)
                                {
                                    // ignored
                                }
                            }

                            if (id3 == -1)
                            {
                                Plugin.Log.LogWarning(
                                    $"Could not find character with id {file.CharacterData.id?.ToString() ?? "null"} and name {file.FindName ?? file.CharacterData.name ?? "null"} using override mode {overrideMode}. Skipping.");
                                break;
                            }

                            Character oldCharacter2 = GameSaveFile.GGKPJPBDFFJ.savedChars[id3];
                            file.CharacterData.MergeIntoCharacter(oldCharacter2);

                            GameSaveFile.GGKPJPBDFFJ.savedChars[id3] = oldCharacter2;
                            if (file.CharacterData.fed != null && file.CharacterData.fed.Value != oldCharacter2.fed)
                            {
                                if (GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].size + 1 ==
                                    GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].roster.Length)
                                {
                                    Array.Resize(
                                        ref GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].roster,
                                        GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].size + 2);
                                    if (GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].size++;
                                GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value]
                                        .roster[GameSaveFile.GGKPJPBDFFJ.savedFeds[file.CharacterData.fed.Value].size] =
                                    id3;
                                GameSaveFile.GGKPJPBDFFJ.savedFeds[oldCharacter2.fed].MFDEPHCPKJC(id3);
                            }

                            Plugin.Log.LogInfo(
                                $"Imported character with id {id3} and name {file.CharacterData.name ?? "null"}, merging with existing character: {oldCharacter2.name}.");
                            break;
                        default:
                            throw new Exception($"Unknown override mode {overrideMode}");
                    }

                    CharacterMappings.CharacterMap.AddPreviouslyImportedCharacter(nameWithGuid,
                        importedCharacter?.id ?? file.CharacterData.id ?? -1);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Error while importing character {nameWithGuid}.");
                    Plugin.Log.LogError(e);
                }
            }

            GameSaveFile.GGKPJPBDFFJ.ADKAODCEGHB(EMGACFNBENO);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Error while importing characters.");
            Plugin.Log.LogError(e);
        }
    }

    [HarmonyPatch(typeof(Roster), nameof(Roster.FFGHCMGIDOB))]
    [HarmonyPostfix]
    public static void Roster_FFGHCMGIDOB(Roster __instance)
    {
        if (Plugin.BaseFedLimit.Value > 48 && __instance.roster.Length < Plugin.BaseFedLimit.Value + 1)
        {
            Array.Resize(ref __instance.roster, Plugin.BaseFedLimit.Value + 1);
        }
    }


    private static bool CheckIfPreviouslyImported(string nameWithGuid)
    {
        if (nameWithGuid.EndsWith(".json"))
        {
            nameWithGuid = nameWithGuid.Substring(0, nameWithGuid.Length - 5);
        }
        else if (nameWithGuid.EndsWith(".character"))
        {
            nameWithGuid = nameWithGuid.Substring(0, nameWithGuid.Length - 10);
        }
        
        return CharacterMappings.CharacterMap.PreviouslyImportedCharacters.Contains(nameWithGuid);
    }


    /*
     * GameSaveFile.IFNAOOEOLLK is called when the player saves the game.
     * This patch saves the current custom content map and exports all characters.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.IFNAOOEOLLK))]
    [HarmonyPostfix]
    public static void GameSaveFile_IFNAOOEOLLK(int EMGACFNBENO)
    {
        SaveCurrentMap();
        CharacterMappings.CharacterMap.Save();
        MetaFile.Data.Save();
        if (Plugin.AutoExportCharacters.Value)
        {
            ModdedCharacterManager.SaveAllCharacters();
        }

        if (Plugin.DeleteImportedCharacters.Value)
        {
            foreach (string file in FilesToDeleteOnSave)
            {
                File.Delete(file);
            }
        }
    }


    /*
    Special cases:
    BodyFemale is negative Flesh[2]
    FaceFemale is negative Material[3]
    SpecialFootwear is negative Material[14] and [15]
    TransparentHairMaterial is negative Material[17]
    TransparentHairHairstyle is negative Shape[17]
    Kneepad is negative Material[24] and [25]
     */

    internal static void SaveCurrentMap()
    {
        ContentMappings.ContentMap.Save();
    }

    internal static ContentMappings LoadPreviousMap()
    {
        return ContentMappings.Load();
    }
}