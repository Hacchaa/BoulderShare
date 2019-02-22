using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.EventSystems;
public class HeatTest : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	private Vector3 lookOffset;
	private Vector3 ikToHead;
	private Quaternion baseRotOffset;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform lookParent;
    [SerializeField] private AimIK aimIK;
 	[SerializeField] private Transform rotBase;
	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		lookParent.position = rotBase.position;
		//lookParent.rotation = rotBase.rotation;
		lookTarget.position = transform.position;

		lookOffset = rotBase.position;

		ikToHead = transform.position - rotBase.position;
		baseRotOffset = rotBase.rotation;
	}	
    void LateUpdate(){
    	Invoke("Correct", 0.0f);
    }
    private void Correct(){
    	//Debug.Log(aimIK.solver.transform.position);
    	//lookParent.rotation = Quaternion.FromToRotation(lookTarget.position - rotBase.position, lookParent.position - rotBase.position);
    	//Debug.Log(Quaternion.RotateTowards(baseRotOffset, rotBase.rotation, -1f));
    	//Debug.Log(rotBase.rotation);
    	//lookTarget.position = transform.position - (rotBase.position - lookOffset);
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
			lookTarget.position = transform.position;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = FINGER_NONE;
		}
	}
}