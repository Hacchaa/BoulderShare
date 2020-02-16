//-----------------------------------------
//			Advanced Input Field
// Copyright (c) 2017 Jeroen van Pienbroek
//------------------------------------------

using UnityEngine;

namespace AdvancedInputFieldPlugin
{
	/// <summary>The TextNavigator for mobile platforms</summary>
	public class MobileTextNavigator: TextNavigator
	{
		/// <summary>The thumb size multiplier used for selection cursors size calculations</summary>
		private const float THUMB_SIZE_RATIO = 0.5f;

		/// <summary>The min ActionBar horizontal offset from the sides</summary>
		private const float ACTION_BAR_MARGIN_X = 0.05f;

		/// <summary>The TouchScreenKeyboard</summary>
		private NativeKeyboard keyboard;

		private MobileCursorsControl mobileCursorsControl;

		/// <summary>Indicates whether at least one character has been inserted (or deleted) afer last click</summary>
		public bool hasInsertedCharAfterClick;

		/// <summary>Indicates whether at least one character has been inserted (or deleted) afer last click</summary>
		public bool HasInsertedCharAfterClick
		{
			get { return hasInsertedCharAfterClick; }
			set
			{
				hasInsertedCharAfterClick = value;

				if(!(InputField.MobileSelectionCursorsEnabled || InputField.ActionBarEnabled)) { return; }
				if(!hasInsertedCharAfterClick && !HasSelection)
				{
					MobileCursorsControl.EnableCurrentCursor();
				}
				else
				{
					MobileCursorsControl.DisableCurrentCursor();
				}
			}
		}

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

		public override int ProcessedCaretPosition
		{
			get { return base.ProcessedCaretPosition; }
			set
			{
				base.ProcessedCaretPosition = value;

				if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
				{
					UpdateNativeSelection();
				}
			}
		}

		public override int ProcessedSelectionStartPosition
		{
			get { return base.ProcessedSelectionStartPosition; }
			protected set
			{
				int originalValue = base.ProcessedSelectionStartPosition;
				base.ProcessedSelectionStartPosition = value;

				if(InputField.MobileSelectionCursorsEnabled)
				{
					if(Canvas != null)
					{
						UpdateMobileSelectionCursors();
					}
				}

				if(InputField.ActionBarEnabled && base.ProcessedSelectionStartPosition != originalValue)
				{
					UpdateSelectionCursorsActionBar();
				}

				if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
				{
					UpdateNativeSelection();
				}
			}
		}

		public override int ProcessedSelectionEndPosition
		{
			get { return base.ProcessedSelectionEndPosition; }
			protected set
			{
				int originalValue = base.ProcessedSelectionEndPosition;
				base.ProcessedSelectionEndPosition = value;

				if(InputField.MobileSelectionCursorsEnabled)
				{
					if(Canvas != null)
					{
						UpdateMobileSelectionCursors();
					}
				}

				if(InputField.ActionBarEnabled && base.ProcessedSelectionEndPosition != originalValue)
				{
					UpdateSelectionCursorsActionBar();
				}

				if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
				{
					UpdateNativeSelection();
				}
			}
		}

		/// <summary>The ActionBar</summary>
		public ActionBar ActionBar { get; set; }

		/// <summary>The MobileCursorsControl</summary>
		public MobileCursorsControl MobileCursorsControl
		{
			get
			{
				if(mobileCursorsControl == null)
				{
					mobileCursorsControl = GameObject.FindObjectOfType<MobileCursorsControl>();
					if(mobileCursorsControl == null) //No existing instance
					{
						mobileCursorsControl = GameObject.Instantiate(Settings.MobileSelectionCursorsPrefab);
					}
				}

				return mobileCursorsControl;
			}
		}

		internal override void SetCaretPosition(int caretPosition, bool invokeCaretPositonChangeEvent = false)
		{
			base.SetCaretPosition(caretPosition, invokeCaretPositonChangeEvent);

			if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
			{
				UpdateNativeSelection();
			}
		}

		internal override void SetSelectionStartPosition(int selectionStartPosition, bool invokeTextSelectionChangeEvent = false)
		{
			int originalValue = base.SelectionStartPosition;
			base.SetSelectionStartPosition(selectionStartPosition, invokeTextSelectionChangeEvent);

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileSelectionCursors();
				}
			}

