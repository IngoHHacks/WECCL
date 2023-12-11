namespace WECCL.Utils;

public static class LogUtils
{
    public static void LogInfo(String message)
    {
        Plugin.Log.LogInfo(message);
    }
    
    public static void LogWarning(String message)
    {
        Plugin.Log.LogWarning(message);
    }
    
    public static void LogError(String message)
    {
        Plugin.Log.LogError(message);
    }
    
    public static void LogDebug(String message)
    {
        Plugin.Log.LogDebug(message);
    }
    
    public static void LogFatal(String message)
    {
        Plugin.Log.LogFatal(message);
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