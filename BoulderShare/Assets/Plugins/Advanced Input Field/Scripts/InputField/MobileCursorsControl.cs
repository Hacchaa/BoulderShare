using UnityEngine;
using UnityEngine.EventSystems;

namespace AdvancedInputFieldPlugin
{
	public class MobileCursorsControl: MonoBehaviour
	{
		public MobileTextNavigator TextNavigator { get; private set; }
		public MobileCursor CurrentCursor { get; private set; }
		public MobileCursor StartCursor { get; private set; }
		public MobileCursor EndCursor { get; private set; }

		private int startCaretPosition;
		private bool initialized;

		public bool ShouldCurrentCursorFrontBeVisible
		{
			get
			{
				if(CurrentCursor.Selected) { return true; }

				return CursorWithinBounds(TextNavigator.TextAreaTransform, CurrentCursor.RectTransform);
			}
		}

		public bool ShouldStartCursorFrontBeVisible
		{
			get
			{
				if(StartCursor.Selected) { return true; }

				return CursorWithinBounds(TextNavigator.TextAreaTransform, StartCursor.RectTransform);
			}
		}

		public bool ShouldEndCursorFrontBeVisible
		{
			get
			{
				if(EndCursor.Selected) { return true; }

				return CursorWithinBounds(TextNavigator.TextAreaTransform, EndCursor.RectTransform);
			}
		}

		private void Awake()
		{
			if(!initialized) { Initialize(); }
		}

		private void Initialize()
		{
			CurrentCursor = transform.Find("CurrentCursor").GetComponent<MobileCursor>();
			StartCursor = transform.Find("StartCursor").GetComponent<MobileCursor>();
			EndCursor = transform.Find("EndCursor").GetComponent<MobileCursor>();

			initialized = true;
		}

		private void Start()
		{
			CurrentCursor.MobileCursorSelected += OnCurrentCursorSelected;
			CurrentCursor.MobileCursorMoved += OnCurrentCursorMoved;
			CurrentCursor.MobileCursorMoveFinished += OnCurrentCursorMoveFinished;
			StartCursor.MobileCursorMoved += OnCursorMoved;
			StartCursor.MobileCursorMoveFinished += OnCursorMoveFinished;
			EndCursor.MobileCursorMoved += OnCursorMoved;
			EndCursor.MobileCursorMoveFinished += OnCursorMoveFinished;
		}

		public void Setup(Transform parent, MobileTextNavigator textNavigator)
		{
			if(!initialized) { Initialize(); }

			if(TextNavigator != null)
			{
				ScrollArea scrollArea = TextNavigator.TextAreaTransform.GetComponent<ScrollArea>();
				scrollArea.OnValueChanged.RemoveListener(OnTextScrollChanged);
			}

			transform.SetParent(parent);
			transform.localScale = Vector3.one;
			transform.localPosition = Vector3.zero;
			transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

			TextNavigator = textNavigator;
			if(TextNavigator != null)
			{
				ScrollArea scrollArea = TextNavigator.TextAreaTransform.GetComponent<ScrollArea>();
				scrollArea.OnValueChanged.AddListener(OnTextScrollChanged);
			}

			CurrentCursor.Type = MobileCursorType.CURRENT_CURSOR;
			StartCursor.Type = MobileCursorType.START_CURSOR;
			EndCursor.Type = MobileCursorType.END_CURSOR;
		}

		public void OnTextScrollChanged(Vector2 scroll)
		{
			CurrentCursor.OnTextScrollChanged(scroll);
			StartCursor.OnTextScrollChanged(scroll);
			EndCursor.OnTextScrollChanged(scroll);

			if(ShouldCurrentCursorFrontBeVisible)
			{
				ShowCurrentCursorFront();
			}
			else
			{
				HideCurrentCursorFront();
			}

			if(ShouldStartCursorFrontBeVisible)
			{
				ShowStartCursorFront();
			}
			else
			{
				HideStartCursorFront();
			}

			if(ShouldEndCursorFrontBeVisible)
			{
				ShowEndCursorFront();
			}
			else
			{
				HideEndCursorFront();
			}
		}

		public void EnableCurrentCursor()
		{
			if(!initialized) { Initialize(); }
			CurrentCursor.enabled = true;
		}

		public void EnableStartCursor()
		{
			if(!initialized) { Initialize(); }
			StartCursor.enabled = true;
		}

		public void EnableEndCursor()
		{
			if(!initialized) { Initialize(); }
			EndCursor.enabled = true;
		}

		public void DisableCurrentCursor()
		{
			if(!initialized) { Initialize(); }
			CurrentCursor.enabled = false;
		}

		public void DisableStartCursor()
		{
			if(!initialized) { Initialize(); }
			StartCursor.enabled = false;
		}

		public void DisableEndCursor()
		{
			if(!initialized) { Initialize(); }
			EndCursor.enabled = false;
		}

		public void ShowCurrentCursorFront()
		{
			if(!initialized) { Initialize(); }
			CurrentCursor.ShowFront();
		}

