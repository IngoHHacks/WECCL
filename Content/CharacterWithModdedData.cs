using System.Collections.Generic;
using WECCL.Saves;

namespace WECCL.Content;

public class CharacterWithModdedData
{
    public Character BaseCharacter { get; set; }
    
    public List<string> CustomMaterials { get; set; } = new();
    
    public List<string> CustomFlesh { get; set; } = new();
    
    public List<string> CustomShapes { get; set; } = new();
    
    public string CustomFemaleBody { get; set; }
    
    public string CustomFemaleFace { get; set; }
    
    public string CustomSpecialFootwear { get; set; }
    
    public string CustomTransparentHairMaterial { get; set; }
    
    public string CustomTransparentHairHairstyle { get; set; }
    
    public string CustomKneepad { get; set; }

    public string CustomThemeName { get; set; }
    
    public bool IsModded => CustomMaterials.Exists(x => x != null) || CustomFlesh.Exists(x => x != null) ||
                            CustomShapes.Exists(x => x != null) || CustomFemaleBody != null ||
                            CustomFemaleFace != null || CustomSpecialFootwear != null ||
                            CustomTransparentHairMaterial != null || CustomTransparentHairHairstyle != null ||
                            CustomKneepad != null || CustomThemeName != null;
    
    public CharacterWithModdedData(Character character)
    {
        BaseCharacter = character;
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
                    CustomMaterials.Add(null);
                    continue;
                }

