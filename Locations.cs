namespace WECCL;

public static class Locations
{
    public static DirectoryInfo Assets { get; } = new(Path.Combine(Plugin.PluginPath, "Assets"));
    public static DirectoryInfo Overrides { get; } = new(Path.Combine(Plugin.PluginPath, "Overrides"));
    public static DirectoryInfo Import { get; } = new(Path.Combine(Plugin.PluginPath, "Import"));
    public static DirectoryInfo Export { get; } = new(Path.Combine(Plugin.PluginPath, "Export"));
    public static DirectoryInfo Libraries { get; } = new(Path.Combine(Plugin.PluginPath, "Libraries"));
    public static DirectoryInfo Cache { get; } = new(Path.Combine(Plugin.PersistentDataPath, ".cache"));
    public static DirectoryInfo Debug { get; } = new(Path.Combine(Plugin.PluginPath, "Debug"));

    public static DirectoryInfo Meta { get; } = new(Path.Combine(Plugin.PersistentDataPath, "Meta.meta"));

    public static DirectoryInfo ContentMappings { get; } =
        new(Path.Combine(Plugin.PersistentDataPath, "ContentMappings.mappings"));

    internal static void CreateDirectories()
    {
        Assets.Create();
        Overrides.Create();
        Import.Create();
        Export.Create();
        if (Plugin.CacheEnabled.Value && !Cache.Exists)
        {
            Cache.Create();
            Cache.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Cache.Refresh();
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