using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMarkHand : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	private Vector3 handBaseAngles;
	private Vector3 defaultPos;
	[SerializeField] private Transform avatar;
	[SerializeField] private Transform target;
	[SerializeField] private List<float> xAngles;
	[SerializeField] private int xAngleIndex;
	[SerializeField] private List<float> yAngles;
	[SerializeField] private int yAngleIndex;
	[SerializeField] private List<float> zAngles;
	[SerializeField] private int zAngleIndex;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		zAngleIndex = 0;
		xAngleIndex = 0;
		yAngleIndex = 0;

		handBaseAngles = avatar.localRotation.eulerAngles;
		//Debug.Log("handbaseAngles:"+handBaseAngles);
		defaultPos = transform.localPosition;
		target.position = transform.position;
	}	

	public void Init(){
		transform.localPosition = defaultPos;
	}
/*
	void LateUpdate(){
		Invoke("Correct", 0.0f);
	}*/
	//手の位置を変えないままtargetの位置を修正する
	public void ModifyPosition(){
		target.position = transform.position;
	}	//avatarとmarkの距離を返す
	public float CalcDiff(){
		return (target.position - avatar.position).magnitude;
	}

	//avatarの位置に移動させる
	public void Correct(){
		transform.position = avatar.position;
		//Debug.Log(handBaseAngles + new Vector3(xAngles[xAngleIndex], yAngles[yAngleIndex], zAngles[zAngleIndex]));
		avatar.localRotation = Quaternion.Euler(handBaseAngles + new Vector3(xAngles[xAngleIndex], yAngles[yAngleIndex], zAngles[zAngleIndex]));
	}

	public Vector3 GetPosition(){
		return target.localPosition;
	}

	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			offset = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			offset = transform.parent.InverseTransformPoint(offset);
			offset = offset - transform.localPosition;

			target.position = transform.position;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - transform.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			p = transform.parent.InverseTransformPoint(p);
		
			target.localPosition = p - offset;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;

		}
	}
}
