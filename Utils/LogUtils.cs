using System.Diagnostics;

namespace WECCL.Utils;

public static class LogUtils
{
    public static void LogInfo(object message)
    {
        var source = GetSource();
        if (source != null)
        {
            Plugin.Log.LogInfo(string.Format("[{1}] {0}", message, source));
        }
        else
        {
            Plugin.Log.LogInfo(message);
        }
    }
    
    public static void LogWarning(object message)
    {
        var source = GetSource();
        if (source != null)
        {
            Plugin.Log.LogWarning(string.Format("[{1}] {0}", message, source));
        }
        else
        {
            Plugin.Log.LogWarning(message);
        }
    }
    
    public static void LogError(object message)
    {
        var source = GetSource();
        if (source != null)
        {
            Plugin.Log.LogError(string.Format("[{1}] {0}", message, source));
        }
        else
        {
            Plugin.Log.LogError(message);
        }
    }
    
    public static void LogDebug(object message)
    {
        var source = GetSource();
        if (source != null)
        {
            Plugin.Log.LogDebug(string.Format("[{1}] {0}", message, source));
        }
        else
        {
            Plugin.Log.LogDebug(message);
        }
    }
    
    public static void LogFatal(object message)
    {
        var source = GetSource();
        if (source != null)
        {
            Plugin.Log.LogFatal(string.Format("[{1}] {0}", message, source));
        }
        else
        {
            Plugin.Log.LogFatal(message);
        }
    }
    
    private static string GetSource()
    {
        var stackTrace = new StackTrace();
        var stackFrames = stackTrace.GetFrames();
        if (stackFrames == null)
        {
            return null;
        }
        foreach (var stackFrame in stackFrames)
        {
            var method = stackFrame.GetMethod();
            if (method == null)
            {
                continue;
            }
            var declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                continue;
            }
            var name = declaringType.Name;
            if (name == null)
            {
                continue;
            }
            if (name == "LogUtils")
            {
                continue;
            }
            while (name.StartsWith("<") && declaringType.DeclaringType != null)
            {
                declaringType = declaringType.DeclaringType;
                name = declaringType.Name;
            }
            if (name.StartsWith("<"))
            {
                continue;
            }
            return name;
        }
        return null;
    }
    
    public static T Linfo<T>(T message)
    {
        LogInfo(message.ToString());
        return message;
    }
    
    public static T Lwarn<T>(T message)
    {
        LogWarning(message.ToString());
        return message;
    }
    
    public static T Lerr<T>(T message)
    {
        LogError(message.ToString());
        return message;
    }
    
    public static T Ldbg<T>(T message)
    {
        LogDebug(message.ToString());
        return message;
    }
    
    public static T Lfat<T>(T message)
    {
        LogFatal(message.ToString());
        return message;
    }
}