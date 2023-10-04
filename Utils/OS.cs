namespace WECCL.Utils;

public static class OS
{
    public static OperatingSystemFamily Os => SystemInfo.operatingSystemFamily;

    public static string GetOSString(string def = "other", bool lowercase = false)
    {
        return Os switch
        {
            OperatingSystemFamily.MacOSX => lowercase ? "macos" : "MacOS",
            OperatingSystemFamily.Windows => lowercase ? "windows" : "Windows",
            OperatingSystemFamily.Linux => lowercase ? "linux" : "Linux",
            _ => lowercase ? def.ToLower() : def
        };
    }
}