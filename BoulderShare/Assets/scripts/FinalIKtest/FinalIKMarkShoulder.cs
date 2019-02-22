using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMarkShoulder : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	private bool isDragging = false;
	[SerializeField] private Transform avatarLeft;
	[SerializeField] private Transform avatarRight;
	[SerializeField] private Transform left;
	[SerializeField] private Transform right;
	[SerializeField] private Transform avatar;

	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
		transform.position = avatar.position;
		left.position = avatarLeft.position;
		right.position = avatarRight.position;
	}	

	public bool IsDragging(){
		return isDragging;
	}

	void LateUpdate(){
		if (!isDragging){
			Invoke("Correct", 0.0f);
		}
	}

	//avatarの位置に移動させる
	private void Correct(){
		transform.position = avatar.position;
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
			offset = transform.parent.InverseTransformPoint(offset);
			offset = offset - transform.localPosition;
			isDragging = true;
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
			p = transform.parent.InverseTransformPoint(p);
		
			transform.localPosition = p - offset;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
			isDragging = false;
		}
	}
}
