using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using WECCL.Content;
using WECCL.Saves;
using WECCL.Utils;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SaveFilePatch
{
    /*
     * GameSaveFile.NOKFJAECIGL is called when the game restores the default data
     * This patch resets the character and federation counts.
     * It also resets the star (wrestler) and booker to 1 if they are greater than the new character count.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.NOKFJAECIGL))]
    [HarmonyPostfix]
    public static void SaveFile_NOKFJAECIGL()
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
            Array.Resize(ref GameSaveFile.GPFFEHKLNLD.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedChars, Characters.no_chars + 1);
            
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
     * GameSaveFile.OLAGCFPPEPB is called when the game loads the save file.
     * This prefix patch is used to update character counts and arrays to accommodate the custom content.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.OLAGCFPPEPB))]
    [HarmonyPrefix]
    public static void GameSaveFile_OLAGCFPPEPB_PRE(int IHLLJIMFJEN)
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
            Array.Resize(ref GameSaveFile.GPFFEHKLNLD.charUnlock, Characters.no_chars + 1);

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
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.OLAGCFPPEPB))]
    [HarmonyPostfix]
    public static void GameSaveFile_OLAGCFPPEPB_POST(int IHLLJIMFJEN)
    {
        string save = Application.persistentDataPath + "/Save.bytes";
        if (!File.Exists(save))
        {
            return;
        }
        if (fedCharCount != null && GameSaveFile.GPFFEHKLNLD.savedFeds != null)
        {
            for (int i = 1; i <= Characters.no_feds; i++)
            {
                var count = Plugin.BaseFedLimit.Value <= 48 ? fedCharCount[i] + 1 : Plugin.BaseFedLimit.Value + 1;
                if (GameSaveFile.GPFFEHKLNLD.savedFeds[i] != null)
                {
                    GameSaveFile.GPFFEHKLNLD.savedFeds[i].size = fedCharCount[i];
                    if (count > GameSaveFile.GPFFEHKLNLD.savedFeds[i].roster.Length)
                    {
                        Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedFeds[i].roster, count);
                    }
                }
                Array.Resize(ref Characters.fedData[i].roster, count);
            }
        }

        try
        {
            SaveRemapper.PatchCustomContent(ref GameSaveFile.GPFFEHKLNLD);
            foreach (Tuple<string,string, Character> triplet in ImportedCharacters)
            {
                Plugin.Log.LogInfo($"Importing character {triplet.Item3.name} with id {triplet.Item3.id}.");
                string nameWithGuid = triplet.Item1;
                string overrideMode = triplet.Item2;
                Character importedCharacter = triplet.Item3;
                
                bool previouslyImported = CheckIfPreviouslyImported(nameWithGuid);
                
                switch (overrideMode.ToLower())
                {
                    case "override":
                        int id = importedCharacter.id;
                        Character oldCharacter = GameSaveFile.GPFFEHKLNLD.savedChars[id];
                        string name = importedCharacter.name;
                        string oldCharacterName = oldCharacter.name;
                        GameSaveFile.GPFFEHKLNLD.savedChars[id] = importedCharacter;
                        if (importedCharacter.fed != oldCharacter.fed)
                        {
                            if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 ==
                                GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster,
                                    GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length >
                                    Characters.fedLimit)
                                {
                                    Characters.fedLimit++;
                                }
                            }

                            GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                            GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                .roster[GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id;
                            GameSaveFile.GPFFEHKLNLD.savedFeds[oldCharacter.fed].HKNHEJHIJLL(id);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id} and name {name}, overwriting character with name {oldCharacterName}.");
                        break;
                    case "append":
                        if (!previouslyImported)
                        {
                            int id2 = Characters.no_chars + 1;
                            importedCharacter.id = id2;
                            if (GameSaveFile.GPFFEHKLNLD.savedChars.Length <= id2)
                            {
                                Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedChars, id2 + 1);
                                Array.Resize(ref GameSaveFile.GPFFEHKLNLD.charUnlock, id2 + 1);
                                Array.Resize(ref Characters.c, id2 + 1);
                                Array.Resize(ref Progress.charUnlock, id2 + 1);
                                GameSaveFile.GPFFEHKLNLD.charUnlock[id2] = 1;
                                Progress.charUnlock[id2] = 1;
                            }
                            else
                            {
                                Plugin.Log.LogWarning(
                                    $"The array of characters is larger than the number of characters. This should not happen. The character {GameSaveFile.GPFFEHKLNLD.savedChars[id2].name} will be overwritten.");
                            }

                            GameSaveFile.GPFFEHKLNLD.savedChars[id2] = importedCharacter;
                            if (importedCharacter.fed != 0)
                            {
                                if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 ==
                                    GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster,
                                        GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                    if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                                GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                    .roster[GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id2;
                            }

                            Characters.no_chars++;
                            Plugin.Log.LogInfo(
                                $"Imported character with id {id2} and name {importedCharacter.name}. Incremented number of characters to {Characters.no_chars}.");
                            break;
                        }
                        importedCharacter.id = GetPreviouslyImportedId(nameWithGuid);
                        goto case "merge";
                    case "merge":
                        int id3 = importedCharacter.id;
                        Character oldCharacter2 = GameSaveFile.GPFFEHKLNLD.savedChars[id3];
                        string name2 = importedCharacter.name;
                        string oldCharacterName2 = oldCharacter2.name;
                        foreach (FieldInfo field in typeof(Character).GetFields())
                        {
                            if (field.GetValue(importedCharacter) != default)
                            {
                                field.SetValue(oldCharacter2, field.GetValue(importedCharacter));
                            }
                        }

                        GameSaveFile.GPFFEHKLNLD.savedChars[id3] = oldCharacter2;
                        if (importedCharacter.fed != oldCharacter2.fed)
                        {
                            if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 ==
                                GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster,
                                    GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                if (GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length >
                                    Characters.fedLimit)
                                {
                                    Characters.fedLimit++;
                                }
                            }

                            GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                            GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                .roster[GameSaveFile.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id3;
                            GameSaveFile.GPFFEHKLNLD.savedFeds[oldCharacter2.fed].HKNHEJHIJLL(id3);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id3} and name {name2}, merging with character with name {oldCharacterName2}.");
                        break;
                }
                ContentMappings.ContentMap.AddPreviouslyImportedCharacter(nameWithGuid, importedCharacter.id);
            }

            GameSaveFile.GPFFEHKLNLD.FGMMAKKGCOG(IHLLJIMFJEN);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    [HarmonyPatch(typeof(Roster), nameof(Roster.ENFAGKBOOAN))]
    [HarmonyPostfix]
    public static void Roster_ENFAGKBOOAN(Roster __instance)
    {
        if (Plugin.BaseFedLimit.Value > 48 && __instance.roster.Length < Plugin.BaseFedLimit.Value + 1)
        {
            Array.Resize(ref __instance.roster, Plugin.BaseFedLimit.Value + 1);
        }
    }


    private static bool CheckIfPreviouslyImported(string nameWithGuid)
    {
        return ContentMappings.ContentMap.PreviouslyImportedCharacters.Contains(nameWithGuid);
    }
    
    
    private static int GetPreviouslyImportedId(string nameWithGuid)
    {
        return ContentMappings.ContentMap.PreviouslyImportedCharacterIds[ContentMappings.ContentMap.PreviouslyImportedCharacters.IndexOf(nameWithGuid)];
    }


    /*
     * GameSaveFile.ICAMLDGDPHC is called when the player saves the game.
     * This patch saves the current custom content map and exports all characters.
     */
    [HarmonyPatch(typeof(GameSaveFile), nameof(GameSaveFile.ICAMLDGDPHC))]
    [HarmonyPostfix]
    public static void GameSaveFile_ICAMLDGDPHC(int IHLLJIMFJEN)
    {
        SaveCurrentMap();
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