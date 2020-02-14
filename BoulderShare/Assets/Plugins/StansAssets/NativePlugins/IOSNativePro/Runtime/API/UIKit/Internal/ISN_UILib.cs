using System;
using System.Collections.Generic;

namespace SA.iOS.UIKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_UILib 
    {
        
        private static ISN_iUIAPI m_api = null;
        public static ISN_iUIAPI API 
        {
            get { return m_api ?? (m_api = ISN_UINativeAPI.Instance); }
        }
        
        [Serializable]
        public class SA_PluginSettingsWindowStylesRequest {
            public List<string> ProductIdentifiers = new List<string>();
        }
    }
}
