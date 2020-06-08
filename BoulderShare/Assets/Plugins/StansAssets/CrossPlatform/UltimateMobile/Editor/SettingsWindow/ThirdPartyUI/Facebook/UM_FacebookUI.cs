using UnityEngine;
using SA.Facebook;
using SA.Foundation.Editor;

namespace SA.CrossPlatform.Editor
{
    class UM_Facebook : UM_PluginSettingsUI
    {
        UM_FacebookResolver m_resolver;

        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Getting-Started-(Facebook)");
            AddFeatureUrl("Sing In", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Sing-In");
            AddFeatureUrl("User Info", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/User-Info");
            AddFeatureUrl("Analytics", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Analytics");
        }

        public override string Title => "Facebook";

        public override string Description => "Build cross-platform games with Facebook rapidly and easily.";

        protected override Texture2D Icon => UM_Skin.GetServiceIcon("um_facebook_icon.png");

        public override SA_iAPIResolver Resolver
        {
            get
            {
                if (m_resolver == null) m_resolver = new UM_FacebookResolver();

                return m_resolver;
            }
        }

        protected override void OnServiceUI()
        {
            SA_FB_EditorWindow.DrawSettingsUI();
        }
    }
}
