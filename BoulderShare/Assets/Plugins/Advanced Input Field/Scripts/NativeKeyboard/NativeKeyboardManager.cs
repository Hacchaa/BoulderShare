//-----------------------------------------
//			Advanced Input Field
// Copyright (c) 2017 Jeroen van Pienbroek
//------------------------------------------

using System.Collections;
using UnityEngine;

namespace AdvancedInputFieldPlugin
{
	/// <summary>Access point for the NativeKeyboard for current platform</summary>
	public class NativeKeyboardManager: MonoBehaviour
	{
		/// <summary>The singleton instance of NativeKeyboardManager</summary>
		private static NativeKeyboardManager instance;

		/// <summary>The NativeKeyboard instance of current platform</summary>
		private NativeKeyboard keyboard;

		/// <summary>The last selected AdvancedInputField instance (if any)</summary>
		public AdvancedInputField lastSelectedInputField;

		/// <summary>The active AdvancedInputField instance (if any) before the app got paused</summary>
		private AdvancedInputField activeInputFieldBeforePause;

		/// <summary>The singleton instance of NativeKeyboardManager</summary>
		public static NativeKeyboardManager Instance
		{
			get
			{
				if(instance == null)
				{
					instance = GameObject.FindObjectOfType<NativeKeyboardManager>();
					if(instance == null)
					{
						GameObject gameObject = new GameObject("NativeKeyboardManager");
						DontDestroyOnLoad(gameObject);
						instance = gameObject.AddComponent<NativeKeyboardManager>();
					}
				}

				return instance;
			}
		}

		/// <summary>The NativeKeyboard instance of current platform</summary>
		public static NativeKeyboard Keyboard
		{
			get { return Instance.keyboard; }
		}

		/// <summary>The active AdvancedInputField instance (if any) before the app got paused</summary>
		public static AdvancedInputField ActiveInputFieldBeforePause
		{
			get { return Instance.activeInputFieldBeforePause; }
			set { Instance.activeInputFieldBeforePause = value; }
		}

		/// <summary>Indicates whether a hardware keyboard is connected</summary>
		public static bool HardwareKeyboardConnected
		{
			get { return Instance.keyboard.HardwareKeyboardConnected; }
		}

		public static bool InstanceValid
		{
			get
			{
				if(instance == null) { return false; }
				else if(instance.gameObject == null && !ReferenceEquals(instance.gameObject, null)) { return false; } //Pending destruction
				return true;
			}
		}

		#region UNITY
		private void Awake()
		{
#if UNITY_EDITOR
#if(UNITY_ANDROID || UNITY_IOS || UNITY_WSA)
			if(Settings.SimulateMobileBehaviourInEditor)
			{
				Canvas mobileKeyboardCanvas = null;
				if(Screen.height > Screen.width)
				{
					mobileKeyboardCanvas = GameObject.Instantiate(Settings.PortraitKeyboardCanvasPrefab);
				}
				else
				{
					mobileKeyboardCanvas = GameObject.Instantiate(Settings.LandscapeKeyboardCanvasPrefab);
				}
				DontDestroyOnLoad(mobileKeyboardCanvas.gameObject);
				keyboard = mobileKeyboardCanvas.GetComponentInChildren<SimulatorKeyboard>();
				keyboard.Init(name);
			}
#endif
#elif UNITY_ANDROID
			keyboard = gameObject.AddComponent<AndroidKeyboard>();
			keyboard.Init(name);
#elif UNITY_IOS
			keyboard = gameObject.AddComponent<IOSKeyboard>();
			keyboard.Init(name);
#elif UNITY_WSA
			keyboard = gameObject.AddComponent<UWPKeyboard>();
			keyboard.Init(name);
#else
			Debug.LogWarning("Native Keyboard is only supported on Android, iOS and UWP");
#endif
		}

		private void OnDestroy()
		{
			instance = null;
		}

		private void OnApplicationPause(bool pause)
		{
			if(!pause)
			{
				if(activeInputFieldBeforePause != null)
				{
					StartCoroutine(DelayedRestore());
				}
			}
		}

		private IEnumerator DelayedRestore()
		{
			yield return new WaitForSeconds(0.1f);
			activeInputFieldBeforePause.ManualSelect();
		}
		#endregion

