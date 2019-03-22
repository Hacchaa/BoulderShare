using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FBBIKBase : MonoBehaviour, IHumanModelMarkComponent, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger = MyUtility.FINGER_NONE;

	[SerializeField] private MyUtility.FullBodyMark bodyID;
	[SerializeField] private List<Transform> relativePosList;
	protected Action OnPostBeginDrag;
	protected Action OnPostDrag;
	protected Action OnPostEndDrag;
	[SerializeField] protected Transform target;
	protected Transform avatar;
	private Camera cam;
	private float baseDepth;
	[SerializeField] private Vector3 initPos;

	public virtual void Init(){

	}

	void LateUpdate(){
		Invoke("CorrectPosition", 0.0f);
	}

	private void CorrectPosition(){
		if (avatar != null){
			transform.position = avatar.position;
		}
	}

	public Vector3 GetWorldPosition(){
		return target.position;
	}
	public Vector3 GetPosition(){
		return target.localPosition;
	}
	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}
	public void InitPosition(){
		target.localPosition = initPos;
	}
	public Vector3 GetInitPosition(){
		return initPos;
	}
	public void SetCamera(Camera camera){
		cam = camera;
	}
	public void SetAvatar(Transform t){
		avatar = t;
	}

	public MyUtility.FullBodyMark GetBodyID(){
		return bodyID;
	}

	public void OnBeginDrag(PointerEventData data){
		Debug.Log("onbeginDrag");
		if (finger == MyUtility.FINGER_NONE){
			finger = data.pointerId;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			if (OnPostBeginDrag != null){
				OnPostBeginDrag();
			}
		}
	}

	public virtual void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			Vector3 pOld = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					baseDepth));

			Vector3 v = p - pOld;
			target.Translate(v, Space.World);

			foreach(Transform t in relativePosList){
				t.Translate(v, Space.World);
			}
			if (OnPostDrag != null){
				OnPostDrag();
			}
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;

			if (OnPostEndDrag != null){
				OnPostEndDrag();
			}
		}
	}
}
