//-----------------------------------------
//			Advanced Input Field
// Copyright (c) 2017 Jeroen van Pienbroek
//------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace AdvancedInputFieldPlugin
{
	/// <summary>Subclass of TextInputHandler for mobile platforms</summary>
	public class MobileTextInputHandler: TextInputHandler
	{
		/// <summary>The TouchScreenKeyboard</summary>
		private NativeKeyboard keyboard;

		/// <summary>The MobileTextNavigator</summary>
		private MobileTextNavigator mobileTextNavigator;

		/// <summary>The MobileTextManipulator</summary>
		private MobileTextManipulator mobileTextManipulator;

		/// <summary>The ActionBar</summary>
		protected ActionBar actionBar;

		/// <summary>The TouchScreenKeyboard</summary>
		private NativeKeyboard Keyboard
		{
			get
			{
				if(keyboard == null)
				{
					keyboard = NativeKeyboardManager.Keyboard;
				}

				return keyboard;
			}
		}

		public override TextNavigator TextNavigator
		{
			get { return mobileTextNavigator; }
			protected set { mobileTextNavigator = (MobileTextNavigator)value; }
		}

		public override TextManipulator TextManipulator
		{
			get { return mobileTextManipulator; }
			protected set { mobileTextManipulator = (MobileTextManipulator)value; }
		}

		public MobileTextInputHandler()
		{
		}

		internal override void Initialize(AdvancedInputField inputField, TextNavigator textNavigator, TextManipulator textManipulator)
		{
			base.Initialize(inputField, textNavigator, textManipulator);

			mobileTextNavigator.ActionBar = actionBar;
		}

		/// <summary>Initializes the ActionBar</summary>
		/// <param name="textRenderer">The text renderer to attach the ActionBar to</param>
		internal void InitActionBar(AdvancedInputField inputField, TextRenderer textRenderer)
		{
			actionBar = inputField.transform.root.GetComponentInChildren<ActionBar>(true);
			if(actionBar == null)
			{
				actionBar = GameObject.Instantiate(Settings.ActionBarPrefab);
			}

			if(mobileTextNavigator != null && mobileTextNavigator.ActionBar == null)
			{
				mobileTextNavigator.ActionBar = actionBar;
			}

			actionBar.transform.SetParent(textRenderer.transform);
			actionBar.transform.localScale = Vector3.one;
			actionBar.transform.localPosition = Vector3.zero;

			actionBar.Initialize(inputField, this);
		}

		internal override void OnDestroy()
		{
			base.OnDestroy();

			if(actionBar != null)
			{
				GameObject.Destroy(actionBar.gameObject);
				actionBar = null;
			}
		}

		internal override void OnCanvasScaleChanged(float canvasScaleFactor)
		{
			base.OnCanvasScaleChanged(canvasScaleFactor);

			if(actionBar != null)
			{
				actionBar.UpdateSize(canvasScaleFactor);
				actionBar.UpdateButtons();
			}
		}

		internal override void Process()
		{
			base.Process();

			if(InputField.ReadOnly)
			{
				return;
			}

#if UNITY_EDITOR
			if(Settings.SimulateMobileBehaviourInEditor)
			{
				HardwareKeyboardProcess();
			}
#endif

			if(Keyboard.State == KeyboardState.HIDDEN)
			{
				LoadKeyboard();
			}

			NativeKeyboard.Event keyboardEvent;
			while(Keyboard.PopEvent(out keyboardEvent))
			{
				switch(keyboardEvent.type)
				{
					case NativeKeyboard.EventType.TEXT_EDIT_UPDATE: ProcessTextEditUpdate(keyboardEvent); break;
					case NativeKeyboard.EventType.SHOW: ProcessShow(keyboardEvent); break;
					case NativeKeyboard.EventType.HIDE: ProcessHide(keyboardEvent); break;
					case NativeKeyboard.EventType.DONE: ProcessDone(keyboardEvent); break;
					case NativeKeyboard.EventType.NEXT: ProcessDone(keyboardEvent); break;
					case NativeKeyboard.EventType.CANCEL: ProcessCancel(keyboardEvent); break;
				}
			}
		}

		public void LoadKeyboard()
		{
			string text = InputField.Text;
			int caretPosition = TextNavigator.CaretPosition;
			int selectionStartPosition = TextNavigator.SelectionStartPosition;
			int selectionEndPosition = TextNavigator.SelectionEndPosition;

			if(!TextNavigator.HasSelection)
			{
				selectionStartPosition = caretPosition;
				selectionEndPosition = caretPosition;
			}

			if(InputField.RichTextEditing)
			{
				RichTextProcessor richTextProcessor = InputField.RichTextProcessor;
				TextEditFrame textEditFrame = richTextProcessor.LastTextEditFrame;
				text = textEditFrame.text;
				selectionStartPosition = textEditFrame.selectionStartPosition;
				selectionEndPosition = textEditFrame.selectionEndPosition;
			}

			NativeKeyboardConfiguration configuration = new NativeKeyboardConfiguration()
			{
				keyboardType = InputField.KeyboardType,
				characterValidation = InputField.CharacterValidation,
				lineType = InputField.LineType,
				autocapitalizationType = InputField.AutocapitalizationType,
				autofillType = InputField.AutofillType,
				returnKeyType = InputField.ReturnKeyType,
				autocorrection = InputField.AutoCorrection,
				secure = InputField.Secure,
				richTextEditing = InputField.RichTextEditing,
				emojisAllowed = InputField.EmojisAllowed,
				hasNext = InputField.HasNext,
				characterLimit = InputField.CharacterLimit
			};

			string characterValidatorJSON = null;
			if(InputField.CharacterValidation == CharacterValidation.Custom && InputField.CharacterValidator != null)
			{
				characterValidatorJSON = JsonUtility.ToJson(InputField.CharacterValidator);
			}
			configuration.characterValidatorJSON = characterValidatorJSON;

#if !UNITY_EDITOR
			Keyboard.State = KeyboardState.PENDING_SHOW;
#endif
			NativeKeyboardManager.ShowKeyboard(text, selectionStartPosition, selectionEndPosition, configuration);
		}

#if UNITY_EDITOR
		internal void HardwareKeyboardProcess()
		{
			Event keyboardEvent = new Event();
			while(Event.PopEvent(keyboardEvent))
			{
				if(keyboardEvent.rawType == EventType.KeyDown)
				{
					bool shouldContinue = ProcessKeyboardEvent(keyboardEvent);
					if(!shouldContinue)
					{
						return;
					}
				}
			}

			SimulatorKeyboard simulatorKeyboard = (SimulatorKeyboard)Keyboard;
			InputEvent inputEvent = null;
			while(InputMethodManager.PopEvent(out inputEvent))
			{
				switch(inputEvent.Type)
				{
					case InputEventType.CHARACTER:
						CharacterInputEvent characterInputEvent = (CharacterInputEvent)inputEvent;
						simulatorKeyboard.Insert(characterInputEvent.character.ToString());
						break;
					case InputEventType.TEXT:
						TextInputEvent textInputEvent = (TextInputEvent)inputEvent;
						simulatorKeyboard.Insert(textInputEvent.text);
						break;
				}
			}
		}

		/// <summary>Processes a keyboard event</summary>
		/// <param name="keyboardEvent">The keyboard event to process</param>
		internal bool ProcessKeyboardEvent(Event keyboardEvent)
		{
			SimulatorKeyboard simulatorKeyboard = (SimulatorKeyboard)Keyboard;

			switch(keyboardEvent.keyCode)
			{
				case KeyCode.Backspace:
					simulatorKeyboard.OnBackspaceClick();
					return true;
				case KeyCode.Delete:
					simulatorKeyboard.OnDeleteClick();
					return true;
				case KeyCode.LeftArrow:
					return true;
				case KeyCode.RightArrow:
					return true;
				case KeyCode.DownArrow:
					return true;
				case KeyCode.UpArrow:
					return true;
				case KeyCode.Return: //Submit
				case KeyCode.KeypadEnter: //Submit
					if(InputField.ShouldSubmit)
					{
						InputField.Deselect(EndEditReason.KEYBOARD_DONE);
						return false;
					}
					break;
				case KeyCode.Escape:
					InputField.Deselect(EndEditReason.KEYBOARD_CANCEL);
					return false;
				case KeyCode.Tab:
					return false;
			}

			char c = keyboardEvent.character;
			if(!InputField.Multiline && (c == '\t' || c == '\r' || c == 10)) //Don't allow return chars or tabulator key to be entered into single line fields.
			{
				return true;
			}

			if(c != 10 && (c < 32 || c > 255)) { return true; }

			if(c == '\r' || (int)c == 3) //Convert carriage return and end-of-text characters to newline.
			{
				c = '\n';
			}

			simulatorKeyboard.Insert(c.ToString());

			return true;
		}
#endif

		internal override void BeginEditMode()
		{
			base.BeginEditMode();

			if(InputField == NativeKeyboardManager.ActiveInputFieldBeforePause)
			{
				NativeKeyboardManager.ActiveInputFieldBeforePause = null;
				Keyboard.RestoreKeyboard();
				Keyboard.State = KeyboardState.PENDING_SHOW;
			}
			else
			{
				Keyboard.ClearEventQueue();
				Keyboard.State = KeyboardState.HIDDEN; //Flag keyboard as inactive, so this inputfield will load it's settings
			}

			if(actionBar != null)
			{
				if(Canvas != null)
				{
					actionBar.UpdateSize(Canvas.scaleFactor);
					actionBar.UpdateButtons();
				}
			}
		}

		/// <summary>Processes text edit update event</summary>
		internal void ProcessTextEditUpdate(NativeKeyboard.Event keyboardEvent)
		{
			TextEditFrame textEditFrame = keyboardEvent.textEditFrame;
			string text = textEditFrame.text;
			string previousText = InputField.Text;
			int previousSelectionStartPosition = TextNavigator.SelectionStartPosition;
			int previousSelectionEndPosition = TextNavigator.SelectionEndPosition;
			if(InputField.LineLimit > 0 && text.Length > 0)
			{
				InputField.TextRenderer.Text = text;
				if(InputField.TextRenderer.LineCount > InputField.LineLimit) //Exceeded line limit, revert this text edit
				{
					InputField.TextRenderer.Text = previousText;
					Keyboard.UpdateTextEdit(previousText, previousSelectionStartPosition, previousSelectionEndPosition);
					return;
				}
			}

			int textLengthBefore = InputField.Text.Length;

			TextManipulator.BlockNativeTextChange = true;
			TextNavigator.BlockNativeSelectionChange = true;
			InputField.ApplyTextEditFrame(textEditFrame);
			TextManipulator.BlockNativeTextChange = false;
			TextNavigator.BlockNativeSelectionChange = false;

			int textLengthAfter = InputField.Text.Length;
			if(textLengthBefore != textLengthAfter)
			{
				if(!mobileTextNavigator.HasInsertedCharAfterClick)
				{
					mobileTextNavigator.HasInsertedCharAfterClick = true;

					if(InputField.ActionBarEnabled)
					{
						mobileTextNavigator.ActionBar.Hide();
						mobileTextNavigator.HideCurrentMobileCursor();
					}
					else if(InputField.MobileSelectionCursorsEnabled)
					{
						mobileTextNavigator.HideCurrentMobileCursor();
					}
				}
			}
		}

		/// <summary>Processes keyboard show event</summary>
		internal void ProcessShow(NativeKeyboard.Event keyboardEvent)
		{
		}

		/// <summary>Processes keyboard hide event</summary>
		internal void ProcessHide(NativeKeyboard.Event keyboardEvent)
		{
		}

		/// <summary>Processes keyboard done event</summary>
		internal void ProcessDone(NativeKeyboard.Event keyboardEvent)
		{
			Keyboard.ClearEventQueue(); //Should be last event to process, so clear queue

			if(InputField.NextInputField != null)
			{
				Keyboard.State = KeyboardState.HIDDEN; //Flag keyboard as inactive, so next inputfield will load it's settings
				InputField.Deselect(EndEditReason.KEYBOARD_NEXT);
				InputField.NextInputField.ManualSelect(BeginEditReason.KEYBOARD_NEXT);
			}
			else
			{
				InputField.Deselect(EndEditReason.KEYBOARD_DONE);
			}
		}

		/// <summary>Processes keyboard cancel event</summary>
		internal void ProcessCancel(NativeKeyboard.Event keyboardEvent)
		{
			Keyboard.ClearEventQueue(); //Should be last event to process, so clear queue

			InputField.Deselect(EndEditReason.KEYBOARD_CANCEL);
		}

		internal override void OnSelect()
		{
			base.OnSelect();

			if(InputField.ReadOnly)
			{
				return;
			}

			if(InputField.RichTextEditing)
			{
				TextEditFrame textEditFrame = InputField.RichTextProcessor.LastTextEditFrame;
				if(TextNavigator.HasSelection)
				{
					Keyboard.UpdateTextEdit(textEditFrame.text, textEditFrame.selectionStartPosition, textEditFrame.selectionEndPosition);
				}
				else
				{
					Keyboard.UpdateTextEdit(textEditFrame.text, textEditFrame.caretPosition, textEditFrame.caretPosition);
				}
			}
			else
			{
				if(TextNavigator.HasSelection)
				{
					Keyboard.UpdateTextEdit(InputField.Text, TextNavigator.SelectionStartPosition, TextNavigator.SelectionEndPosition);
				}
				else
				{
					Keyboard.UpdateTextEdit(InputField.Text, TextNavigator.CaretPosition, TextNavigator.CaretPosition);
				}
			}
		}

		internal override void OnHold(Vector2 position)
		{
			base.OnHold(position);
			if(InputField.Text.Length > 0)
			{
				TextNavigator.SelectCurrentWord();
			}
			else if(InputField.ActionBarEnabled)
			{
				mobileTextNavigator.ShowActionBar();
			}
		}

		internal override void OnSingleTap(Vector2 position)
		{
			base.OnSingleTap(position);

			if(InputField.ActionBarEnabled)
			{
				mobileTextNavigator.HasInsertedCharAfterClick = false;
			}
		}

		/// <summary>Event callback when cut button has been clicked</summary>
		public void OnCut()
		{
			TextManipulator.Cut();
		}

		/// <summary>Event callback when copy button has been clicked</summary>
		public void OnCopy()
		{
			TextManipulator.Copy();
		}

		/// <summary>Event callback when paste button has been clicked</summary>
		public void OnPaste()
		{
			TextManipulator.Paste();
		}

		/// <summary>Event callback when select all button has been clicked</summary>
		public void OnSelectAll()
		{
			TextNavigator.SelectAll();
		}

		internal override void CancelInput()
		{
			base.CancelInput();

			if(Keyboard != null)
			{
				GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
				if(currentSelectedGameObject != null)
				{
					AdvancedInputField currentSelectedInputField = currentSelectedGameObject.GetComponentInParent<AdvancedInputField>();
					if(currentSelectedInputField != null && !currentSelectedInputField.ReadOnly)
					{
						if(currentSelectedInputField != InputField.NextInputField)
						{
							BeginEditReason beginEditReason = BeginEditReason.PROGRAMMATIC_SELECT;
							if(currentSelectedInputField.UserPressing)
							{
								beginEditReason = BeginEditReason.USER_SELECT;
							}

							currentSelectedInputField.ManualSelect(beginEditReason);
						}

						return; //Don't hide keyboard, next inputfield is selected
					}
				}

#if !UNITY_EDITOR
				Keyboard.State = KeyboardState.PENDING_HIDE;
#endif
				Keyboard.HideKeyboard();
				Keyboard.ClearEventQueue(); //Should be last event to process, so clear queue
			}
		}
	}
}