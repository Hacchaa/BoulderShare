using UnityEngine;
using System.IO;
using SA.Android;
using SA.iOS;
using SA.iOS.XCode;
using SA.Foundation.Utility;

namespace SA.CrossPlatform.Editor
{
    internal static class UM_SettingsManager
    {
        public static void Export(string filepath)
        {
            if (filepath.Length != 0)
            {
                UM_ExportedSettings exportedSettings = new UM_ExportedSettings();
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
                    UM_ExportedSettings importedSettings = JsonUtility.FromJson<UM_ExportedSettings>(fileContent);
                    JsonUtility.FromJsonOverwrite(importedSettings.Settings, UM_Settings.Instance);
                    JsonUtility.FromJsonOverwrite(importedSettings.AndroidSettings.AndroidSettings, AN_Settings.Instance);
                    SA_FilesUtil.Write(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH, importedSettings.AndroidSettings.XmlSettings.GamesIds);
                    JsonUtility.FromJsonOverwrite(importedSettings.ISNSettings.ISNSettings, ISN_Settings.Instance);
                    JsonUtility.FromJsonOverwrite(importedSettings.ISNSettings.ISDSettings, ISD_Settings.Instance);
                }
            }
        }
    }
}
