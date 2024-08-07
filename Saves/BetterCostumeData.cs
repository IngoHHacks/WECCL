using Newtonsoft.Json;
using WECCL.Content;
// ReSharper disable InconsistentNaming
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace WECCL.Saves;

internal class BetterCostumeData
{
    public float[] b = new float[40];

    public int charID;

    public string[] fleshC = new string[40];

    public float[] g = new float[40];
    public int id;

    public int limb;

    public float[] r = new float[40];

    public string[] shapeC = new string[40];

    public string[] textureC = new string[40];


    public static BetterCostumeData FromRegularCostumeData(Costume costume)
    {
        if (costume == null)
        {
            return null;
        }

        BetterCostumeData bcd = JsonConvert.DeserializeObject<BetterCostumeData>(JsonConvert.SerializeObject(costume))!;
        for (int i = 0; i < costume.texture.Length; i++)
        {
            if (costume.texture[i] > VanillaCounts.Data.MaterialCounts[i])
            {
                int index = costume.texture[i] - VanillaCounts.Data.MaterialCounts[i] - 1;
                string material = ContentMappings.ContentMap.MaterialNameMap[i][index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 3 && costume.texture[i] < -VanillaCounts.Data.FaceFemaleCount)
            {
                int index = -costume.texture[i] - VanillaCounts.Data.FaceFemaleCount - 1;
                string material = ContentMappings.ContentMap.FaceFemaleNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 14 && costume.texture[i] < -VanillaCounts.Data.SpecialFootwearCount)
            {
                int index = -costume.texture[i] - VanillaCounts.Data.SpecialFootwearCount - 1;
                string material = ContentMappings.ContentMap.SpecialFootwearNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 17 && costume.texture[i] < -VanillaCounts.Data.TransparentHairMaterialCount)
            {
                int index = -costume.texture[i] - VanillaCounts.Data.TransparentHairMaterialCount - 1;
                string material = ContentMappings.ContentMap.TransparentHairMaterialNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if ((i == 25 || i == 26) && costume.texture[i] < -VanillaCounts.Data.KneepadCount)
            {
                int index = -costume.texture[i] - VanillaCounts.Data.KneepadCount - 1;
                string material = ContentMappings.ContentMap.KneepadNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else
            {
                int index = costume.texture[i];
                bcd.textureC[i] = "Vanilla/" + index;
            }
        }

        for (int i = 0; i < costume.flesh.Length; i++)
        {
            if (i == 17 && costume.flesh[i] == 100)
            {
                int index = costume.flesh[i];
                bcd.fleshC[i] = "Vanilla/" + index;
            }
            else if (costume.flesh[i] > VanillaCounts.Data.FleshCounts[i])
            {
                int index = costume.flesh[i] - VanillaCounts.Data.FleshCounts[i] - 1;
                if (index >= ContentMappings.ContentMap.FleshNameMap[i].Count || index < 0)
                {
                    bcd.fleshC[i] = "Vanilla/" + costume.flesh[i];
                }
                else
                {
                    string material = ContentMappings.ContentMap.FleshNameMap[i][index];
                    bcd.fleshC[i] = "Custom/" + material;
                }
            }
            else if (i == 2 && costume.flesh[i] < -VanillaCounts.Data.BodyFemaleCount)
            {
                int index = -costume.flesh[i] - VanillaCounts.Data.BodyFemaleCount - 1;
                string material = ContentMappings.ContentMap.BodyFemaleNameMap[index];
                bcd.fleshC[i] = "Custom/" + material;
            }
            else
            {
                int index = costume.flesh[i];
                bcd.fleshC[i] = "Vanilla/" + index;
            }
        }

        for (int i = 0; i < costume.shape.Length; i++)
        {
            if ((costume.shape[i] > 50 && costume.shape[i] % 10 == 0) || VanillaCounts.Data.ShapeCounts[i] == 0)
            {
                int index = costume.shape[i];
                bcd.shapeC[i] = "Vanilla/" + index;
            }
            else if (costume.shape[i] > VanillaCounts.Data.ShapeCounts[i])
            {
                int index = costume.shape[i] - VanillaCounts.Data.ShapeCounts[i] - 1;
                if (index >= ContentMappings.ContentMap.ShapeNameMap[i].Count || index < 0)
                {
                    bcd.shapeC[i] = "Vanilla/" + costume.shape[i];
                }
                else
                {
                    string material = ContentMappings.ContentMap.ShapeNameMap[i][index];
                    bcd.shapeC[i] = "Custom/" + material;
                }
            }
            else if (i == 17 && costume.shape[i] < -VanillaCounts.Data.TransparentHairHairstyleCount)
            {
                int index = -costume.shape[i] - VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                string material = ContentMappings.ContentMap.TransparentHairHairstyleNameMap[index];
                bcd.shapeC[i] = "Custom/" + material;
            }
            else
            {
                int index = costume.shape[i];
                bcd.shapeC[i] = "Vanilla/" + index;
            }
        }

        return bcd;
    }

    public Costume ToRegularCostume()
    {
        Costume costume = JsonConvert.DeserializeObject<Costume>(JsonConvert.SerializeObject(this))!;

        for (int i = 0; i < costume.texture.Length; i++)
        {
            costume.texture[i] = ConvertTexture(this.textureC[i], i);
        }

        for (int i = 0; i < costume.flesh.Length; i++)
        {
            costume.flesh[i] = ConvertFlesh(this.fleshC[i], i);
        }

        for (int i = 0; i < costume.shape.Length; i++)
        {
            costume.shape[i] = ConvertShape(this.shapeC[i], i);
        }

        return costume;
    }

    private int ConvertTexture(string texture, int index)
    {

        if (texture.StartsWith("Custom/"))
        {
            string material = texture.Substring(7);
            try
            {
                int? foundIndex = null;
                switch(index)
                {
                    case 3:
                        foundIndex = FindIndex(ContentMappings.ContentMap.FaceFemaleNameMap, material, VanillaCounts.Data.FaceFemaleCount);
                        break;
                    case 14:
                        foundIndex = FindIndex(ContentMappings.ContentMap.SpecialFootwearNameMap, material, VanillaCounts.Data.SpecialFootwearCount);
                        break;
                    case 17:
                        foundIndex = FindIndex(ContentMappings.ContentMap.TransparentHairMaterialNameMap, material, VanillaCounts.Data.TransparentHairMaterialCount);
                        break;
                    case 25:
                        foundIndex = FindIndex(ContentMappings.ContentMap.KneepadNameMap, material, VanillaCounts.Data.KneepadCount);
                        break;
                    case 26:
                        foundIndex = FindIndex(ContentMappings.ContentMap.KneepadNameMap, material, VanillaCounts.Data.KneepadCount);
                        break;
                }
                if(foundIndex == null)
                {
                    foundIndex = FindIndexDefault(ContentMappings.ContentMap.MaterialNameMap[index], material, VanillaCounts.Data.MaterialCounts[index]);
                }
                if(foundIndex == null)
                {
                    LogWarning($"Failed to find texture from name {texture}, setting to 0.");
                    return 0;
                }
                else
                {
                    return foundIndex.Value;
                }
            }
            catch
            {
                LogWarning($"Failed to find texture from name {texture}, setting to 0.");
                return 0;
            }
        }
        else if (texture.StartsWith("Vanilla/"))
        {
            return int.Parse(texture.Substring(8));
        }

        return 0;
    }

    private int ConvertFlesh(string flesh, int index)
    {
        if (flesh.StartsWith("Custom/"))
        {
            string material = flesh.Substring(7);
            try
            {
                int? foundIndex = null;
                if (index == 2)
                {
                    foundIndex = FindIndex(ContentMappings.ContentMap.BodyFemaleNameMap, material, VanillaCounts.Data.BodyFemaleCount);
                }
                if(foundIndex == null)
                {
                    foundIndex = FindIndexDefault(ContentMappings.ContentMap.FleshNameMap[index], material, VanillaCounts.Data.FleshCounts[index]); ;
                }
                if(foundIndex == null)
                {
                    LogWarning($"Failed to find flesh from name {flesh}, setting to 0.");
                    return 0;
                }
                else
                {
                    return foundIndex.Value;
                }

            }
            catch
            {
                LogWarning($"Failed to find flesh from name {flesh}, setting to 0.");
                return 0;
            }
        }
        else if (flesh.StartsWith("Vanilla/"))
        {
            return int.Parse(flesh.Substring(8));
        }

        return 0;
    }

    private int ConvertShape(string shape, int index)
    {
        if (shape.StartsWith("Custom/"))
        {
            string material = shape.Substring(7);
            try
            {
                int? foundIndex = null;
                if (index == 17)
                {
                    foundIndex = FindIndex(ContentMappings.ContentMap.TransparentHairHairstyleNameMap, material, VanillaCounts.Data.TransparentHairHairstyleCount);
                }
                if (foundIndex == null)
                {
                    foundIndex = FindIndexDefault(ContentMappings.ContentMap.ShapeNameMap[index], material, VanillaCounts.Data.ShapeCounts[index]);
                }
                if (foundIndex == null)
                {
                    LogWarning($"Failed to find shape from name {shape}, setting to 0.");
                    return 0;
                }
                else
                {
                    return foundIndex.Value;
                }
            }
            catch
            {
                LogWarning($"Failed to find shape from name {shape}, setting to 0.");
                return 0;
            }
        }
        else if (shape.StartsWith("Vanilla/"))
        {
            return int.Parse(shape.Substring(8));
        }

        return 0;
    }

    private int? FindIndex(List<string> map, string material, int count)
    {
        int index = map.IndexOf(material);
        if (index != -1)
        {
            return -index - count - 1;
        }
        return null;
    }
    private int? FindIndexDefault(List<string> map, string material, int count)
    {
        int index = map.IndexOf(material);
        if (index != -1)
        {
            return index + count + 1;
        }
        return null;
    }

}