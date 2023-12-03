namespace WECCL.Content;
public class CustomMatch
{
    internal static Dictionary<string, int> CustomPresetsPos = new();
    internal static Dictionary<string, int> CustomPresetsNeg = new();
    internal static Dictionary<string, int> CustomCagesPos = new();
    internal static Dictionary<string, int> CustomCagesNeg = new();
    public static int? RegisterCustomPreset(string Name, bool PositiveValue)
    {
        int value;
        if (PositiveValue)
        {
            if (Register(Name, CustomPresetsPos, "Preset", out value))
            {
                Plugin.Log.LogInfo("REGISTERED " + Name + " " + -value);
                CustomPresetsPos.Add(Name, value);
            }
            return value;
        }
        else
        {
            if (Register(Name, CustomPresetsNeg, "Preset", out value))
            {
                Plugin.Log.LogInfo("REGISTERED " + Name + " " + -value);
                CustomPresetsNeg.Add(Name, -value);
            }
            return -value;
        }
    }
    private static bool Register(string Name, Dictionary<string, int> dictionary, string type, out int pos)
    {
        if (dictionary.TryGetValue(Name, out pos))
        {
            Plugin.Log.LogWarning(Name + " is already registered as " + type + " " + pos);
            return false;
        }
        pos = 10001 + dictionary.Count;
        return true;
    }
    public static int? RegisterCustomCage(string Name, bool PositiveValue)
    {
        int value;
        if (PositiveValue)
        {
            if (Register(Name, CustomCagesPos, "Cage", out value))
            {
                Plugin.Log.LogInfo("REGISTERED " + Name + " " + value);
                CustomCagesPos.Add(Name, value);
            }
            return value;
        }
        else
        {
            if (Register(Name, CustomCagesNeg, "Cage", out value))
            {
                Plugin.Log.LogInfo("REGISTERED " + Name + " " + -value);
                CustomCagesNeg.Add(Name, -value);
            }
            return -value;
        }
    }
}
