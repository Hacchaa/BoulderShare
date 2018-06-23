using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformObj : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	public Camera cam;
	public AvatarControl ac;
	public Transform target = null;
	public Transform avatar = null;
	private bool isFixed = false;
	public Hold_Pose hp;
	private static string BODY_NAME = "CollisionBody";
	private bool isRHD = false;
	private bool isRFD = false;
	private bool isLHD = false;
	private bool isLFD = false;

	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;

		if (gameObject.name.Equals("CollisionRHD")){
			isRHD = true;
		}
		if (gameObject.name.Equals("CollisionRFD")){
			isRFD = true;
		}
		if (gameObject.name.Equals("CollisionLHD")){
			isLHD = true;
		}
		if (gameObject.name.Equals("CollisionLFD")){
			isLFD = true;
		}
	}	

	void LateUpdate(){
		if (avatar != null){
			transform.position = avatar.position;
		}
	}
	
	public void SetFixed(bool b){
		isFixed = b;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
			target.position = transform.position;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - target.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					target.position.z - cam.transform.position.z));
		
			target.position = p;

			if (isFixed){
				float r = 0.0f;
				Vector3 pos = Vector3.zero;
				if (isRHD){
					r = hp.GetR((int)SceneFocus.Choice.RH);
					pos = hp.GetHoldPos((int)SceneFocus.Choice.RH);
				}else if(isRFD){
					r = hp.GetR((int)SceneFocus.Choice.RF);
					pos = hp.GetHoldPos((int)SceneFocus.Choice.RF);
				}
				else if(isLHD){
					r = hp.GetR((int)SceneFocus.Choice.LH);
					pos = hp.GetHoldPos((int)SceneFocus.Choice.LH);
				}
				else if(isLFD){
					r = hp.GetR((int)SceneFocus.Choice.LF);
					pos = hp.GetHoldPos((int)SceneFocus.Choice.LF);
				}

				//バウンド処理
				Vector3 v = target.localPosition - pos;

				if (v.magnitude > r){
					target.localPosition = pos + v.normalized * r;
				}
			}
			/*
			if (gameObject.name.Equals(BODY_NAME)){
				//target.position = new Vector3(p.x, p.y, ac.CalcBodyZPos(p));
				target.position = p;
			}else{
				target.position = new Vector3(p.x, p.y, ac.CalcZPos(p));
			}*/
			target.localRotation = transform.localRotation;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
