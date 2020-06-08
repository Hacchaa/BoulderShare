using UnityEditor;
using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

namespace SA.Facebook
{
    [InitializeOnLoad]
    public class SA_FB_InstallationProcessing : SA_PluginInstallationProcessor<SA_FB_Settings>
    {
        const string k_FacebookLibName = "Facebook.Unity.dll";
        const string k_FbInstalledDefine = "SA_FB_INSTALLED";

        static SA_FB_InstallationProcessing()
        {
            var installation = new SA_FB_InstallationProcessing();
            installation.Init();
        }

        //--------------------------------------
        //  SA_PluginInstallationProcessor
        //--------------------------------------

        protected override void OnInstall()
        {
            // Let's check if we have FB SKD in the project.
            ProcessAssets();
        }

        //--------------------------------------
        //  Public Methods
        //--------------------------------------

        public static void ProcessAssets()
        {
            var projectLibs = SA_AssetDatabase.FindAssetsWithExtentions("Assets", ".dll");
            foreach (var lib in projectLibs) ProcessAssetImport(lib);
        }

        public static void ProcessAssetImport(string assetPath)
        {
            var isFbLibDetected = IsPathEqualsFacebookSdkName(assetPath);
            if (isFbLibDetected) UpdateLibState(true);
        }

        public static void ProcessAssetDelete(string assetPath)
        {
            var isFbLibDetected = IsPathEqualsFacebookSdkName(assetPath);
            if (isFbLibDetected) UpdateLibState(false);
        }

        //--------------------------------------
        //  Private Methods
        //--------------------------------------

        static bool IsPathEqualsFacebookSdkName(string assetPath)
        {
            var fileName = SA_PathUtil.GetFileName(assetPath);
            return fileName.Equals(k_FacebookLibName);
        }

        static void UpdateLibState(bool fbLibFound)
        {
            if (fbLibFound)
            {
                if (!SA_EditorDefines.HasCompileDefine(k_FbInstalledDefine)) SA_EditorDefines.AddCompileDefine(k_FbInstalledDefine);
            }
            else
            {
                if (SA_EditorDefines.HasCompileDefine(k_FbInstalledDefine)) SA_EditorDefines.RemoveCompileDefine(k_FbInstalledDefine);
            }
        }
    }
}
