#if !UNITY_EDITOR && UNITY_WSA

#if ENABLE_WINMD_SUPPORT
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace AdvancedInputFieldPlugin
{
	/// <summary>Class that acts as a bridge for the Native UWP Keyboard</summary>
	public class UWPKeyboard: NativeKeyboard
	{
		private NativeKeyboardUWP.NativeKeyboard keyboard;
		private UWPKeyboardProxy proxy;

#if ENABLE_WINMD_SUPPORT
	[DllImport("__Internal")]
	extern static int GetPageContent([MarshalAs(UnmanagedType.IInspectable)]object frame, [MarshalAs(UnmanagedType.IInspectable)]out object pageContent);
#endif

		internal override void Setup()
		{
			if(ThreadHelper.Instance == null) { ThreadHelper.CreateInstance(); }
			proxy = new UWPKeyboardProxy(this);
			keyboard = new NativeKeyboardUWP.NativeKeyboard();

			UnityEngine.WSA.Application.InvokeOnUIThread(() =>
			{
#if ENABLE_WINMD_SUPPORT
				Panel panel = TryGetPanel();
				if(panel == null)
				{
					panel = TryGetIl2CppPanel();
				}

				keyboard.Initialize(proxy, panel);
#endif
			}, false);
		}

#if ENABLE_WINMD_SUPPORT
		internal Panel TryGetPanel()
		{
			UnityEngine.Debug.Log("TryGetPanel");
			Panel panel = null;

			Window window = Window.Current;
			Frame frame = window.Content as Frame;
			if(frame == null) { return null; }

			Page page = frame.Content as Page;
			if(page == null) { return null; }

			panel = page.Content as Panel;
			return panel;
		}

		internal Panel TryGetIl2CppPanel()
		{
			UnityEngine.Debug.Log("TryGetIl2CppPanel");
			Panel panel = null;

			object pageContent;
			var result = GetPageContent(Window.Current.Content, out pageContent);
			UnityEngine.Debug.Log("Result: " + result);
			if(result < 0)
			{
				Marshal.ThrowExceptionForHR(result);
			}

			UnityEngine.Debug.Log("PageContent: " + pageContent);
			var dxSwapChainPanel = pageContent as Windows.UI.Xaml.Controls.SwapChainPanel;
			UnityEngine.Debug.Log("SwapChainPanel: " + dxSwapChainPanel);
			panel = pageContent as Panel;
			UnityEngine.Debug.Log("Panel: " + panel);
			return panel;
		}
#endif

		public override void EnableUpdates()
		{
			keyboard.EnableUpdates();
		}

		public override void DisableUpdates()
		{
			keyboard.DisableUpdates();
		}

		public override void EnableHardwareKeyboardUpdates()
		{
			keyboard.EnableHardwareKeyboardUpdates();
		}

		public override void DisableHardwareKeyboardUpdates()
		{
			keyboard.DisableHardwareKeyboardUpdates();
		}

		public override void UpdateTextEdit(string text, int selectionStartPosition, int selectionEndPosition)
		{
			keyboard.UpdateTextEdit(text, selectionStartPosition, selectionEndPosition);
		}

		public override void ShowKeyboard(string text, int selectionStartPosition, int selectionEndPosition, string configurationJSON)
		{
			keyboard.ShowKeyboard(text, selectionStartPosition, selectionEndPosition, configurationJSON);
		}

		public override void HideKeyboard()
		{
			keyboard.HideKeyboard();
		}
	}
}
#endif
