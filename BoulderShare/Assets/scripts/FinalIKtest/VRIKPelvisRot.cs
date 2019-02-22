using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRIKPelvisRot : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger ;
	protected const int FINGER_NONE = -10;
	[SerializeField] protected Camera cam;
	[SerializeField] protected Transform target;
	[SerializeField] private float weight = 0.5f;
	[SerializeField] private Transform leftPelvisAvatar;
	[SerializeField] private Transform rightPelvisAvatar;
	[SerializeField] private Transform avatar;

	[SerializeField] private float defaultDeg;
	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//defaultDeg = target.localRotation.eulerAngles.y;
		transform.position = avatar.position;
	}
	void LateUpdate(){
		Invoke("Correct", 0.0f);

	}
	//avatarの位置に移動させる
	public void Correct(){
		if (avatar != null){
			transform.position = avatar.position;
		}
	}
	public void ResetPos(){
		target.localRotation = Quaternion.Euler(0.0f, defaultDeg, 0.0f);
	}	

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 angles = target.localRotation.eulerAngles;
			float degree = angles.y;
			Vector3 axis = cam.WorldToScreenPoint(leftPelvisAvatar.position) - cam.WorldToScreenPoint(rightPelvisAvatar.parent.position);
			axis = axis.normalized;
			float dot = Vector2.Dot(new Vector2(-axis.y, axis.x), data.delta.normalized);
			degree += dot * weight;
			target.localRotation = Quaternion.Euler(angles.x, degree, angles.z);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
