using UnityEngine.UI;
using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    /*
     * Patch:
     * - Tick loop for the editor.
     * - Renames theme names to custom theme names or prefixes them with 'Vanilla' if they are vanilla.
     */
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyPostfix]
    public static void Scene_Editor_Update()
    {
        if (MappedMenus.tab == 1 && MappedMenus.page == 0)
        {
            UnmappedPlayer gMIKIMHFABP = NJBJIIIACEP.OAAMGFLINOB[1];
            Character iPNKFGHIDJP = gMIKIMHFABP.EMDMDLNJFKP;
            if (iPNKFGHIDJP.music > VanillaCounts.Data.MusicCount)
            {
                int index = iPNKFGHIDJP.music - VanillaCounts.Data.MusicCount - 1;
                string name = CustomClips[index].Name;
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = name;
            }
            else if (iPNKFGHIDJP.music == 0)
            {
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = "None";
            }
            else if (CustomClips.Count > 0)
            {
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = "Vanilla " + iPNKFGHIDJP.music;
            }
            UnmappedMenus.BBICLKGGIGB();
            
        }
    }
    
    /*
     * Patch:
     * - Shows WECCL version next to the game version in the main menu.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.MOKABGFFGLC))]
    [HarmonyPostfix]
    public static void Menus_MOKABGFFGLC()
    {
         var text = GameObject.Find("Version").GetComponent<Text>(); 
         text.text = "WECCL " + Plugin.PluginVerLong + "\t\t Game " + text.text;
         text.horizontalOverflow = HorizontalWrapMode.Overflow;
    }
}