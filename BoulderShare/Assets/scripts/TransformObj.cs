using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformObj : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger = -1;
	public Camera cam;
	public AvatarControl ac;
	public Transform target = null;
	public Transform avatar = null;
	private static string BODY_NAME = "CollisionBody";
	// Use this for initialization
	void Start () {
	}	

	void LateUpdate(){
		if (avatar != null){
			transform.position = avatar.position;
			if (gameObject.name == "CollisionLF" || gameObject.name == "CollisionRF"){
				transform.localRotation = Quaternion.Euler(
					transform.localRotation.eulerAngles.x,
					avatar.localRotation.eulerAngles.y,
					transform.localRotation.eulerAngles.z);
			}
		}
	}
	

	public void OnBeginDrag(PointerEventData data){
		if (finger == -1){
			finger = data.pointerId;
			target.position = transform.position;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					target.position.z - cam.transform.position.z));
		
			if (gameObject.name.Equals(BODY_NAME)){
				target.position = new Vector3(p.x, p.y, ac.CalcBodyZPos(p));
			}else{
				target.position = new Vector3(p.x, p.y, ac.CalcZPos(p));
			}
			target.localRotation = transform.localRotation;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = -1;
		}
	}
}
