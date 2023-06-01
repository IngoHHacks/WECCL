using Newtonsoft.Json;
using WECCL.Content;

namespace WECCL.Saves;

public class BetterCostumeData
{
    public int id;

    public int charID;

    public int limb;

    public string[] textureC = new string[40];

    public float[] r = new float[40];

    public float[] g = new float[40];

    public float[] b = new float[40];

    public string[] fleshC = new string[40];

    public string[] shapeC = new string[40];


    public static BetterCostumeData FromRegularCostumeData(Costume costume)
    {
        if (costume == null) return null;
        BetterCostumeData bcd = JsonConvert.DeserializeObject<BetterCostumeData>(JsonConvert.SerializeObject(costume))!;
        for (int i = 0; i < costume.texture.Length; i++)
        {
            if (costume.texture[i] > VanillaCounts.MaterialCounts[i])
            {
                var index = costume.texture[i] - VanillaCounts.MaterialCounts[i] - 1;
                var material = ContentMappings.ContentMap.MaterialNameMap[i][index];
                bcd.textureC[i] = "Custom/" + material;
            } else if (i == 3 && costume.texture[i] < -VanillaCounts.FaceFemaleCount)
            {
                var index = -costume.texture[i] - VanillaCounts.FaceFemaleCount - 1;
                var material = ContentMappings.ContentMap.FaceFemaleNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 14 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
            {
                var index = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                var material = ContentMappings.ContentMap.SpecialFootwearNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 15 && costume.texture[i] < -VanillaCounts.SpecialFootwearCount)
            {
                var index = -costume.texture[i] - VanillaCounts.SpecialFootwearCount - 1;
                var material = ContentMappings.ContentMap.SpecialFootwearNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 17 && costume.texture[i] < -VanillaCounts.TransparentHairMaterialCount)
            {
                var index = -costume.texture[i] - VanillaCounts.TransparentHairMaterialCount - 1;
                var material = ContentMappings.ContentMap.TransparentHairMaterialNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 24 && costume.texture[i] < -VanillaCounts.KneepadCount)
            {
                var index = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                var material = ContentMappings.ContentMap.KneepadNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 25 && costume.texture[i] < -VanillaCounts.KneepadCount)
            {
                var index = -costume.texture[i] - VanillaCounts.KneepadCount - 1;
                var material = ContentMappings.ContentMap.KneepadNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else
            {
                var index = costume.texture[i];
                bcd.textureC[i] = "Vanilla/" + index;
            }
        }
        for (int i = 0; i < costume.flesh.Length; i++)
        {
            if (i == 17 && costume.flesh[i] == 100)
            {
                var index = costume.flesh[i];
                bcd.fleshC[i] = "Vanilla/" + index;
            }
            else if (costume.flesh[i] > VanillaCounts.FleshCounts[i])
            {
                var index = costume.flesh[i] - VanillaCounts.FleshCounts[i] - 1;
                if (index >= ContentMappings.ContentMap.FleshNameMap[i].Count || index < 0)
                {
                    bcd.fleshC[i] = "Vanilla/" + costume.flesh[i];
                }
                else
                {
                    var material = ContentMappings.ContentMap.FleshNameMap[i][index];
                    bcd.fleshC[i] = "Custom/" + material;
                }
            }
            else if (i == 2 && costume.flesh[i] < -VanillaCounts.BodyFemaleCount)
            {
                var index = -costume.flesh[i] - VanillaCounts.BodyFemaleCount - 1;
                var material = ContentMappings.ContentMap.BodyFemaleNameMap[index];
                bcd.fleshC[i] = "Custom/" + material;
            }
            else
            {
                var index = costume.flesh[i];
                bcd.fleshC[i] = "Vanilla/" + index;
            }
        }
        for (int i = 0; i < costume.shape.Length; i++)
        {
            if ((costume.shape[i] > 50 && costume.shape[i] % 10 == 0) || VanillaCounts.ShapeCounts[i] == 0)
            {
                var index = costume.shape[i];
                bcd.shapeC[i] = "Vanilla/" + index;
            }
            else if (costume.shape[i] > VanillaCounts.ShapeCounts[i])
            {
                var index = costume.shape[i] - VanillaCounts.ShapeCounts[i] - 1;
                if (index >= ContentMappings.ContentMap.ShapeNameMap[i].Count || index < 0)
                {
                    bcd.shapeC[i] = "Vanilla/" + costume.shape[i];
                }
                else
                {
                    var material = ContentMappings.ContentMap.ShapeNameMap[i][index];
                    bcd.shapeC[i] = "Custom/" + material;
                }
            }
            else if (i == 17 && costume.shape[i] < -VanillaCounts.TransparentHairHairstyleCount)
            {
                var index = -costume.shape[i] - VanillaCounts.TransparentHairHairstyleCount - 1;
                var material = ContentMappings.ContentMap.TransparentHairHairstyleNameMap[index];
                bcd.shapeC[i] = "Custom/" + material;
            }
            else
            {
                var index = costume.shape[i];
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
            if (textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.MaterialNameMap[i].IndexOf(material);
                costume.texture[i] = index + VanillaCounts.MaterialCounts[i] + 1;
            }
            else if (i == 3 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.KneepadNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.KneepadCount - 1;
            }
            else if (i == 14 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.SpecialFootwearNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.SpecialFootwearCount - 1;
            }
            else if (i == 15 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.SpecialFootwearNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.SpecialFootwearCount - 1;
            }
            else if (i == 17 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.TransparentHairMaterialNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.TransparentHairMaterialCount - 1;
            }
            else if (i == 24 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.KneepadNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.KneepadCount - 1;
            }
            else if (i == 25 && textureC[i].StartsWith("Custom/"))
            {
                var material = textureC[i].Substring(7);
                var index = ContentMappings.ContentMap.KneepadNameMap.IndexOf(material);
                costume.texture[i] = -index - VanillaCounts.KneepadCount - 1;
            }
            else
            {
                var index = textureC[i].Substring(8);
                costume.texture[i] = int.Parse(index);
            }
        }
        for (int i = 0; i < costume.flesh.Length; i++)
        {
            if (fleshC[i].StartsWith("Custom/"))
            {
                var material = fleshC[i].Substring(7);
                var index = ContentMappings.ContentMap.FleshNameMap[i].IndexOf(material);
                costume.flesh[i] = index + VanillaCounts.FleshCounts[i] + 1;
            }
            else if (i == 2 && fleshC[i].StartsWith("Custom/"))
            {
                var material = fleshC[i].Substring(7);
                var index = ContentMappings.ContentMap.BodyFemaleNameMap.IndexOf(material);
                costume.flesh[i] = -index - VanillaCounts.BodyFemaleCount - 1;
            }
            else
            {
                var index = fleshC[i].Substring(8);
                costume.flesh[i] = int.Parse(index);
            }
        }
        for (int i = 0; i < costume.shape.Length; i++)
        {
            if (shapeC[i].StartsWith("Custom/"))
            {
                var material = shapeC[i].Substring(7);
                var index = ContentMappings.ContentMap.ShapeNameMap[i].IndexOf(material);
                costume.shape[i] = index + VanillaCounts.ShapeCounts[i] + 1;
            }
            else if (i == 17 && shapeC[i].StartsWith("Custom/"))
            {
                var material = shapeC[i].Substring(7);
                var index = ContentMappings.ContentMap.TransparentHairHairstyleNameMap.IndexOf(material);
                costume.shape[i] = -index - VanillaCounts.TransparentHairHairstyleCount - 1;
            }
            else
            {
                var index = shapeC[i].Substring(8);
                costume.shape[i] = int.Parse(index);
            }
        }
        return costume;
    }
}