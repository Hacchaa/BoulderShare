using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FBBIKBase : MonoBehaviour, IHumanModelMarkComponent, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger = MyUtility.FINGER_NONE;
	public enum MoveType {Delta, Point};
	[SerializeField] private MyUtility.FullBodyMark bodyID;
	[SerializeField] protected List<Transform> relativePosList;
	protected Action OnPostBeginDrag;
	protected Action OnPostDrag;
	protected Action OnPostEndDrag;
	protected Action OnPreCorrectPosition;
	protected Action OnPostCorrectPosition;
	[SerializeField] protected Transform target;
	protected Transform avatar;
	protected Camera cam;
	private float baseDepth;
	[SerializeField] private Vector3 initPos;
	[SerializeField] private Vector3 avatarOffset;
	protected MoveType moveType = MoveType.Delta;
	protected bool isInit = false;

	public virtual void Init(){
		isInit = true;
	}

	public void AddOnPostBeginDragAction(Action a){
		if (a != null){
			OnPostBeginDrag += a;
		}
	}

	public void RemoveOnPostBeginDragAction(Action a){
		if (a != null){
			OnPostBeginDrag -= a;
		}
	}

	public void CorrectPosition(){
		//Debug.Log("correctPosition target:"+target.position);
		if (OnPreCorrectPosition != null){
			OnPreCorrectPosition();
		}
		if (avatar != null){
			transform.position = GetAvatarOffsetPos();
		}
		if (OnPostCorrectPosition != null){
			OnPostCorrectPosition();
		}
	}

	protected Vector3 GetAvatarOffsetPos(){
		//return avatar.position;
		return avatar.TransformPoint(avatarOffset);
	}

	public Vector3 GetWorldPosition(){
		return target.position;
	}
	public Vector3 GetPosition(){
		return target.localPosition;
	}

	public void SetWorldPosition(Vector3 p){
		target.position = p;
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
			if (moveType == MoveType.Delta){
				MoveDelta(data);
			}else if (moveType == MoveType.Point){
				MovePoint(data);
			}
			if (OnPostDrag != null){
				OnPostDrag();
			}
		}
	}

	private void MoveDelta(PointerEventData data){
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
	}
	private void MovePoint(PointerEventData data){
		Vector3 p = cam.ScreenToWorldPoint(
			new Vector3(
				data.position.x, 
				data.position.y, 
				baseDepth));

		Debug.Log("target:"+ target.position + ", p:"+p);

		Vector3 v = p - target.position;
		target.position = p;
		//target.Translate(v, Space.World);

		foreach(Transform t in relativePosList){
			t.Translate(v, Space.World);
		}		
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;

			if (OnPostEndDrag != null){
				OnPostEndDrag();
			}
		}
		Debug.Log("onendDrag target:"+ target.localPosition);
	}
}
