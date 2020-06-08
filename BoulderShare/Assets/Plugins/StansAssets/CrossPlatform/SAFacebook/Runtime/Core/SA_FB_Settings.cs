using System.Collections.Generic;
using SA.Foundation.Patterns;
using SA.Foundation.Config;

namespace SA.Facebook
{
    public class SA_FB_Settings : SA_ScriptableSingleton<SA_FB_Settings>
    {
        public const string PluginTitle = "Ultimate Facebook";
        public const string PluginFolder = SA_Config.StansAssetsCrossPlatformPluginsPath + "SAFacebook/";
        public const string DocumentationUrl = "https://unionassets.com/ultimate-mobile-pro/manual#facebook";

        /// <summary>
        /// When a person logs into your app via Facebook Login you can access a subset of that person's data stored on Facebook.
        /// Permissions or "Scopes" are how you ask someone if you can access that data.
        /// A person's privacy settings combined with what you ask for will determine what you can access.
        /// </summary>
        public List<string> Scopes = new List<string>();

        /// <summary>
        /// Set's the Facebook App Id, the same operation can be made using the editor settings
        /// </summary>
        /// <param name="appId"></param>
        public void SetAppId(string appId)
        {
            SA_FB_Proxy.SetAppId(appId);
        }

        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------

        protected override string BasePath => PluginFolder;
        public override string PluginName => PluginTitle;
        public override string DocumentationURL => DocumentationUrl;
        public override string SettingsUIMenuItem => SA_Config.EditorMenuRoot + "Cross-Platform/3rd-Party";
    }
}
