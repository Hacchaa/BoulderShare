using System.Collections.Generic;
using SA.Foundation.Editor;
using UnityEngine;

namespace SA.iOS
{
    public class ISN_AuthenticationServicesUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() 
        {
            base.OnAwake();
            AddFeatureUrl("Sign in with Apple", "https://unionassets.com/ios-native-pro/apple-sing-in-852");
        }

        public override string Title 
        {
            get { return "Authentication Services"; }
        }

        public override string Description 
        {
            get { return "Make it easy for users to log into apps and services."; }
        }

        protected override Texture2D Icon 
        {
            get { return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "CoreLocation_icon.png"); }
        }

        public override SA_iAPIResolver Resolver 
        {
            get { return ISN_Preprocessor.GetResolver<ISN_AuthenticationServicesResolver>(); }
        }
        
        protected override IEnumerable<string> SupportedPlatforms 
        {
            get { return new List<string>() { "iOS", "tvOS" }; }
        }

        protected override void OnServiceUI() { }
    }
}