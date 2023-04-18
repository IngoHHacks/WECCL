namespace WECCL.Patches;

[HarmonyPatch]
internal class StackTracePatch
{
    [HarmonyPatch(typeof(StackTraceUtility), nameof(ExtractStringFromExceptionInternal))]
    [HarmonyPostfix]
    public static void ExtractStringFromExceptionInternal(ref string stackTrace)
    {
        Plugin.Log.LogError("Extracted StackTrace:\n" + stackTrace);
    }
}