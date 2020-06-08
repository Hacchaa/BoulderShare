using UnityEditor;
using SA.Foundation.Editor;
using SA.Foundation.UtilitiesEditor;
using SA.iOS.XCode;
using UnityEngine;

namespace SA.iOS
{
#if UNITY_2018
    [InitializeOnLoad]
    public static class AssetsResolver
    {
        static AssetsResolver()
        {
            var xcodeFolder = ISN_Settings.IOSNativeXcode.TrimEnd('/');
            AssetDatabase.ImportAsset(xcodeFolder);
            EnableLibsAtPath(xcodeFolder);
        }

        static void EnableLibsAtPath(string path)
        {
            var assets = AssetDatabase.FindAssets(string.Empty, new[] { path });

            foreach (var asset in assets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(asset);
                if (!assetPath.Contains(".txt"))
                {
                    var assetAtPath = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    if(assetAtPath is TextAsset)
                        AssetDatabase.ImportAsset(assetPath);

                    continue;
                }

                var newAssetPath = assetPath.Replace(".txt", string.Empty);
                AssetDatabase.MoveAsset(assetPath, newAssetPath);
                AssetDatabase.ImportAsset(newAssetPath);
            }
        }
    }
#endif

    abstract class ISN_APIResolver : SA_iAPIResolver
    {
        ISN_XcodeRequirements m_Requirements;

        //--------------------------------------
        // Virtual
        //--------------------------------------

        public virtual void RunAdditionalPreprocess() { }

        //--------------------------------------
        // Abstract
        //--------------------------------------

        protected abstract string LibFolder { get; }
        public abstract string DefineName { get; }

        //Method is executed when Resolver is created.
        //Resolvers can be refreshed from the editor, and GenerateRequirements will be triggered again
        //Those requirement will apply / remove only on build stage
        protected abstract ISN_XcodeRequirements GenerateRequirements();
        public abstract bool IsSettingsEnabled { get; set; }

        public void ResetRequirementsCache()
        {
            m_Requirements = null;
        }

        public ISN_XcodeRequirements XcodeRequirements
        {
            get
            {
                if (m_Requirements == null)
                    m_Requirements = GenerateRequirements();

                return m_Requirements;
            }
        }

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void Run(bool pluginVersionUpdated = false)
        {
            if (IsSettingsEnabled && !IsAPIEnabled || IsSettingsEnabled && pluginVersionUpdated)
                Enable();

            if (!IsSettingsEnabled && IsAPIEnabled)
                Disable();

            //We want to run Xcode Requirement's every time on build preprocess
            //RemoveXcodeRequirements only excepted once if API appears to be turned off
            if (IsSettingsEnabled)
                AddXcodeRequirements();

            //Defines & custom postprocess should be executed every time
            ChangeDefines(IsSettingsEnabled);
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public bool IsAPIEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(LibFolder))
                    return true;

                return SA_AssetDatabase.IsDirectoryExists(LibFolderPath);
            }
        }

        public string LibFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(LibFolder))
                    return string.Empty;

                return ISN_Settings.IOSNativeXcode + LibFolder;
            }
        }

        //--------------------------------------
        // Private Methods
        //--------------------------------------

        void Enable()
        {
            //There is no additional lib dependencies for this API
            if (string.IsNullOrEmpty(LibFolder))
                return;

            var source = ISN_Settings.IOSNativeXcodeSource + LibFolder;
            var destination = LibFolderPath;

            InstallLibFolder(source, destination);
        }

        void InstallLibFolder(string source, string destination)
        {
            source = source.Trim('/');
            destination = destination.Trim('/');

            if (!AssetDatabase.IsValidFolder(source))
            {
                Debug.LogError("Can't find the source lib folder at path: " + source);
                return;
            }

            AssetDatabase.DeleteAsset(destination);
            AssetDatabase.CopyAsset(source, destination);
            EnableLibsAtPath(destination);
        }

        public static void EnableLibsAtPath(string path)
        {
            var assets = AssetDatabase.FindAssets(string.Empty, new[] { path });
            for (var i = 0; i < assets.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                var newAssetPath = assetPath.Replace(".txt", string.Empty);
                var progress = (i + 1) / (float)assets.Length;
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Installing: " + newAssetPath, progress);
                AssetDatabase.MoveAsset(assetPath, newAssetPath);
            }

            EditorUtility.ClearProgressBar();
        }

        void Disable()
        {
            SA_PluginsEditor.UninstallLibFolder(LibFolderPath);
            RemoveXcodeRequirements();
        }

        protected virtual void RemoveXcodePlistKey(ISD_PlistKey key)
        {
            ISD_API.RemoveInfoPlistKey(key);
        }

        //Method is executed on post process build only
        protected virtual void RemoveXcodeRequirements()
        {
            foreach (var framework in XcodeRequirements.Frameworks)
                ISD_API.RemoveFramework(framework.Type);

            foreach (var lib in XcodeRequirements.Libraries)
                ISD_API.RemoveLibrary(lib.Type);

            foreach (var property in XcodeRequirements.Properties)
                ISD_API.RemoveBuildProperty(property);

            foreach (var key in XcodeRequirements.PlistKeys)
                RemoveXcodePlistKey(key);
        }

        //Method is executed on post process build only
        protected virtual void AddXcodeRequirements()
        {
            foreach (var framework in XcodeRequirements.Frameworks)
                ISD_API.AddFramework(framework);

            foreach (var lib in XcodeRequirements.Libraries)
                ISD_API.AddLibrary(lib);

            foreach (var property in XcodeRequirements.Properties)
                ISD_API.SetBuildProperty(property);

            foreach (var key in XcodeRequirements.PlistKeys)
                AddXcodePlistKey(key);
        }

        protected virtual void AddXcodePlistKey(ISD_PlistKey key)
        {
            ISD_API.SetInfoPlistKey(key);
        }

        void ChangeDefines(bool isEnabled)
        {
            if (isEnabled)
                SA_EditorDefines.AddCompileDefine(DefineName, BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX);
            else
                SA_EditorDefines.RemoveCompileDefine(DefineName, BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX);
        }
    }
}
