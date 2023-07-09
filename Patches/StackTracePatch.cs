namespace WECCL.Patches;

[HarmonyPatch]
internal class StackTracePatch
{
    private static List<string> previouslyReportedExceptions = new();
    
    [HarmonyPatch(typeof(StackTraceUtility), nameof(ExtractStringFromExceptionInternal))]
    [HarmonyPostfix]
    public static void ExtractStringFromExceptionInternal(ref string message, ref string stackTrace)
    {
        Plugin.Log.LogError("Extracted Message:\n" + message);
        Plugin.Log.LogError("Extracted StackTrace:\n" + stackTrace);
        
        if (Plugin.DataSharingLevel.Value != "None" && !previouslyReportedExceptions.Contains(message))
        {
            previouslyReportedExceptions.Add(message + ":" + stackTrace);
            Plugin.Log.LogDebug("Sending data to server...");
            SendExceptionToServer(message, stackTrace, Plugin.DataSharingLevel.Value);
        }
    }
}