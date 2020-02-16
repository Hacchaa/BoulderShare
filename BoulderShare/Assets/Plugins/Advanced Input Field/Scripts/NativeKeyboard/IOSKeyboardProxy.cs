﻿#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
using System;
using AOT;
using UnityEngine;

namespace AdvancedInputFieldPlugin
{
    public class IOSKeyboardProxy
    {
        [DllImport("__Internal")]
        private static extern void _nativeKeyboard_initialize(NKBOnTextEditUpdate onTextEditUpdate, NKBOnKeyboardShow onKeyboardShow, NKBOnKeyboardHide onKeyboardHide, NKBOnKeyboardDone onKeyboardDone, NKBOnKeyboardNext onKeyboardNext, NKBOnKeyboardCancel onKeyboardCancel, NKBOnKeyboardHeightChanged onKeyboardHeightChanged, NKBOnHardwareKeyboardChanged onHardwareKeyboardChanged);

        public delegate void NKBOnTextEditUpdate(string text, int selectionStartPosition, int selectionEndPosition);
        public delegate void NKBOnKeyboardShow();
        public delegate void NKBOnKeyboardHide();
        public delegate void NKBOnKeyboardDone();
        public delegate void NKBOnKeyboardNext();
        public delegate void NKBOnKeyboardCancel();
        public delegate void NKBOnKeyboardHeightChanged(int height);
        public delegate void NKBOnHardwareKeyboardChanged(bool connected);

        private static IOSKeyboard keyboard;

        public IOSKeyboardProxy(IOSKeyboard keyboard)
        {
            IOSKeyboardProxy.keyboard = keyboard;
            _nativeKeyboard_initialize(OnTextEditUpdate, OnKeyboardShow, OnKeyboardHide, OnKeyboardDone, OnKeyboardNext, OnKeyboardCancel, OnKeyboardHeightChanged, OnHardwareKeyboardChanged);
        }

        [MonoPInvokeCallback(typeof(NKBOnTextEditUpdate))]
        public static void OnTextEditUpdate(string text, int selectionStartPosition, int selectionEndPosition)
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnTextEditUpdate(text, selectionStartPosition, selectionEndPosition); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardShow))]
        public static void OnKeyboardShow()
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardShow(); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardHide))]
        public static void OnKeyboardHide()
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardHide(); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardDone))]
        public static void OnKeyboardDone()
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardDone(); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardNext))]
        public static void OnKeyboardNext()
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardNext(); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardCancel))]
        public static void OnKeyboardCancel()
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardCancel(); });
        }

        [MonoPInvokeCallback(typeof(NKBOnKeyboardHeightChanged))]
        public static void OnKeyboardHeightChanged(int height)
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardHeightChanged(height); });
        }

        [MonoPInvokeCallback(typeof(NKBOnHardwareKeyboardChanged))]
        public static void OnHardwareKeyboardChanged(bool connected)
        {
            ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnHardwareKeyboardChanged(connected); });
        }
    }
}
#endif