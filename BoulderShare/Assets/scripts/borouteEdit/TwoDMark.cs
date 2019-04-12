using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TwoDMark : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
	public enum HFType {NONE = -1, RH, LH, RF, LF};
	[SerializeField] private Transform touchInfo;
	[SerializeField] private Transform dummy;
	private static int finger ;
	private const int FINGER_NONE = -10;
	private static float SCALE_MIN = 0.12f;
	private static float SCALE_MAX = 1.5f;
	private static float BASEALPHA = 80.0f / 255.0f;
	private static float FOCUSALPHA = 200.0f / 255.0f;
	[SerializeField]
	private SpriteRenderer rend;
	[SerializeField]
	private Camera cam;
	[SerializeField] private Transform anotherCam;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField] private WallManager wallManager;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
	}

	public void Clear(){
		foreach(Transform t in touchInfo){
			t.gameObject.SetActive(false);
		}
		foreach(Transform t in dummy){
			t.gameObject.SetActive(false);
		}
	}

	public void SetScale(float r){
		r = Mathf.Min(r, SCALE_MAX);
		r = Mathf.Max(r, SCALE_MIN);
		transform.localScale = Vector3.one * r;
	}

	public void SetTouchInfo(HFType bodyType, bool isTouched){
		touchInfo.GetChild((int)bodyType).gameObject.SetActive(isTouched);
	}
	public void SetDummyTouchInfo(HFType bodyType, bool isTouched){
		dummy.GetChild((int)bodyType).gameObject.SetActive(isTouched);
	}

	public void OnPointerUp(PointerEventData data){
		finger = FINGER_NONE;
	}

	public void OnPointerDown(PointerEventData data){
		//フォーカスをあてる
		twoDWallMarks.SetFocus(this);

		//マークの中で一番上に表示する
		transform.SetAsFirstSibling();
	}

	public void Focus(bool b){
		Color c = rend.color;
		if(b){
			c.a = FOCUSALPHA;
		}else{
			c.a = BASEALPHA;
		}
		rend.color = c;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;

			//uiより前に表示させる
			rend.sortingLayerName = "Mark";
			gameObject.layer = LayerMask.NameToLayer("UI");
			anotherCam.position = cam.transform.position;

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

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;

			Vector3 p ;
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

	    	//描画順を元に戻す
			rend.sortingLayerName = "Default";
			gameObject.layer = LayerMask.NameToLayer("2D");
		}
	}
}
