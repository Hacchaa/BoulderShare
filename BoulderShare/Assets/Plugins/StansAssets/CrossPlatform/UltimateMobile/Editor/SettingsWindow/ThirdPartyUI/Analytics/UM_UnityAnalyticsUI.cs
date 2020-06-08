using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.CrossPlatform.Editor
{
    static class UM_UnityAnalyticsUI
    {
        static readonly GUIContent s_LimitUserTrackingLabel = new GUIContent("Limit User Tracking");
        static readonly GUIContent s_DeviceStatsEnabledLabel = new GUIContent("Device Stats Enabled");

        public static void OnGUI()
        {
            var unityClient = UM_Settings.Instance.Analytics.UnityClient;

            EditorGUILayout.HelpBox("Controls whether to limit user tracking at runtime.", MessageType.Info);
            unityClient.LimitUserTracking = SA_EditorGUILayout.ToggleFiled(s_LimitUserTrackingLabel, unityClient.LimitUserTracking, SA_StyledToggle.ToggleType.YesNo);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Controls whether the sending of device stats at runtime is enabled.", MessageType.Info);
            unityClient.DeviceStatsEnabled = SA_EditorGUILayout.ToggleFiled(s_DeviceStatsEnabledLabel, unityClient.DeviceStatsEnabled, SA_StyledToggle.ToggleType.YesNo);
        }
    }
}
