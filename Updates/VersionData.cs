using Newtonsoft.Json;
using WECCL.Content;

namespace WECCL.Updates;

internal class VersionData
{
    public static void WriteVersionData()
    {
        var json = JsonConvert.SerializeObject(new VanillaCounts());
        if (!Directory.Exists(Locations.Debug.FullName))
            Directory.CreateDirectory(Locations.Debug.FullName);
        
        File.WriteAllText(Path.Combine(Locations.Debug.FullName, "VanillaCounts.json"), json);
    }
}