                if (costume.texture[i] > VanillaCounts.MaterialCounts[i])
                {
                    var name = CustomContentSaveFile.ContentMap.MaterialNameMap[i][
                        costume.texture[i] - VanillaCounts.MaterialCounts[i] - 1];
                    CustomMaterials.Add(name);
                }
                else if (i == 3 && costume.texture[i] < -VanillaCounts.FaceFemaleCount)
                {
                    var name = CustomContentSaveFile.ContentMap.FaceFemaleNameMap[
                        -costume.texture[i] - VanillaCounts.FaceFemaleCount - 1];
                    CustomFemaleFace = name;
                }
                else if (i == 14 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                {
                    var name = CustomContentSaveFile.ContentMap.SpecialFootwearNameMap[
                        -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1];
                    CustomSpecialFootwear = name;
                }
                else if (i == 15 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                {
                    var name = CustomContentSaveFile.ContentMap.SpecialFootwearNameMap[
                        -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1];
                    CustomSpecialFootwear = name;
                }
                else if (i == 17 && costume.texture[i] < -VanillaCounts.TransparentHairMaterialCount)
                {
                    var name = CustomContentSaveFile.ContentMap.TransparentHairMaterialNameMap[
                        -costume.texture[i] - VanillaCounts.TransparentHairMaterialCount - 1];
                    CustomTransparentHairMaterial = name;
                }
                else if (i == 24 && costume.texture[i] < -VanillaCounts.KneepadCount)
                {
                    var name = CustomContentSaveFile.ContentMap.KneepadNameMap[
                        -costume.texture[i] - VanillaCounts.KneepadCount - 1];
                    CustomKneepad = name;
                }
                else if (i == 25 && costume.texture[i] < -VanillaCounts.KneepadCount)
                {
                    var name = CustomContentSaveFile.ContentMap.KneepadNameMap[
                        -costume.texture[i] - VanillaCounts.KneepadCount - 1];
                    CustomKneepad = name;
                }
                else
                {
                    CustomMaterials.Add(null);
                }
            }
            for (int i = 0; i < costume.flesh.Length; i++)
            {
                if (VanillaCounts.FleshCounts[i] == 0)
                {
                    CustomFlesh.Add(null);
                    continue;
                }

                if (costume.flesh[i] > VanillaCounts.FleshCounts[i])
                {
                    var name = CustomContentSaveFile.ContentMap.FleshNameMap[i][
                        costume.flesh[i] - VanillaCounts.FleshCounts[i] - 1];
                    CustomFlesh.Add(name);
                }
                else if (i == 2 && costume.flesh[i] < -VanillaCounts.BodyFemaleCount)
                {
                    var name = CustomContentSaveFile.ContentMap.BodyFemaleNameMap[
                        -costume.flesh[i] - VanillaCounts.BodyFemaleCount - 1];
                    CustomFemaleBody = name;
                }
                else
                {
                    CustomFlesh.Add(null);
                }
            }
            for (int i = 0; i < costume.shape.Length; i++)
            {
                if (VanillaCounts.ShapeCounts[i] == 0)
                {
                    CustomShapes.Add(null);
                    continue;
                }

                if (costume.shape[i] > VanillaCounts.ShapeCounts[i])
                {
                    var name = CustomContentSaveFile.ContentMap.ShapeNameMap[i][
                        costume.shape[i] - VanillaCounts.ShapeCounts[i] - 1];
                    CustomShapes.Add(name);
                }
                else if (i == 17 && costume.shape[i] < -VanillaCounts.TransparentHairHairstyleCount)
                {
                    var name = CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap[
                        -costume.shape[i] - VanillaCounts.TransparentHairHairstyleCount - 1];
                    CustomTransparentHairHairstyle = name;
                }
                else
                {
                    CustomShapes.Add(null);
                }
            }
        }
        if (character.music > VanillaCounts.MusicCount)
        {
            var name = CustomContentSaveFile.ContentMap.MusicNameMap[
                character.music - VanillaCounts.MusicCount - 1];
            CustomThemeName = name;
        }
    }
    
    public CharacterWithModdedData() { }

    public Character ModdedToCharacter()
    {
        var character = BaseCharacter;
        if (!IsModded)
        {
            return character;
        }
        for (int i = 0; i < character.costume.Length; i++)
        {
            if (character.costume[i] == null)
            {
                continue;
            }

            for (int j = 0; j < character.costume[i].texture.Length; j++)
            {
                if (CustomMaterials[j] != null)
                {
                    if (CustomContentSaveFile.ContentMap.MaterialNameMap[j].Contains(CustomMaterials[j]))
                    {
                        character.costume[i].texture[j] =
                            CustomContentSaveFile.ContentMap.MaterialNameMap[j].IndexOf(CustomMaterials[j]) +
                            VanillaCounts.MaterialCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom material {CustomMaterials[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 3 && CustomFemaleFace != null)
                {
                    if (CustomContentSaveFile.ContentMap.FaceFemaleNameMap.Contains(CustomFemaleFace))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.FaceFemaleNameMap.IndexOf(CustomFemaleFace) -
                            VanillaCounts.FaceFemaleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom female face {CustomFemaleFace} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
                if (j == 14 && CustomSpecialFootwear != null)
                {
                    if (CustomContentSaveFile.ContentMap.SpecialFootwearNameMap.Contains(CustomSpecialFootwear))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.SpecialFootwearNameMap.IndexOf(CustomSpecialFootwear) -
                            VanillaCounts.SpecialFootwearCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom special footwear {CustomSpecialFootwear} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
                if (j == 15 && CustomSpecialFootwear != null)
                {
                    if (CustomContentSaveFile.ContentMap.SpecialFootwearNameMap.Contains(CustomSpecialFootwear))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.SpecialFootwearNameMap.IndexOf(CustomSpecialFootwear) -
                            VanillaCounts.SpecialFootwearCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom special footwear {CustomSpecialFootwear} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
                if (j == 17 && CustomTransparentHairMaterial != null)
                {
                    if (CustomContentSaveFile.ContentMap.TransparentHairMaterialNameMap.Contains(CustomTransparentHairMaterial))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.TransparentHairMaterialNameMap.IndexOf(CustomTransparentHairMaterial) -
                            VanillaCounts.TransparentHairMaterialCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom transparent hair material {CustomTransparentHairMaterial} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
                if (j == 24 && CustomKneepad != null)
                {
                    if (CustomContentSaveFile.ContentMap.KneepadNameMap.Contains(CustomKneepad))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.KneepadNameMap.IndexOf(CustomKneepad) -
                            VanillaCounts.KneepadCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom kneepad {CustomKneepad} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
                if (j == 25 && CustomKneepad != null)
                {
                    if (CustomContentSaveFile.ContentMap.KneepadNameMap.Contains(CustomKneepad))
                    {
                        character.costume[i].texture[j] =
                            -CustomContentSaveFile.ContentMap.KneepadNameMap.IndexOf(CustomKneepad) -
                            VanillaCounts.KneepadCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom kneepad {CustomKneepad} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }
            for (int j = 0; j < character.costume[i].shape.Length; j++)
            {
                if (CustomShapes[j] != null)
                {
                    if (CustomContentSaveFile.ContentMap.ShapeNameMap[j].Contains(CustomShapes[j]))
                    {
                        character.costume[i].shape[j] =
                            CustomContentSaveFile.ContentMap.ShapeNameMap[j].IndexOf(CustomShapes[j]) +
                            VanillaCounts.ShapeCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom shape {CustomShapes[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 17 && CustomTransparentHairHairstyle != null)
                {
                    if (CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap.Contains(CustomTransparentHairHairstyle))
                    {
                        character.costume[i].shape[j] =
                            -CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap.IndexOf(CustomTransparentHairHairstyle) -
                            VanillaCounts.TransparentHairHairstyleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError($"The character {character.name} has a custom transparent hair hairstyle {CustomTransparentHairHairstyle} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }

            for (int j = 0; j < character.costume[i].shape.Length; j++)
            {
                if (CustomShapes[j] != null)
                {
                    if (CustomContentSaveFile.ContentMap.ShapeNameMap[j].Contains(CustomShapes[j]))
                    {
                        character.costume[i].shape[j] =
                            CustomContentSaveFile.ContentMap.ShapeNameMap[j].IndexOf(CustomShapes[j]) +
                            VanillaCounts.ShapeCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom shape {CustomShapes[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 17 && CustomTransparentHairHairstyle != null)
                {
                    if (CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap.Contains(
                            CustomTransparentHairHairstyle))
                    {
                        character.costume[i].shape[j] =
                            -CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap.IndexOf(
                                CustomTransparentHairHairstyle) -
                            VanillaCounts.TransparentHairHairstyleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom transparent hair hairstyle {CustomTransparentHairHairstyle} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }

            if (CustomThemeName != null)
            {
                if (CustomContentSaveFile.ContentMap.MusicNameMap.Contains(CustomThemeName))
                {
                    character.music = CustomContentSaveFile.ContentMap.MusicNameMap.IndexOf(CustomThemeName) +
                                      VanillaCounts.MusicCount + 1;
                }
                else
                {
                    Plugin.Log.LogError(
                        $"The character {character.name} has a custom theme {CustomThemeName} that does not exist in the custom content save file. The character will not be loaded.");
                    return null;
                }
            }
        }
        Plugin.Log.LogInfo($"Loaded character {character.name}.");  
        return character;
    }
}