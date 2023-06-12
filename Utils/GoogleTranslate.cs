using System.Net;
using System.Text;

namespace WECCL.Utils;

public static class GoogleTranslate
{
    public static string Translate(string text, string from, string to)
    {
        string url =
            $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={Uri.EscapeUriString(text)}";
        string result = new WebClient().DownloadString(url);
        return ToText(result);
    }

    private static string ToText(string result)
    {
        int start = result.IndexOf("\"", StringComparison.Ordinal) + 1;
        StringBuilder sb = new();
        bool ignore = false;
        for (int i = start; i < result.Length; i++)
        {
            char current = result[i];
            if (current == '\"')
            {
                if (ignore)
                {
                    ignore = false;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (current == '\\')
                {
                    if (ignore)
                    {
                        sb.Append('\\');
                        ignore = false;
                    }
                    else
                    {
                        ignore = true;
                    }
                }
                else
                {
                    sb.Append(current);
                }
            }
        }

        return sb.ToString();
    }
}