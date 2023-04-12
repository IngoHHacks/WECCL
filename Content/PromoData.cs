namespace WECCL.Content;

public class PromoData
{
    public class PromoLine
    {
        public string Line1 { get; set; } = "Line 1";
        public string Line2 { get; set; } = "Line 2";

        public int From { get; set; } = 1;
        public int To { get; set; } = 2;

        public float Demeanor { get; set; } = 0;
        public int TauntAnim { get; set; } = 0;
    }
    
    public List<PromoLine> PromoLines { get; set; } = new();
    
    public string Title { get; set; } = "Title";
    public string Description { get; set; } = "Description";

    public int[] Characters { get; set; } = new int[3];
    
    public int NumLines => PromoLines.Count;

    public static PromoData FromString(List<string> lines)
    {
        try
        {
            var promoData = new PromoData();
            int c;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("title:"))
                {
                    promoData.Title = line.Substring(6).Trim();
                    continue;
                }
                if (line.StartsWith("description:"))
                {
                    promoData.Description = line.Substring(12).Trim();
                    continue;
                }
                if (line.StartsWith("characters:"))
                {
                    if (!line.Contains(","))
                    {
                        var num = int.Parse(line.Substring(11).Trim());
                        promoData.Characters = GenerateArrayForNum(num);
                    }
                    else
                    {
                        var chars = line.Substring(11).Split(',').Select(x => int.Parse(x.Trim()));
                        IEnumerable<int> enumerable = chars.ToList();
                        if (enumerable.First() != 0)
                        {
                            enumerable = enumerable.Prepend(0);
                        }
                        promoData.Characters = enumerable.ToArray();
                    }
                    continue;
                }

                c = 0;
                var promoLine = new PromoLine();
                bool stringMode = false;
                string currentString = "";
                bool escape = false;
                int last = -1;
                for (int j = 0; j < line.Length; j++)
                {
                    char ch = line[j];
                    if (escape)
                    {
                        currentString += ch;
                        escape = false;
                        continue;
                    }
                    if (ch == '\\')
                    {
                        escape = true;
                        continue;
                    }
                    if (ch == '"')
                    {
                        stringMode = !stringMode;
                        continue;
                    }
                    if (ch == ',' && !stringMode)
                    {
                        switch (c)
                        {
                            case 0:
                                promoLine.Line1 = currentString.Trim();
                                break;
                            case 1:
                                promoLine.Line2 = currentString.Trim();
                                break;
                        }
                        c++;
                        currentString = "";
                        if (c == 2)
                        {
                            last = j;
                            break;
                        }
                        continue;
                    }
                    currentString += ch;
                }
                if (last == -1)
                {
                    continue;
                }
                var meta2 = line.Substring(last + 1).Split(',');
                promoLine.From = meta2.Length > 0 ? int.Parse(meta2[0].Trim()) : 1;
                promoLine.To = meta2.Length > 1 ? int.Parse(meta2[1].Trim()) : 2;
                promoLine.TauntAnim = meta2.Length > 2 ? int.Parse(meta2[2].Trim()) : 0;
                promoLine.Demeanor = meta2.Length > 3 ? float.Parse(meta2[3].Trim()) : 0;
                promoData.PromoLines.Add(promoLine);
            }
            return promoData;
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            return null;
        }
    }

    public static PromoData CreatePromo(string file)
    {
        var lines = File.ReadAllLines(file).ToList();
        return FromString(lines);
    }
    
    private static int[] GenerateArrayForNum(int characters)
    {
        int[] arr = new int[characters+1];
        for (int i = 1; i <= characters; i++)
        {
            arr[i] = i;
        }
        return arr;
    }
}