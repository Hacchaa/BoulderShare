using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;
public class VRIKBend : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger ;
	protected const int FINGER_NONE = -10;
	[SerializeField] protected Camera cam;
	private Vector3 defaultPos;
	[SerializeField] protected Transform avatar;
	[SerializeField] protected Transform target;
	private Vector3 center;
	[SerializeField] private float radius = 0.5f;
	[SerializeField] private float weight = 0.5f;
	[SerializeField] private Transform parentAvatar;
	[SerializeField] private Transform childAvatar;
	private float lAP;
	private float lAC;
	private float lPC;
	private float pivotRate;
	private Vector3 axis;
	private bool isDragging = false;
	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		defaultPos = target.localPosition;
		Init();
	}

	public void Init(){		
		ResetPos();
		//target.position = center + Vector3.forward * radius;
	}


	void LateUpdate(){
		Invoke("Correct", 0.0f);
	}

	public void ResetPos(){
		target.localPosition = defaultPos;
	}	

	public Vector3 GetPosition(){
		return target.localPosition;
	}

	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}
	public Quaternion GetRotation(){
		return target.localRotation;
	}
	public void SetRotation(Quaternion rot){
		target.localRotation = rot;
	}

	//avatarの位置に移動させる
	public void Correct(){
		//transform.position = avatar.position;
		
		if (isDragging){
			transform.position = avatar.position;
		}else{
			target.position += (avatar.position - transform.position);
			transform.position = avatar.position;
			//target.position =  center + (avatar.position - center).normalized * radius;
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;
			
			lPC = (childAvatar.position - parentAvatar.position).sqrMagnitude;
			pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

			axis = (childAvatar.position - parentAvatar.position);
			center = parentAvatar.position + (axis * pivotRate);
			isDragging = true;
			target.position =  center + (avatar.position - center).normalized * radius;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 camAxis = cam.WorldToScreenPoint(childAvatar.position) - cam.WorldToScreenPoint(parentAvatar.parent.position);
			camAxis = camAxis.normalized;
			float dot = Vector2.Dot(new Vector2(-camAxis.y, camAxis.x), data.delta.normalized);
			target.RotateAround(center, axis, dot * weight);
			/*
			swivel += -1 * dot * weight;
			if (swivel > 180.0f){
				swivel -= 360;
			}else if(swivel < -180){
				swivel += 360;
			}*/
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
			isDragging = false;
		}
	}
}