		public void ShowStartCursorFront()
		{
			if(!initialized) { Initialize(); }
			StartCursor.ShowFront();
		}

		public void ShowEndCursorFront()
		{
			if(!initialized) { Initialize(); }
			EndCursor.ShowFront();
		}

		public void HideCursors()
		{
			if(!initialized) { Initialize(); }
			CurrentCursor.enabled = false;
			StartCursor.enabled = false;
			EndCursor.enabled = false;
		}

		public void HideCurrentCursorFront()
		{
			if(!initialized) { Initialize(); }
			CurrentCursor.HideFront();
		}

		public void HideStartCursorFront()
		{
			if(!initialized) { Initialize(); }
			StartCursor.HideFront();
		}

		public void HideEndCursorFront()
		{
			if(!initialized) { Initialize(); }
			EndCursor.HideFront();
		}

		public bool CursorWithinBounds(RectTransform boundsTransform, RectTransform cursorTransform)
		{
			Vector3[] boundsCorners = new Vector3[4]; //BottomLeft, TopLeft, TopRight, BottomRight
			boundsTransform.GetWorldCorners(boundsCorners);

			Vector3[] cursorBounds = new Vector3[4]; //BottomLeft, TopLeft, TopRight, BottomRight
			cursorTransform.GetWorldCorners(cursorBounds);
			Vector2 cursorCenter = (cursorBounds[2] + cursorBounds[0]) * 0.5f;
			Vector2 cursorSize = cursorBounds[2] - cursorBounds[0];
			cursorSize.x *= 1.1f; //Make width slighly bigger to avoid accuracy problems
			cursorBounds[0] = cursorCenter + new Vector2(-(cursorSize.x * 0.5f), -(cursorSize.y * 0.5f));
			cursorBounds[1] = cursorCenter + new Vector2(-(cursorSize.x * 0.5f), (cursorSize.y * 0.5f));
			cursorBounds[2] = cursorCenter + new Vector2((cursorSize.x * 0.5f), (cursorSize.y * 0.5f));
			cursorBounds[3] = cursorCenter + new Vector2((cursorSize.x * 0.5f), -(cursorSize.y * 0.5f));

			Vector2 min = boundsCorners[0];
			Vector2 max = boundsCorners[2];

			int length = cursorBounds.Length;
			int outOfBoundsCount = 0;
			int yOutOfBoundsCount = 0;
			for(int i = 0; i < length; i++)
			{
				Vector2 point = cursorBounds[i];

				if(point.x < min.x || point.x > max.x)
				{
					if(i != 0 && i != 3 && (point.y < min.y || point.y > max.y)) //Don't check bottom points
					{
						yOutOfBoundsCount++;
					}

					outOfBoundsCount++;
				}
				else if(i != 0 && i != 3 && (point.y < min.y || point.y > max.y)) //Don't check bottom points
				{
					yOutOfBoundsCount++;
					outOfBoundsCount++;
				}
			}

			if(outOfBoundsCount == length)
			{
				return false;
			}
			else
			{
				if(yOutOfBoundsCount > 0)
				{
					return false;
				}
				return true;
			}
		}

		public void UpdateCursorSize(float cursorSize)
		{
			CurrentCursor.UpdateSize(new Vector2(cursorSize, cursorSize));
			StartCursor.UpdateSize(new Vector2(cursorSize, cursorSize));
			EndCursor.UpdateSize(new Vector2(cursorSize, cursorSize));
		}

		public void OnCursorMoved(MobileCursor mobileCursor, PointerEventData eventData)
		{
			mobileCursor.OutOfBounds = PositionOutOfBounds(eventData);

			Vector2 localMousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(TextNavigator.TextAreaTransform, eventData.position, eventData.pressEventCamera, out localMousePosition);
			localMousePosition.x += (TextNavigator.TextAreaTransform.rect.width * 0.5f);
			localMousePosition.y -= (TextNavigator.TextAreaTransform.rect.height * 0.5f);
			localMousePosition += mobileCursor.Offset;

			Vector2 anchoredPosition = localMousePosition;
			anchoredPosition -= TextNavigator.TextContentTransform.anchoredPosition;
			CursorClampMode cursorClampMode = TextNavigator.InputField.CursorClampMode;
			if(cursorClampMode == CursorClampMode.TEXT_BOUNDS)
			{
				anchoredPosition = ClampPositionTextBounds(anchoredPosition);
			}
			else if(cursorClampMode == CursorClampMode.INPUTFIELD_BOUNDS)
			{
				anchoredPosition = ClampPositionInputFieldBounds(anchoredPosition);
			}
			mobileCursor.UpdatePosition(anchoredPosition);

			if(mobileCursor.Type == MobileCursorType.START_CURSOR)
			{
				bool switchToEnd;
				Vector2 cursorPosition;
				TextNavigator.UpdateSelectionStart(localMousePosition, out cursorPosition, out switchToEnd);
				mobileCursor.TargetPosition = cursorPosition;

				if(switchToEnd)
				{
					mobileCursor.Type = MobileCursorType.END_CURSOR;
				}
			}
			else if(mobileCursor.Type == MobileCursorType.END_CURSOR)
			{
				bool switchToStart;
				Vector2 cursorPosition;
				TextNavigator.UpdateSelectionEnd(localMousePosition, out cursorPosition, out switchToStart);
				mobileCursor.TargetPosition = cursorPosition;

				if(switchToStart)
				{
					mobileCursor.Type = MobileCursorType.START_CURSOR;
				}
			}
		}

