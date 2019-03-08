using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;

public class FinalIKMarkBody : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private Vector3 baseShoulder;
	private Vector3 basePelvis;
	private Vector3 baseBody;
	private float baseDepth;
	private Vector3 defaultPos;
	private bool isMoved;

	[SerializeField]
	private Transform avatar;

	[SerializeField]
	private Transform shoulderParent;
	[SerializeField]
	private Transform pelvisParent;
	[SerializeField] private ThreeD threeD;
	[SerializeField] private CameraManager cameraManager;

	// Use this for initialization
	void Awake() {
		finger = FINGER_NONE;
		transform.position = avatar.position;
		defaultPos = transform.localPosition;
		isMoved = false;
	}	


	public void Init(){
		transform.localPosition = defaultPos;
	}
	public void Correct(){
		transform.position = avatar.position;
		if (isMoved){
			Vector3 dir = transform.position - baseBody;
			shoulderParent.position = baseShoulder + dir;
			pelvisParent.position = basePelvis + dir;
		}
	}

	public bool IsMoved(){
		return isMoved;
	}

	public Vector3 GetPosition(){
		return transform.localPosition;
	}
	public Vector3 GetWorldPosition(){
		return transform.position;
	}
	public void SetPosition(Vector3 p){
		transform.localPosition = p;
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			/*
			offset = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			offset = transform.parent.InverseTransformPoint(offset);
			offset = offset - transform.localPosition;*/

			baseBody = transform.position;
			baseShoulder = shoulderParent.position;
			basePelvis = pelvisParent.position;
			isMoved = true;
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
			Vector3 pOld = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					baseDepth));
			//p = transform.parent.InverseTransformPoint(p);
			//transform.localPosition = p - offset;
			transform.Translate(p - pOld, Space.World);

			Vector3 dir = transform.position - avatar.position;
			dir = transform.position - baseBody;

			shoulderParent.position = baseShoulder + dir;
			pelvisParent.position = basePelvis + dir;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			//threeD.LookAtModel();
			//cameraManager.SetPosWithFixedHierarchyPos(threeD.GetModelBodyPosition());
			finger = FINGER_NONE;
			isMoved = false;
		}
	}
}
