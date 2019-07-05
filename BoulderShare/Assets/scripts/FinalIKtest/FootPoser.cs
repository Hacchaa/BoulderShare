using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class FootPoser : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger = MyUtility.FINGER_NONE;
	[SerializeField] private MyUtility.FullBodyMark targetAvatarBodyID;
	[SerializeField] private Transform kneeAvatar;
	[SerializeField] private Transform footAvatar;
	[SerializeField] private Transform toeAvatar;
	[SerializeField] private Camera cam;
	[SerializeField] private FBBIKMarkFoot footMark;
	[SerializeField] private Vector3 axis;
	private float baseDepth;
	private Plane plane;
	private Vector3 baseDir;
	private float r = 0.5f;
	private float baseStretchAngle;

	public void Show(){
		OnPreShow();

		gameObject.SetActive(true);
	}

	public void Hide(){
		gameObject.SetActive(false);
	}

	public MyUtility.FullBodyMark GetTargetAvatarBodyID(){
		return targetAvatarBodyID;
	}

	public void OnPreShow(){
		plane = new Plane(toeAvatar.position, footAvatar.position, kneeAvatar.position);

		//baseDir = Quaternion.AngleAxis(offsetAngle, plane.normal) * toeAvatar.position - footAvatar.position;
		baseDir = toeAvatar.TransformPoint(axis * r) - toeAvatar.position;
		transform.position = toeAvatar.position + baseDir.normalized * r;
		baseStretchAngle = footMark.GetStretchAngle();
		//DrawLine();
	}

	private void DrawLine(){
		Debug.DrawLine(toeAvatar.position, transform.position, Color.white, 1.0f);
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == MyUtility.FINGER_NONE){
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
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
			transform.Translate(v, Space.World);
			///transform.position = p;

			v = plane.ClosestPointOnPlane(transform.position);
			Vector3 curDir = (v - toeAvatar.position);
			transform.position = toeAvatar.position + curDir.normalized * r;

    		float angle = Vector3.Angle(baseDir, curDir);
    		Vector3 cross = Vector3.Cross(baseDir, curDir);

    		if (!plane.GetSide(toeAvatar.position + cross)){
    			angle *= -1;
    		}

    		footMark.SetStretchAngle(baseStretchAngle + angle);
    		//float newAngle = footMark.GetStretchAngle();

    		//transform.position = footAvatar.position + baseDir.normalized * r;
    		//transform.RotateAround(footAvatar.position, plane.normal, (newAngle - baseStretchAngle));
    		footMark.FixRot();
    		//DrawLine();
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;
		}
	}
}
