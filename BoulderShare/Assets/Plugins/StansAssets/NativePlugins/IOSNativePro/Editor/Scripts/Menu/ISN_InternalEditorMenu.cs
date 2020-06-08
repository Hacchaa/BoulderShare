#if SA_DEVELOPMENT_PROJECT
using SA.Foundation.Config;
using UnityEditor;
using UnityEngine;

namespace SA.iOS
{
    public class ISN_InternalEditorMenu
    {
        [MenuItem(SA_Config.EditorMenuRoot + "iOS/Internal/DisableLibs", false, 301)]
        public static void DisableLibs()
        {
            var xcodeFolder = ISN_Settings.IOSNativeXcodeSource.TrimEnd('/');
            var assets = AssetDatabase.FindAssets(string.Empty, new[] { xcodeFolder });
            foreach (var asset in assets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(asset);
                if (assetPath.Contains(".txt"))
                {
                    Debug.Log("Skip txt: " + assetPath);
                    continue;
                }

                if (!assetPath.Contains(".m") && !assetPath.Contains(".h"))
                {
                    Debug.Log("Skip non-native: " + assetPath);
                    continue;
                }

                var newAssetPath = assetPath + ".txt";
                AssetDatabase.MoveAsset(assetPath, newAssetPath);
                AssetDatabase.ImportAsset(newAssetPath);

                Debug.Log(assetPath);
            }
        }
    }
}
#endif
