using UnityEngine;
using SA.iOS.XCode;
using System;

namespace SA.iOS.Editor
{
    [Serializable]
    public class ISN_ExportedSettings
    {
        public string ISNSettings
        {
            get
            {
                return m_ISNSettings;
            }
        }

        public string ISDSettings
        {
            get
            {
                return m_ISDSettings;
            }
        }

        [SerializeField]
        private string m_ISNSettings;
        [SerializeField]
        private string m_ISDSettings;

        public ISN_ExportedSettings()
        {
            m_ISNSettings = JsonUtility.ToJson(ISN_Settings.Instance);
            m_ISDSettings = JsonUtility.ToJson(ISD_Settings.Instance);
        }
    }
}
