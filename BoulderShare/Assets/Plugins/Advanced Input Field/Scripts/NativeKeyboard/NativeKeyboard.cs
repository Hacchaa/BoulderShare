//-----------------------------------------
//			Advanced Input Field
// Copyright (c) 2017 Jeroen van Pienbroek
//------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace AdvancedInputFieldPlugin
{
	/// <summary>The delegate for Keyboard Height Changed event</summary>
	public delegate void OnKeyboardHeightChangedHandler(int keyboardHeight);

	/// <summary>The delegate for Hardware Keyboard Changed event</summary>
	public delegate void OnHardwareKeyboardChangedHandler(bool connected);

	public enum KeyboardState
	{
		HIDDEN, PENDING_SHOW, VISIBLE, PENDING_HIDE
	}

	/// <summary>Base class that acts as a bridge for the Native Keyboard for a specific platform</summary>
	public abstract class NativeKeyboard: MonoBehaviour
	{
		/// <summary>Event type of the keyboard callbacks</summary>
		public enum EventType
		{
			TEXT_EDIT_UPDATE,
			SHOW,
			HIDE,
			DONE,
			NEXT,
			CANCEL,
			DELETE
		}

		/// <summary>Event for keyboard callbacks</summary>
		public struct Event
		{
			public EventType type;
			public TextEditFrame textEditFrame;

			public Event(EventType type, TextEditFrame textEditFrame = default(TextEditFrame))
			{
				this.type = type;
				this.textEditFrame = textEditFrame;
			}
		}

		/// <summary>Queue with Keyboard events</summary>
		protected ThreadsafeQueue<Event> nativeEventQueue;

		/// <summary>The name of the GameObject used for callbacks</summary>
		protected string gameObjectName;

		/// <summary>The event for Keyboard Height Changed</summary>
		protected event OnKeyboardHeightChangedHandler onKeyboardHeightChanged;

		/// <summary>The event for Hardware Keyboard Changed</summary>
		protected event OnHardwareKeyboardChangedHandler onHardwareKeyboardChanged;

		/// <summary>Indicates whether the state of the keyboard</summary>
		public KeyboardState State { get; set; }

		/// <summary>Indicates whether the native binding is active or not</summary>
		public bool Active { get; private set; }

		/// <summary>Indicates whether a hardware keyboard is connected</summary>
		public bool HardwareKeyboardConnected { get; protected set; }

		/// <summary>Initializes this class</summary>
		/// <param name="gameObjectName">The name of the GameObject to use for callbacks</param>
		internal void Init(string gameObjectName)
		{
			this.gameObjectName = gameObjectName;
			nativeEventQueue = new ThreadsafeQueue<Event>(30);
			Setup();
		}

		/// <summary>Gets and removes next keyboard event</summary>
		/// <param name="keyboardEvent">The output keyboard event</param>
		internal bool PopEvent(out Event keyboardEvent)
		{
			if(nativeEventQueue.Count == 0)
			{
				keyboardEvent = default(Event);
				return false;
			}

			keyboardEvent = nativeEventQueue.Dequeue();
			return true;
		}

		/// <summary>Clears the keyboard event queue</summary>
		internal void ClearEventQueue()
		{
			nativeEventQueue.Clear();
		}

		/// <summary>Adds a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to add</param>
		public void AddKeyboardHeightChangedListener(OnKeyboardHeightChangedHandler listener)
		{
			onKeyboardHeightChanged += listener;
		}

		/// <summary>Removes a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to remove</param>
		public void RemoveKeyboardHeightChangedListener(OnKeyboardHeightChangedHandler listener)
		{
			onKeyboardHeightChanged -= listener;
		}

		/// <summary>Adds a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The HardwareKeyboardChangedListener to add</param>
		public void AddHardwareKeyboardChangedListener(OnHardwareKeyboardChangedHandler listener)
		{
			onHardwareKeyboardChanged += listener;
		}

		/// <summary>Removes a KeyboardHeightChangedListener</summary>
		/// <param name="listener">The KeyboardHeightChangedListener to remove</param>
		public void RemoveHardwareKeyboardChangedListener(OnHardwareKeyboardChangedHandler listener)
		{
			onHardwareKeyboardChanged -= listener;
		}

		/// <summary>Setups the bridge to the Native Keyboard</summary>
		internal virtual void Setup() { }

		/// <summary>Checks whether the native binding should be active or not</summary>
		internal void UpdateActiveState()
		{
			bool inputFieldSelected = false;
			GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
			if(currentSelection != null)
			{
				inputFieldSelected = currentSelection.GetComponentInParent<AdvancedInputField>() || currentSelection.GetComponentInParent<CanvasFrontRenderer>();
			}

			if(Active != inputFieldSelected)
			{
				Active = inputFieldSelected;
				if(Active)
				{
					if(!HardwareKeyboardConnected)
					{
						EnableUpdates();
					}

					if(Settings.MobileKeyboardBehaviour == MobileKeyboardBehaviour.USE_HARDWARE_KEYBOARD_WHEN_AVAILABLE)
					{
						EnableHardwareKeyboardUpdates();
					}
				}
				else
				{
					if(!HardwareKeyboardConnected)
					{
						DisableUpdates();
					}

					DisableHardwareKeyboardUpdates();
				}
			}
		}

		/// <summary>Enables updates in the native binding</summary>
		public virtual void EnableUpdates() { }

		/// <summary>Disables updates in the native binding</summary>
		public virtual void DisableUpdates() { }

		/// <summary>Enables hardware keyboard updates in the native binding</summary>
		public virtual void EnableHardwareKeyboardUpdates() { }

		/// <summary>Disables hardware keyboard updates in the native binding</summary>
		public virtual void DisableHardwareKeyboardUpdates() { }

		/// <summary>Updates the native text and selection</summary>
		public virtual void UpdateTextEdit(string text, int selectionStartPosition, int selectionEndPosition) { }

		/// <summary>Requests a text edit update (after OS autofills a value)</summary>
		public virtual void RequestTextEditUpdate() { }

		/// <summary>Shows the TouchScreenKeyboard for current platform</summary>
		public virtual void ShowKeyboard(string text, int selectionStartPosition, int selectionEndPosition, string configurationJSON) { }

		/// <summary>Shows the TouchScreenKeyboard for current platform without changing settings</summary>
		public virtual void RestoreKeyboard() { }

		/// <summary>Hides the TouchScreenKeyboard for current platform</summary>
		public virtual void HideKeyboard() { }

		/// <summary>Resets the autofill service for current platform</summary>
		public virtual void ResetAutofill() { }

		/// <summary>Event callback when the selection got changed natively</summary>
		public void OnTextEditUpdate(string text, int selectionStartPosition, int selectionEndPosition)
		{
			TextEditFrame textEditFrame = new TextEditFrame(text, selectionStartPosition, selectionStartPosition, selectionEndPosition);
			nativeEventQueue.Enqueue(new Event(EventType.TEXT_EDIT_UPDATE, textEditFrame));
		}

		/// <summary>Event callback when other inputfield got autofilled</summary>
		public void OnAutofillUpdate(string text, AutofillType autofillType)
		{
			AdvancedInputField[] inputfields = GameObject.FindObjectsOfType<AdvancedInputField>();
			int length = inputfields.Length;
			for(int i = 0; i < length; i++) //Find an enabled inputfield with given autofillType
			{
				if(inputfields[i].AutofillType == autofillType)
				{
					inputfields[i].Text = text;
				}
			}
		}

		/// <summary>Event callback when the keyboard gets shown</summary>
		public void OnKeyboardShow()
		{
			nativeEventQueue.Enqueue(new Event(EventType.SHOW));
			State = KeyboardState.VISIBLE;
		}

		/// <summary>Event callback when the keyboard gets hidden</summary>
		public void OnKeyboardHide()
		{
			nativeEventQueue.Enqueue(new Event(EventType.HIDE));
			State = KeyboardState.HIDDEN;
		}

		/// <summary>Event callback when the done key of the keyboard gets pressed</summary>
		public void OnKeyboardDone()
		{
			nativeEventQueue.Enqueue(new Event(EventType.DONE));
		}

		/// <summary>Event callback when the next key of the keyboard gets pressed</summary>
		public void OnKeyboardNext()
		{
			nativeEventQueue.Enqueue(new Event(EventType.NEXT));
		}

		/// <summary>Event callback when the cancel key of the keyboard (back key on Android) gets pressed</summary>
		public void OnKeyboardCancel()
		{
			nativeEventQueue.Enqueue(new Event(EventType.CANCEL));
		}

		/// <summary>Event callback when the delete/backspace key of the keyboard gets pressed</summary>
		public void OnKeyboardDelete()
		{
			nativeEventQueue.Enqueue(new Event(EventType.DELETE));
		}

		/// <summary>Event callback when the height of the keyboard has changed</summary>
		public void OnKeyboardHeightChanged(int height)
		{
			if(onKeyboardHeightChanged != null)
			{
				onKeyboardHeightChanged.Invoke(height);
			}

			if(height == 0) //Safety check if something external caused the keyboard to hide
			{
				State = KeyboardState.HIDDEN;
			}
		}

		/// <summary>Event callback when the connectivity of the hardware keyboard has changed</summary>
		public void OnHardwareKeyboardChanged(bool connected)
		{
			HardwareKeyboardConnected = connected;
			if(HardwareKeyboardConnected)
			{
				HideKeyboard();
				DisableUpdates();
			}
			else
			{
				EnableUpdates();
			}

			if(onHardwareKeyboardChanged != null)
			{
				onHardwareKeyboardChanged.Invoke(connected);
			}
		}
	}
}
