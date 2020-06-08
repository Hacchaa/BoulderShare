using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Foundation.Utility;

namespace SA.CrossPlatform.Editor
{
    class UM_PlaymakerUI : UM_PluginSettingsUI
    {
        class UM_PlaymakerResolver : SA_iAPIResolver
        {
            public bool IsSettingsEnabled
            {
                get => UM_DefinesResolver.IsPlayMakerInstalled;

                set { }
            }

            public void ResetRequirementsCache() { }
        }

        const string PLAYMAKER_UI_CLASS_NAME = "SA.CrossPlatform.Addons.PlayMaker.UM_PlaymakerActionsUI";
        const string PLAYMAKER_STORE_URL = "https://assetstore.unity.com/packages/tools/visual-scripting/playmaker-368";

        SA_iGUILayoutElement m_playmakerSettingsUI;
        UM_PlaymakerResolver m_playmakerResolver;

        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Getting-Started-(Playmaker)");
            AddFeatureUrl("In App Purchases", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/In-App-Purchases");
            AddFeatureUrl("Game Services", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Game-Services");
            AddFeatureUrl("Social", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Social");
            AddFeatureUrl("Camera & Gallery", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Camera-&-Gallery");
            AddFeatureUrl("Local Notifications", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Local-Notifications");
            AddFeatureUrl("Native UI", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Native-UI");
            AddFeatureUrl("Advertisement", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Advertisement");
            AddFeatureUrl("Analytics", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Analytics-(Playmaker)");
        }

        public override string Title => "Playmaker";

        public override string Description => "Use Ultimate Mobile API with Playmaker visual scripting solution.";

        protected override Texture2D Icon => UM_Skin.GetServiceIcon("um_playmaker.png");

        public override SA_iAPIResolver Resolver
        {
            get
            {
                if (m_playmakerResolver == null) m_playmakerResolver = new UM_PlaymakerResolver();

                return m_playmakerResolver;
            }
        }

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Playmaker")))
            {
                if (UM_DefinesResolver.IsPlayMakerInstalled)
                {
                    EditorGUILayout.HelpBox("PlayMaker Plugin Installed!", MessageType.Info);
                    DrawPlayMakerSettings();
                }
                else
                {
                    EditorGUILayout.HelpBox("PlayMaker Plugin is Missing!", MessageType.Warning);
                    using (new SA_GuiBeginHorizontal())
                    {
                        GUILayout.FlexibleSpace();
                        var click = GUILayout.Button("Get Playmaker", EditorStyles.miniButton, GUILayout.Width(120));
                        if (click) Application.OpenURL(PLAYMAKER_STORE_URL);

                        var refreshClick = GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(120));
                        if (refreshClick) UM_DefinesResolver.ProcessAssets();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Dev mode section!", MessageType.Info);
#if SA_DEVELOPMENT_PROJECT
                    DrawPlayMakerSettings();
#endif
                }
            }
        }

        void DrawPlayMakerSettings()
        {
            if (m_playmakerSettingsUI == null)
            {
                var settingsUI = SA_Reflection.CreateInstance(PLAYMAKER_UI_CLASS_NAME);
                if (settingsUI != null)
                {
                    m_playmakerSettingsUI = settingsUI as SA_iGUILayoutElement;
                    m_playmakerSettingsUI.OnLayoutEnable();
                }
            }

            if (m_playmakerSettingsUI == null)
                UM_SettingsUtil.DrawAddonRequestUI(UM_Addon.Playmaker);
            else
                m_playmakerSettingsUI.OnGUI();
        }
    }
}
