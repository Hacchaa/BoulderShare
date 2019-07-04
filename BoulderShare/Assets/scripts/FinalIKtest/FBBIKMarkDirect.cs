using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FBBIKMarkDirect : FBBIKBase
{
	[SerializeField] private Transform parentAvatar;
	[SerializeField] private Transform childAvatar;
	[SerializeField] private float radius = 1.0f;
	protected Vector3 center;
	protected Vector3 axis;
	protected Plane plane;

	public override void Init(){
		base.Init();

		OnPostBeginDrag += Prepare;
	}

	private void Prepare(){
		//軸を分割する比を求める
		float lAP = (avatar.position - parentAvatar.position).sqrMagnitude;
		float lAC = (avatar.position - childAvatar.position).sqrMagnitude;
		float lPC = (childAvatar.position - parentAvatar.position).sqrMagnitude;

		float pivotRate =  (lAP + lPC - lAC)/ (2*lPC) ;

		axis = (childAvatar.position - parentAvatar.position);
		center = parentAvatar.position + (axis * pivotRate);

		axis = axis.normalized;

		plane = new Plane(axis, center);
	}

	public override void OnDrag(PointerEventData data){
		//DrawPlane(center, axis);
		if (finger == data.pointerId){
            Ray ray = cam.ScreenPointToRay(data.position);

            float enter = 0.0f;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
         
                target.position = center + (hitPoint - center).normalized * radius;
            }

			if (OnPostDrag != null){
				OnPostDrag();
			}
		}
	}

	private void DrawPlane(Vector3 position, Vector3 normal) {
		Vector3 v3 ;

		if (normal.normalized != Vector3.forward)
		 v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
		else
		 v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
		 
		var corner0 = position + v3;
		var corner2 = position - v3;
		var q = Quaternion.AngleAxis(90.0f, normal);
		v3 = q * v3;
		var corner1 = position + v3;
		var corner3 = position - v3;

		Debug.DrawLine(corner0, corner2, Color.green);
		Debug.DrawLine(corner1, corner3, Color.green);
		Debug.DrawLine(corner0, corner1, Color.green);
		Debug.DrawLine(corner1, corner2, Color.green);
		Debug.DrawLine(corner2, corner3, Color.green);
		Debug.DrawLine(corner3, corner0, Color.green);
		Debug.DrawRay(position, normal, Color.red);
	}
}
