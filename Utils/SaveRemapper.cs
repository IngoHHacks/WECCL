using WECCL.Content;
using WECCL.Patches;
using WECCL.Saves;
using WECCL.Updates;

namespace WECCL.Utils;

public class SaveRemapper
{
    internal static void PatchCustomContent(ref SaveData saveData)
    {
        ContentMappings newMap = ContentMappings.ContentMap;
        ContentMappings savedMap = SaveFilePatch.LoadPreviousMap();

        bool changed = false;

        int oldVersion = Mathf.RoundToInt(ContentMappings.ContentMap.GameVersion * 100);
        int newVersion = Mathf.RoundToInt(Plugin.PluginVersion * 100);

        VersionDiff versionDiff = null;

        if (oldVersion != newVersion)
        {
            Plugin.Log.LogInfo($"Game version changed from {oldVersion} to {newVersion}. Updating custom content map.");
            List<VersionDiff> versionDiffs = new();
            if (oldVersion < 156 && newVersion >= 156)
            {
                versionDiffs.Add(new V155toV156());   
            }
            if (oldVersion < 159 && newVersion >= 159)
            {
                versionDiffs.Add(new V158toV159());
            }
            if (oldVersion < 160 && newVersion >= 160)
            {
                versionDiffs.Add(new V159toV160());
            }
            versionDiff = new VersionDiffGroup(versionDiffs.ToArray());
        }

        if (!VanillaCounts.IsInitialized)
        {
            Plugin.Log.LogError("Vanilla counts not initialized. Skipping custom content patch.");
            return;
        }

        try
        {
            foreach (Character character in saveData.savedChars)
            {
                if (character == null)
                {
                    continue;
                }

                foreach (Costume costume in character.costume)
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

                        if (versionDiff != null)
                        {
                            if (versionDiff.MaterialCountsDiff[i] != 0 && costume.texture[i] >
                                VanillaCounts.MaterialCounts[i] - versionDiff.MaterialCountsDiff[i])
                            {
                                costume.texture[i] += versionDiff.MaterialCountsDiff[i];
                                changed = true;
                            }
                            else if (i == 3 && versionDiff.FaceFemaleCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.FaceFemaleCount + versionDiff.FaceFemaleCountDiff)
                            {
                                costume.texture[i] -= versionDiff.FaceFemaleCountDiff;
                                changed = true;
                            }
                            else if (i == 14 && versionDiff.SpecialFootwearCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.SpecialFootwearCount + versionDiff.SpecialFootwearCountDiff)
                            {
                                costume.texture[i] -= versionDiff.SpecialFootwearCountDiff;
                                changed = true;
                            }
                            else if (i == 15 && versionDiff.SpecialFootwearCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.SpecialFootwearCount + versionDiff.SpecialFootwearCountDiff)
                            {
                                costume.texture[i] -= versionDiff.SpecialFootwearCountDiff;
                                changed = true;
                            }
                            else if (i == 17 && versionDiff.TransparentHairMaterialCountDiff != 0 &&
                                     costume.texture[i] < -VanillaCounts.TransparentHairMaterialCount +
                                     versionDiff.TransparentHairMaterialCountDiff)
                            {
                                costume.texture[i] -= versionDiff.TransparentHairMaterialCountDiff;
                                changed = true;
                            }
                            else if (i == 24 && versionDiff.KneepadCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.KneepadCount + versionDiff.KneepadCountDiff)
                            {
                                costume.texture[i] -= versionDiff.KneepadCountDiff;
                                changed = true;
                            }
                            else if (i == 25 && versionDiff.KneepadCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.KneepadCount + versionDiff.KneepadCountDiff)
                            {
                                costume.texture[i] -= versionDiff.KneepadCountDiff;
                                changed = true;
                            }
                        }

                        if (costume.texture[i] > VanillaCounts.MaterialCounts[i])
                        {
                            int oldIndex = costume.texture[i] - VanillaCounts.MaterialCounts[i] - 1;
                            if (oldIndex >= savedMap.MaterialNameMap[i].Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.MaterialNameMap[i][oldIndex];
                            int newIndex = newMap.MaterialNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.MaterialCounts[i] + 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.FaceFemaleCount - 1;
                            if (oldIndex >= savedMap.FaceFemaleNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.FaceFemaleNameMap[oldIndex];
                            int newIndex = newMap.FaceFemaleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.FaceFemaleCount - 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            int newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.SpecialFootwearCount - 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            int newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.SpecialFootwearCount - 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.TransparentHairMaterialCount - 1;
                            if (oldIndex >= savedMap.TransparentHairMaterialNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.TransparentHairMaterialNameMap[oldIndex];
                            int newIndex = newMap.TransparentHairMaterialNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.TransparentHairMaterialCount - 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.KneepadNameMap[oldIndex];
                            int newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.KneepadCount - 1;
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
                            int oldIndex = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.MaterialCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.KneepadNameMap[oldIndex];
                            int newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.KneepadCount - 1;
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

                        if (versionDiff != null)
                        {
                            if (versionDiff.FleshCountsDiff[i] != 0 && costume.flesh[i] >
                                VanillaCounts.FleshCounts[i] - versionDiff.FleshCountsDiff[i])
                            {
                                costume.flesh[i] += versionDiff.FleshCountsDiff[i];
                                changed = true;
                            }
                            else if (i == 2 && versionDiff.BodyFemaleCountDiff != 0 && costume.flesh[i] <
                                     -VanillaCounts.BodyFemaleCount + versionDiff.BodyFemaleCountDiff)
                            {
                                costume.flesh[i] -= versionDiff.BodyFemaleCountDiff;
                                changed = true;
                            }
                        }

                        if (costume.flesh[i] > VanillaCounts.FleshCounts[i])
                        {
                            int oldIndex = costume.flesh[i] - VanillaCounts.FleshCounts[i] - 1;
                            if (oldIndex >= savedMap.FleshNameMap[i].Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.FleshCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.FleshNameMap[i][oldIndex];
                            int newIndex = newMap.FleshNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.FleshCounts[i] + 1;
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
                            int oldIndex = -costume.flesh[i] - VanillaCounts.BodyFemaleCount - 1;
                            if (oldIndex >= savedMap.BodyFemaleNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.FleshCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.BodyFemaleNameMap[oldIndex];
                            int newIndex = newMap.BodyFemaleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.BodyFemaleCount - 1;
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

                        if (versionDiff != null)
                        {
                            if (versionDiff.ShapeCountsDiff[i] != 0 && costume.shape[i] >
                                VanillaCounts.ShapeCounts[i] - versionDiff.ShapeCountsDiff[i])
                            {
                                costume.shape[i] += versionDiff.ShapeCountsDiff[i];
                                changed = true;
                            }
                            else if (i == 17 && versionDiff.TransparentHairHairstyleCountDiff != 0 && costume.shape[i] <
                                     -VanillaCounts.TransparentHairHairstyleCount +
                                     versionDiff.TransparentHairHairstyleCountDiff)
                            {
                                costume.shape[i] -= versionDiff.TransparentHairHairstyleCountDiff;
                                changed = true;
                            }
                        }

                        if (costume.shape[i] > VanillaCounts.ShapeCounts[i])
                        {
                            int oldIndex = costume.shape[i] - VanillaCounts.ShapeCounts[i] - 1;
                            if (oldIndex >= savedMap.ShapeNameMap[i].Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.ShapeCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.ShapeNameMap[i][oldIndex];
                            int newIndex = newMap.ShapeNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.ShapeCounts[i] + 1;
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
                            int oldIndex = -costume.shape[i] - VanillaCounts.TransparentHairHairstyleCount - 1;
                            if (oldIndex >= savedMap.TransparentHairHairstyleNameMap.Count)
                            {
                                Plugin.Log.LogWarning(
                                    $"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.ShapeCounts[i];
                                changed = true;
                                continue;
                            }

                            string oldName = savedMap.TransparentHairHairstyleNameMap[oldIndex];
                            int newIndex = newMap.TransparentHairHairstyleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.TransparentHairHairstyleCount - 1;
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

                if (versionDiff != null && versionDiff.MusicCountDiff != 0 &&
                    character.music > VanillaCounts.MusicCount - versionDiff.MusicCountDiff)
                {
                    character.music += versionDiff.MusicCountDiff;
                    changed = true;
                }

                if (character.music > VanillaCounts.MusicCount)
                {
                    int oldIndex = character.music - VanillaCounts.MusicCount - 1;
                    if (oldIndex >= savedMap.MusicNameMap.Count)
                    {
                        Plugin.Log.LogWarning(
                            $"Custom music index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                        character.music = VanillaCounts.MusicCount;
                        changed = true;
                    }
                    else
                    {
                        string oldName = savedMap.MusicNameMap[oldIndex];
                        int newIndex = newMap.MusicNameMap.IndexOf(oldName);
                        int internalIndex = newIndex + VanillaCounts.MusicCount + 1;
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
        }
    }

    public static void FixBrokenSaveData()
    {
        Plugin.Log.LogInfo("Validating save data...");
        var saveData = UnmappedSaveData.APPDIBENDAH;
        var numChars = saveData.savedChars.Length;
        if (saveData.wrestler > numChars)
        {
            Plugin.Log.LogWarning(
                $"Wrestler index {saveData.wrestler} is out of bounds. Resetting to 1.");
            saveData.wrestler = 1;
        }
        if (saveData.booker > numChars)
        {
            Plugin.Log.LogWarning(
                $"Booker index {saveData.booker} is out of bounds. Resetting to 1.");
            saveData.booker = 1;
        }
        if (saveData.missionClient > numChars)
        {
            Plugin.Log.LogWarning(
                $"Mission client index {saveData.missionClient} is out of bounds. Resetting to 1.");
            saveData.missionClient = 1;
        }
        if (saveData.missionTarget > numChars)
        {
            Plugin.Log.LogWarning(
                $"Mission target index {saveData.missionTarget} is out of bounds. Resetting to 1.");
            saveData.missionTarget = 1;
        }
        if (saveData.bookingMissionClient > numChars)
        {
            Plugin.Log.LogWarning(
                $"Booking mission client index {saveData.bookingMissionClient} is out of bounds. Resetting to 1.");
            saveData.bookingMissionClient = 1;
        }
        if (saveData.bookingMissionTarget > numChars)
        {
            Plugin.Log.LogWarning(
                $"Booking mission target index {saveData.bookingMissionTarget} is out of bounds. Resetting to 1.");
            saveData.bookingMissionTarget = 1;
        }
        if (saveData.charUnlock.Length != numChars)
        {
            Plugin.Log.LogWarning(
                $"Character unlock array length {saveData.charUnlock.Length} does not match number of characters {numChars}. Fixing.");
            int oldLength = saveData.charUnlock.Length;
            Array.Resize(ref saveData.charUnlock, numChars);
            if (oldLength < numChars)
            {
                for (int i = oldLength; i < numChars; i++)
                {
                    saveData.charUnlock[i] = 1;
                }
            }
        }
        for (int index = 1; index < saveData.opponent.Length; index++)
        {
            int opponent = saveData.opponent[index];
            if (opponent > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Opponent index {opponent} is out of bounds. Resetting to 1.");
                saveData.opponent[index] = 1;
            }
        }
        for (int index = 1; index < saveData.bookingOpponent.Length; index++)
        {
            int opponent = saveData.bookingOpponent[index];
            if (opponent > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Booking opponent index {opponent} is out of bounds. Resetting to 1.");
                saveData.bookingOpponent[index] = 1;
            }
        }
        foreach (var card in saveData.card)
        {
            if (card == null)
            {
                continue;
            }
            foreach (var segment in card.segment)
            {
                if (segment == null)
                {
                    continue;
                }
                if (segment.leftChar > numChars)
                {
                    Plugin.Log.LogWarning(
                        $"Left character index {segment.leftChar} is out of bounds. Resetting to 1.");
                    segment.leftChar = 1;
                }
                if (segment.rightChar > numChars)
                {
                    Plugin.Log.LogWarning(
                        $"Right character index {segment.rightChar} is out of bounds. Resetting to 1.");
                    segment.rightChar = 1;
                }
                if (segment.winner > numChars)
                {
                    Plugin.Log.LogWarning(
                        $"Winner index {segment.winner} is out of bounds. Resetting to 1.");
                    segment.winner = 1;
                }
            }
        }
        for (int index = 1; index < saveData.hiChar.Length; index++)
        {
            int hiChar = saveData.hiChar[index];
            if (hiChar > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Hi character index {hiChar} is out of bounds. Resetting to 1.");
                saveData.hiChar[index] = 1;
            }
        }
        foreach (var stockFurniture in saveData.stockFurniture)
        {
            if (stockFurniture == null)
            {
                continue;
            }
            if (stockFurniture.owner > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Furniture owner index {stockFurniture.owner} is out of bounds. Resetting to 1.");
                stockFurniture.owner = 1;
            }
        }
        foreach (var stockWeapon in saveData.stockWeapons)
        {
            if (stockWeapon == null)
            {
                continue;
            }
            if (stockWeapon.owner > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Weapon owner index {stockWeapon.owner} is out of bounds. Resetting to 1.");
                stockWeapon.owner = 1;
            }
        }
        foreach (var character in saveData.savedChars)
        {
            if (character == null)
            {
                continue;
            }
            for (int i = 1; i < character.relationship.Length; i++)
            {
                if (character.relationship[i] > numChars)
                {
                    Plugin.Log.LogWarning(
                        $"Relationship character index {character.relationship[i]} is out of bounds. Resetting to 1.");
                    character.relationship[i] = 1;
                }
            }
            if (character.grudge > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Grudge character index {character.grudge} is out of bounds. Resetting to 1.");
                character.grudge = 1;
            }
        }

        foreach (var fedData in saveData.savedFeds)
        {
            if (fedData == null)
            {
                continue;
            }
            if (fedData.owner > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Fed owner index {fedData.owner} is out of bounds. Resetting to 1.");
                fedData.owner = 1;
            }
            if (fedData.booker > numChars)
            {
                Plugin.Log.LogWarning(
                    $"Fed booker index {fedData.booker} is out of bounds. Resetting to 1.");
                fedData.booker = 1;
            }
            for (int index = 1; index < fedData.roster.Length; index++)
            {
                int roster = fedData.roster[index];
                if (roster > numChars)
                {
                    Plugin.Log.LogWarning(
                        $"Fed roster index {roster} is out of bounds. Removing.");
                    for (int i = index; i < fedData.roster.Length - 1; i++)
                    {
                        fedData.roster[i] = fedData.roster[i + 1];
                    }
                    fedData.size--;
                }
            }
            for (int index = 1; index < fedData.champ.GetLength(0); index++)
            {
                for (int index2 = 1; index2 < fedData.champ.GetLength(1); index2++)
                {
                    int champ = fedData.champ[index, index2];
                    if (champ > numChars)
                    {
                        Plugin.Log.LogWarning(
                            $"Fed champ index {champ} is out of bounds. Resetting to 1.");
                        fedData.champ[index, index2] = 1;
                    }
                }
            }
        }
        Plugin.Log.LogInfo("Save data validation complete.");
    }
}