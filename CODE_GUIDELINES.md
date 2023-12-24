# Code Guidelines
General guidelines for contributing to WECCL.  
The guidelines are not very strict, but please read them if you plan on contributing.

## General
Please follow basic C# coding conventions. Most importantly:
* Use PascalCase for class names, methods, and properties.
* Use camelCase or _camelCase for variables, fields, and parameters. (Using _camelCase for private fields is preferred, but not required.)
* Wrap long lines of code if necessary.
* Use spaces instead of tabs.

## Organization
* All Harmony patches should be placed in the `Patches` folder.
* API classes should be placed in the `API` folder and be public.
* Utility classes should be placed in the `Utils` folder and be public or internal depending on whether they are intended to be used by other mods.
* All other classes can be placed anywhere and must either be internal or private.

## Patches
* Please follow the following format for patch methods:
```csharp
[HarmonyPatch]
internal static class PatchClass
{
    [HarmonyPatch(typeof(OriginalClass), nameof(OriginalClass.OriginalMethod))]
    [HarmonyPrefix/Postfix/Transpiler, etc.]
    public static [Type] ClassName_OriginalMethod()
    {
        // Your code here
    }
}
```
* Note: OriginalClass must be the readable name of the class, not the obfuscated name.
* For example, `UnmappedPlayer` instead of `DFOGOCNBECG`. Classes that are not obfuscated (e.g. `Character`) can be used as-is.
* If multiple patches are required for the same method, simply add `_Pre`, `_Post`, `_Trans`, etc. to the end of the method name.
* If multiple patches of the same type are required for the same method, add a number to the end of the method name, e.g. `ClassName_OriginalMethod1`, `ClassName_OriginalMethod2`, or `ClassName_OriginalMethod_Pre1`, `ClassName_OriginalMethod_Pre2`, etc.
* Try to keep related patches in the same class, but if a class becomes too large, feel free to split it into multiple classes.

## Internal Game Methods
* WECCL uses internal mappings to access game methods and fields. These mappings are kept secret. Ask @IngoH for more information.
* Since you can't build the project without these mappings, please test your code in a separate project before submitting a pull request if you don't have access to the mappings.