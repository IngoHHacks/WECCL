namespace WECCL.Content;

public class Aliases
{
    public static Dictionary<string, string> AliasMap = new();
    
    public static void Load()
    {
        var aliasLoc = Locations.Assets;
        foreach (var file in aliasLoc.GetFiles("*.aliases", SearchOption.AllDirectories))
        {
            var lines = File.ReadAllLines(file.FullName);
            var pluginName = FindPluginName(file.Directory);
            foreach (var line in lines)
            {
                var split = line.Replace(':', '=').Split('=');
                if (split.Length != 2) continue;
                var from = split[0].Trim();
                var to = split[1].Trim();
                if (!from.StartsWith("../"))
                {
                    from = pluginName + "/" + from;
                } else from = from.Substring(3);
                AliasMap.Add(from, "*" + to);
            }
        }
    }
}