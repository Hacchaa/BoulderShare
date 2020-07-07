 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

namespace BoulderNotes{
public class MoveImageController : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IInitializePotentialDragHandler{
	[SerializeField] private DisplayImageControllerManager manager;
	[SerializeField] private RectTransform boundsRect;
	[SerializeField] private RectTransform dummyRect;
	[SerializeField] private RectTransform cursor;
	[SerializeField] private float animationDuration = 0.3f;
	[SerializeField] private float elasticity = 0.1f;
	[SerializeField] private float decelerationRate = 0.1f;
	private RectTransform moveRect;
	private RectTransform displayArea;
	private const float BOUNDSDELTARATE = 0.3f;
	private const float BOUNDEDZOOM = 0.3f;
	private float zoomRateLimit = 4.5f;
	private bool isAlreadyUpdate = false;
	private bool doneZoomAnim = false;
	private bool needVelocity;
	private bool m_Dragging;
	private Bounds m_ViewBounds;
	private Bounds m_ContentBounds;
	private Vector2 m_PrevPosition;
	private Vector2 m_Velocity;
	private Vector2 firstSize; 
	private Vector2 prevCenterPositionWithDoubleFinger;
	private Vector2 prevSizeDelta;
	[SerializeField] private DisplayImage displayImage;
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void BoulderNotes_Audio(int n);
	#endif
 	public void Init (DisplayImage dImage) {
		displayImage = dImage;
		moveRect = dImage.GetImageRect();
		displayArea = dImage.GetViewRect();

		CalculateBoundsArea(moveRect.sizeDelta);
		firstSize = dImage.GetSize();

		m_Velocity = Vector2.zero;
	}	

	public bool IsAlreadyMoved(){
		return displayImage.IsAlreadyMoved();
	}
	public bool IsAttemptingToGoOver(Vector2 dir){

		if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y)){
			return false;
		}

		//右端
		if (displayImage.IsInTheRightSide()){
			//Debug.Log("right");
			if (dir.x < 0f){
				return true;
			}
		}
		//左端
		if (displayImage.IsInTheLeftSide()){
			//Debug.Log("left");
			if (dir.x > 0f){
				return true;
			}
		}

