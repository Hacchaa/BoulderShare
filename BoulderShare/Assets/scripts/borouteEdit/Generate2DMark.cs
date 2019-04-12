using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Generate2DMark : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
	private int finger ;
	private const int FINGER_NONE = -10;
	//初期カメラ深さ(10unit)でマークの大きさが1unitになるように設定
	private const float MARK_RATE = 0.1f;
	private Transform target;
	private Renderer rend;

	[SerializeField]
	private Camera cam;
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

	public void OnPointerDown(PointerEventData data){
		if (finger == FINGER_NONE){
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
			//動かすオブジェクトとして登録
			target = obj.transform;
			//描画するカメラを変えるためレイヤーの変更
			obj.layer = LayerMask.NameToLayer("UI");

			twoDWallMarks.ReleaseFocus();
		}
	}

	public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;

			if (twoDWallImage.IsOnPointerEnter()){
				rend.sortingLayerName = "2D";
				target.gameObject.layer = LayerMask.NameToLayer("2D");

				//bounds
				Vector3 p = target.position;
				//wallの幅とサイズを取得
				Vector2 size = wallManager.GetMasterWallSize();
				float height = size.y;
				float width = size.x;

				if (p.x < -width / 2 || p.x > width / 2 || p.y < -height / 2 || p.y > height / 2){
					twoDWallMarks.DeleteMark(target.gameObject.name);
				}else{
					twoDWallMarks.SetFocus(target.GetComponent<TwoDMark>());
					//マークの中で一番上に表示する
					target.SetAsFirstSibling();
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
