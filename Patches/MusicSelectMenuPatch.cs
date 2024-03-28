using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.UIElements;
using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MusicSelectMenuPatch
{
    /*
     * Patch:
     * - Allows right clicking on the character search screen.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.PIELJFKJFKF))]
    [HarmonyPostfix]
    public static void Menus_PIELJFKJFKF()
    {
        RightClickPatch.RightClickFoc = 0;
        MappedController controller = MappedControls.pad[MappedControls.host];
        for (MappedMenus.cyc = 1; MappedMenus.cyc <= MappedMenus.no_menus; MappedMenus.cyc++)
        {
            if (Input.GetMouseButton((int)MouseButton.RightMouse))
            {
                MappedMenu menu = MappedMenus.menu[MappedMenus.cyc];
                var clickX = Input.mousePosition.x;
                var clickY = Input.mousePosition.y;
                if (menu.Inside(clickX, clickY, 10f) <= 0 || MappedKeyboard.preventInput != 0)
                {
                    continue;
                }
                RightClickPatch.RightClickFoc = menu.id;
                MappedMenus.foc = RightClickPatch.RightClickFoc;
            }
            else if (MappedMenus.Control() > 0 && controller.type > 1 && MappedMenus.cyc == MappedMenus.foc)
            {
                if (controller.button[1] > 0)
                {
                    RightClickPatch.RightClickFoc = MappedMenus.foc;
                }
            }
        }
    }
    
    /*
     * Patch:
     * - Disables vanilla code for tab 1 if page is not 0.
     * - Disables music resetting on page -1.
     * (Lists use page -1)
     */
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Editor_Update(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction prev = null;
        CodeInstruction prev2 = null;
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            if (prev2 != null) {
                if (prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.CHLJMEPFJOK)))
                {
                    if (prev.opcode == OpCodes.Ldc_I4_1)
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(instruction);
                    }
                } else if (prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(CHLPMKEGJBJ), nameof(CHLPMKEGJBJ.CNNKEACKKCD)))
                {
                    if (prev.opcode == OpCodes.Ldsfld && (FieldInfo)prev.operand ==
                        AccessTools.Field(typeof(CHLPMKEGJBJ), nameof(CHLPMKEGJBJ.GEDDILDLILI)))
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_M1);
                        yield return new CodeInstruction(instruction);
                    }
                }
            } 
            prev2 = prev;
            prev = instruction;
        }
    }
    
    /*
     * Patch:
     * - Disables vanilla code for tab 1 if page is not 0.
     * (Lists use page -1)
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.ICGNAJFLAHL))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Menus_ICGNAJFLAHL(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction prev = null;
        CodeInstruction prev2 = null;
        int screen = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            if (prev != null && prev.opcode == OpCodes.Ldsfld && (FieldInfo)prev.operand ==
                AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.FAKHAFKOBPB)))
            {
                if (instruction.opcode == OpCodes.Ldc_I4_S)
                {
                    screen = (sbyte)instruction.operand;
                }
            }
            else if (screen == 60)
            {
                if (prev2 != null && prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.CHLJMEPFJOK)))
                {
                    if (prev.opcode == OpCodes.Ldc_I4_1)
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(instruction);
                    }
                }
            }
            prev2 = prev;
            prev = instruction;
        }
    }
    
    /*
     * Patch:
     * - Creates list ids for music selection list.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.BCGNJIGEDBM))]
    [HarmonyPrefix]
    public static void Menus_BCGNJIGEDBM()
    {
        MappedMenus.listReturnPage = MappedMenus.page;
        //MappedMenus.listReturnFoc = MappedMenus.foc;
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1 && MappedMenus.foc == 8)
        {
            MappedMenus.listSource = Enumerable.Range(0, MappedSound.no_themes).ToArray();
        }
    }
    
    /*
     * Patch:
     * - Sets list item name for music selection list.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.LFEHAGOEBNK))]
    [HarmonyPrefix]
    public static bool Menus_LFEHAGOEBNK(ref string __result, int KJELLNJFNGO)
    {
        var cyc = KJELLNJFNGO;
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1 && MappedMenus.listReturnFoc == 8)
        {
            if (cyc > VanillaCounts.Data.MusicCount)
            {
                int index = cyc - VanillaCounts.Data.MusicCount - 1;
                string name = CustomClips[index].Name;
                __result = name;
                return false;
            }
            if (cyc == 0)
            {
                __result = "None";
                return false;
            }
            if (CustomClips.Count > 0)
            {
                __result = "Vanilla " + cyc;
                return false;
            }
        }
        return true;
    }
    
    /*
     * Patch:
     * - Sets initial selection for music selection list.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.CFOIMDIGPDC))]
    [HarmonyPrefix]
    public static bool Menus_CFOIMDIGPDC(ref int __result, int GOOKPABIPBC)
    {
        var charID = GOOKPABIPBC;
        if (charID <= 0)
        {
            return true;
        }
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1 && MappedMenus.listReturnFoc == 8)
        {
            MappedCharacter character = MappedCharacters.c[charID];
            for (int i = 0; i <= MappedMenus.listSize; i++)
            {
                var id = MappedMenus.listID[i];
                if (id == character.music)
                {
                    __result = i;
                    return false;
                }
            }
        }
        return true;
    }
    
    /*
     * Patch:
     * - Applies selection to character from music selection list.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.EEIPIFGEDNP))]
    [HarmonyPrefix]
    public static bool Menus_EEIPIFGEDNP(int GOOKPABIPBC)
    {
        var charID = GOOKPABIPBC;
        if (charID <= 0)
        {
            return true;
        }
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1 && MappedMenus.listReturnFoc == 8)
        {
            MappedCharacter character = MappedCharacters.c[charID];
            character.music = MappedMenus.listID[MappedMenus.listFoc];
        }
        return true;
    }
    
    /*
     * Patch:
     * - Plays selected music in music selection list.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.OGOMMBJBBDB))]
    [HarmonyPostfix]
    public static void Menus_OGOMMBJBBDB_Post()
    {
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1 && MappedMenus.listReturnFoc == 8)
        {
            var music = MappedMenus.listID[MappedMenus.listFoc];
            if (MappedSound.musicPlaying != music)
            {
                if (MappedSound.musicPlaying == MappedSound.musicMain)
                {
                    Debug.Log("Interrupting main theme at " + MappedSound.musicChannel.time);
                    MappedSound.musicRestore = MappedSound.musicChannel.time;
                }
                MappedSound.PlayMusic(music, Characters.c[Characters.edit].musicSpeed, (MappedSound.musicVol + MappedSound.soundVol) / 2f);
            }
        }
    }
}