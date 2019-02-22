using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMarkFoot : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private Vector3 oldPos;
	private float baseDepth;
	private Vector3 toeBaseRot;
	private Vector3 defaultPos;
	[SerializeField] private float toeAngle;
	[SerializeField] private Transform avatarToe; 
	[SerializeField] private Transform avatar;
	[SerializeField] private List<float> zAngles;
	[SerializeField] private int zAngleIndex;
	[SerializeField] private Transform target;
	[SerializeField] bool isTreadOn = false;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		zAngleIndex = 0;
		toeBaseRot = avatarToe.localRotation.eulerAngles;
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

	//avatarとmarkの距離を返す
	public float CalcDiff(){
		return (target.position - avatar.position).magnitude;
	}
	//足の位置を変えないままtargetの位置を修正する
	public void ModifyPosition(){
		target.position = transform.position;
	}
	public Vector3 GetPosition(){
		return target.localPosition;
	}
	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}
	//avatarの位置に移動させる
	public void Correct(){
		transform.position = avatar.position;
		avatar.localRotation = Quaternion.Euler(0.0f, 0.0f, zAngles[zAngleIndex]);

		if(isTreadOn){
			avatarToe.localRotation = Quaternion.Euler(toeBaseRot + Vector3.forward * toeAngle);
		}else{
			avatarToe.localRotation = Quaternion.Euler(toeBaseRot);
		}
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
			oldPos = offset;
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
			/*
			p = transform.parent.InverseTransformPoint(p);
		
			transform.localPosition = p - offset;*/

			target.Translate(p - oldPos, Space.World);
			oldPos = p;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
