using UnityEngine;
using System.IO;
using SA.iOS.XCode;

namespace SA.iOS.Editor
{
    internal static class ISN_SettingsManager
    {
        public static void Export(string filepath)
        {
            if (filepath.Length != 0)
            {
                ISN_ExportedSettings exportedSettings = new ISN_ExportedSettings();
                string dataJson = JsonUtility.ToJson(exportedSettings);
                if (dataJson != null)
                    File.WriteAllBytes(filepath, dataJson.ToBytes());
            }
        }

        public static void Import(string filepath)
        {
            if (filepath.Length != 0)
            {
                string fileContent = File.ReadAllText(filepath);
                if (fileContent != null)
                {
                    ISN_ExportedSettings importedSettings = JsonUtility.FromJson<ISN_ExportedSettings>(fileContent);
                    JsonUtility.FromJsonOverwrite(importedSettings.ISNSettings, ISN_Settings.Instance);
                    JsonUtility.FromJsonOverwrite(importedSettings.ISDSettings, ISD_Settings.Instance);
                }
            }
        }

        public static ISN_ExportedSettings GetExportedSettings()
        {
            return new ISN_ExportedSettings();
        }

    }
}

