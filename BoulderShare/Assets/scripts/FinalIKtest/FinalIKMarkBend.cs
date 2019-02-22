using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMarkBend : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private bool isDragging = false;
	private float pivotRate ;
	private Vector3 axis;
	private Vector3 pivot;
	private Vector2 camAxis;
	private float lAP;
	private float lAC;
	private float lPC;
	private Vector3 defaultPos;
	private Vector3 defaultChildPos;
	[SerializeField] private Transform avatar;
	[SerializeField] private Transform avatarChild;
	[SerializeField] private Transform child;
	[SerializeField] private Transform nearBasePos;
	[SerializeField] private Transform farBasePos;
	[SerializeField] private float length = 1.0f;
	[SerializeField] private bool isLeft;
	private float rotWeight = 0.1f;
		
	// Use this for initialization
	void Awake(){
		finger = FINGER_NONE;

		//軸を分割する比を求める
		lAP = (avatar.position - avatar.parent.position).sqrMagnitude;
		lAC = (avatar.position - avatarChild.position).sqrMagnitude;
		lPC = (avatarChild.position - avatar.parent.position).sqrMagnitude;

		pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

		//transform.position = avatar.position;
		defaultPos = transform.localPosition;

		axis = (avatarChild.position - avatar.parent.position);
		pivot = avatar.parent.position + (axis * pivotRate);
		//child.position =  pivot + (avatar.position - pivot).normalized * length;
		defaultChildPos = child.localPosition;
	}

	public void Init(){
		transform.localPosition = defaultPos;
		child.localPosition = defaultChildPos;
	}
	public Vector3 GetPosition(){
		return child.localPosition;
	}
	public void SetPosition(Vector3 p){
		child.localPosition = p;
	}
/*
	void Update(){
		//Debug.Log("update");
		//Invoke("AfterUpdate", 0.0f);

		Plane plane ;
		if(!isLeft){
			plane = new Plane(nearBasePos.position, farBasePos.position, avatarChild.position);
		}else{
			plane = new Plane(farBasePos.position, nearBasePos.position, avatarChild.position);
		}

		if (!plane.GetSide(child.position)){
			Debug.Log("getside");
			
			Vector3 v = plane.ClosestPointOnPlane(child.position);

			lPC = (avatarChild.position - avatar.parent.position).sqrMagnitude;
			pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

			axis = (avatarChild.position - avatar.parent.position);
			pivot = avatar.parent.position + (axis * pivotRate);
		}
	}*/

	void LateUpdate(){
		//nvoke("Correct", 0.0f);
	}

	//avatarの位置に移動させる
	public void Correct(){
		//Debug.DrawLine(pivot, child.position);
		//Debug.DrawLine(avatar.parent.position, avatarChild.position, Color.red);
		
		if (isDragging){
			Vector3 p = child.position;
			transform.position = avatar.position;
			child.position = p;
		}else{
			transform.position = avatar.position;
			
			lPC = (avatarChild.position - avatar.parent.position).sqrMagnitude;
			pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

			axis = (avatarChild.position - avatar.parent.position);
			pivot = avatar.parent.position + (axis * pivotRate);
			child.position =  pivot + (avatar.position - pivot).normalized * length;
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			//カメラから見た軸の傾きを求める
			//camAxis = cam.WorldToScreenPoint(avatarChild.position) - cam.WorldToScreenPoint(avatar.parent.position);
			//camAxis = camAxis.normalized;
			lPC = (avatarChild.position - avatar.parent.position).sqrMagnitude;
			pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

			axis = (avatarChild.position - avatar.parent.position);
			pivot = avatar.parent.position + (axis * pivotRate);

			isDragging = true;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//カメラから見た軸の傾きを求める
			camAxis = cam.WorldToScreenPoint(avatarChild.position) - cam.WorldToScreenPoint(avatar.parent.position);
			camAxis = camAxis.normalized;
			float dot = Vector2.Dot(new Vector2(-camAxis.y, camAxis.x), data.delta);
			Debug.Log(dot);
			child.RotateAround(pivot, axis, dot * rotWeight);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
			isDragging = false;
		}
	}
}
