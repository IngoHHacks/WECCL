namespace WECCL.Utils;

public class Indices
{
    public static Dictionary<string, int> CrowdAudio = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Train", 0 },
        { "Oh", 1 },
        { "Cheer", 2 },
        { "Boo", 3 },
        { "Yay", 4 },
        { "Groan", 5 },
        { "Excitement", 6 },
        { "Murmur", 7 },
        { "Laughter", 8 },
        { "Applause", 9 },
        { "Chant_Generic", 10 },
        { "Stomping", 11 },
        { "Chant_Yes", 12 },
        { "Chant_Sucks", 13 },
        { "Chant_Got", 14 },
        { "Chant_Awesome", 15 },
        { "Chant_CW", 16 },
        { "Chant_Holy", 17 },
        { "Chant_Fucked", 18 },
        { "Chant_Wrestle", 19 },
        { "Chant_Bullshit", 20 },
        { "Chant_Boring", 21 },
        { "Chant_USA", 22 },
        { "Countdown", 30 },
        { "Chant_Count01", 31 },
        { "Chant_Count02", 32 },
        { "Chant_Count03", 33 },
        { "Chant_Count04", 34 },
        { "Chant_Count05", 35 },
        { "Chant_Count06", 36 },
        { "Chant_Count07", 37 },
        { "Chant_Count08", 38 },
        { "Chant_Count09", 39 },
        { "Chant_Count10", 40 },
        { "Oh02", 41 },
        { "Cheer02", 42 },
        { "Boo02", 43 },
        { "Yay02", 44 },
        { "Groan02", 45 },
        { "Excitement02", 46 },
        { "Murmur02", 47 },
        { "Laughter02", 48 },
        { "Applause02", 49 },
        { "Oh03", 51 },
        { "Cheer03", 52 },
        { "Boo03", 53 },
        { "Yay03", 54 },
        { "Groan03", 55 },
        { "Excitement03", 56 },
        { "Murmur03", 57 },
        { "Laughter03", 58 },
        { "Applause03", 59 }
    };

    public static Dictionary<string, int> TauntAnims
    {
        get
        {
            field ??= BuildTauntAnims();
            return field;
        }
    }

    private static Dictionary<string, int> BuildTauntAnims()
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var anim = new UnmappedTaunt();
        for (int i = 0; i <= MBLIOKEDHHB.LNPCGEEMLOB; i++)
        {
            anim.DDNPOAKEOMF(i);
            if (!string.IsNullOrEmpty(anim.CMECDGMCMLC))
                dict.TryAdd(anim.CMECDGMCMLC, i);
        }
        return dict;
    }

    public static int ParseCrowdAudio(string audio)
    {
        if (int.TryParse(audio, out int index))
        {
            return index < CrowdAudio.Count
                ? index
                : throw new Exception($"Crowd audio index out of range: {audio}, max: {CrowdAudio.Count - 1}");
        }

        if (CrowdAudio.TryGetValue(audio, out index))
        {
            return index;
        }

        throw new Exception($"Unknown crowd audio: {audio}");
    }

    public static int ParseTauntAnim(string anim)
    {
        if (int.TryParse(anim, out int index))
        {
            return index < TauntAnims.Count
                ? index
                : throw new Exception($"Taunt animation index out of range: {anim}, max: {TauntAnims.Count - 1}");
        }

        if (TauntAnims.TryGetValue(anim.Replace("_", " "), out index))
        {
            return index;
        }

        throw new Exception($"Unknown taunt animation: {anim}");
    }
}