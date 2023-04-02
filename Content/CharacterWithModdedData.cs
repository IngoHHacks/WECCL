using WECCL.Saves;

namespace WECCL.Content;

public class CharacterWithModdedData
{
    public CharacterWithModdedData(Character character)
    {
        this.BaseCharacter = character;
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
                    this.CustomMaterials.Add(null);
                    continue;
                }

                if (costume.texture[i] > VanillaCounts.MaterialCounts[i])
                {
                    string name = ContentMappings.ContentMap.MaterialNameMap[i][
                        costume.texture[i] - VanillaCounts.MaterialCounts[i] - 1];
                    this.CustomMaterials.Add(name);
                }
                else if (i == 3 && costume.texture[i] < -VanillaCounts.FaceFemaleCount)
                {
                    string name = ContentMappings.ContentMap.FaceFemaleNameMap[
                        -costume.texture[i] - VanillaCounts.FaceFemaleCount - 1];
                    this.CustomFemaleFace = name;
                }
                else if (i == 14 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                {
                    string name = ContentMappings.ContentMap.SpecialFootwearNameMap[
                        -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1];
                    this.CustomSpecialFootwear = name;
                }
                else if (i == 15 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
                {
                    string name = ContentMappings.ContentMap.SpecialFootwearNameMap[
                        -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1];
                    this.CustomSpecialFootwear = name;
                }
                else if (i == 17 && costume.texture[i] < -VanillaCounts.TransparentHairMaterialCount)
                {
                    string name = ContentMappings.ContentMap.TransparentHairMaterialNameMap[
                        -costume.texture[i] - VanillaCounts.TransparentHairMaterialCount - 1];
                    this.CustomTransparentHairMaterial = name;
                }
                else if (i == 24 && costume.texture[i] < -VanillaCounts.KneepadCount)
                {
                    string name = ContentMappings.ContentMap.KneepadNameMap[
                        -costume.texture[i] - VanillaCounts.KneepadCount - 1];
                    this.CustomKneepad = name;
                }
                else if (i == 25 && costume.texture[i] < -VanillaCounts.KneepadCount)
                {
                    string name = ContentMappings.ContentMap.KneepadNameMap[
                        -costume.texture[i] - VanillaCounts.KneepadCount - 1];
                    this.CustomKneepad = name;
                }
                else
                {
                    this.CustomMaterials.Add(null);
                }
            }

            for (int i = 0; i < costume.flesh.Length; i++)
            {
                if (VanillaCounts.FleshCounts[i] == 0)
                {
                    this.CustomFlesh.Add(null);
                    continue;
                }

                if (costume.flesh[i] > VanillaCounts.FleshCounts[i])
                {
                    string name = ContentMappings.ContentMap.FleshNameMap[i][
                        costume.flesh[i] - VanillaCounts.FleshCounts[i] - 1];
                    this.CustomFlesh.Add(name);
                }
                else if (i == 2 && costume.flesh[i] < -VanillaCounts.BodyFemaleCount)
                {
                    string name = ContentMappings.ContentMap.BodyFemaleNameMap[
                        -costume.flesh[i] - VanillaCounts.BodyFemaleCount - 1];
                    this.CustomFemaleBody = name;
                }
                else
                {
                    this.CustomFlesh.Add(null);
                }
            }

            for (int i = 0; i < costume.shape.Length; i++)
            {
                if (VanillaCounts.ShapeCounts[i] == 0)
                {
                    this.CustomShapes.Add(null);
                    continue;
                }

                if (costume.shape[i] > VanillaCounts.ShapeCounts[i])
                {
                    string name = ContentMappings.ContentMap.ShapeNameMap[i][
                        costume.shape[i] - VanillaCounts.ShapeCounts[i] - 1];
                    this.CustomShapes.Add(name);
                }
                else if (i == 17 && costume.shape[i] < -VanillaCounts.TransparentHairHairstyleCount)
                {
                    string name = ContentMappings.ContentMap.TransparentHairHairstyleNameMap[
                        -costume.shape[i] - VanillaCounts.TransparentHairHairstyleCount - 1];
                    this.CustomTransparentHairHairstyle = name;
                }
                else
                {
                    this.CustomShapes.Add(null);
                }
            }
        }