		return false;
	}

	public void OnInitializePotentialDrag(PointerEventData data){
        
    }

    public void OnBeginDrag(PointerEventData data){
		m_Dragging = true;
		manager.DisableScroller();
    }

	public void OnDrag(PointerEventData data){
		//Debug.Log("drag "+data.pointerId);
        Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;

		int firstTouch = manager.GetFinger(0);
		int secondTouch = manager.GetFinger(1);
		//data.pointerIdが現在扱っている指かどうか
		if (isAlreadyUpdate || (data.pointerId != firstTouch && data.pointerId != secondTouch)){
			return ;
		}       
		//Debug.Log("pass");
		p1 = data.position;
		dP1 = data.delta;
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == firstTouch){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == secondTouch){
				p2 = touch.position;
				dP2 = touch.deltaPosition;
			}
		} 

        float length = Vector2.Distance(p1, p2);

        //一本指の場合
        if (!manager.HasFinger(1)){
			Vector2 del = dP1 / CanvasResolutionManager.Instance.GetRatioOfPtToPx();
			Vector2 prev;
			del = CalcBoundsDelta(del);

			if (displayImage.IsInTheRightSide()){
				moveRect.anchoredPosition += new Vector2(0f, del.y);
				manager.AddScrollPositionX(del.x);
				del = manager.GetScrollContentPosition();
				if (del.x > 0f){
					moveRect.anchoredPosition += new Vector2(del.x, 0f);
					manager.ResetScrollPosition();
				}
			}else if(displayImage.IsInTheLeftSide()){
				moveRect.anchoredPosition += new Vector2(0f, del.y);
				manager.AddScrollPositionX(del.x);
				del = manager.GetScrollContentPosition();
				if (del.x < 0f){
					moveRect.anchoredPosition += new Vector2(del.x, 0f);
					manager.ResetScrollPosition();
				}
			}else{
				prev = moveRect.anchoredPosition;
				moveRect.anchoredPosition += del;
				if(displayImage.IsInTheRightSide()){
					displayImage.ClampOnRight();
					manager.AddScrollPositionX((prev + del - moveRect.anchoredPosition).x);
				}else if(displayImage.IsInTheLeftSide()){
					displayImage.ClampOnLeft();
					manager.AddScrollPositionX((prev + del - moveRect.anchoredPosition).x);
				}
			}

            Vector2 v;
        	RectTransformUtility.ScreenPointToLocalPointInRectangle(displayArea, data.position, data.pressEventCamera, out v);
			v += new Vector2((displayArea.pivot.x - 0.5f) * displayArea.rect.width, (displayArea.pivot.y - 0.5f) * displayArea.rect.height);
            isAlreadyUpdate = true;
			needVelocity = true;
			cursor.anchoredPosition = v;
			prevCenterPositionWithDoubleFinger = v;
            return ;
        }

		if (doneZoomAnim){
			return ;
		}

		//manager.ResetScrollPosition();

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

		isAlreadyUpdate = true;
		needVelocity = false;
	}
	public void OnEndDrag(PointerEventData data){
		if (!manager.HasFinger(0)){
			m_Dragging = false;
			manager.EnableScroller();
		}
		BoundSizeWithAnim();
    }
	public void OnLateUpdate(){
		if (doneZoomAnim){
			return ;
		}
		UpdateBounds();
		float deltaTime = Time.unscaledDeltaTime;
		Vector2 offset = CalculateOffset(Vector2.zero);
		//Debug.Log("offset:"+offset.x + "," + offset.y);
		if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
		{
			Vector2 position = moveRect.anchoredPosition;
			//Debug.Log("position Before:"+position.x);
			for (int axis = 0; axis < 2; axis++)
			{
				// Apply spring physics if movement is elastic and content has an offset from the view.
				if (offset[axis] != 0)
				{
					float speed = m_Velocity[axis];
					if (axis == 0){
						//外側のscrollRectに速度を渡す
						manager.AddScrollVelocityX(speed);
						m_Velocity[axis] = 0f;
						float diff ;
						if (offset[axis] < 0f){
							//Debug.Log("clampOnLeft");
							diff = displayImage.ClampOnLeft();
						}else{
							//Debug.Log("clampOnright");
							diff = displayImage.ClampOnRight();
						}
						//Debug.Log("diff:"+diff);
						manager.AddScrollPositionX(-diff);
						position[axis]=moveRect.anchoredPosition.x;
						//Debug.Log("position.x:"+position.x);
					}else{
						float smoothTime = elasticity;

						position[axis] = Mathf.SmoothDamp(moveRect.anchoredPosition[axis], moveRect.anchoredPosition[axis] + offset[axis], ref speed, smoothTime, Mathf.Infinity, deltaTime);
						if (Mathf.Abs(speed) < 1)
							speed = 0;
						m_Velocity[axis] = speed;
					}
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
			if (needVelocity){
				Vector3 newVelocity = (moveRect.anchoredPosition - m_PrevPosition) / deltaTime;
				m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
			}else{
				m_Velocity = Vector2.zero;
			}
		}

		if (moveRect.anchoredPosition != m_PrevPosition)
		{
			UpdatePrevData();
		}

		isAlreadyUpdate = false;
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
		//boundsRectのローカル座標からみたmoveRectのBounds
		m_ContentBounds = GetBounds();
	}
	private void UpdatePrevData()
	{
		if (moveRect == null)
			m_PrevPosition = Vector2.zero;
		else
			m_PrevPosition = moveRect.anchoredPosition;

	}
	private Vector2 CalculateOffset(Vector2 delta){
		return InnerCalculateOffset(ref m_ContentBounds, ref m_ViewBounds, moveRect, delta);
	}
	private Vector2 InnerCalculateOffset(ref Bounds contentBounds, ref Bounds viewBounds, RectTransform contentRect, Vector2 delta, bool debug = false)
	{
		Vector2 offset = Vector2.zero;

		Vector2 min = contentBounds.min;
		Vector2 max = contentBounds.max;

		// min/max offset extracted to check if approximately 0 and avoid recalculating layout every frame (case 1010178)
		min.x += delta.x;
		max.x += delta.x;

		float maxOffset = viewBounds.max.x - max.x;
		float minOffset = viewBounds.min.x - min.x;
		if (debug){
			Debug.Log("contentBounds.extents :"+contentBounds.extents.x +","+contentBounds.extents.y);
			Debug.Log("viewBounds.extents :"+viewBounds.extents.x +","+viewBounds.extents.y);			
		}

		//contentBoundsがviewBoundsより小さい場合は上手く計算できないので、contentBoundsの中央までの距離をoffsetにする
		if (viewBounds.extents.x > contentBounds.extents.x)
			offset.x = -contentRect.anchoredPosition.x;
		else if (minOffset < -0.001f)
			offset.x = minOffset;
		else if (maxOffset > 0.001f)
			offset.x = maxOffset;


		min.y += delta.y;
		max.y += delta.y;

		maxOffset = viewBounds.max.y - max.y;
		minOffset = viewBounds.min.y - min.y;

		if (m_ViewBounds.extents.y > contentBounds.extents.y)
			offset.y = -contentRect.anchoredPosition.y;
		else if (maxOffset > 0.001f)
			offset.y = maxOffset;
		else if (minOffset < -0.001f)
			offset.y = minOffset;

		return offset;
	}	

	private readonly Vector3[] m_Corners = new Vector3[4];
	private Bounds GetBounds()
	{
		return GetBounds(moveRect, boundsRect);
	}
	private Bounds GetBounds(RectTransform contentRect, RectTransform viewRect){
		if (contentRect == null)
			return new Bounds();
		contentRect.GetWorldCorners(m_Corners);
		var viewWorldToLocalMatrix = viewRect.worldToLocalMatrix;
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
	
	public void ZoomAt(float rate, Vector2 screenPosition, PointerEventData data){
		//スクリーン座標からmoveRectのローカル座標に変換
		Vector2 center;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(displayArea, screenPosition, data.pressEventCamera, out center);
		center += new Vector2((displayArea.pivot.x - 0.5f) * displayArea.rect.width, (displayArea.pivot.y - 0.5f) * displayArea.rect.height);
		prevCenterPositionWithDoubleFinger = center;
		cursor.anchoredPosition = center;
		if (moveRect.sizeDelta.x < firstSize.x || moveRect.sizeDelta.x > firstSize.x * zoomRateLimit){
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

	public void CalculateBoundsArea(Vector2 size, bool consideredMimBouonds = false, bool debug = false){
		float x, y;

		x = Mathf.Min(size.x, displayArea.rect.width);
		y = Mathf.Min(size.y, displayArea.rect.height);

		if (consideredMimBouonds){
			x = Mathf.Max(x, firstSize.x);
			y = Mathf.Max(y, firstSize.y);
		}
		if (debug){
			Debug.Log("size:"+size.x + ","+size.y);
			Debug.Log("displayArea:"+displayArea.rect.width + "," +displayArea.rect.height);
			Debug.Log("firstSize:"+firstSize.x + ","+firstSize.y);
			Debug.Log("dist:"+x+","+y);
		}		
		boundsRect.sizeDelta = new Vector2(x, y);
	}

	//表示範囲の外側に移動させるときは移動量を少なくする
	public Vector2 CalcBoundsDelta(Vector2 delta){
		float x, y;
		if (IsOutBoundX()){
			x = delta.x * BOUNDSDELTARATE;
		}else{
			x = delta.x;
		}

		if (IsOutBoundY()){
			y = delta.y * BOUNDSDELTARATE;
		}else{
			y = delta.y;
		}
		return new Vector2(x, y);
	}


	public bool IsOutBoundX(){
		//左側 rect.xとrect.y は中心から見た左上の座標
		//Debug.Log("boundsRect.rect.x < (moveRect.anchoredPosition.x - moveRect.rect.width/2f) " + boundsRect.rect.x + " < "+(moveRect.anchoredPosition.x - moveRect.rect.width/2f));
		if (boundsRect.rect.x <= moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			return true;
		}
		//右側
		if (-boundsRect.rect.x >= moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			return true;
		}

		return false;
	}
	public bool IsOutBoundY(){
		//Debug.Log("-boundsRect.rect.y > (moveRect.anchoredPosition.y + moveRect.rect.height/2f) " + -boundsRect.rect.y + " > "+(moveRect.anchoredPosition.y + moveRect.rect.height/2f));
		//上側
		if (-boundsRect.rect.y >= moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			return true;
		}
		//下側
		if (boundsRect.rect.y <= moveRect.anchoredPosition.y - moveRect.rect.height/2f){
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


	private void BoundSizeWithAnim(){
		//Debug.Log("boundssizewithanim");
		if (doneZoomAnim){
			return ;
		}
		if (moveRect.sizeDelta.x >= firstSize.x && moveRect.sizeDelta.x <= firstSize.x*zoomRateLimit){
			return ;
		}
		if (moveRect.sizeDelta.y >= firstSize.y && moveRect.sizeDelta.y <= firstSize.y*zoomRateLimit){
			return ;
		}
		doneZoomAnim = true;
		Vector2 targetSize;
		targetSize = Vector2.Max(firstSize, moveRect.sizeDelta);
		targetSize = Vector2.Min(firstSize*zoomRateLimit, targetSize);
		CalculateBoundsArea(targetSize, true, false);
		UpdateBounds();
		//移動量を求める
		float r = targetSize.x / moveRect.sizeDelta.x;
		//アニメーション後のmoveRectを計算するためにdummyRectを使う
		dummyRect.anchoredPosition = moveRect.anchoredPosition;
		dummyRect.sizeDelta = moveRect.sizeDelta*r;
		dummyRect.anchoredPosition -= ((prevCenterPositionWithDoubleFinger-moveRect.anchoredPosition) * (r - 1f));
		//Debug.Log("dummy anchore:"+dummyRect.anchoredPosition.x + ","+ dummyRect.anchoredPosition.y);
		//Debug.Log("dummy sizedelta:"+dummyRect.sizeDelta.x + ","+ dummyRect.sizeDelta.y);
		Bounds b = GetBounds(dummyRect, boundsRect);
		Vector2 offset = InnerCalculateOffset(ref b, ref m_ViewBounds, dummyRect, Vector2.zero, false);
		//Debug.Log("offset "+offset.x+","+offset.y);
		dummyRect.anchoredPosition += offset;

		Sequence seq = DOTween.Sequence();
		seq.OnStart(()=>{
			//Debug.Log("anim start");
			//二本指操作の禁止　この処理を行う時は指を離したときで、多くても一本の指でしか操作していないことを仮定している
			doneZoomAnim = true;
			prevSizeDelta = moveRect.sizeDelta;

			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer){　
					BoulderNotes_Audio(1519);
				}
            #endif
		})
		.Append(moveRect.DOSizeDelta(targetSize, animationDuration))
		.Join(moveRect.DOAnchorPos(dummyRect.anchoredPosition, animationDuration))
		.OnComplete(()=>{
			//二本指操作の禁止解除
			//Debug.Log("anim end");
			doneZoomAnim = false;
		});

		seq.Play();
	}
}
}