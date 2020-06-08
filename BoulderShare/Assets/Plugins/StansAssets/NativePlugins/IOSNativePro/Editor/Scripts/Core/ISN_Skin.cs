using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.iOS
{
    static class ISN_Skin
    {
        public const string IconsPath = ISN_Settings.IOSNativeFolder + "Editor/Art/Icons/";
        public const string SocialIconsPath = ISN_Settings.IOSNativeFolder + "Editor/Art/Social/";

        public static Texture2D SettingsWindowIcon =>
            EditorGUIUtility.isProSkin
                ? SA_EditorAssets.GetTextureAtPath(IconsPath + "ios_pro.png")
                : SA_EditorAssets.GetTextureAtPath(IconsPath + "ios.png");

        public static Texture2D GetIcon(string iconName)
        {
            return SA_EditorAssets.GetTextureAtPath(IconsPath + iconName);
        }
    }
}
