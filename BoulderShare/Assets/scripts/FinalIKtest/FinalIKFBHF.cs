using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKFBHF : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	[SerializeField]
	private float distSqrMag;
	[SerializeField]
	private Transform avatar;
	public EditorManager.BODYS bType;
		

	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
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

			Vector3 dir = transform.position - avatar.position;
			float d = dir.sqrMagnitude;

			if (d > distSqrMag){
				transform.position = avatar.position + dir.normalized * Mathf.Sqrt(distSqrMag);
			}
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
