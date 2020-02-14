using System;
using UnityEngine;
using SA.Android.Editor;
using SA.iOS.Editor;

namespace SA.CrossPlatform.Editor
{
    [Serializable]
    internal class UM_ExportedSettings
    {
        public string Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public AN_ExportedSettings AndroidSettings
        {
            get
            {
                return m_AndroidSettings;
            }
        }

        public ISN_ExportedSettings ISNSettings
        {
            get
            {
                return m_ISNSettings;
            }
        }

        [SerializeField]
        private string m_Settings;

        [SerializeField]
        private AN_ExportedSettings m_AndroidSettings;

        [SerializeField]
        private ISN_ExportedSettings m_ISNSettings;

        public UM_ExportedSettings()
        {
            m_Settings = JsonUtility.ToJson(UM_Settings.Instance);
            m_AndroidSettings = AN_SettingsManager.GetExportedSettings();
            m_ISNSettings = ISN_SettingsManager.GetExportedSettings();
        }
    }
}
