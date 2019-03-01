using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;

public class FinalIKMarkHead : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;

	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private Camera cam;
	[SerializeField]
	private Transform avatar ;
	private Vector3 offset;
	public LayerMask targetMask;
	
	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
	}
	void Awake(){
		transform.position = avatar.position + Vector3.forward * 0.5f;
	}

	public Vector3 GetPosition(){
		return transform.localPosition;
	}

	public void SetPosition(Vector3 p){
		transform.localPosition = p;
	}

	public void SetIsLookingActive(bool b){
		Vector3 p = threeDWall.CalcWallPoint(avatar.position);
		transform.position = p;
		gameObject.SetActive(b);
	}

	public bool IsLookingActive(){
		return gameObject.activeSelf;
	}

/*
	private void CalcRot(){
		
		Quaternion baseRot = avatar.rotation;
		Quaternion avatarToTarget = Quaternion.FromToRotation(avatar.position, transform.position);

		transform.rotation = avatarToTarget * baseRot ;
	}*/

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){		
		
			Ray ray = cam.ScreenPointToRay(data.position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask)){
				transform.position = hit.point;
			}/*
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x , 
					data.position.y , 
					cam.gameObject.transform.InverseTransformPoint(transform.position).z));
			
			Vector3 oldP = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					cam.gameObject.transform.InverseTransformPoint(transform.position).z));
		
			transform.Translate(p - oldP, Space.World);


			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			Vector3 wallWorldPos = threeDWall.GetWallWorldPos();

			x = Mathf.Min(x, wallWorldPos.x + bounds.size.x/2);
			x = Mathf.Max(x, wallWorldPos.x - bounds.size.x/2);
			y = Mathf.Min(y, wallWorldPos.y + bounds.size.y/2);
			y = Mathf.Max(y, wallWorldPos.y - bounds.size.y/2);

			transform.position = threeDWall.CalcWallPoint(new Vector3(x, y, z));
			//CalcRot();	*/
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
