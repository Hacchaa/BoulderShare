using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMarkPelvisAndShoulder : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	private Vector3 defaultPos;
	[SerializeField]
	private Transform avatar;
	[SerializeField] Transform target;
	[SerializeField] FinalIKROM finalIK;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		defaultPos = transform.localPosition;
		target.position = transform.position;
	}	

	public void Init(){
		transform.localPosition = defaultPos;
	}

	//avatarの位置に移動させる
	public void Correct(){
		transform.position = avatar.position;
	}
	public Vector3 GetPosition(){
		return target.localPosition;
	}
	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}


	//腰の位置を変えないままtargetの位置を修正する
	public void ModifyPosition(){
		target.position = transform.position;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(target.position).z;
			offset = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			offset = target.parent.InverseTransformPoint(offset);
			offset = offset - target.localPosition;

			finalIK.ModifyPositions();
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
			p = target.parent.InverseTransformPoint(p);
		
			target.localPosition = p - offset;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = FINGER_NONE;
			finalIK.ModifyPositions();
		}
	}
}
