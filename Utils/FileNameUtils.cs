namespace WECCL.Utils;

public class FileNameUtils
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
}