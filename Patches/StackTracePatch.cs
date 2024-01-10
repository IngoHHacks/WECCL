namespace WECCL.Patches;

[HarmonyPatch]
internal class StackTracePatch
{
    private static List<string> previouslyReportedExceptions = new();

    public static bool HasException = false;
    public static string ExceptionMessage = "";
    public static string ExceptionStackTrace = "";
    public static string ExceptionScreen = "";

    [HarmonyPatch(typeof(StackTraceUtility), nameof(ExtractStringFromExceptionInternal))]
    [HarmonyPostfix]
    public static void ExtractStringFromExceptionInternal(string message, string stackTrace)
    {
        HasException = true;
        ExceptionMessage = message;
        ExceptionStackTrace = stackTrace;
        ExceptionScreen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (Plugin.DataSharingLevel.Value != "None" && !previouslyReportedExceptions.Contains(message))
        {
            previouslyReportedExceptions.Add(message + ":" + stackTrace);
            Plugin.Log.LogDebug("Sending data to server...");
            SendExceptionToServer(message, stackTrace, Plugin.DataSharingLevel.Value);
        }
    }
}