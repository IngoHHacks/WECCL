namespace WECCL.Utils;

public class AllMods
{
    private static AllMods _instance;
    
    public static AllMods Instance => _instance ??= new AllMods();
    
    public List<PluginInfo> Mods { get; private set; } = new();
    
    public int NumMods => Mods.Count;
    
    public void LoadMods()
    {
        foreach (var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            Mods.Add(plugin);
        }
    }
    
    public void ReloadMods()
    {
        Mods.Clear();
        LoadMods();
    }
    
    public AllMods()
    {
        LoadMods();
    }
}