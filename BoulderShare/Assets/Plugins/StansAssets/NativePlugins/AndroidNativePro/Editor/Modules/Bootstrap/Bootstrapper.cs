using System.Collections.Generic;
using StansAssets.Foundation.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SA.Android.Editor
{
    [InitializeOnLoad]
    static class Bootstrapper
    {
        const string k_GoogleScopeRegistryUrl = "https://unityregistry-pa.googleapis.com";
        const string k_Edm4UName = "com.google.external-dependency-manager";
        const string k_Edm4UVersion = "1.2.153";

        static ScopeRegistry GoogleScopeRegistry =>
            new ScopeRegistry("Game Package Registry by Google",
                k_GoogleScopeRegistryUrl,
                new HashSet<string>
                {
                    "com.google"
                });

        static Bootstrapper()
        {
            if (AN_Settings.Instance.EnforceEdm4UDependency)
                InstallEdm4U();
        }

        static void InstallEdm4U()
        {

            if (AssetDatabase.IsValidFolder("Assets/PlayServicesResolver"))
            {
                Debug.Log("go?");
                // Looks like user has it installed, let's remove the package to avoid a conflict
                Client.Remove(k_Edm4UName);
                return;
            }

            var manifest = new StansAssets.Foundation.Editor.Manifest();
            manifest.Fetch();

            var manifestUpdated = false;

            if (!manifest.IsRegistryExists(k_GoogleScopeRegistryUrl))
            {
                manifest.AddScopeRegistry(GoogleScopeRegistry);
                manifestUpdated = true;
            }

            if (!manifest.IsDependencyExists(k_Edm4UName))
            {
                manifest.AddDependency(k_Edm4UName, k_Edm4UVersion);
                manifestUpdated = true;
            }

            if (manifestUpdated)
                manifest.ApplyChanges();
        }
    }
}
