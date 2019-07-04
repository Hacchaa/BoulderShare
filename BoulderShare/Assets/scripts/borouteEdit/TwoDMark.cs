using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TwoDMark : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {
	public enum HFType {NONE = -1, RH, LH, RF, LF};
	public enum FocusType {NORMAL, SCALE};
	[SerializeField] private Transform touchInfo;
	[SerializeField] private Transform dummy;
	[SerializeField] private GameObject noActiveObj;
	private static int finger = FINGER_NONE;
	private const int FINGER_NONE = -10;
	private static float SCALE_MIN = 0.12f;
	private static float SCALE_MAX = 1.5f;
	private static Action OnBeginDragAction;
	private static Action OnPointerUpAction;
	private static Action OnPointerDownAction;

	[SerializeField] private SpriteRenderer rend;
	[SerializeField] private Camera cam;
	[SerializeField] private Transform anotherCam;
	[SerializeField] private TwoDWall twoDWall;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private TwoDWallImage twoDWallImage;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDMarkScale markScale;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private float defaultR = 0.5f;
	private bool isDragging = false;
	private FocusType type = FocusType.NORMAL;

	public SpriteRenderer GetRenderOverUI(){
		return rend;
	}

	public void SetType(FocusType t){
		type = t;
	}

	public void Clear(){
		foreach(Transform t in touchInfo){
			t.gameObject.SetActive(false);
		}
		foreach(Transform t in dummy){
			t.gameObject.SetActive(false);
		}
	}
	public static void AddOnPointerUpAction(Action a){
		if (a != null){
			OnPointerUpAction += a;
		}
	}

	public static void AddOnBeginDragAction(Action a){
		if (a != null){
			OnBeginDragAction += a;
		}
	}

	public static void AddOnPointerDownAction(Action a){
		if (a != null){
			OnPointerDownAction += a;
		}
	}

	public static void ResetOnPointerUpAction(){
		OnPointerUpAction = null;
	}

	public static void ResetOnBeginDragAction(){
		OnBeginDragAction = null;
	}

	public static void ResetOnPointerDownAction(){
		OnPointerDownAction = null;
	}

	public void ActivateNoActiveObj(bool b){
		noActiveObj.SetActive(b);
	}
/*
	public void SetScale(float r){
		r = Mathf.Min(r, SCALE_MAX);
		r = Mathf.Max(r, SCALE_MIN);
		transform.localScale = Vector3.one * r;
	}*/

	public void SetTouchInfo(HFType bodyType, bool isTouched){
		touchInfo.GetChild((int)bodyType).gameObject.SetActive(isTouched);
	}
	public void SetDummyTouchInfo(HFType bodyType, bool isTouched){
		dummy.GetChild((int)bodyType).gameObject.SetActive(isTouched);
	}

	public void OnPointerUp(PointerEventData data){
		//Debug.Log("OnPointerUp");
		type = FocusType.SCALE;
		twoDWallMarks.SetFocus(this);	
		if (OnPointerUpAction != null){
			OnPointerUpAction();
		}

		if (isDragging){
			isDragging = false;

			if (data.pointerId == finger){
				finger = FINGER_NONE;

				Vector3 p = transform.position;
				/*
				if (!twoDWallImage.IsOnPointerEnter()){
					Vector2 off = twoDWallImage.GetOffTouchPos();
					p = cam.ScreenToWorldPoint(
					new Vector3(
						off.x, 
						off.y, 
						-cam.transform.position.z));
				}else{
					p = transform.position;
				}
				Debug.Log("p:"+p);*/


				//bounds
				//wallの幅とサイズを取得
				//高さは4units
				Vector2 size = wallManager.GetMasterWallSize();
				float height = size.y;
				float width = size.x;
				p.x = Mathf.Min(p.x, width/2);
		    	p.x = Mathf.Max(p.x, -width/2);
		    	p.y = Mathf.Min(p.y, height/2);
		    	p.y = Mathf.Max(p.y, -height/2);

		    	transform.position = p;
		    	//Debug.Log("After p:"+p);
		    	//Debug.Log("size:"+size);
		    	//描画順を元に戻す
				rend.sortingLayerName = "Default";
				rend.gameObject.layer = LayerMask.NameToLayer("2D");
			}			
		}
	}

	public void OnPointerDown(PointerEventData data){
		//Debug.Log("OnPointerDown");
		//フォーカスをあてる
		type = FocusType.NORMAL;
		twoDWallMarks.SetFocus(this);

		//cManager.LookAt2D(transform);

		//マークの中で一番上に表示する
		transform.SetAsFirstSibling();
		if (OnPointerDownAction != null){
			OnPointerDownAction();
		}
	}

	public float GetR(){
		return defaultR;
	}

	public void Focus(){
		rend.gameObject.SetActive(true);
		if (type == FocusType.SCALE){
			markScale.gameObject.SetActive(true);
			markScale.FixScale();
		}
	}

	public void ReleaseFocus(){
		rend.gameObject.SetActive(false);
		markScale.gameObject.SetActive(false);
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
			isDragging = true;

			//uiより前に表示させる
			rend.sortingLayerName = "Mark";
			rend.gameObject.layer = LayerMask.NameToLayer("MarkOverUI");
			anotherCam.position = cam.transform.position;

			if (OnBeginDragAction != null){
				OnBeginDragAction();
			}
		}
	}

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z));
			
			Vector3 oldP = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					-cam.transform.position.z));

        	transform.Translate(p - oldP);
		}
	}
}
