using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using WECCL.Content;
using WECCL.Saves;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SaveFilePatch
{
    private static int[] _fedCharCount;

    /*
     * Patch
     * - Resets the character and federation counts when default data is loaded.
     * - Resets the star (wrestler) and booker to 1 if they are greater than the new character count when default data is loaded.
     */
    [HarmonyPatch(typeof(UnmappedSaveSystem), nameof(UnmappedSaveSystem.NJMFCPGCKNL))]
    [HarmonyPrefix]
    public static void SaveSystem_NJMFCPGCKNL_Pre()
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
            Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.savedChars, Characters.no_chars + 1);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    /*
     * Patch:
     * - Fixes corrupted save data before rosters are loaded.
     */
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.CDLIDDFKFEL))]
    [HarmonyPrefix]
    public static void SaveData_CDLIDDFKFEL(SaveData __instance, int FIHDANPPMGC)
    {
        if (GLPGLJAJJOP.APPDIBENDAH.savedChars != null)
        {
            SaveRemapper.FixBrokenSaveData();
        }
    }
    
    /*
     * Patch:
     * - Clears the previously imported characters list after default data is loaded.
     * - Fixes corrupted save data after default data is loaded.
     */
    [HarmonyPatch(typeof(UnmappedSaveSystem), nameof(UnmappedSaveSystem.NJMFCPGCKNL))]
    [HarmonyPostfix]
    public static void SaveSystem_NJMFCPGCKNL_Post()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            return;
        }
        try
        {
            SaveRemapper.FixBrokenSaveData();
            
            CharacterMappings.CharacterMap.PreviouslyImportedCharacters.Clear();
            CharacterMappings.CharacterMap.PreviouslyImportedCharacterIds.Clear();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    /*
     * Patch:
     * - Changes the save file name to 'ModdedSave.bytes' (or whatever the user has set) during user load.
     */
    [HarmonyPatch(typeof(UnmappedSaveSystem), nameof(UnmappedSaveSystem.BONMDGJIBFP))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SaveSystem_BONMDGJIBFP(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string str && str == "Save")
            {
                instruction.operand = Plugin.SaveFileName.Value;
            }
            yield return instruction;
        }
    }
    
    /*
     * Patch:
     * - Changes the save file name to 'ModdedSave.bytes' (or whatever the user has set) during user save.
     */
    [HarmonyPatch(typeof(UnmappedSaveSystem), nameof(UnmappedSaveSystem.OIIAHNGBNIF))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SaveSystem_OIIAHNGBNIF_Trans(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string str && str == "Save")
            {
                instruction.operand = Plugin.SaveFileName.Value;
            }
            yield return instruction;
        }
    }

    /*
     * SaveData.BONMDGJIBFP is called when the game loads the save file.
     * This prefix patch is used to update character counts and arrays to accommodate the custom content.
     */
    [HarmonyPatch(typeof(GLPGLJAJJOP), nameof(GLPGLJAJJOP.BONMDGJIBFP))]
    [HarmonyPrefix]
    public static void SaveData_BONMDGJIBFP_PRE(int FIHDANPPMGC)
    {
        try
        {
            string save = Locations.SaveFile.FullName;
            if (!File.Exists(save))
            {
                string  vanillaSave = Locations.SaveFileVanilla.FullName;
                if (File.Exists(vanillaSave))
                {
                    File.Copy(vanillaSave, save);
                }
                else
                {
                    return;
                }
            }

            FileStream fileStream = new(save, FileMode.Open);
            SaveData data = new BinaryFormatter().Deserialize(fileStream) as SaveData;
            Characters.no_chars = data!.savedChars.Length - 1;
            _fedCharCount = new int[Characters.no_feds + 1];
            foreach (Character c in data.savedChars)
            {
                if (c != null)
                {
                    _fedCharCount[c.fed]++;
                }
            }

            Characters.fedLimit = Math.Max(Plugin.BaseFedLimit.Value, _fedCharCount.Max() + 1);
            Array.Resize(ref Characters.c, Characters.no_chars + 1);
            Array.Resize(ref Progress.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.charUnlock, Characters.no_chars + 1);

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
    [HarmonyPatch(typeof(GLPGLJAJJOP), nameof(GLPGLJAJJOP.BONMDGJIBFP))]
    [HarmonyPostfix]
    public static void SaveData_BONMDGJIBFP_POST(int FIHDANPPMGC)
    {
        string save = Locations.SaveFile.FullName;
        if (!File.Exists(save))
        {
            string vanillaSave = Locations.SaveFileVanilla.FullName;
            if (File.Exists(vanillaSave))
            {
                File.Copy(vanillaSave, save);
            }
            else
            {
                return;
            }
        }

        if (_fedCharCount != null && GLPGLJAJJOP.APPDIBENDAH.savedFeds != null)
        {
            for (int i = 0; i <= Characters.no_feds; i++)
            {
                int count = Math.Max(_fedCharCount[i] + 1, Plugin.BaseFedLimit.Value + 1);
                if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[i] != null)
                {
                    GLPGLJAJJOP.APPDIBENDAH.savedFeds[i].size = _fedCharCount[i];
                    if (count > GLPGLJAJJOP.APPDIBENDAH.savedFeds[i].roster.Length)
                    {
                        Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.savedFeds[i].roster, count);
                    }
                }

                Array.Resize(ref Characters.fedData[i].roster, count);
            }
        }

        try
        {
            SaveRemapper.FixBrokenSaveData();
            SaveRemapper.PatchCustomContent(ref GLPGLJAJJOP.APPDIBENDAH);
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
                        importedCharacter = file.CharacterData.ToRegularCharacter(GLPGLJAJJOP.APPDIBENDAH.savedChars);
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
                                    id = GLPGLJAJJOP.APPDIBENDAH.savedChars
                                        .Single(c => c != null && c.name != null && c.name == find).id;
                                }
                                catch (Exception)
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

                            Character oldCharacter = GLPGLJAJJOP.APPDIBENDAH.savedChars[id];
                            string name = importedCharacter.name;
                            string oldCharacterName = oldCharacter.name;
                            GLPGLJAJJOP.APPDIBENDAH.savedChars[id] = importedCharacter;
                            if (importedCharacter.fed != oldCharacter.fed)
                            {
                                if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size + 1 ==
                                    GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster,
                                        GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size + 2);
                                    if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size++;
                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed]
                                    .roster[GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size] = id;
                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[oldCharacter.fed].BIDMDHABIGJ(id);
                            }

                            Plugin.Log.LogInfo(
                                $"Imported character with id {id} and name {name}, overwriting character with name {oldCharacterName}.");
                            break;
                        case "append":
                            Plugin.Log.LogInfo($"Appending character {importedCharacter.name ?? "null"} to next available id.");
                            int id2 = Characters.no_chars + 1;
                            importedCharacter.id = id2;
                            if (GLPGLJAJJOP.APPDIBENDAH.savedChars.Length <= id2)
                            {
                                Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.savedChars, id2 + 1);
                                Array.Resize(ref GLPGLJAJJOP.APPDIBENDAH.charUnlock, id2 + 1);
                                Array.Resize(ref Characters.c, id2 + 1);
                                Array.Resize(ref Progress.charUnlock, id2 + 1);
                                GLPGLJAJJOP.APPDIBENDAH.charUnlock[id2] = 1;
                                Progress.charUnlock[id2] = 1;
                            }
                            else
                            {
                                Plugin.Log.LogWarning(
                                    $"The array of characters is larger than the number of characters. This should not happen. The character {GLPGLJAJJOP.APPDIBENDAH.savedChars[id2].name} will be overwritten.");
                            }

                            GLPGLJAJJOP.APPDIBENDAH.savedChars[id2] = importedCharacter;
                            if (importedCharacter.fed != 0)
                            {
                                if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size + 1 ==
                                    GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster.Length)
                                {
                                    Array.Resize(
                                        ref GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster,
                                        GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size + 2);
                                    if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size++;
                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed]
                                    .roster[GLPGLJAJJOP.APPDIBENDAH.savedFeds[importedCharacter.fed].size] = id2;
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
                                    id3 = GLPGLJAJJOP.APPDIBENDAH.savedChars
                                        .Single(c => c != null && c.name != null && c.name == find).id;
                                }
                                catch (Exception)
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

                            Character oldCharacter2 = GLPGLJAJJOP.APPDIBENDAH.savedChars[id3];
                            file.CharacterData.MergeIntoCharacter(oldCharacter2);

                            GLPGLJAJJOP.APPDIBENDAH.savedChars[id3] = oldCharacter2;
                            if (file.CharacterData.fed != null && file.CharacterData.fed.Value != oldCharacter2.fed)
                            {
                                if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].size + 1 ==
                                    GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].roster.Length)
                                {
                                    Array.Resize(
                                        ref GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].roster,
                                        GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].size + 2);
                                    if (GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].roster.Length >
                                        Characters.fedLimit)
                                    {
                                        Characters.fedLimit++;
                                    }
                                }

                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].size++;
                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value]
                                        .roster[GLPGLJAJJOP.APPDIBENDAH.savedFeds[file.CharacterData.fed.Value].size] =
                                    id3;
                                GLPGLJAJJOP.APPDIBENDAH.savedFeds[oldCharacter2.fed].BIDMDHABIGJ(id3);
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

            GLPGLJAJJOP.APPDIBENDAH.CDLIDDFKFEL(FIHDANPPMGC);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Error while importing characters.");
            Plugin.Log.LogError(e);
        }
    }

    /*
     * Patch:
     * - Increases the character limit if the user has set it to be higher than the default during roster reset.
     * - Also increases the character limit if there are more characters than the limit.
     */
    [HarmonyPatch(typeof(Roster), nameof(Roster.PIMGMPBCODM))]
    [HarmonyPostfix]
    public static void Roster_PIMGMPBCODM(Roster __instance)
    {
        if (Plugin.BaseFedLimit.Value > 48 && __instance.roster.Length < Plugin.BaseFedLimit.Value + 1)
        {
            Array.Resize(ref __instance.roster, Plugin.BaseFedLimit.Value + 1);
        }
    }

