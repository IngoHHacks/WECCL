# 📦 Asset Bundles

<show-structure for="chapter" depth="2"/>

<link-summary>
How to create asset bundles.
</link-summary>

<warning>
<p>
Make sure you have read the <a href="Unity.md">Unity</a> topic before reading this topic.
</p>
</warning>

## Creating Asset Bundles

Asset bundles are used by Unity to load assets at runtime.  
To manually create an asset bundle, you can place the following code in a script and run it in the editor:
```C#
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}
```
Place this script inside the `Assets/Editor` folder.  
Then, you have to assign the asset bundle name to each asset you want to include in the bundle by selecting the asset and changing the asset bundle name in the inspector (under the Asset Labels section).
Finally, you can build the asset bundle(s) by selecting `Assets/Build AssetBundles` in the menu bar.  
The asset bundles will be placed in the `Assets/AssetBundles` folder.

<seealso style="cards">
<category name="Unity" ref="unity">
<a href="https://docs.unity3d.com/2020.3/Documentation/Manual/AssetBundlesIntro.html">Unity Manual: Asset Bundles</a>
</category>
</seealso>