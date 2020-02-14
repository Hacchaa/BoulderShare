using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.CrossPlatform.Advertisement
{
    public static class UM_AdvertisementUnityAdsUI
    {
        private static void FillExampleSettings() 
        {
            var settings = UM_UnityAdsSettings.Instance;
            var android = settings.AndroidIds;

            android.AppId = "2941900";
            android.BannerId = "banner";
            android.RewardedId = "video";
            android.NonRewardedId = "rewardedVideo";

            var ios = settings.IOSIds;
            ios.AppId = "2941899";
            ios.BannerId = "banner";
            ios.RewardedId = "video";
            ios.NonRewardedId = "rewardedVideo";
            
            UM_UnityAdsSettings.Save();
        }

        public static void OnGUI() 
        {
            EditorGUILayout.HelpBox("Unity Magnetization SDK Installed.", MessageType.Info);
            using (new SA_GuiBeginHorizontal()) 
            {
                GUILayout.FlexibleSpace();
                var example = GUILayout.Button("Example", EditorStyles.miniButton, GUILayout.Width(80));
                if (example) 
                    FillExampleSettings();
                
                var click = GUILayout.Button("Dashboard", EditorStyles.miniButton, GUILayout.Width(80));
                if (click) 
                    Application.OpenURL("https://operate.dashboard.unity3d.com/");
            }
            
            EditorGUI.BeginChangeCheck();

            var settings = UM_UnityAdsSettings.Instance;
            using (new SA_H2WindowBlockWithSpace(new GUIContent("IOS"))) 
                UM_AdvertisementUI.DrawPlatformIds(settings.IOSIds);

            using (new SA_H2WindowBlockWithSpace(new GUIContent("ANDROID"))) 
                UM_AdvertisementUI.DrawPlatformIds(settings.AndroidIds);

            using (new SA_H2WindowBlockWithSpace(new GUIContent("SETTINGS"))) 
            {
                settings.TestMode = SA_EditorGUILayout.ToggleFiled("Test Mode",
                  settings.TestMode,
                  SA_StyledToggle.ToggleType.YesNo);
            }
            
            if(EditorGUI.EndChangeCheck()) 
                UM_UnityAdsSettings.Save();
        }
    }
}