#pragma warning disable Harmony003
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
#pragma warning restore Harmony003

    /*
     * - Saves the current custom content map and exports all characters during user save.
     */
    [HarmonyPatch(typeof(UnmappedSaveSystem), nameof(UnmappedSaveSystem.OIIAHNGBNIF))]
    [HarmonyPostfix]
    public static void SaveSystem_OIIAHNGBNIF_Post(int FIHDANPPMGC)
    {
        SaveCurrentMap();
        CharacterMappings.CharacterMap.Save();
        MetaFile.Data.Save();

        if (NonSavedData.DeletedCharacters.Count > 0)
        {
            Plugin.Log.LogInfo($"Saving {NonSavedData.DeletedCharacters.Count} characters to purgatory.");
            foreach (Character character in NonSavedData.DeletedCharacters)
            {
                BetterCharacterData moddedCharacter = BetterCharacterData.FromRegularCharacter(character, Characters.c);
                BetterCharacterDataFile file = new() { characterData = moddedCharacter, overrideMode = "append" };
                string json = JsonConvert.SerializeObject(file, Formatting.Indented);
                string path = Path.Combine(Locations.DeletedCharacters.FullName, $"{character.id}_{Escape(character.name)}.character");
                if (!Directory.Exists(Locations.DeletedCharacters.FullName))
                {
                    Directory.CreateDirectory(Locations.DeletedCharacters.FullName);
                }
                File.WriteAllText(path, json);
            }
        }
        
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
    
    /*
     * Patch:
     * - Increases the character limit if the user has set it to be higher than the default during user progress save.
     */
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.PLCEMOKLLCP))]
    [HarmonyPrefix]
    public static void SaveData_PLCEMOKLLCP(SaveData __instance, int NAOMGHCFPKK = 1)
    {
        if (__instance.charUnlock.Length < Characters.no_chars + 1)
        {
            Array.Resize(ref __instance.charUnlock, Characters.no_chars + 1);
        }
    }
    
    /*
     * Patch:
     * - Increases the character limit if the user has set it to be higher than the default during user progress load.
     */
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.PEDMCEBEOCE))]
    [HarmonyPrefix]
    public static void SaveData_PEDMCEBEOCE(SaveData __instance, int IPBOLLAFINB = 1)
    {
        if (Progress.charUnlock.Length < __instance.savedChars.Length)
        {
            Array.Resize(ref Progress.charUnlock, __instance.savedChars.Length);
        }
    }
}