		public static void TryDestroy()
		{
			if(instance != null && instance.gameObject != null)
			{
				Destroy(instance.gameObject);
			}
		}

		/// <summary>Checks whether the native binding should be active or not</summary>
		public static void UpdateKeyboardActiveState()
		{
			if(Keyboard == null) { return; }
			Keyboard.UpdateActiveState();
		}

		/// <summary>
		/// Enables hardware keyboard connectivity checks in the native binding.
		/// Use this when you want connectivity checks when no inputfield is selected.
		/// </summary>
		public static void EnableHardwareKeyboardUpdates()
		{
			if(Keyboard == null) { return; }
			Keyboard.EnableHardwareKeyboardUpdates();
		}

		/// <summary>
		/// Disables hardware keyboard connectivity checks in the native binding.
		/// Use this when you want to disable connectivity checks after using EnableHardwareKeyboardUpdates.
		/// </summary>
		public static void DisableHardwareKeyboardUpdates()
		{
			if(Keyboard == null) { return; }
			Keyboard.DisableHardwareKeyboardUpdates();
		}

		/// <summary>Updates the native text and selection</summary>
		public static void UpdateTextEdit(string text, int selectionStartPosition, int selectionEndPosition)
		{
			if(Keyboard == null) { return; }
			Keyboard.UpdateTextEdit(text, selectionStartPosition, selectionEndPosition);
		}

		/// <summary>Shows the TouchScreenKeyboard for current platform</summary>
		/// <param name="keyboardType">The keyboard type to use</param>
		/// <param name="characterValidation">The characterValidation to use</param>
		/// <param name="lineType">The lineType to use</param>
		/// <param name="autocorrection">Indicates whether autocorrection is enabled</param>
		/// <param name="characterLimit">The character limit for the text</param>
		/// <param name="secure">Indicates whether input should be secure</param>
		public static void ShowKeyboard(string text, int selectionStartPosition, int selectionEndPosition, NativeKeyboardConfiguration configuration)
		{
			if(Keyboard == null) { return; }
			string configurationJSON = JsonUtility.ToJson(configuration);
			Keyboard.ShowKeyboard(text, selectionStartPosition, selectionEndPosition, configurationJSON);
		}

		/// <summary>Shows the TouchScreenKeyboard for current platform without changing settings</summary>
		public static void RestoreKeyboard()
		{
			if(Keyboard == null) { return; }
			Keyboard.RestoreKeyboard();
		}

		/// <summary>Hides the TouchScreenKeyboard for current platform</summary>
		public static void HideKeyboard()
		{
			if(Keyboard == null) { return; }
			Keyboard.HideKeyboard();
		}

		/// <summary>Resets the autofill service for current platform</summary>
		public static void ResetAutofill()
		{
			if(Keyboard == null) { return; }
			Keyboard.ResetAutofill();
		}

		/// <summary>Adds a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to add</param>
		public static void AddKeyboardHeightChangedListener(OnKeyboardHeightChangedHandler listener)
		{
			if(Keyboard == null) { return; }
			Keyboard.AddKeyboardHeightChangedListener(listener);
		}

		/// <summary>Removes a KeyboardHeightChangdeListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to remove</param>
		public static void RemoveKeyboardHeightChangedListener(OnKeyboardHeightChangedHandler listener)
		{
			if(instance == null || instance.keyboard == null) { return; } //No need to remove event listener if instance is null
			Keyboard.RemoveKeyboardHeightChangedListener(listener);
		}

		/// <summary>Adds a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The HardwareKeyboardChangedListener to add</param>
		public static void AddHardwareKeyboardChangedListener(OnHardwareKeyboardChangedHandler listener)
		{
			if(Keyboard == null) { return; }
			Keyboard.AddHardwareKeyboardChangedListener(listener);
		}

		/// <summary>Removes a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to remove</param>
		public static void RemoveHardwareKeyboardChangedListener(OnHardwareKeyboardChangedHandler listener)
		{
			if(instance == null || instance.keyboard == null) { return; } //No need to remove event listener if instance is null
			Keyboard.RemoveHardwareKeyboardChangedListener(listener);
		}
	}
}
