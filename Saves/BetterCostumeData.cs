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
            else if (i == 15 && costume.texture[i] < -VanillaCounts.Data.SpecialFootwearCount)
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
            else if (i == 24 && costume.texture[i] < -VanillaCounts.Data.KneepadCount)
            {
                int index = -costume.texture[i] - VanillaCounts.Data.KneepadCount - 1;
                string material = ContentMappings.ContentMap.KneepadNameMap[index];
                bcd.textureC[i] = "Custom/" + material;
            }
            else if (i == 25 && costume.texture[i] < -VanillaCounts.Data.KneepadCount)
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
            if (i == 3 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.FaceFemaleNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.FaceFemaleCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            else if (i == 14 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.SpecialFootwearNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.SpecialFootwearCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            else if (i == 15 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.SpecialFootwearNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.SpecialFootwearCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            else if (i == 17 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.TransparentHairMaterialNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.TransparentHairMaterialCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            else if (i == 24 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.KneepadNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.KneepadCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            else if (i == 25 && this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.KneepadNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = -index - VanillaCounts.Data.KneepadCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            if (this.textureC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.textureC[i].Substring(7);
                    int index = ContentMappings.ContentMap.MaterialNameMap[i].IndexOf(material);
                    if (index >= 0) {
                        costume.texture[i] = index + VanillaCounts.Data.MaterialCounts[i] + 1;
                        continue;
                    }
                    LogWarning($"Failed to find material from name {this.textureC[i]}, setting to 0.");
                    costume.texture[i] = 0;
                }
                catch {
                    LogWarning($"Failed to find material from name {this.textureC[i]}, setting to 0.");
                    costume.texture[i] = 0;
                }
            }
            else
            {
                string index = this.textureC[i].Substring(8);
                costume.texture[i] = int.Parse(index);
            }
        }

        for (int i = 0; i < costume.flesh.Length; i++)
        {
            if (i == 2 && this.fleshC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.fleshC[i].Substring(7);
                    int index = ContentMappings.ContentMap.BodyFemaleNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.flesh[i] = -index - VanillaCounts.Data.BodyFemaleCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            if (this.fleshC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.fleshC[i].Substring(7);
                    int index = ContentMappings.ContentMap.FleshNameMap[i].IndexOf(material);
                    if (index >= 0) {
                        costume.flesh[i] = index + VanillaCounts.Data.FleshCounts[i] + 1;
                        continue;
                    }
                    LogWarning($"Failed to find flesh from name {this.fleshC[i]}, setting to 0.");
                    costume.flesh[i] = 0;
                }
                catch
                {
                    LogWarning($"Failed to find flesh from name {this.fleshC[i]}, setting to 0.");
                    costume.flesh[i] = 0;
                }
            }
            else
            {
                string index = this.fleshC[i].Substring(8);
                costume.flesh[i] = int.Parse(index);
            }
        }

        for (int i = 0; i < costume.shape.Length; i++)
        {
            costume.shape[i] = 0;
            if (i == 17 && this.shapeC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.shapeC[i].Substring(7);
                    int index = ContentMappings.ContentMap.TransparentHairHairstyleNameMap.IndexOf(material);
                    if (index >= 0) {
                        costume.shape[i] = -index - VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                        continue;
                    }
                }
                catch {}
            }
            if (this.shapeC[i].StartsWith("Custom/"))
            {
                try
                {
                    string material = this.shapeC[i].Substring(7);
                    int index = ContentMappings.ContentMap.ShapeNameMap[i].IndexOf(material);
                    if (index >= 0) {
                        costume.shape[i] = index + VanillaCounts.Data.ShapeCounts[i] + 1;
                        continue;
                    }
                    LogWarning($"Failed to find shape from name {this.shapeC[i]}, setting to 0.");
                    costume.shape[i] = 0;
                }
                catch
                {
                    LogWarning($"Failed to find shape from name {this.shapeC[i]}, setting to 0.");
                    costume.shape[i] = 0;
                }
            }
            else
            {
                string index = this.shapeC[i].Substring(8);
                costume.shape[i] = int.Parse(index);
            }
        }

        return costume;
    }
}