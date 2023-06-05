using System.Runtime.Serialization.Formatters.Binary;
using WECCL.Content;
using WECCL.Saves;
using WECCL.Utils;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SaveFilePatch
{
    /*
     * GameSaveFile.LJAOBLOCLFK is called when the game restores the default data
     * This patch resets the character and federation counts.
     * It also resets the star (wrestler) and booker to 1 if they are greater than the new character count.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.LJAOBLOCLFK))]
    [HarmonyPostfix]
    public static void SaveFile_LJAOBLOCLFK()
    {
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
            Array.Resize(ref GameSaveFile.IOKDNAOAENK.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedChars, Characters.no_chars + 1);
            
            ContentMappings.ContentMap.PreviouslyImportedCharacters.Clear();
            ContentMappings.ContentMap.PreviouslyImportedCharacterIds.Clear();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    private static int[] fedCharCount;
    
    /*
     * GameSaveFile.HCKKGEAPBMK is called when the game loads the save file.
     * This prefix patch is used to update character counts and arrays to accommodate the custom content.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.HCKKGEAPBMK))]
    [HarmonyPrefix]
    public static void GameSaveFile_HCKKGEAPBMK_PRE(int BHCMGLCHEGO)
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
            Array.Resize(ref GameSaveFile.IOKDNAOAENK.charUnlock, Characters.no_chars + 1);

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
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.HCKKGEAPBMK))]
    [HarmonyPostfix]
    public static void GameSaveFile_HCKKGEAPBMK_POST(int BHCMGLCHEGO)
    {
        string save = Application.persistentDataPath + "/Save.bytes";
        if (!File.Exists(save))
        {
            return;
        }
        if (fedCharCount != null && GameSaveFile.IOKDNAOAENK.savedFeds != null)
        {
            for (int i = 1; i <= Characters.no_feds; i++)
            {
                var count = Plugin.BaseFedLimit.Value <= 48 ? fedCharCount[i] + 1 : Plugin.BaseFedLimit.Value + 1;
                if (GameSaveFile.IOKDNAOAENK.savedFeds[i] != null)
                {
                    GameSaveFile.IOKDNAOAENK.savedFeds[i].size = fedCharCount[i];
                    if (count > GameSaveFile.IOKDNAOAENK.savedFeds[i].roster.Length)
                    {
                        Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedFeds[i].roster, count);
                    }
                }
                Array.Resize(ref Characters.fedData[i].roster, count);
            }
        }

        try
        {
            SaveRemapper.PatchCustomContent(ref GameSaveFile.IOKDNAOAENK);
            foreach (BetterCharacterDataFile file in ImportedCharacters)
            {
                Plugin.Log.LogInfo($"Importing character {file.CharacterData.name} with id {file.CharacterData.id}.");
                string nameWithGuid = file._guid;
                string overrideMode = file.OverrideMode + file.FindMode;
                Character importedCharacter =
                    file.CharacterData.ToRegularCharacter(GameSaveFile.IOKDNAOAENK.savedChars);
                
                bool previouslyImported = CheckIfPreviouslyImported(nameWithGuid);

                overrideMode = overrideMode.ToLower();
                
                switch (overrideMode)
                {
                    case "override-id":
                    case "override-name":
                    case "override-name_then_id":
                        int id = overrideMode.Contains("id") ? importedCharacter.id : -1;
                        if (overrideMode.Contains("name"))
                        {
                            var find = file.FindName ?? importedCharacter.name;
                            try
                            {
                                id = GameSaveFile.IOKDNAOAENK.savedChars.Single(c => c != null && c.name != null && c.name == find).id;
                            }
                            catch (Exception e)
                            {
                                Plugin.Log.LogWarning($"Could not find character with name {find}, or multiple characters with the same name exist.");
                            }
                        }

                        if (id == -1)
                        {
                            Plugin.Log.LogError($"Could not find character with id {importedCharacter.id} and name {importedCharacter.name} using override mode {overrideMode}. Skipping.");
                            break;
                        }
                        Character oldCharacter = GameSaveFile.IOKDNAOAENK.savedChars[id];
                        string name = importedCharacter.name;
                        string oldCharacterName = oldCharacter.name;
                        GameSaveFile.IOKDNAOAENK.savedChars[id] = importedCharacter;
                        if (importedCharacter.fed != oldCharacter.fed)
                        {
                            if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 1 ==
                                GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster,
                                    GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 2);
                                if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length >
                                    Characters.fedLimit)
                                {
                                    Characters.fedLimit++;
                                }
                            }

                            GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size++;
                            GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed]
                                .roster[GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size] = id;
                            GameSaveFile.IOKDNAOAENK.savedFeds[oldCharacter.fed].JHDJHBMIEOG(id);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id} and name {name}, overwriting character with name {oldCharacterName}.");
                        break;
                    case "append":
                        if (!previouslyImported)
                        {
                            int id2 = Characters.no_chars + 1;
                            importedCharacter.id = id2;
                            if (GameSaveFile.IOKDNAOAENK.savedChars.Length <= id2)
                            {
                                Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedChars, id2 + 1);
                                Array.Resize(ref GameSaveFile.IOKDNAOAENK.charUnlock, id2 + 1);
                                Array.Resize(ref Characters.c, id2 + 1);
                                Array.Resize(ref Progress.charUnlock, id2 + 1);
                                GameSaveFile.IOKDNAOAENK.charUnlock[id2] = 1;
                                Progress.charUnlock[id2] = 1;
                            }
                            else
                            {
                                Plugin.Log.LogWarning(
                                    $"The array of characters is larger than the number of characters. This should not happen. The character {GameSaveFile.IOKDNAOAENK.savedChars[id2].name} will be overwritten.");
                            }

                            GameSaveFile.IOKDNAOAENK.savedChars[id2] = importedCharacter;
                            if (importedCharacter.fed != 0)
                            {
                                if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 1 ==
                                    GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster,
                                        GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 2);
                                    if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size++;
                                GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed]
                                    .roster[GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size] = id2;
                            }

                            Characters.no_chars++;
                            Plugin.Log.LogInfo(
                                $"Imported character with id {id2} and name {importedCharacter.name}. Incremented number of characters to {Characters.no_chars}.");
                            break;
                        }
                        importedCharacter.id = GetPreviouslyImportedId(nameWithGuid);
                        overrideMode = "merge-name_then_id";
                        goto case "merge-name_then_id";
                    case "merge-id":
                    case "merge-name":
                    case "merge-name_then_id":
                        int id3 = (overrideMode.Contains("id") ? importedCharacter.id : -1);
                        if (overrideMode.Contains("name"))
                        {
                            var find = file.FindName ?? importedCharacter.name;
                            try
                            {
                                
                                id3 = GameSaveFile.IOKDNAOAENK.savedChars.Single(c => c != null && c.name != null && c.name == find).id;
                            }
                            catch (Exception e)
                            {
                                Plugin.Log.LogWarning($"Could not find character with name {find}, or multiple characters with the same name exist.");
                            }
                        }
                        if (id3 == -1)
                        {
                            Plugin.Log.LogError($"Could not find character with id {importedCharacter.id} and name {importedCharacter.name} using override mode {overrideMode}. Skipping.");
                            break;
                        }
                        Character oldCharacter2 = GameSaveFile.IOKDNAOAENK.savedChars[id3];
                        file.CharacterData.MergeIntoCharacter(oldCharacter2);

                        GameSaveFile.IOKDNAOAENK.savedChars[id3] = oldCharacter2;
                        if (importedCharacter.fed != oldCharacter2.fed)
                        {
                            if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 1 ==
                                GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster,
                                    GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size + 2);
                                if (GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].roster.Length >
                                    Characters.fedLimit)
                                {
                                    Characters.fedLimit++;
                                }
                            }

                            GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size++;
                            GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed]
                                .roster[GameSaveFile.IOKDNAOAENK.savedFeds[importedCharacter.fed].size] = id3;
                            GameSaveFile.IOKDNAOAENK.savedFeds[oldCharacter2.fed].JHDJHBMIEOG(id3);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id3} and name {importedCharacter.name}, merging with existing character: {oldCharacter2.name}.");
                        break;
                }
                ContentMappings.ContentMap.AddPreviouslyImportedCharacter(nameWithGuid, importedCharacter.id);
            }

            GameSaveFile.IOKDNAOAENK.NCOHBLMFLLN(BHCMGLCHEGO);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    [HarmonyPatch(typeof(Roster), nameof(Roster.MMIIOIIPKHB))]
    [HarmonyPostfix]
    public static void Roster_MMIIOIIPKHB(Roster __instance)
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
        
        return ContentMappings.ContentMap.PreviouslyImportedCharacters.Contains(nameWithGuid);
    }
    
    
    private static int GetPreviouslyImportedId(string nameWithGuid)
    {
        return ContentMappings.ContentMap.PreviouslyImportedCharacterIds[ContentMappings.ContentMap.PreviouslyImportedCharacters.IndexOf(nameWithGuid)];
    }


    /*
     * GameSaveFile.PPDKHELLMKL is called when the player saves the game.
     * This patch saves the current custom content map and exports all characters.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.PPDKHELLMKL))]
    [HarmonyPostfix]
    public static void GameSaveFile_PPDKHELLMKL(int BHCMGLCHEGO)
    {
        SaveCurrentMap();
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