			if(InputField.ActionBarEnabled && base.SelectionStartPosition != originalValue)
			{
				UpdateSelectionCursorsActionBar();
			}

			if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
			{
				UpdateNativeSelection();
			}
		}

		internal override void SetSelectionEndPosition(int selectionEndPosition, bool invokeTextSelectionChangeEvent = false)
		{
			int originalValue = base.selectionEndPosition;
			base.SetSelectionEndPosition(selectionEndPosition, invokeTextSelectionChangeEvent);

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileSelectionCursors();
				}
			}

			if(InputField.ActionBarEnabled && base.SelectionEndPosition != originalValue)
			{
				UpdateSelectionCursorsActionBar();
			}

			if(Keyboard.State == KeyboardState.VISIBLE && !InputField.ReadOnly && !BlockNativeSelectionChange)
			{
				UpdateNativeSelection();
			}
		}

		internal override void ResetCaret(Vector2 position)
		{
			base.ResetCaret(position);

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileCurrentCursor(true);
				}
			}
		}

		internal override void OnCanvasScaleChanged(float canvasScaleFactor)
		{
			base.OnCanvasScaleChanged(canvasScaleFactor);

			UpdateCursorSize(canvasScaleFactor);
		}

		internal void UpdateCursorSize(float canvasScaleFactor)
		{
#if UNITY_EDITOR
			int thumbSize = -1;
#else
			int thumbSize = Util.DetermineThumbSize();
#endif
			float cursorSize;
			if(thumbSize <= 0) //Unknown DPI
			{
				if(InputField.TextRenderer.ResizeTextForBestFit)
				{
					cursorSize = InputField.TextRenderer.FontSizeUsedForBestFit * 1.5f;
				}
				else
				{
					cursorSize = InputField.TextRenderer.FontSize * 1.5f;
				}
			}
			else
			{
				cursorSize = (thumbSize * THUMB_SIZE_RATIO) / canvasScaleFactor;
			}

			cursorSize *= Settings.MobileSelectionCursorsScale;

			MobileCursorsControl.UpdateCursorSize(cursorSize);
		}

		internal override void BeginEditMode()
		{
			base.BeginEditMode();

			MobileCursorsControl.Setup(TextContentTransform, this);
			MobileCursorsControl.HideCursors();

			if(Canvas != null)
			{
				UpdateCursorSize(Canvas.scaleFactor);
			}
		}

		internal override void EndEditMode()
		{
			EditMode = false;
			caretBlinkTime = InputField.CaretBlinkRate;
			CaretRenderer.enabled = false;
			UpdateSelection(0, 0);

			if(MobileCursorsControl != null)
			{
				MobileCursorsControl.HideCursors();
			}

			if(ActionBar != null)
			{
				ActionBar.Hide();
			}

			ScrollArea scrollArea = TextAreaTransform.GetComponent<ScrollArea>();
			switch(InputField.ScrollBehaviourOnEndEdit)
			{
				case ScrollBehaviourOnEndEdit.START_OF_TEXT: scrollArea.MoveContentImmediately(Vector2.zero); break;
			}
			scrollArea.EditMode = false;
		}

		internal void HideCurrentMobileCursor()
		{
			MobileCursorsControl.DisableCurrentCursor();
		}

		internal void UpdateNativeSelection()
		{
			if(InputField.RichTextEditing)
			{
				TextEditFrame textEditFrame = InputField.RichTextProcessor.LastTextEditFrame;
				if(HasSelection)
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
				if(HasSelection)
				{
					Keyboard.UpdateTextEdit(InputField.Text, selectionStartPosition, selectionEndPosition);
				}
				else
				{
					Keyboard.UpdateTextEdit(InputField.Text, caretPosition, caretPosition);
				}
			}
		}

		public void UpdateSelectionStart(Vector2 position, out Vector2 cursorPosition, out bool switchToEnd)
		{
			TextRenderer activeTextRenderer = InputField.GetActiveTextRenderer();
			int charIndex = GetCharacterIndexFromPosition(activeTextRenderer, position);
			if(charIndex <= VisibleSelectionEndPosition)
			{
				VisibleSelectionStartPosition = charIndex;
				CaretPosition = charIndex;
				switchToEnd = false;
			}
			else
			{
				VisibleSelectionStartPosition = VisibleSelectionEndPosition;
				VisibleSelectionEndPosition = charIndex;
				CaretPosition = charIndex;
				switchToEnd = true;
			}

			CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
			int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
			LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

			cursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
		}

		public void UpdateSelectionEnd(Vector2 position, out Vector2 cursorPosition, out bool switchToStart)
		{
			TextRenderer activeTextRenderer = InputField.GetActiveTextRenderer();
			int charIndex = GetCharacterIndexFromPosition(activeTextRenderer, position);
			if(charIndex >= VisibleSelectionStartPosition)
			{
				VisibleSelectionEndPosition = charIndex;
				CaretPosition = charIndex;
				switchToStart = false;
			}
			else
			{
				VisibleSelectionEndPosition = VisibleSelectionStartPosition;
				VisibleSelectionStartPosition = charIndex;
				CaretPosition = charIndex;
				switchToStart = true;
			}

			CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
			int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
			LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

			cursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
		}

		public void UpdateCurrentCursor(Vector2 position, out Vector2 cursorPosition)
		{
			TextRenderer activeTextRenderer = InputField.GetActiveTextRenderer();
			int charIndex = GetCharacterIndexFromPosition(activeTextRenderer, position);
			VisibleCaretPosition = charIndex;

			CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
			int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
			LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

			cursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
		}

		internal void ToggleActionBar()
		{
			if(ActionBar.Visible)
			{
				ActionBar.Hide();
			}
			else
			{
				ShowActionBar();
			}
		}

		internal void ShowActionBar()
		{
			bool paste = !InputField.ReadOnly && InputField.ActionBarPaste;
			bool selectAll = InputField.ActionBarSelectAll && InputField.Text.Length > 0;
			ActionBar.Show(false, false, paste, selectAll);
			UpdateActionBarPosition();
		}

		/// <summary>Updates the mobile selection cursors</summary>
		internal void UpdateMobileSelectionCursors(bool resetMobileCursorPosition = false)
		{
			TextRenderer activeTextRenderer = InputField.GetActiveTextRenderer();
			if(SelectionEndPosition > SelectionStartPosition || MobileCursorsControl.StartCursor.Selected || MobileCursorsControl.EndCursor.Selected)
			{
				float lineHeight = activeTextRenderer.GetLineInfo(0).height;

				if(SelectionStartPosition >= 0)
				{
					if(resetMobileCursorPosition)
					{
						int charIndex = Mathf.Clamp(VisibleSelectionStartPosition, 0, activeTextRenderer.CharacterCount - 1);
						CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
						int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
						LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

						Vector2 startCursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
						MobileCursorsControl.StartCursor.UpdatePosition(startCursorPosition);
					}

					MobileCursorsControl.EnableStartCursor();
					if(MobileCursorsControl.ShouldStartCursorFrontBeVisible)
					{
						MobileCursorsControl.ShowStartCursorFront();
					}
					else
					{
						MobileCursorsControl.HideStartCursorFront();
					}
				}
				else
				{
					MobileCursorsControl.DisableStartCursor();
				}

				if(SelectionEndPosition >= 0)
				{
					if(resetMobileCursorPosition)
					{
						int charIndex = Mathf.Clamp(VisibleSelectionEndPosition, 0, activeTextRenderer.CharacterCount - 1);
						CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
						int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
						LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

						Vector2 endCursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
						MobileCursorsControl.EndCursor.UpdatePosition(endCursorPosition);
					}

					MobileCursorsControl.EnableEndCursor();
					if(MobileCursorsControl.ShouldEndCursorFrontBeVisible)
					{
						MobileCursorsControl.ShowEndCursorFront();
					}
					else
					{
						MobileCursorsControl.HideEndCursorFront();
					}
				}
				else
				{
					MobileCursorsControl.DisableEndCursor();
				}

				MobileCursorsControl.DisableCurrentCursor();
			}
			else
			{
				MobileCursorsControl.DisableStartCursor();
				MobileCursorsControl.DisableEndCursor();
			}
		}

		internal void UpdateActionBarPosition()
		{
			int maxCharIndex = Mathf.Max(TextRenderer.CharacterCount - 1, 0);
			Vector2 actionBarPosition;

			if(SelectionEndPosition > SelectionStartPosition)
			{
				int startCharIndex = Mathf.Clamp(VisibleSelectionStartPosition, 0, maxCharIndex);
				int endCharIndex = Mathf.Clamp(VisibleSelectionEndPosition, 0, maxCharIndex);
				Vector3 startPosition = TextRenderer.GetCharacterInfo(startCharIndex).position;
				Vector3 endPosition = TextRenderer.GetCharacterInfo(endCharIndex).position;

				actionBarPosition = startPosition;
				actionBarPosition.x = (startPosition.x + endPosition.x) / 2f;
			}
			else
			{
				int charIndex = Mathf.Clamp(VisibleCaretPosition, 0, maxCharIndex);
				actionBarPosition = TextRenderer.GetCharacterInfo(charIndex).position;
			}

			actionBarPosition += TextContentTransform.anchoredPosition;
			actionBarPosition.x -= (TextAreaTransform.rect.width * 0.5f);
			actionBarPosition.y = Mathf.Min(actionBarPosition.y, 0);
			ActionBar.UpdatePosition(actionBarPosition);

			KeepActionBarWithinBounds();
		}

		internal void UpdateSelectionCursorsActionBar()
		{
			if(SelectionEndPosition > SelectionStartPosition)
			{
				ActionBar.transform.SetParent(InputField.transform);
				ActionBar.transform.localScale = Vector3.one;
				bool cut = !InputField.Secure && !InputField.ReadOnly && InputField.ActionBarCut;
				bool copy = !InputField.Secure && InputField.ActionBarCopy;
				bool paste = !InputField.ReadOnly && InputField.ActionBarPaste;
				bool selectAll = InputField.ActionBarSelectAll;
				ActionBar.Show(cut, copy, paste, selectAll);

				UpdateActionBarPosition();
			}
			else
			{
				ActionBar.Hide();
			}
		}

		private void KeepActionBarWithinBounds()
		{
			RectTransform rectTransform = ActionBar.RectTransform;
			Vector3[] corners = new Vector3[4]; //BottomLeft, TopLeft, TopRight, BottomRight
			rectTransform.GetWorldCorners(corners);
			float height = corners[1].y - corners[0].y;
			float width = corners[3].x - corners[0].x;

			float leftX = corners[1].x;
			float rightX = corners[2].x;
			float topY = corners[1].y;
			float normalizedLeftX = 0;
			float normalizedRightX = 0;
			float normalizedTopY = 0;
			if(Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				normalizedLeftX = leftX / Screen.width;
				normalizedRightX = rightX / Screen.width;
				normalizedTopY = topY / Screen.height;
			}
			else
			{
				Camera camera = Canvas.worldCamera;
				float orthographicHeight = camera.orthographicSize;
				float orthographicWidth = camera.orthographicSize / Screen.height * Screen.width;
				normalizedLeftX = (leftX + orthographicWidth) / (orthographicWidth * 2);
				normalizedRightX = (rightX + orthographicWidth) / (orthographicWidth * 2);
				normalizedTopY = (topY + orthographicHeight) / (orthographicHeight * 2);
			}

			if(normalizedTopY > 1) //Out of bounds, move to bottom of InputField
			{
				Vector2 actionBarPosition = rectTransform.anchoredPosition;
				actionBarPosition.y -= (InputField.Size.y + ActionBar.RectTransform.rect.height);
				ActionBar.UpdatePosition(actionBarPosition);
			}

			if(normalizedLeftX < ACTION_BAR_MARGIN_X)
			{
				Vector2 actionBarPosition = rectTransform.anchoredPosition;
				actionBarPosition.x += (ACTION_BAR_MARGIN_X - normalizedLeftX) * (Canvas.pixelRect.width / Canvas.scaleFactor);
				ActionBar.UpdatePosition(actionBarPosition);
			}
			else if(normalizedRightX > 1 - ACTION_BAR_MARGIN_X)
			{
				Vector2 actionBarPosition = rectTransform.anchoredPosition;
				actionBarPosition.x += ((1 - ACTION_BAR_MARGIN_X) - normalizedRightX) * (Canvas.pixelRect.width / Canvas.scaleFactor);
				ActionBar.UpdatePosition(actionBarPosition);
			}
		}

		/// <summary>Updates the current cursor</summary>
		internal void UpdateMobileCurrentCursor(bool resetMobileCursorPosition = false)
		{
			TextRenderer activeTextRenderer = InputField.GetActiveTextRenderer();
			if(MobileCursorsControl.StartCursor.Selected || MobileCursorsControl.EndCursor.Selected)
			{
				MobileCursorsControl.DisableCurrentCursor();
				return;
			}

			bool fixEmptyCaret = false;
			if(activeTextRenderer.CharacterCount == 0 || activeTextRenderer.Text.Length == 0) //Workaround to make sure the text generator will give a correct position for the first character
			{
				fixEmptyCaret = true;
				activeTextRenderer.UpdateImmediately(" ");
			}

			int charIndex = Mathf.Clamp(VisibleCaretPosition, 0, activeTextRenderer.CharacterCount - 1);
			if(resetMobileCursorPosition)
			{
				CharacterInfo charInfo = activeTextRenderer.GetCharacterInfo(charIndex);
				int lineIndex = DetermineCharacterLine(activeTextRenderer, charIndex);
				LineInfo lineInfo = activeTextRenderer.GetLineInfo(lineIndex);

				Vector2 currentCursorPosition = new Vector2(charInfo.position.x, lineInfo.topY - lineInfo.height);
				if(CaretPosition >= activeTextRenderer.CharacterCountVisible)
				{
					currentCursorPosition.x += charInfo.width;
				}

				MobileCursorsControl.CurrentCursor.UpdatePosition(currentCursorPosition);
			}

			if(HasSelection)
			{
				MobileCursorsControl.DisableCurrentCursor();
			}
			else if(!HasInsertedCharAfterClick && InputField.ActionBarEnabled)
			{
				MobileCursorsControl.EnableCurrentCursor();
			}

			if(InputField.ActionBarEnabled && !HasSelection)
			{
				ActionBar.transform.SetParent(InputField.transform);
				ActionBar.transform.localScale = Vector3.one;

				UpdateActionBarPosition();
			}

			if(fixEmptyCaret)
			{
				activeTextRenderer.UpdateImmediately(string.Empty);
			}
		}

		internal override void UpdateCaret()
		{
			base.UpdateCaret();

			if(EditMode && (InputField.ActionBarEnabled || InputField.MobileSelectionCursorsEnabled))
			{
				if(Canvas != null)
				{
					UpdateMobileCurrentCursor(InputField.GetActiveTextRenderer());
				}
			}
		}

		internal override void SelectCurrentWord()
		{
			base.SelectCurrentWord();

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileSelectionCursors(true);
				}
			}
			else if(InputField.ActionBarEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileCurrentCursor(true);
				}
			}
			UpdateNativeSelection();
		}

		internal override void SelectAll()
		{
			base.SelectAll();

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileSelectionCursors(true);
				}
			}
			else if(InputField.ActionBarEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileCurrentCursor(true);
				}
			}
			UpdateNativeSelection();
		}

		internal override void UpdateSelectionArea(int currentPosition, int pressPosition, bool autoSelectWord)
		{
			base.UpdateSelectionArea(currentPosition, pressPosition, autoSelectWord);

			if(InputField.MobileSelectionCursorsEnabled)
			{
				UpdateMobileSelectionCursors(true);
			}
		}

		/// <summary>Updates the selection without updating selection in native text editor</summary>
		/// <param name="position">The new caret position</param>
		internal void UpdateSelection(int startPosition, int endPosition)
		{
			BlockNativeSelectionChange = true;
			if(startPosition + 1 <= selectionStartPosition)
			{
				base.SelectionStartPosition = startPosition;
				base.SelectionEndPosition = endPosition;
				base.CaretPosition = startPosition;
			}
			else
			{
				base.SelectionStartPosition = startPosition;
				base.SelectionEndPosition = endPosition;
				base.CaretPosition = endPosition;
			}

			if(InputField.MobileSelectionCursorsEnabled)
			{
				if(Canvas != null)
				{
					UpdateMobileSelectionCursors();
				}
			}

			if(InputField.ActionBarEnabled)
			{
				UpdateSelectionCursorsActionBar();
			}
			BlockNativeSelectionChange = false;
		}
	}
}