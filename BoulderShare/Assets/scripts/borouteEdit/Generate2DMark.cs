using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Generate2DMark : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
	private int finger ;
	private const int FINGER_NONE = -10;
	//初期カメラ深さ(10unit)でマークの大きさが1unitになるように設定
	private const float MARK_RATE = 0.1f;
	private Transform target;
	private TwoDMark targetMark;
	private Renderer rend;
	private Renderer renderOverUI;

	[SerializeField]
	private Camera cam;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField] private WallManager wallManager;

	private Action OnGenerateMarkAction = null;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
	}

	public void AddOnGenerateMarkAction(Action action){
		if (action != null){
			OnGenerateMarkAction += action;
		}
	}

	public void ResetOnGenerateMarkAction(){
		OnGenerateMarkAction = null;
	}

	public void OnPointerDown(PointerEventData data){
		if (finger == FINGER_NONE){
			twoDWallMarks.IgnoreFocusAction();
			finger = data.pointerId;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
				data.position.x, 
				data.position.y, 
				-cam.transform.position.z));

			//マークオブジェクトを生成
			GameObject obj = twoDWallMarks.MakeMark();
			obj.transform.position = p;
			obj.transform.localScale = Vector3.one * MARK_RATE * -cam.transform.position.z;
			rend = obj.GetComponent<SpriteRenderer>();
			//uiより前に表示させる
			rend.sortingLayerName = "Mark";
			rend.gameObject.layer = LayerMask.NameToLayer("MarkOverUI");
			//動かすオブジェクトとして登録
			target = obj.transform;
			targetMark = obj.GetComponent<TwoDMark>();

			//描画するカメラを変えるためレイヤーの変更
			renderOverUI = targetMark.GetRenderOverUI();
			renderOverUI.gameObject.layer = LayerMask.NameToLayer("MarkOverUI");
			renderOverUI.sortingLayerName = "Mark";

			targetMark.SetType(TwoDMark.FocusType.NORMAL);
			twoDWallMarks.SetFocus(targetMark);
		}
	}

	public void OnPointerUp(PointerEventData data){
		//Debug.Log("OnpointerUp"+ data.pointerId + " "+ finger);
		if (data.pointerId == finger){
			twoDWallMarks.AcceptFocusAction();
			finger = FINGER_NONE;

			if (twoDWallImage.IsOnPointerEnter()){
				rend.sortingLayerName = "2D";
				rend.gameObject.layer = LayerMask.NameToLayer("2D");
				renderOverUI.gameObject.layer = LayerMask.NameToLayer("2D");
				renderOverUI.sortingLayerName = "2D";
				//bounds
				Vector3 p = target.position;
				//wallの幅とサイズを取得
				Vector2 size = wallManager.GetMasterWallSize();
				float height = size.y;
				float width = size.x;
				//Debug.Log("p:"+p+" , width:"+width +" height:"+ height);
				if (p.x < -width / 2 || p.x > width / 2 || p.y < -height / 2 || p.y > height / 2){
					twoDWallMarks.DeleteMark(target.gameObject.name);
				}else{
					targetMark.SetType(TwoDMark.FocusType.SCALE);
					twoDWallMarks.SetFocus(targetMark);
					//マークの中で一番上に表示する
					target.SetAsFirstSibling();

					if (OnGenerateMarkAction != null){
						OnGenerateMarkAction();
					}
				}
			}else{
				twoDWallMarks.DeleteMark(target.gameObject.name);
			}
			rend = null;
	    	target = null;
		}
	}

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			//Debug.Break();
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
				data.position.x, 
				data.position.y, 
				-cam.transform.position.z));

       	 	target.position = p;
/*
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

        	target.Translate(p - oldP);*/
	    }
	}

}
