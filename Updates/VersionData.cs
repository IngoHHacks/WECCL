using Newtonsoft.Json;
using WECCL.Content;

namespace WECCL.Updates;

internal class VersionData
{
    public static void WriteVersionData()
    {
        var json = JsonConvert.SerializeObject(new VanillaCounts());
        if (!Directory.Exists(Plugin.DebugFilesDir.FullName))
            Directory.CreateDirectory(Plugin.DebugFilesDir.FullName);
        
        File.WriteAllText(Path.Combine(Plugin.DebugFilesDir.FullName, "VanillaCounts.json"), json);
    }
}