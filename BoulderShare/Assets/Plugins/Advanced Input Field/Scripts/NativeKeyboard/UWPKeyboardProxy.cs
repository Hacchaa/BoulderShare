#if !UNITY_EDITOR && UNITY_WSA
using UnityEngine;

namespace AdvancedInputFieldPlugin
{
	public class UWPKeyboardProxy: NativeKeyboardUWP.INativeKeyboardCallback
	{
		private UWPKeyboard keyboard;

		public UWPKeyboardProxy(UWPKeyboard keyboard)
		{
			this.keyboard = keyboard;
		}

		public void OnTextEditUpdate(string text, int selectionStartPosition, int selectionEndPosition)
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnTextEditUpdate(text, selectionStartPosition, selectionEndPosition); });
		}

		public void OnKeyboardShow()
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardShow(); });
		}

		public void OnKeyboardHide()
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardHide(); });
		}

		public void OnKeyboardDone()
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardDone(); });
		}

		public void OnKeyboardNext()
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardNext(); });
		}

		public void OnKeyboardCancel()
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardCancel(); });
		}

		public void OnKeyboardHeightChanged(int height)
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnKeyboardHeightChanged(height); });
		}

		public void OnHardwareKeyboardChanged(bool connected)
		{
			ThreadHelper.ScheduleActionOnUnityThread(() => { keyboard.OnHardwareKeyboardChanged(connected); });
		}

		public void DebugLine(string message)
		{
			UnityEngine.Debug.Log(message);
		}
	}
}
#endif