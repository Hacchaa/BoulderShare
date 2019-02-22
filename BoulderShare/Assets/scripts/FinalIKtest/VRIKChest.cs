using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRIKChest : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger ;
	protected const int FINGER_NONE = -10;
	[SerializeField] protected Camera cam;
	protected float baseDepth;
	private Vector3 defaultPos;
	[SerializeField] protected Transform avatar;
	[SerializeField] protected Transform target;
	private Vector3 basePos;
	private Vector3 center;
	[SerializeField] private float radius = 0.5f;
	[SerializeField] private float weight = 0.5f;
	[SerializeField] private Transform leftShoulderAvatar;
	[SerializeField] private Transform rightShoulderAvatar;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		defaultPos = target.localPosition;
		Init();
	}

	public void Init(){
		ResetPos();
		center = avatar.position;
		target.position = center + Vector3.forward * radius;
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
		if (avatar != null){
			transform.position = avatar.position;
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 axis = cam.WorldToScreenPoint(leftShoulderAvatar.position) - cam.WorldToScreenPoint(rightShoulderAvatar.parent.position);
			axis = axis.normalized;
			float dot = Vector2.Dot(new Vector2(-axis.y, axis.x), data.delta.normalized);
			target.RotateAround(center, Vector3.up, -1 * dot * weight);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
