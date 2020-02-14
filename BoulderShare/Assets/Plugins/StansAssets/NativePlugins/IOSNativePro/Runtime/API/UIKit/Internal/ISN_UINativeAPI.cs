////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;
using SA.Foundation.Events;

#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.UIKit.Internal
{
    internal class ISN_UINativeAPI : ISN_Singleton<ISN_UINativeAPI>, ISN_iUIAPI 
    {

#if UNITY_IPHONE
        [DllImport("__Internal")] static extern void _ISN_UI_SaveToCameraRoll(int length, IntPtr byteArrPtr);
        [DllImport("__Internal")] static extern string _ISN_UI_GetAvailableMediaTypesForSourceType(int type);
        [DllImport("__Internal")] static extern bool _ISN_UI_IsSourceTypeAvailable(int type);
        [DllImport("__Internal")] static extern void _ISN_UI_PresentPickerController(string data);

        [DllImport("__Internal")] static extern void _ISN_UI_PresentUIAlertController(string data);
        [DllImport("__Internal")] static extern void _ISN_UI_DismissUIAlertController(int alertId);

        [DllImport("__Internal")] static extern void _ISN_UI_PreloaderLockScreen();
        [DllImport("__Internal")] static extern void _ISN_UI_PreloaderUnlockScreen();
        [DllImport("__Internal")] static extern void _ISN_UIWheelPicker(IntPtr callback, string data);
        
        // UIScreen
        [DllImport("__Internal")] static extern ulong _ISN_UI_UIScreen_MainScreen();
        [DllImport("__Internal")] static extern ulong _ISN_UI_UIScreen_TraitCollection(ulong uiScreenHash);
        
        //TraitCollection
        [DllImport("__Internal")] static extern int _ISN_UI_TraitCollection_UserInterfaceStyle(ulong traitCollectionHash);
        
#endif

        // UIScreen
        public ulong UIScreen_MainScreen()
        {
#if UNITY_IPHONE
            return _ISN_UI_UIScreen_MainScreen();
#else
            return ISN_NativeObject.k_NullObjectHash;
#endif
        }
        
        public ulong UIScreen_TraitCollection(ulong uiScreenHash)
        {
#if UNITY_IPHONE
            return _ISN_UI_UIScreen_TraitCollection(uiScreenHash);
#else
            return ISN_NativeObject.k_NullObjectHash;
#endif
        }
        
        //TraitCollection

        public ISN_UIUserInterfaceStyle TraitCollection_UserInterfaceStyle(ulong traitCollectionHash)
        {
#if UNITY_IPHONE
            return (ISN_UIUserInterfaceStyle) _ISN_UI_TraitCollection_UserInterfaceStyle(traitCollectionHash);
#else
            return ISN_UIUserInterfaceStyle.Unspecified;
#endif
        }
        
        private SA_Event<ISN_UIAlertActionId> m_onUIAlertActionPerformed = new SA_Event<ISN_UIAlertActionId>();
        
   
        Action<SA_Result> m_onImageSave;
        public void SaveTextureToCameraRoll(Texture2D texture, Action<SA_Result> callback) 
        {
            m_onImageSave = callback;

            #if UNITY_IPHONE
            var data = texture.EncodeToPNG();
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            _ISN_UI_SaveToCameraRoll(data.Length, handle.AddrOfPinnedObject());
            handle.Free();
            #endif
        }

        private void OnImageSave(string data) 
        {
            var result = JsonUtility.FromJson<SA_Result>(data);
            m_onImageSave.Invoke(result);
        }

        public List<string> GetAvailableMediaTypes(ISN_UIImagePickerControllerSourceType type) 
        {
            #if UNITY_IPHONE
            var data = _ISN_UI_GetAvailableMediaTypesForSourceType((int)type);
            var result = JsonUtility.FromJson<ISN_UIAvailableMediaTypes>(data);
            return result.Types;
            #else
            return new List<string>();
            #endif
        }

        public bool IsSourceTypeAvailable(ISN_UIImagePickerControllerSourceType type) 
        {
            #if UNITY_IPHONE
            return _ISN_UI_IsSourceTypeAvailable((int)type);
            #else
            return true;
            #endif
        }

        Action<ISN_UIPickerControllerResult> m_didFinishPickingMedia;
        public void PresentPickerController(ISN_UIPickerControllerRequest request, Action<ISN_UIPickerControllerResult> callback) 
        {
            m_didFinishPickingMedia = callback;
            #if UNITY_IPHONE
            _ISN_UI_PresentPickerController(JsonUtility.ToJson(request));
            #endif
        }

        void didFinishPickingMedia(string data) 
        {
            var result = JsonUtility.FromJson<ISN_UIPickerControllerResult>(data);
            m_didFinishPickingMedia.Invoke(result);
        }
        
        public void PresentUIAlertController(ISN_UIAlertController alert) 
        {
            #if UNITY_IPHONE
            var data = JsonUtility.ToJson(alert);
            _ISN_UI_PresentUIAlertController(data);
            #endif
        }

        public void DismissUIAlertController(ISN_UIAlertController alert) 
        {
            #if UNITY_IPHONE
            _ISN_UI_DismissUIAlertController(alert.Id);
            #endif
        }

        public SA_iEvent<ISN_UIAlertActionId> OnUIAlertActionPerformed 
        { 
            get { return m_onUIAlertActionPerformed; }
        }

        void OnUIAlertAction(string data) 
        {
            var result = JsonUtility.FromJson<ISN_UIAlertActionId>(data);
            m_onUIAlertActionPerformed.Invoke(result);
        }

        public void PreloaderLockScreen() 
        {
            #if UNITY_IPHONE
            _ISN_UI_PreloaderLockScreen();
            #endif
        }

        public void PreloaderUnlockScreen() 
        {
            #if UNITY_IPHONE
            _ISN_UI_PreloaderUnlockScreen();
            #endif
        }

        /// <summary>
        /// Create Wheel UIPickerView.
        /// It will create UIPickerView with cancel and done buttons.
        /// </summary>
        public void ShowUIWheelPicker(ISN_UIWheelPickerController controller, Action<ISN_UIWheelPickerResult> callback)
        {
            #if UNITY_IPHONE
             var data = JsonUtility.ToJson(controller);
            _ISN_UIWheelPicker(ISN_MonoPCallback.ActionToIntPtr (callback), data);
            #endif
        }
    }
}
