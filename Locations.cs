using WECCL.Content;

namespace WECCL;

public static class Locations
{
    public static DirectoryInfo Root { get; } = new(Paths.PluginPath); // Not Plugin.PluginPath!
    public static DirectoryInfo Export { get; } = new(Path.Combine(Plugin.PluginPath, "Export"));
    public static DirectoryInfo DeletedCharacters { get; } = new(Path.Combine(Plugin.PluginPath, "Purgatory"));
    public static DirectoryInfo Cache { get; } = new(Path.Combine(Plugin.PersistentDataPath, ".cache"));
    public static DirectoryInfo Debug { get; } = new(Path.Combine(Plugin.PluginPath, "Debug"));
    public static DirectoryInfo Data { get; } = new(Path.Combine(Plugin.PluginPath, "Data"));
    public static DirectoryInfo Meta { get; } = new(Path.Combine(Plugin.PersistentDataPath, "Meta.meta"));
    
    public static FileInfo SaveFileVanilla { get; } = new(Path.Combine(Application.persistentDataPath, "Save.bytes"));
    
    public static FileInfo SaveFile { get; } = new(Path.Combine(Application.persistentDataPath, Plugin.SaveFileName.Value + ".bytes"));

    public static DirectoryInfo ContentMappings { get; } =
        new(Path.Combine(Plugin.PersistentDataPath, "ContentMappings.mappings"));
    public static DirectoryInfo CharacterMappings { get; } =
        new(Path.Combine(Plugin.PersistentDataPath, "CharacterMappings.mappings"));

    internal static void CreateDirectories()
    {
        if (Plugin.CacheEnabled.Value && !Cache.Exists)
        {
            Cache.Create();
        }
        else if (Plugin.CacheEnabled.Value && Cache.Attributes.HasFlag(FileAttributes.Hidden))
        {
            Cache.Attributes &= ~FileAttributes.Hidden;
        }
        else if (!Plugin.CacheEnabled.Value && Cache.Exists)
        {
            Cache.Delete(true);
        }

        Debug.Create();
    }

    internal static void MoveLegacyLocations()
    {
        LegacyLocations.MoveLegacyLocations();
    }

    public static void LoadData()
    {
        var animationController = AssetBundle.LoadFromFile(Path.Combine(Data.FullName, "animationcontroller")).LoadAllAssets<RuntimeAnimatorController>().FirstOrDefault();
        if (animationController == null) throw new Exception("Failed to load data. Please make sure you copied the 'Data' folder alongside the plugin if installed manually.");
        AO.AnimationController = animationController;
        HubLocationPrefab = AssetBundle.LoadFromFile(Path.Combine(Data.FullName, "hub")).LoadAllAssets<GameObject>().First();
    }
}

internal static class LegacyLocations
{
    public static DirectoryInfo OldCache { get; } = new(Path.Combine(Plugin.PluginPath, ".cache"));
    public static DirectoryInfo OldMetadata { get; } = new(Path.Combine(Plugin.PluginPath, "Meta.meta"));
    public static DirectoryInfo OldMappings { get; } = new(Path.Combine(Plugin.PluginPath, "ContentMappings.mappings"));

    public static DirectoryInfo OldMetadata2 { get; } =
        new(Path.Combine(Plugin.PluginPath, "CustomConfigsSaveFile.json"));

    public static DirectoryInfo OldMappings2 { get; } =
        new(Path.Combine(Plugin.PluginPath, "CustomContentSaveFile.json"));

    public static DirectoryInfo OldMetadata3 { get; } = new(Path.Combine(Plugin.PluginPath, "Meta.json"));
    public static DirectoryInfo OldMappings3 { get; } = new(Path.Combine(Plugin.PluginPath, "ContentMappings.json"));

    public static void MoveLegacyLocations()
    {
        if (OldCache.Exists)
        {
            if (!Locations.Cache.Exists)
            {
                OldCache.MoveTo(Locations.Cache.FullName);
            }
            else
            {
                OldCache.Delete(true);
            }
        }

        if (OldMetadata.Exists)
        {
            if (!Locations.Meta.Exists)
            {
                OldMetadata.MoveTo(Locations.Meta.FullName);
            }
            else
            {
                OldMetadata.Delete(true);
            }
        }

        if (OldMappings.Exists)
        {
            if (!Locations.ContentMappings.Exists)
            {
                OldMappings.MoveTo(Locations.ContentMappings.FullName);
            }
            else
            {
                OldMappings.Delete(true);
            }
        }

        if (OldMetadata2.Exists)
        {
            if (!Locations.Meta.Exists)
            {
                OldMetadata2.MoveTo(Locations.Meta.FullName);
            }
            else
            {
                OldMetadata2.Delete(true);
            }
        }

        if (OldMappings2.Exists)
        {
            if (!Locations.ContentMappings.Exists)
            {
                OldMappings2.MoveTo(Locations.ContentMappings.FullName);
            }
            else
            {
                OldMappings2.Delete(true);
            }
        }

        if (OldMetadata3.Exists)
        {
            if (!Locations.Meta.Exists)
            {
                OldMetadata3.MoveTo(Locations.Meta.FullName);
            }
            else
            {
                OldMetadata3.Delete(true);
            }
        }

        if (OldMappings3.Exists)
        {
            if (!Locations.ContentMappings.Exists)
            {
                OldMappings3.MoveTo(Locations.ContentMappings.FullName);
            }
            else
            {
                OldMappings3.Delete(true);
            }
        }
    }
}