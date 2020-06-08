////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using SA.Foundation.Async;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.AVFoundation.Internal
{
    class ISN_AVEditorAPI : ISN_iAVAPI
    {
        ISN_AVAudioSessionCategory m_audioSessionCategory = ISN_AVAudioSessionCategory.SoloAmbient;
        ISN_AVAudioSessionCategoryOptions m_audioSessionCategoryOptions = ISN_AVAudioSessionCategoryOptions.MixWithOthers;
        readonly SA_Event<ISN_AVAudioSessionRouteChangeReason> m_onAudioSessionRouteChange = new SA_Event<ISN_AVAudioSessionRouteChangeReason>();

        //--------------------------------------
        // ISN_AVCaptureDevice
        //--------------------------------------

        public ISN_AVAuthorizationStatus GetAuthorizationStatus(ISN_AVMediaType type)
        {
            return ISN_AVAuthorizationStatus.Authorized;
        }

        public void RequestAccess(ISN_AVMediaType type, Action<ISN_AVAuthorizationStatus> callback)
        {
            SA_Coroutine.WaitForSecondsRandom(1f, 3f, () =>
            {
                callback.Invoke(ISN_AVAuthorizationStatus.Authorized);
            });
        }

        //--------------------------------------
        // ISN_AVAssetImageGenerator
        //--------------------------------------

        public Texture2D CopyCGImageAtTime(string movieUrl, double seconds)
        {
            return new Texture2D(0, 0);
        }

        //--------------------------------------
        // ISN_AVAudioSession
        //--------------------------------------

        public SA_iEvent<ISN_AVAudioSessionRouteChangeReason> OnAudioSessionRouteChange => m_onAudioSessionRouteChange;

        public ISN_AVAudioSessionCategory AudioSessionCategory => m_audioSessionCategory;

        public ISN_AVAudioSessionCategoryOptions AudioSessionCategoryOptions => m_audioSessionCategoryOptions;

        public SA_Result AudioSessionSetCategory(ISN_AVAudioSessionCategory category)
        {
            m_audioSessionCategory = category;
            return new SA_Result();
        }

        public SA_Result AudioSessionSetCategoryWithOptions(ISN_AVAudioSessionCategory category, ISN_AVAudioSessionCategoryOptions options)
        {
            m_audioSessionCategory = category;
            m_audioSessionCategoryOptions = options;
            return new SA_Result();
        }

        public SA_Result AudioSessionSetActive(bool isActive)
        {
            return new SA_Result();
        }

        ISN_AVAuthorizationStatus ISN_iAVAPI.GetAuthorizationStatus(ISN_AVMediaType type)
        {
            return ISN_AVAuthorizationStatus.Authorized;
        }

        void ISN_iAVAPI.RequestAccess(ISN_AVMediaType type, Action<ISN_AVAuthorizationStatus> callback) { }

        SA_Result ISN_iAVAPI.AudioSessionSetCategory(ISN_AVAudioSessionCategory category)
        {
            return new SA_Result();
        }

        SA_Result ISN_iAVAPI.AudioSessionSetActive(bool isActive)
        {
            return new SA_Result();
        }

        int ISN_iAVAPI.AudioSessionGetRecordPermission()
        {
            return 0;
        }

        void ISN_iAVAPI.AudioSessionRequestRecordPermission(Action<bool> callback)
        {
            callback.Invoke(true);
        }
    }
}
