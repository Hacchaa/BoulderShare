 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace BoulderNotes{
public class MoveImageController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -100;
	private float prevLength;
	private bool isUpdate = false;
	private const float WEIGHT = 0.2f;
	[SerializeField] private RectTransform boundsRect;
	private const float BOUNDSDELTARATE = 0.3f;
	private const float BOUNDEDZOOM = 0.3f;
    [SerializeField] private RectTransform moveRect;
	[SerializeField] private Image displayImage;
	[SerializeField] private RectTransform displayArea;
	[SerializeField] private Bounds m_ViewBounds;
	[SerializeField] private Bounds m_ContentBounds;
	[SerializeField] private bool m_Dragging;
	[SerializeField] private Vector2 m_PrevPosition;
	[SerializeField] private Vector2 m_Velocity;
	private bool needVec;
	[SerializeField] private float elasticity = 0.1f;
	[SerializeField] private float decelerationRate = 0.1f;
	[SerializeField] private Camera eventCamera;
	[SerializeField] private RectTransform cursor;
	private float zoomRateLimit = 3.0f;
	private Vector2 firstSize; 
	private Vector2 prevCenterPositionWithDoubleFinger;
	private Vector2 prevSizeDelta;
	private bool doneZoomAnim = false;
	[SerializeField] private float animationDuration = 0.3f;
 	public void Init (Sprite sprite) {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};

		FitImage(sprite);
		///Debug.Log("moveRect:"+moveRect.sizeDelta.x + " "+moveRect.sizeDelta.y);
		boundsRect.sizeDelta = moveRect.sizeDelta;
		boundsRect.anchoredPosition = moveRect.anchoredPosition;
		firstSize = moveRect.sizeDelta;

		m_Velocity = Vector2.zero;
	}	

	protected void LateUpdate(){
		UpdateBounds();
		float deltaTime = Time.unscaledDeltaTime;
		Vector2 offset = CalculateOffset(Vector2.zero);
		//Debug.Log("offset:"+offset.x + "," + offset.y);
		if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
		{
			Vector2 position = moveRect.anchoredPosition;
			for (int axis = 0; axis < 2; axis++)
			{
				// Apply spring physics if movement is elastic and content has an offset from the view.
				if (offset[axis] != 0)
				{
					float speed = m_Velocity[axis];
					float smoothTime = elasticity;

					position[axis] = Mathf.SmoothDamp(moveRect.anchoredPosition[axis], moveRect.anchoredPosition[axis] + offset[axis], ref speed, smoothTime, Mathf.Infinity, deltaTime);
					if (Mathf.Abs(speed) < 1)
						speed = 0;
					m_Velocity[axis] = speed;
				}
				// Else move content according to velocity with deceleration applied.
				else
				{
					m_Velocity[axis] *= Mathf.Pow(decelerationRate, deltaTime);
					if (Mathf.Abs(m_Velocity[axis]) < 1)
						m_Velocity[axis] = 0;
					position[axis] += m_Velocity[axis] * deltaTime;
				}
			}
			SetContentAnchoredPosition(position);
		}

		if (m_Dragging)
		{
			if (needVec){
				Vector3 newVelocity = (moveRect.anchoredPosition - m_PrevPosition) / deltaTime;
				m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
			}else{
				m_Velocity = Vector2.zero;
			}
		}
		if (doneZoomAnim){
			m_Velocity = Vector2.zero;
		}

		if (moveRect.anchoredPosition != m_PrevPosition)
		{
			UpdatePrevData();
		}

		isUpdate = false;
	}
	private void SetContentAnchoredPosition(Vector2 position)
	{
		if (position != moveRect.anchoredPosition)
		{
			moveRect.anchoredPosition = position;
			UpdateBounds();
		}
	}
	private void UpdateBounds(){
		m_ViewBounds = new Bounds(boundsRect.rect.center, boundsRect.rect.size);
		m_ContentBounds = GetBounds();
	}
	private void UpdatePrevData()
	{
		if (moveRect == null)
			m_PrevPosition = Vector2.zero;
		else
			m_PrevPosition = moveRect.anchoredPosition;

	}
	private Vector2 CalculateOffset(Vector2 delta)
	{
		Vector2 offset = Vector2.zero;

		Vector2 min = m_ContentBounds.min;
		Vector2 max = m_ContentBounds.max;

		// min/max offset extracted to check if approximately 0 and avoid recalculating layout every frame (case 1010178)
		min.x += delta.x;
		max.x += delta.x;

		float maxOffset = m_ViewBounds.max.x - max.x;
		float minOffset = m_ViewBounds.min.x - min.x;

		if (m_ViewBounds.extents.x > m_ContentBounds.extents.x)
			offset.x = -moveRect.anchoredPosition.x;
		else if (minOffset < -0.001f)
			offset.x = minOffset;
		else if (maxOffset > 0.001f)
			offset.x = maxOffset;


		min.y += delta.y;
		max.y += delta.y;

		maxOffset = m_ViewBounds.max.y - max.y;
		minOffset = m_ViewBounds.min.y - min.y;

		if (m_ViewBounds.extents.y > m_ContentBounds.extents.y)
			offset.y = -moveRect.anchoredPosition.y;
		else if (maxOffset > 0.001f)
			offset.y = maxOffset;
		else if (minOffset < -0.001f)
			offset.y = minOffset;

		return offset;
	}	

	private readonly Vector3[] m_Corners = new Vector3[4];
	private Bounds GetBounds()
	{
		if (moveRect == null)
			return new Bounds();
		moveRect.GetWorldCorners(m_Corners);
		var viewWorldToLocalMatrix = boundsRect.worldToLocalMatrix;
		return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
	}

	internal static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
	{
		var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

		for (int j = 0; j < 4; j++)
		{
			Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
			vMin = Vector3.Min(v, vMin);
			vMax = Vector3.Max(v, vMax);
		}

		var bounds = new Bounds(vMin, Vector3.zero);
		bounds.Encapsulate(vMax);
		return bounds;
	}
	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(!doneZoomAnim && eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
			prevCenterPositionWithDoubleFinger = Vector2.zero;
		}
	}
    public void OnBeginDrag(PointerEventData data){
		m_Dragging = true;
    }

	public void OnDrag(PointerEventData data){
		//Debug.Log("drag "+data.pointerId);
        Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;
		//data.pointerIdが現在扱っている指かどうか
		if (isUpdate || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}       
		//Debug.Log("pass");
		p1 = data.position;
		dP1 = data.delta;
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
				dP2 = touch.deltaPosition;
			}
		} 

        float length = Vector2.Distance(p1, p2);


        //一本指の場合
        if (eTouches[1] == FINGER_NONE){
			Vector2 del = dP1 / CanvasResolutionManager.Instance.GetRatioOfPtToPx();

			moveRect.anchoredPosition += CalcBoundsDelta(del);
            Vector2 v;
        	RectTransformUtility.ScreenPointToLocalPointInRectangle(displayArea, data.position, data.pressEventCamera, out v);
			//Debug.Log("localPos"+v.x + ","+v.y);
            isUpdate = true;
			needVec = true;
			cursor.anchoredPosition = v;
			prevCenterPositionWithDoubleFinger = v;
            return ;
        }

		//拡大縮小
        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = p1 - dP1;
        Vector2 touchOnePrevPos = p2 - dP2;
        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (p1 - p2).magnitude;
        // Find the difference in the distances between each frame.
        float rate = touchDeltaMag / prevTouchDeltaMag;
		Vector2 old = new Vector2((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f);
        Vector2 cur = new Vector2((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f);
		ZoomAt(rate, cur, data);

		//移動
        Vector2 diff = (cur - old) / CanvasResolutionManager.Instance.GetRatioOfPtToPx();;
        moveRect.anchoredPosition += CalcBoundsDelta(diff);

		isUpdate = true;
		needVec = false;
	}
	public void ZoomAt(float rate, Vector2 screenPosition, PointerEventData data){
		//スクリーン座標からmoveRectのローカル座標に変換
		Vector2 center;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(displayArea, screenPosition, data.pressEventCamera, out center);
		prevCenterPositionWithDoubleFinger = center;
		cursor.anchoredPosition = center;
		if (moveRect.sizeDelta.x >= firstSize.x && moveRect.sizeDelta.x <= firstSize.x * zoomRateLimit){
			rate = (rate - 1f)*BOUNDEDZOOM + 1f;
		}
		moveRect.sizeDelta *= rate;
		CalculateBoundsArea(moveRect.sizeDelta);
		moveRect.anchoredPosition -= ((center-moveRect.anchoredPosition) * (rate - 1f));
	}

	public void ZoomButton(){
		float rate = 1.05f;
		//Debug.Log("diff "+(moveRect.sizeDelta*(rate-1f)));
		//Debug.Log("sizeDelta before:"+moveRect.sizeDelta.x + ", "+moveRect.sizeDelta.y);
		//Debug.Log("anchoredPosition before:"+moveRect.anchoredPosition.x + ", "+moveRect.anchoredPosition.y);
		moveRect.sizeDelta *= rate;
		CalculateBoundsArea(moveRect.sizeDelta);
		moveRect.anchoredPosition -= ((cursor.anchoredPosition-moveRect.anchoredPosition) * (rate - 1f));	
		//Debug.Log("sizeDelta after :"+moveRect.sizeDelta.x + ", "+moveRect.sizeDelta.y);
		//Debug.Log("anchoredPosition after :"+moveRect.anchoredPosition.x + ", "+moveRect.anchoredPosition.y);	
	}

	public void ShrinkButton(){
		float rate = 0.95f;
		//Debug.Log("diff "+(moveRect.sizeDelta*(rate-1f)));
		moveRect.sizeDelta *= rate;
		CalculateBoundsArea(moveRect.sizeDelta);
		moveRect.anchoredPosition -= ((cursor.anchoredPosition-moveRect.anchoredPosition) * (rate - 1f));	
	}
	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;
            }
			m_Dragging = false;
			//BoundSizeWithAnim();
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
			BoundSizeWithAnim();
			//m_Dragging = false;
		}
    }

	public void CalculateBoundsArea(Vector2 size){
		float x, y;
		x = Mathf.Min(size.x, displayArea.rect.width);
		y = Mathf.Min(size.y, displayArea.rect.height);
		boundsRect.sizeDelta = new Vector2(x, y);
	}

	public Vector2 CalcBoundsDelta(Vector2 delta){
		float x, y;
		if (IsOutBoundX()){
			//Debug.Log("boudsX");
			x = delta.x * BOUNDSDELTARATE;
		}else{
			x = delta.x;
		}

		if (IsOutBoundY()){
			//Debug.Log("BoundsY");
			y = delta.y * BOUNDSDELTARATE;
		}else{
			y = delta.y;
		}
		//Debug.Log("delta"+delta.x +" "+delta.y);
		//Debug.Log("newDelta"+x+" "+y);
		return new Vector2(x, y);
		
	}

	public bool IsOutBoundX(){
		//左側
		//Debug.Log("boundsRect.rect.x < (moveRect.anchoredPosition.x - moveRect.rect.width/2f) " + boundsRect.rect.x + " < "+(moveRect.anchoredPosition.x - moveRect.rect.width/2f));
		if (boundsRect.rect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			return true;
		}
		//右側
		if (-boundsRect.rect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			return true;
		}

		return false;
	}
	public bool IsOutBoundY(){
		//Debug.Log("-boundsRect.rect.y > (moveRect.anchoredPosition.y + moveRect.rect.height/2f) " + -boundsRect.rect.y + " > "+(moveRect.anchoredPosition.y + moveRect.rect.height/2f));
		//上側
		if (-boundsRect.rect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			return true;
		}
		//下側
		if (boundsRect.rect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			return true;
		}

		return false;
	}

	public void Bounds(){
		float x = 0;
		float y = 0;

		if (boundsRect.rect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			x = boundsRect.rect.x - (moveRect.anchoredPosition.x - moveRect.rect.width/2f);
		}else if (-boundsRect.rect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			x = -boundsRect.rect.x - (moveRect.anchoredPosition.x + moveRect.rect.width/2f);
		}		

		if (-boundsRect.rect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			y = -boundsRect.rect.y - (moveRect.anchoredPosition.y + moveRect.rect.height/2f);
		}else if (boundsRect.rect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			y = boundsRect.rect.y - (moveRect.anchoredPosition.y - moveRect.rect.height/2f);
		}

		moveRect.anchoredPosition += new Vector2(x, y);
	}

	private void FitImage(Sprite spr){
        float fitHeight = displayArea.rect.height;
        float fitWidth = displayArea.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h, r;

        if (fitHeight / fitWidth < texHeight / texWidth){
            r = fitHeight / texHeight; 
            h = fitHeight;
            w = texWidth * r; 
        }else{
            r = fitWidth / texWidth; 
            w = fitWidth;
            h = texHeight * r;   
        }  

        moveRect.anchorMin = new Vector2(0.5f, 0.5f);
        moveRect.anchorMax = new Vector2(0.5f, 0.5f);
        moveRect.sizeDelta = new Vector2(w, h);
		moveRect.anchoredPosition = Vector2.zero;

        displayImage.sprite = spr;
    }

	private void BoundSizeWithAnim(){
		if (moveRect.sizeDelta.x >= firstSize.x && moveRect.sizeDelta.x <= firstSize.x*zoomRateLimit){
			return ;
		}
		if (moveRect.sizeDelta.y >= firstSize.y && moveRect.sizeDelta.y <= firstSize.y*zoomRateLimit){
			return ;
		}
		Sequence seq = DOTween.Sequence();
		Vector2 targetSize;
		targetSize = Vector2.Max(firstSize, moveRect.sizeDelta);
		targetSize = Vector2.Min(firstSize*zoomRateLimit, targetSize);
		moveRect.DOSizeDelta(targetSize, animationDuration)
		.OnStart(()=>{
			Debug.Log("anim start");
			//二本指操作の禁止
			doneZoomAnim = true;
			CalculateBoundsArea(targetSize);
			prevSizeDelta = moveRect.sizeDelta;
		})
		.OnUpdate(()=>{
			Debug.Log("sizeDelta:"+moveRect.sizeDelta.x + ", "+moveRect.sizeDelta.y);
			Debug.Log("anchoredPosition before:"+moveRect.anchoredPosition.x + ", "+moveRect.anchoredPosition.y);
			Vector2 p = (prevCenterPositionWithDoubleFinger-moveRect.anchoredPosition);
			Debug.Log("center - anchore:"+p.x + ","+p.y);
			float rate = moveRect.sizeDelta.x / prevSizeDelta.x;
			Debug.Log("rate:"+rate);
			moveRect.anchoredPosition -= ((prevCenterPositionWithDoubleFinger-moveRect.anchoredPosition) * (rate - 1f));
			prevSizeDelta = moveRect.sizeDelta;
			Debug.Log("anchoredPosition after:"+moveRect.anchoredPosition.x + ", "+moveRect.anchoredPosition.y);
			Debug.Log("");
		})
		.OnComplete(()=>{
			//二本指操作の禁止解除
			Debug.Log("anim end");
			doneZoomAnim = false;
		});
	}
}
}