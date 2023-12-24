namespace WECCL.Content;

internal class Aliases
{
    public static Dictionary<string, string> AliasMap = new();

    public static void Load()
    {
        DirectoryInfo aliasLoc = Locations.Root;
        foreach (FileInfo file in aliasLoc.GetFiles("*.aliases", SearchOption.AllDirectories))
        {
            string[] lines = File.ReadAllLines(file.FullName);
            string pluginName = FindPluginName(file.Directory);
            foreach (string line in lines)
            {
                string[] split = line.Replace(':', '=').Split('=');
                if (split.Length != 2)
                {
                    continue;
                }

                string from = split[0].Trim();
                string to = split[1].Trim();
                if (!from.StartsWith("../"))
                {
                    from = pluginName + "/" + from;
                }
                else
                {
                    from = from.Substring(3);
                }

                AliasMap.Add(from, "*" + to);
            }
        }
    }
}