        if (character.music > VanillaCounts.MusicCount)
        {
            string name = ContentMappings.ContentMap.MusicNameMap[
                character.music - VanillaCounts.MusicCount - 1];
            this.CustomThemeName = name;
        }
    }

    public CharacterWithModdedData() { }
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

    public string OverrideMode { get; set; } = "append";

    public bool IsModded =>
        this.CustomMaterials.Exists(x => x != null) || this.CustomFlesh.Exists(x => x != null) ||
        this.CustomShapes.Exists(x => x != null) || this.CustomFemaleBody != null || this.CustomFemaleFace != null ||
        this.CustomSpecialFootwear != null || this.CustomTransparentHairMaterial != null ||
        this.CustomTransparentHairHairstyle != null || this.CustomKneepad != null || this.CustomThemeName != null;

    public Character ModdedToCharacter()
    {
        Character character = this.BaseCharacter;
        if (!this.IsModded)
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
                if (this.CustomMaterials[j] != null)
                {
                    if (ContentMappings.ContentMap.MaterialNameMap[j].Contains(this.CustomMaterials[j]))
                    {
                        character.costume[i].texture[j] =
                            ContentMappings.ContentMap.MaterialNameMap[j].IndexOf(this.CustomMaterials[j]) +
                            VanillaCounts.MaterialCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom material {this.CustomMaterials[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 3 && this.CustomFemaleFace != null)
                {
                    if (ContentMappings.ContentMap.FaceFemaleNameMap.Contains(this.CustomFemaleFace))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.FaceFemaleNameMap.IndexOf(this.CustomFemaleFace) -
                            VanillaCounts.FaceFemaleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom female face {this.CustomFemaleFace} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }

                if (j == 14 && this.CustomSpecialFootwear != null)
                {
                    if (ContentMappings.ContentMap.SpecialFootwearNameMap.Contains(this.CustomSpecialFootwear))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.SpecialFootwearNameMap
                                .IndexOf(this.CustomSpecialFootwear) -
                            VanillaCounts.SpecialFootwearCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom special footwear {this.CustomSpecialFootwear} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }

                if (j == 15 && this.CustomSpecialFootwear != null)
                {
                    if (ContentMappings.ContentMap.SpecialFootwearNameMap.Contains(this.CustomSpecialFootwear))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.SpecialFootwearNameMap
                                .IndexOf(this.CustomSpecialFootwear) -
                            VanillaCounts.SpecialFootwearCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom special footwear {this.CustomSpecialFootwear} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }

                if (j == 17 && this.CustomTransparentHairMaterial != null)
                {
                    if (ContentMappings.ContentMap.TransparentHairMaterialNameMap.Contains(
                            this.CustomTransparentHairMaterial))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.TransparentHairMaterialNameMap.IndexOf(
                                this.CustomTransparentHairMaterial) -
                            VanillaCounts.TransparentHairMaterialCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom transparent hair material {this.CustomTransparentHairMaterial} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }

                if (j == 24 && this.CustomKneepad != null)
                {
                    if (ContentMappings.ContentMap.KneepadNameMap.Contains(this.CustomKneepad))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.KneepadNameMap.IndexOf(this.CustomKneepad) -
                            VanillaCounts.KneepadCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom kneepad {this.CustomKneepad} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }

                if (j == 25 && this.CustomKneepad != null)
                {
                    if (ContentMappings.ContentMap.KneepadNameMap.Contains(this.CustomKneepad))
                    {
                        character.costume[i].texture[j] =
                            -ContentMappings.ContentMap.KneepadNameMap.IndexOf(this.CustomKneepad) -
                            VanillaCounts.KneepadCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom kneepad {this.CustomKneepad} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }

            for (int j = 0; j < character.costume[i].shape.Length; j++)
            {
                if (this.CustomShapes[j] != null)
                {
                    if (ContentMappings.ContentMap.ShapeNameMap[j].Contains(this.CustomShapes[j]))
                    {
                        character.costume[i].shape[j] =
                            ContentMappings.ContentMap.ShapeNameMap[j].IndexOf(this.CustomShapes[j]) +
                            VanillaCounts.ShapeCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom shape {this.CustomShapes[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 17 && this.CustomTransparentHairHairstyle != null)
                {
                    if (ContentMappings.ContentMap.TransparentHairHairstyleNameMap.Contains(
                            this.CustomTransparentHairHairstyle))
                    {
                        character.costume[i].shape[j] =
                            -ContentMappings.ContentMap.TransparentHairHairstyleNameMap.IndexOf(
                                this.CustomTransparentHairHairstyle) -
                            VanillaCounts.TransparentHairHairstyleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom transparent hair hairstyle {this.CustomTransparentHairHairstyle} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }

            for (int j = 0; j < character.costume[i].shape.Length; j++)
            {
                if (this.CustomShapes[j] != null)
                {
                    if (ContentMappings.ContentMap.ShapeNameMap[j].Contains(this.CustomShapes[j]))
                    {
                        character.costume[i].shape[j] =
                            ContentMappings.ContentMap.ShapeNameMap[j].IndexOf(this.CustomShapes[j]) +
                            VanillaCounts.ShapeCounts[j] + 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom shape {this.CustomShapes[j]} that does not exist in the custom content save file. The character will not be loaded.");
                        return null;
                    }
                }

                if (j == 17 && this.CustomTransparentHairHairstyle != null)
                {
                    if (ContentMappings.ContentMap.TransparentHairHairstyleNameMap.Contains(
                            this.CustomTransparentHairHairstyle))
                    {
                        character.costume[i].shape[j] =
                            -ContentMappings.ContentMap.TransparentHairHairstyleNameMap.IndexOf(
                                this.CustomTransparentHairHairstyle) -
                            VanillaCounts.TransparentHairHairstyleCount - 1;
                    }
                    else
                    {
                        Plugin.Log.LogError(
                            $"The character {character.name} has a custom transparent hair hairstyle {this.CustomTransparentHairHairstyle} that does not exist in the custom content save file. The character will not be loaded.");
                    }
                }
            }

            if (this.CustomThemeName != null)
            {
                if (ContentMappings.ContentMap.MusicNameMap.Contains(this.CustomThemeName))
                {
                    character.music = ContentMappings.ContentMap.MusicNameMap.IndexOf(this.CustomThemeName) +
                                      VanillaCounts.MusicCount + 1;
                }
                else
                {
                    Plugin.Log.LogError(
                        $"The character {character.name} has a custom theme {this.CustomThemeName} that does not exist in the custom content save file. The character will not be loaded.");
                    return null;
                }
            }
        }

        Plugin.Log.LogInfo($"Loaded character {character.name}.");
        return character;
    }
}