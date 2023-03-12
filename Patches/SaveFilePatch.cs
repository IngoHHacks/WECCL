using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WECCL.Content;
using WECCL.Saves;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SaveFilePatch
{
    [HarmonyPatch(typeof(FPNEAHPFCHF), nameof(FPNEAHPFCHF.OLAGCFPPEPB))]
    [HarmonyPrefix]
    public static void FixCharSize(int IHLLJIMFJEN)
    {
        try
        {
            var save = Application.persistentDataPath + "/Save.bytes";
            if (!File.Exists(save))
            {
                return;
            }
            FileStream fileStream = new FileStream(save, FileMode.Open);
            SaveData data = new BinaryFormatter().Deserialize(fileStream) as SaveData;
            Characters.no_chars = data!.savedChars.Length - 1;
            int[] fedCharCount = new int[Characters.no_feds + 1];
            foreach (var c in data.savedChars)
            {
                if (c != null) fedCharCount[c.fed]++;
            }

            Characters.fedLimit = Math.Max(48, fedCharCount.Max() + 1);
            Array.Resize(ref Characters.c, Characters.no_chars + 1);
            Array.Resize(ref Progress.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.charUnlock, Characters.no_chars + 1);
            if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds != null)
            {
                for (int i = 1; i <= Characters.no_feds; i++)
                {
                    if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[i] != null)
                    {
                        FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[i].size = fedCharCount[i] + 1;
                    }
                }
            }

            fileStream.Close();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    [HarmonyPatch(typeof(FPNEAHPFCHF), nameof(FPNEAHPFCHF.NOKFJAECIGL))]
    [HarmonyPostfix]
    public static void FPNEAHPFCHF_NOKFJAECIGL()
    {
        try
        {
            Characters.no_chars = 350;
            Characters.fedLimit = 48;
            Array.Resize(ref Characters.c, Characters.no_chars + 1);
            Array.Resize(ref Progress.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.charUnlock, Characters.no_chars + 1);
            Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.savedChars, Characters.no_chars + 1);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    [HarmonyPatch(typeof(FPNEAHPFCHF), nameof(FPNEAHPFCHF.OLAGCFPPEPB))]
    [HarmonyPostfix]
    public static void FPNEAHPFCHF_OLAGCFPPEPB(int IHLLJIMFJEN)
    {
        var save = Application.persistentDataPath + "/Save.bytes";
        if (!File.Exists(save))
        {
            return;
        }
        try
        {
            PatchCustomContent(ref FPNEAHPFCHF.GPFFEHKLNLD, IHLLJIMFJEN);
            foreach (var pair in CustomContent.ImportedCharacters)
            {
                Plugin.Log.LogInfo($"Importing character {pair.Item2.name} with id {pair.Item2.id}.");
                var overrideMode = pair.Item1;
                var importedCharacter = pair.Item2;
                switch (overrideMode.ToLower())
                {
                    case "override":
                        var id = importedCharacter.id;
                        var oldCharacter = FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id];
                        var name = importedCharacter.name;
                        var oldCharacterName = oldCharacter.name;
                        FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id] = importedCharacter;
                        if (importedCharacter.fed != oldCharacter.fed)
                        {
                            if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 == FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster, FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length > Characters.fedLimit) Characters.fedLimit++;
                            }
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                .roster[FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id;
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[oldCharacter.fed].HKNHEJHIJLL(id);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id} and name {name}, overwriting character with name {oldCharacterName}.");
                        break;
                    case "append":
                        var id2 = Characters.no_chars + 1;
                        importedCharacter.id = id2;
                        if (FPNEAHPFCHF.GPFFEHKLNLD.savedChars.Length <= id2)
                        {
                            Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.savedChars, id2 + 1);
                            Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.charUnlock, id2 + 1);
                            Array.Resize(ref Characters.c, id2 + 1);
                            Array.Resize(ref Progress.charUnlock, id2 + 1);
                            FPNEAHPFCHF.GPFFEHKLNLD.charUnlock[id2] = 1;
                            Progress.charUnlock[id2] = 1;
                        }
                        else
                        {
                            Plugin.Log.LogWarning(
                                $"The array of characters is larger than the number of characters. This should not happen. The character {FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id2].name} will be overwritten.");
                        }

                        FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id2] = importedCharacter;
                        if (importedCharacter.fed != 0)
                        {
                            if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 == FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster, FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length > Characters.fedLimit) Characters.fedLimit++;
                            }
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                .roster[FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id2;
                        }

                        Characters.no_chars++;
                        Plugin.Log.LogInfo(
                            $"Imported character with id {id2} and name {importedCharacter.name}. Incremented number of characters to {Characters.no_chars}.");
                        break;
                    case "merge":
                        var id3 = importedCharacter.id;
                        var oldCharacter2 = FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id3];
                        var name2 = importedCharacter.name;
                        var oldCharacterName2 = oldCharacter2.name;
                        foreach (var field in typeof(Character).GetFields())
                        {
                            if (field.GetValue(importedCharacter) != default)
                            {
                                field.SetValue(oldCharacter2, field.GetValue(importedCharacter));
                            }
                        }

                        FPNEAHPFCHF.GPFFEHKLNLD.savedChars[id3] = oldCharacter2;
                        if (importedCharacter.fed != oldCharacter2.fed)
                        {
                            if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 1 == FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length)
                            {
                                Array.Resize(ref FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster, FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size + 2);
                                if (FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].roster.Length > Characters.fedLimit) Characters.fedLimit++;
                            }
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size++;
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed]
                                .roster[FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[importedCharacter.fed].size] = id3;
                            FPNEAHPFCHF.GPFFEHKLNLD.savedFeds[oldCharacter2.fed].HKNHEJHIJLL(id3);
                        }

                        Plugin.Log.LogInfo(
                            $"Imported character with id {id3} and name {name2}, merging with character with name {oldCharacterName2}.");
                        break;
                }

            }
            FPNEAHPFCHF.GPFFEHKLNLD.FGMMAKKGCOG(IHLLJIMFJEN);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    [HarmonyPatch(typeof(FPNEAHPFCHF), nameof(FPNEAHPFCHF.ICAMLDGDPHC))]
    [HarmonyPostfix]
    public static void FPNEAHPFCHF_ICAMLDGDPHC(int IHLLJIMFJEN)
    {
        SaveCurrentMap();
        if (Plugin.Instance.AutoExportCharacters.Value)
        {
            ModdedCharacterManager.SaveAllCharacters();
        }

        if (Plugin.Instance.DeleteImportedCharacters.Value)
        {
            foreach (string file in CustomContent.FilesToDeleteOnSave)
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
    internal static void PatchCustomContent(ref SaveData saveData, int IHLLJIMFJEN)
    {
        var newMap = CustomContentSaveFile.ContentMap;
        var savedMap = LoadPreviousMap();
        
        bool changed = false;

        if (!VanillaCounts.IsInitialized)
        {
            Plugin.Log.LogError("Vanilla counts not initialized. Skipping custom content patch.");
            return;
        }

        try
        {
            foreach (var character in saveData.savedChars)
            {
                if (character == null)
                {
                    continue;
                }

                foreach (var costume in character.costume)
                {
                    if (costume == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < costume.texture.Length; i++)
                    {
                        if (VanillaCounts.MaterialCounts[i] == 0)
                        {
                            continue;
                        }
                        if (costume.texture[i] > VanillaCounts.MaterialCounts[i])
                        {
                            var oldIndex = costume.texture[i] - VanillaCounts.MaterialCounts[i] - 1;
                            if (oldIndex >= savedMap.MaterialNameMap[i].Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.MaterialNameMap[i][oldIndex];
                            var newIndex = newMap.MaterialNameMap[i].IndexOf(oldName);
                            var internalIndex = newIndex + VanillaCounts.MaterialCounts[i] + 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex; 
                                changed = true;
                            }
                        }

                        else if (i == 3 && costume.texture[i] < -VanillaCounts.FaceFemaleCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.FaceFemaleCount - 1;
                            if (oldIndex >= savedMap.FaceFemaleNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.FaceFemaleNameMap[oldIndex];
                            var newIndex = newMap.FaceFemaleNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.FaceFemaleCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 14 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            var newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.SpecialFootwearCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 15 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            var newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.SpecialFootwearCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 17 && costume.texture[i] < -VanillaCounts.TransparentHairMaterialCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.TransparentHairMaterialCount - 1;
                            if (oldIndex >= savedMap.TransparentHairMaterialNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.TransparentHairMaterialNameMap[oldIndex];
                            var newIndex = newMap.TransparentHairMaterialNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.TransparentHairMaterialCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 24 && costume.texture[i] < -VanillaCounts.KneepadCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.KneepadNameMap[oldIndex];
                            var newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.KneepadCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 25 && costume.texture[i] < -VanillaCounts.KneepadCount)
                        {
                            var oldIndex = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.KneepadNameMap[oldIndex];
                            var newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.KneepadCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                                changed = true;
                            }
                        }
                    }

                    for (int i = 0; i < costume.flesh.Length; i++)
                    {
                        if (VanillaCounts.FleshCounts[i] == 0)
                        {
                            continue;
                        }

                        if (costume.flesh[i] > VanillaCounts.FleshCounts[i])
                        {
                            var oldIndex = costume.flesh[i] - VanillaCounts.FleshCounts[i] - 1;
                            if (oldIndex >= savedMap.FleshNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.FleshCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.FleshNameMap[i][oldIndex];
                            var newIndex = newMap.FleshNameMap[i].IndexOf(oldName);
                            var internalIndex = newIndex + VanillaCounts.FleshCounts[i] + 1;
                            if (costume.flesh[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom flesh {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for flesh {i} of character {character.name} ({character.id}).");
                                costume.flesh[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 2 && costume.flesh[i] < -VanillaCounts.BodyFemaleCount)
                        {
                            var oldIndex = -costume.flesh[i] - VanillaCounts.BodyFemaleCount - 1;
                            if (oldIndex >= savedMap.BodyFemaleNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.FleshCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.BodyFemaleNameMap[oldIndex];
                            var newIndex = newMap.BodyFemaleNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.BodyFemaleCount - 1;
                            if (costume.flesh[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom flesh {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for flesh {i} of character {character.name} ({character.id}).");
                                costume.flesh[i] = internalIndex;
                                changed = true;
                            }
                        }
                    }

                    for (int i = 0; i < costume.shape.Length; i++)
                    {
                        if (VanillaCounts.ShapeCounts[i] == 0)
                        {
                            continue;
                        }

                        if (costume.shape[i] > VanillaCounts.ShapeCounts[i])
                        {
                            var oldIndex = costume.shape[i] - VanillaCounts.ShapeCounts[i] - 1;
                            if (oldIndex >= savedMap.ShapeNameMap[i].Count)
                            {
                                Plugin.Log.LogWarning($"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.ShapeCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.ShapeNameMap[i][oldIndex];
                            var newIndex = newMap.ShapeNameMap[i].IndexOf(oldName);
                            var internalIndex = newIndex + VanillaCounts.ShapeCounts[i] + 1;
                            if (costume.shape[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom shape {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for shape {i} of character {character.name} ({character.id}).");
                                costume.shape[i] = internalIndex;
                                changed = true;
                            }
                        }

                        else if (i == 17 && costume.shape[i] < -VanillaCounts.TransparentHairHairstyleCount)
                        {
                            var oldIndex = -costume.shape[i] - VanillaCounts.TransparentHairHairstyleCount - 1;
                            if (oldIndex >= savedMap.TransparentHairHairstyleNameMap.Count)
                            {
                                Plugin.Log.LogWarning($"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.ShapeCounts[i];
                                changed = true;
                                continue;
                            }
                            var oldName = savedMap.TransparentHairHairstyleNameMap[oldIndex];
                            var newIndex = newMap.TransparentHairHairstyleNameMap.IndexOf(oldName);
                            var internalIndex = -newIndex - VanillaCounts.TransparentHairHairstyleCount - 1;
                            if (costume.shape[i] != internalIndex)
                            {
                                Plugin.Log.LogInfo(
                                    $"Custom shape {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for shape {i} of character {character.name} ({character.id}).");
                                costume.shape[i] = internalIndex;
                                changed = true;
                            }
                        }
                    }
                }

                if (character.music > VanillaCounts.MusicCount)
                {
                    var oldIndex = character.music - VanillaCounts.MusicCount - 1;
                    if (oldIndex >= savedMap.MusicNameMap.Count)
                    {
                        Plugin.Log.LogWarning(
                            $"Custom music index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                        character.music = VanillaCounts.MusicCount;
                        changed = true;
                    }
                    else
                    {
                        var oldName = savedMap.MusicNameMap[oldIndex];
                        var newIndex = newMap.MusicNameMap.IndexOf(oldName);
                        var internalIndex = newIndex + VanillaCounts.MusicCount + 1;
                        if (character.music != internalIndex)
                        {
                            Plugin.Log.LogInfo(
                                $"Custom music {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for character {character.name} ({character.id}).");
                            character.music = internalIndex;
                            changed = true;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Failed to remap custom content!");
            Plugin.Log.LogError(e);
            return;
        }
    }
    
    internal static void SaveCurrentMap()
    {
        CustomContentSaveFile.ContentMap.Save();
    }

    internal static CustomContentSaveFile LoadPreviousMap()
    {
        return CustomContentSaveFile.Load();
    }

}