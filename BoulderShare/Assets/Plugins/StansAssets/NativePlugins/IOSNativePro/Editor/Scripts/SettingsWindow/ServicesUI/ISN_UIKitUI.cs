using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.iOS.UIKit;

namespace SA.iOS
{
    class ISN_UIKitUI : ISN_ServiceSettingsUI
    {
        readonly GUIContent PhotoLibraryUsageDescription = new GUIContent("Photo Library Usage Description[?]:", "Describes the reason that the app accesses the device’s photo library. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent CameraUsageDescription = new GUIContent("Camera Usage Description[?]:", "Describes the reason that the app accesses the device’s camera. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent PhotoLibraryAddUsageDescription = new GUIContent("Photo Library Add Usage Description[?]:", "Describes the reason for the app to add content to the device’s photo library. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent MicrophoneUsageDescription = new GUIContent("Microphone Usage Description[?]:", "Describes the reason that the app accesses the device’s microphone. When the system prompts the user to allow access, this string is displayed as part of the alert.");

        SA_PluginActiveTextLink m_privacyLink;

        public override void OnAwake()
        {
            base.OnAwake();

            m_privacyLink = new SA_PluginActiveTextLink("[?] Read More");

            AddFeatureUrl("Suspend Application", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/UIApplication");

            AddFeatureUrl("Native Pop-ups", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Popups-&-Preloaders");
            AddFeatureUrl("Native Preloader", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Popups-&-Preloaders#preloader");
            AddFeatureUrl("Device Info", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Device-Info");
            AddFeatureUrl("Application Badges", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/UIApplication#application-bages-number");
            AddFeatureUrl("Identifier For Vendor", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Device-Info#identifier-for-vendor");
            AddFeatureUrl("User Interface Idiom", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Device-Info#user-interface-idiom");

            AddFeatureUrl("Time Picker", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Date-Time-Picker#time-picker");
            AddFeatureUrl("Date Picker", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Date-Time-Picker#date-picker");
            AddFeatureUrl("Date & Time Picker", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Date-Time-Picker");
            AddFeatureUrl("Wheel Picker Dialog", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Wheel-Picker-Dialog");
            AddFeatureUrl("Countdown Timer", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Date-Time-Picker#countdown-timer");

            AddFeatureUrl("Calendar Date Picker", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Calendar");
            AddFeatureUrl("Save Image to Camera Roll", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Save-Image-to-Camera-Roll");
            AddFeatureUrl("Save Video to Camera Roll", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Save-Video-to-Camera-Roll");

            AddFeatureUrl("URL Schemes", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Using-URL-Schemes");
            AddFeatureUrl("Add URL Scheme", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Using-URL-Schemes#registering-url-schemes");
            AddFeatureUrl("Check if App installed", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Using-URL-Schemes#how-to-check-programmatically-if-app-is-installed");

            AddFeatureUrl("Open URL", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Open-URL");
            AddFeatureUrl("App Settings Page", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Open-URL#open-app-settings-page");
            AddFeatureUrl("iOS System URLs ", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Open-URL#ios-system-urls");

            AddFeatureUrl("Img Picker Controller", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/UIImage-Picker-Controller");
            AddFeatureUrl("Pick an Image", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Pick-or-Capture-an-Image#pick-an-image-from-photo-library");
            AddFeatureUrl("Capture an Image", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Pick-or-Capture-an-Image#capture-an-image-from-camera");
            AddFeatureUrl("Pick a Video", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Pick-or-Capture-a-Video#pick-a-video-from-photo-library");
            AddFeatureUrl("Capture a Video", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Pick-or-Capture-a-Video#capture-a-video-from-camera");

            AddFeatureUrl("Guided Access", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Guided-Access");
            AddFeatureUrl("Dark Mode", "https://github.com/StansAssets/com.stansassets.ios-native/wiki/Dark-Mode");
        }

        public override string Title => "UIKit";

        public override string Description => "Construct and manage a graphical, event-driven user interface for your app.";

        protected override Texture2D Icon => SA_EditorAssets.GetTextureAtPath(ISN_Skin.IconsPath + "UIKit_icon.png");

        public override SA_iAPIResolver Resolver => ISN_Preprocessor.GetResolver<ISN_UIKitResolver>();

        protected override bool CanBeDisabled => false;

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Protecting the User's Privacy")))
            {
                EditorGUILayout.HelpBox("Once you link with iOS 10 you must declare access to any user private data types. " +
                    "Since by default Unity includes libraries that may access API user private data, " +
                    "the app info.plist mus contains the key's spsifayed bellow. " +
                    "How ever user will only see this message if you call API that requires private permission. " +
                    "If you not using such API, you can leave it as is.", MessageType.Info);

                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    var click = m_privacyLink.DrawWithCalcSize();
                    if (click)
                        Application.OpenURL("https://developer.apple.com/documentation/uikit/core_app/protecting_the_user_s_privacy?language=objc");
                }

                const int textAreaHeight = 35;
                ISN_Settings.Instance.CameraUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(CameraUsageDescription, ISN_Settings.Instance.CameraUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                if (ISN_Settings.Instance.CameraUsageDescriptionEnabled)
                    using (new SA_GuiIndentLevel(1))
                    {
                        ISN_Settings.Instance.CameraUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.CameraUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(textAreaHeight));
                    }

                EditorGUILayout.Space();
                ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(PhotoLibraryUsageDescription, ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                if (ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled)
                    using (new SA_GuiIndentLevel(1))
                    {
                        ISN_Settings.Instance.PhotoLibraryUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.PhotoLibraryUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(textAreaHeight));
                    }

                EditorGUILayout.Space();
                ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(PhotoLibraryAddUsageDescription, ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                if (ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled)
                    using (new SA_GuiIndentLevel(1))
                    {
                        ISN_Settings.Instance.PhotoLibraryAddUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.PhotoLibraryAddUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(textAreaHeight));
                    }

                EditorGUILayout.Space();
                ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(MicrophoneUsageDescription, ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                if (ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled)
                    using (new SA_GuiIndentLevel(1))
                    {
                        ISN_Settings.Instance.MicrophoneUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.MicrophoneUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(textAreaHeight));
                    }
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Allowed schemes to query")))
            {
                SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.ApplicationQueriesSchemes,
                    (scheme) =>
                    {
                        return scheme.Identifier;
                    },
                    (scheme) =>
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Identifier");
                        scheme.Identifier = EditorGUILayout.TextField(scheme.Identifier);
                        EditorGUILayout.EndHorizontal();
                    },
                    () =>
                    {
                        var newUlr = new ISN_UIUrlType("url_scheme");
                        ISN_Settings.Instance.ApplicationQueriesSchemes.Add(newUlr);
                    });
            }
        }
    }
}
