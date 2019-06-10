using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HFController : MonoBehaviour{
	private float arrowDWidth;
	private TwoDMark selectedMark;
	private TwoDMark.HFType selectedBodyInfo;
	private const float HEIGHT_MIN = 0.65f;

	[SerializeField]
	private SpriteRenderer arrow;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField]
	private Camera cam;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;

	private Action OnTouchMarkAction = null;

	void Awake(){
		arrowDWidth = arrow.size.x;
		selectedMark = null;
		selectedBodyInfo = TwoDMark.HFType.NONE;
	}

	public void AddOnTouchMarkAction(Action action){
		if (action != null){
			OnTouchMarkAction += action;
		}
	}

	public void ResetOnTouchMarkAction(){
		OnTouchMarkAction = null;
	}

	public void SetBodyType(TwoDMark.HFType type){
		selectedBodyInfo = type;
	}

	public void AdjustArrowScale(){
		arrow.transform.localScale = Vector3.one * -cam.transform.position.z * 0.1f;
	}

	public void Drag(PointerEventData data){
		//マークが存在しないなら何もしない
		if(!twoDWallMarks.IsMarkExist()){
			return ;
		}

		if (twoDWallImage.IsOnPointerEnter()){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
				data.position.x, 
				data.position.y, 
				-cam.transform.position.z));
			//タッチ座標に最も近いマークを探す
			selectedMark = twoDWallMarks.CalcNearestMark(new Vector2(p.x, p.y));
			Vector3 target = selectedMark.transform.position;

			twoDWallMarks.SetFocus(selectedMark);

			//タッチした座標からtargetの座標に向かうベクトルの取得
			Vector3 dir = target - p ;
			float mag = dir.magnitude;
			if (mag / arrow.transform.localScale.x < HEIGHT_MIN){
				Vector2 tmp = new Vector2(-dir.x, -dir.y).normalized * HEIGHT_MIN * arrow.transform.localScale.x / 2;
				arrow.gameObject.transform.position = new Vector3(target.x + tmp.x, target.y + tmp.y , p.z);
				mag = HEIGHT_MIN ;
			}else{
				//矢印の座標をタッチ座標にする
				arrow.gameObject.transform.position = new Vector3(p.x + dir.x/2, p.y + dir.y/2, p.z);
				mag /= arrow.transform.localScale.x;
			}

			//矢印をtargetに向かせる
			float ang = Vector2.Angle(new Vector2(dir.x, dir.y), Vector2.up);
			if (dir.x > 0){
				ang *= -1;
			}
			arrow.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, ang);

			//矢印の長さを調節する
			arrow.size =  new Vector2(arrowDWidth, mag);

			arrow.gameObject.SetActive(true);
		}else{
			arrow.gameObject.SetActive(false);
			twoDWallMarks.ReleaseFocus();
			selectedMark = null;
		}
	}

	public void EndDrag(){
		//マークが存在しないなら何もしない
		if(!twoDWallMarks.IsMarkExist()){
			return ;
		}
		if(twoDWallImage.IsOnPointerEnter() && selectedMark != null){
			twoDWallMarks.Touch(selectedMark, selectedBodyInfo);
			if (OnTouchMarkAction != null){
				OnTouchMarkAction();
			}
			arrow.gameObject.SetActive(false);
			twoDWallMarks.ReleaseFocus();
			selectedMark = null;
			selectedBodyInfo = TwoDMark.HFType.NONE;
		}
	}
}
