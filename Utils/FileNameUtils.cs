namespace WECCL.Utils;

public static class FileNameUtils
{
    public static string Escape(string fileName)
    {
        var replace = Path.GetInvalidFileNameChars();
        foreach (var c in replace)
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }

    public static string FindPluginName(DirectoryInfo source)
    {
        while (source.Name != "plugins" && source.Parent != null)
        {
            source = source.Parent;
        }
        if (source.Parent?.Name == "BepInEx")
        {
            return "Assets";
        }
        return source.Parent?.Name ?? "Assets";
    }
}