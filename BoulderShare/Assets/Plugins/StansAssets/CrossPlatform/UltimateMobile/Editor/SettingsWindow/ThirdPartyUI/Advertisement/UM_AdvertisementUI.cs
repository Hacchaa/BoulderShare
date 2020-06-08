using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.CrossPlatform.Advertisement;
using UnityEditor.PackageManager;

namespace SA.CrossPlatform.Editor
{
    public class UM_AdvertisementUI : UM_PluginSettingsUI
    {
        static SA_iGUILayoutElement s_AdMobSettingsLayout;
        static SA_iGUILayoutElement s_UnityAdsSettingsLayout;

        public static void RegisterAdMobUILayout(SA_iGUILayoutElement layoutElement)
        {
            s_AdMobSettingsLayout = layoutElement;
        }

        public static void RegisterUnityAdsUILayout(SA_iGUILayoutElement layoutElement)
        {
            s_UnityAdsSettingsLayout = layoutElement;
        }

        class UM_AdsResolver : SA_iAPIResolver
        {
            public bool IsSettingsEnabled
            {
                get => UM_DefinesResolver.IsAdMobInstalled || UM_DefinesResolver.IsUnityAdsInstalled;
                set { }
            }

            public void ResetRequirementsCache() { }
        }

        class UM_GoogleAdsResolver : SA_iAPIResolver
        {
            public bool IsSettingsEnabled
            {
                get => UM_DefinesResolver.IsAdMobInstalled;
                set { }
            }

            public void ResetRequirementsCache() { }
        }

        public class UM_UnityAdsResolver : SA_iAPIResolver
        {
            public bool IsSettingsEnabled
            {
                get => UM_DefinesResolver.IsUnityAdsInstalled;
                set { }
            }

            public void ResetRequirementsCache() { }
        }

        public class UM_ChartBoostResolver : SA_iAPIResolver
        {
            public bool IsSettingsEnabled
            {
                get => false;
                set { }
            }

            public void ResetRequirementsCache() { }
        }

        const string k_AdMobSdkDownloadUrl = "https://github.com/googleads/googleads-mobile-unity/releases/download/v5.0.1/GoogleMobileAds-v5.0.1.unitypackage";

        UM_AdsResolver m_ServiceResolver;

        UM_AdvertisementPlatformUI m_AdMobBlock;
        UM_AdvertisementPlatformUI m_UnityAdBlock;
        UM_AdvertisementPlatformUI m_ChartboostBlock;

        public override void OnAwake()
        {
            base.OnAwake();
            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Getting-Started-(Advertisement)");
            AddFeatureUrl("Initialization", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Enabling-the-Ads-Service");
            AddFeatureUrl("Banner Ads", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Banner-Ads");
            AddFeatureUrl("Non-rewarded Ads", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Non-rewarded-Ads");
            AddFeatureUrl("Rewarded Ads", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Rewarded-Ads");

            AddFeatureUrl("Unity Ads", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Unity-Ads");
            AddFeatureUrl("Google AdMob", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Google-AdMob");
            AddFeatureUrl("Google EU Consent", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Google-AdMob#consent-from-european-users");
            AddFeatureUrl("Chartboost", "https://github.com/StansAssets/com.stansassets.ultimate-mobile/wiki/Chartboost");
        }

        public override void OnLayoutEnable()
        {
            base.OnLayoutEnable();
            m_UnityAdBlock = new UM_AdvertisementPlatformUI("Unity Ads", "unity_icon.png", new UM_UnityAdsResolver(), DrawUnityAdsUI);

            m_AdMobBlock = new UM_AdvertisementPlatformUI("Google AdMob", "google_icon.png", new UM_GoogleAdsResolver(), DrawAdMobUI);

            m_ChartboostBlock = new UM_AdvertisementPlatformUI("Chartboost", "chartboost_icon.png", new UM_ChartBoostResolver(), () =>
            {
                EditorGUILayout.HelpBox("COMING SOON!", MessageType.Info);
            });
        }

        public override string Title => "Advertisement";
        public override string Description => "Integrate banner, rewarded and non-rewarded adsfor you game, using the supported ads platfroms.";
        protected override Texture2D Icon => UM_Skin.GetServiceIcon("um_advertisement_icon.png");
        public override SA_iAPIResolver Resolver => m_ServiceResolver ?? (m_ServiceResolver = new UM_AdsResolver());

        protected override void OnServiceUI()
        {
            m_UnityAdBlock.OnGUI();
            m_AdMobBlock.OnGUI();
            m_ChartboostBlock.OnGUI();
        }

        void DrawAdMobUI()
        {
            if (UM_DefinesResolver.IsAdMobInstalled)
            {
                EditorGUILayout.HelpBox("Google Mobile Ads SDK Installed!", MessageType.Info);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    ShowImportGoogleMobileAdsSdkButton("Re-Import SDK");
                }

                GUILayout.Space(10);
                DrawAdMobSettings();
            }
            else
            {
                EditorGUILayout.HelpBox("Google Mobile Ads SDK Missing!", MessageType.Warning);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    ShowImportGoogleMobileAdsSdkButton("Import SDK");
                    var refreshClick = GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(120));
                    if (refreshClick) UM_DefinesResolver.ProcessAssets();
                }
            }
        }

        void ShowImportGoogleMobileAdsSdkButton(string buttonName)
        {
            var click = GUILayout.Button(buttonName, EditorStyles.miniButton, GUILayout.Width(120));
            if (click) SA_PackageManager.DownloadAndImport("Google Mobile Ads SDK", k_AdMobSdkDownloadUrl, false);
        }

        static void DrawAdMobSettings()
        {
            if (s_AdMobSettingsLayout == null)
                UM_SettingsUtil.DrawAddonRequestUI(UM_Addon.AdMob);
            else
                s_AdMobSettingsLayout.OnGUI();
        }

        static void DrawUnityAdsSettings()
        {
            if (s_UnityAdsSettingsLayout == null)
                UM_SettingsUtil.DrawAddonRequestUI(UM_Addon.UnityAds);
            else
                s_UnityAdsSettingsLayout.OnGUI();
        }

        static void DrawUnityAdsUI()
        {
            if (UM_DefinesResolver.IsUnityAdsInstalled)
            {
                DrawUnityAdsSettings();
            }
            else
            {
                EditorGUILayout.HelpBox("Unity SDK Package Missing!", MessageType.Warning);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    var click = GUILayout.Button("Install Unity Ads", EditorStyles.miniButton, GUILayout.Width(120));
                    if (click)
                    {
                        Client.Add(UM_DefinesResolver.UnityAdsPackageName);
                    }
                }
            }
        }


        public static void DrawPlatformIds(UM_PlatformAdIds platform)
        {
            platform.AppId = EditorGUILayout.TextField("App Id: ", platform.AppId);
            platform.BannerId = EditorGUILayout.TextField("Banner Id: ", platform.BannerId);
            platform.RewardedId = EditorGUILayout.TextField("Rewarded Id: ", platform.RewardedId);
            platform.NonRewardedId = EditorGUILayout.TextField("Non-Rewarded Id: ", platform.NonRewardedId);
        }
    }
}
