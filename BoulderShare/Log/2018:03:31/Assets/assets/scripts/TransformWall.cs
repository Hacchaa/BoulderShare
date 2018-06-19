using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TransformWall : MonoBehaviour, IDragHandler ,IPointerDownHandler,IPointerUpHandler{
	private float prevLength = -1;
	private int targetTC = 0;
	private int[] fingers;
	private RectTransform rectTransform;
	private RectTransform canvasRectTransform;
	public GameObject test;

	public void Start(){
		//rectTransform = GetComponent<RectTransform>();
		rectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
		canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		fingers = new int[]{-1, -1};
	}
/*
	public void OnPointerDown(PointerEventData data){
		targetTC++;

		if (targetTC == 2){
			prevLength = -1;
		}
	}*/

	public void OnPointerDown(PointerEventData data){
		if (fingers[0] == -1){
			fingers[0] = data.pointerId;
		}else if(fingers[1] == -1){
			fingers[1] = data.pointerId;
		}

		if (fingers[1] != -1){
			prevLength = -1;
		}
	}

	public void OnDrag(PointerEventData data){
		Vector2 p1, p2;
		bool isP1Exist, isP2Exist;
		isP1Exist = isP2Exist = false;
		p1 = Vector2.zero;
		p2 = Vector2.zero;

		//扱っている２本の指かどうか
		if (data.pointerId != fingers[0] && data.pointerId != fingers[1]){
			return ;
		}

		//扱っている指の情報を取得する
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == fingers[0]){
				p1 = touch.position;
				isP1Exist = true;
			}else if (touch.fingerId == fingers[1]){
				p2 = touch.position;
				isP2Exist = true;
			}
		}

		//拡大縮小操作
		if (isP1Exist && isP2Exist){
			//２点間の距離
			float length ,diffRate;
			Vector2 pivot, diff;
			Vector3 wPivot = Vector3.zero;

			//二つの指の軸毎の距離の絶対値
			diff = new Vector2(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y));
			//絶対距離の大きい方をlengthにする
			if (diff.x < diff.y){
				length = diff.y;
			}else{
				length = diff.x;
			}

			//prevLengthが設定さえていない場合
			if (prevLength < 0){
				prevLength = length;
				return ;
			}

			//スクリーン座標における２つの指の中心点
			pivot = new Vector2((p1.x + p2.x)/2.0f, (p1.y + p2.y)/2.0f);
			//スクリーン座標からワールド座標に変換
			RectTransformUtility.ScreenPointToWorldPointInRectangle(
				canvasRectTransform, pivot, null, out wPivot);
			//ワールド座標からローカル座標に変換
			pivot = rectTransform.InverseTransformPoint(wPivot);
			test.transform.position = wPivot;
			//lengthの変化の割合を求める
			diffRate = length / prevLength;

			//pivotの位置からdiffRate倍する	
			/*		
			rectTransform.Translate(
				pivot.x * (1 - diffRate),
				pivot.y * (1 - diffRate),
				0,
				Space.Self);*/
			Debug.Log("pivot="+pivot);
			Debug.Log("localPositionBeforModification="+rectTransform.localPosition);
			Debug.Log("positionBM="+rectTransform.position);

			rectTransform.localScale = rectTransform.localScale * diffRate;
			/*
			rectTransform.localPosition += new Vector3(
				pivot.x * (rectTransform.localScale - diffRate) ,
				pivot.y * (1.0f - diffRate) ,
				0);*/

			rectTransform.localPosition += new Vector3(
				pivot.x / diffRate,
				pivot.y / diffRate,
				0);

			Debug.Log("localPosition="+rectTransform.localPosition);
			Debug.Log("position="+rectTransform.position);
			Debug.Log("diffRate="+diffRate);
			
			prevLength = length;
		}

		//移動
		if (isP1Exist && isP2Exist && data.pointerId == fingers[0]){
			return ;
		}
		rectTransform.position = rectTransform.position + new Vector3(data.delta.x, data.delta.y);

	}
	/*
	public void OnDrag(PointerEventData data){

		if (targetTC == 1){
			transform.position = transform.position + new Vector3(data.delta.x, data.delta.y);

		}else if(targetTC == 2){
			//２点間の距離
			float length ,diffRate;
			Vector2 pivot, diff, p1, p2;
			Vector3 wPivot = Vector3.zero;

			p1 = Input.touches[0].position;
			p2 = Input.touches[1].position;
			//二つの指の軸毎の距離
			diff = new Vector2(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y));
			//絶対距離の大きい方をlengthにする
			if (diff.x < diff.y){
				length = diff.y;
			}else{
				length = diff.x;
			}

			//prevLengthが設定さえていない場合
			if (prevLength < 0){
				prevLength = length;
				return ;
			}

			//スクリーン座標における２つの指の中心点
			pivot = new Vector2((p1.x + p2.x)/2.0f, (p1.y + p2.y)/2.0f);
			//スクリーン座標からワールド座標に変換
			RectTransformUtility.ScreenPointToWorldPointInRectangle(
				canvasRectTransform, pivot, null, out wPivot);
			//ワールド座標からローカル座標に変換
			pivot = rectTransform.InverseTransformPoint(wPivot);

			//lengthの変化の割合を求める
			diffRate = length / prevLength;

			//pivotの位置からdiffRate倍する			
			rectTransform.Translate(
				pivot.x * (1 - diffRate),
				pivot.y * (1 - diffRate), 0);

			rectTransform.sizeDelta = rectTransform.sizeDelta * diffRate;

			prevLength = length;
		}
	}
	public void OnPointerUp(PointerEventData data){
		targetTC--;
		if (targetTC == 2){
			prevLength = -1;
		}
	}*/
	public void OnPointerUp(PointerEventData data){
		if (fingers[0] == data.pointerId){
			fingers[0] = fingers[1];
			fingers[1] = -1;
		}else if(fingers[1] == data.pointerId){
			fingers[1] = -1;
		}
	}
}