		public Vector2 ClampPositionTextBounds(Vector2 anchoredPosition)
		{
			Vector2 textContentSize = TextNavigator.TextContentTransform.rect.size;

			float minX = 0;
			float maxX = textContentSize.x;
			anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minX, maxX);

			float minY = -textContentSize.y;
			float maxY = 0;
			anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, minY, maxY);

			return anchoredPosition;
		}

		public Vector2 ClampPositionInputFieldBounds(Vector2 anchoredPosition)
		{
			RectTransform canvasRectTransform = TextNavigator.Canvas.GetComponent<RectTransform>();

			Vector3[] inputFieldCorners = new Vector3[4];
			TextNavigator.InputField.RectTransform.GetWorldCorners(inputFieldCorners);
			for(int i = 0; i < 4; i++)
			{
				inputFieldCorners[i] = canvasRectTransform.InverseTransformPoint(inputFieldCorners[i]);
			}

			Vector3[] contentCorners = new Vector3[4];
			TextNavigator.TextContentTransform.GetWorldCorners(contentCorners);
			for(int i = 0; i < 4; i++)
			{
				contentCorners[i] = canvasRectTransform.InverseTransformPoint(contentCorners[i]);
			}
			Vector2 contentSize = contentCorners[2] - contentCorners[0];

			float leftMargin = contentCorners[0].x - inputFieldCorners[0].x;
			float rightMargin = inputFieldCorners[3].x - contentCorners[3].x;
			float minX = -leftMargin;
			float maxX = contentSize.x + rightMargin;
			anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minX, maxX);

			float bottomMargin = contentCorners[0].y - inputFieldCorners[0].y;
			float topMargin = inputFieldCorners[1].y - contentCorners[1].y;
			float minY = -(contentSize.y + bottomMargin);
			float maxY = topMargin;
			anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, minY, maxY);

			return anchoredPosition;
		}

		public bool PositionOutOfBounds(PointerEventData eventData)
		{
			return !RectTransformUtility.RectangleContainsScreenPoint(TextNavigator.TextAreaTransform, eventData.position, eventData.pressEventCamera);
		}

		public bool RectTransformWithinBounds(RectTransform rectTransform)
		{
			RectTransform boundsTransform = TextNavigator.TextAreaTransform;
			Vector3[] boundsCorners = new Vector3[4]; //BottomLeft, TopLeft, TopRight, BottomRight
			boundsTransform.GetWorldCorners(boundsCorners);

			Vector3[] corners = new Vector3[4]; //BottomLeft, TopLeft, TopRight, BottomRight
			rectTransform.GetWorldCorners(boundsCorners);

			int length = corners.Length;
			for(int i = 0; i < length; i++)
			{
				Vector3 corner = corners[i];
				if(corner.x >= boundsCorners[0].x && corner.x <= boundsCorners[2].x && corner.y >= boundsCorners[0].y && corner.y <= boundsCorners[2].y)
				{
					return true;
				}
			}

			return false;
		}

		public void OnCursorMoveFinished()
		{
			TextNavigator.UpdateMobileSelectionCursors(true);
			StartCursor.Type = MobileCursorType.START_CURSOR;
			EndCursor.Type = MobileCursorType.END_CURSOR;
		}

		public void OnCurrentCursorSelected()
		{
			startCaretPosition = TextNavigator.CaretPosition;
		}

		public void OnCurrentCursorMoved(MobileCursor mobileCursor, PointerEventData eventData)
		{
			mobileCursor.OutOfBounds = PositionOutOfBounds(eventData);

			Vector2 localMousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(TextNavigator.TextAreaTransform, eventData.position, eventData.pressEventCamera, out localMousePosition);
			localMousePosition.x += (TextNavigator.TextAreaTransform.rect.width * 0.5f);
			localMousePosition.y -= (TextNavigator.TextAreaTransform.rect.height * 0.5f);
			localMousePosition += mobileCursor.Offset;

			Vector2 anchoredPosition = localMousePosition;
			anchoredPosition -= TextNavigator.TextContentTransform.anchoredPosition;
			mobileCursor.UpdatePosition(anchoredPosition);

			Vector2 cursorPosition;
			TextNavigator.UpdateCurrentCursor(localMousePosition, out cursorPosition);
			mobileCursor.TargetPosition = cursorPosition;
		}

		public void OnCurrentCursorMoveFinished()
		{
			if(TextNavigator.CaretPosition == startCaretPosition)
			{
				TextNavigator.ToggleActionBar();
			}
		}
	}
}
