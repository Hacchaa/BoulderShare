using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FBBIKMarkDirectFoot : FBBIKMarkDirect
{
	[SerializeField] private FBBIKMarkFoot foot;
	[SerializeField] private float r = 1.0f;
	[SerializeField] private float baseBendAngle;
	[SerializeField] private Vector3 baseDir;
	public override void Init(){
		base.Init();
		OnPostBeginDrag += DetermineBase;
	}
	private void DetermineBase(){
		baseDir = avatar.position - center;
		baseBendAngle = foot.GetBendAngle();
	}
	public override void OnDrag(PointerEventData data){
		//DrawPlane(center, axis);
		if (finger == data.pointerId){
			MovePoint(data);
			Vector3 v = plane.ClosestPointOnPlane(target.position);
    		target.position = center + (v - center).normalized * r;

            Vector3 dirTo = target.position - center;
            Vector3 cross = Vector3.Cross(baseDir, dirTo);
            float angle = Vector3.Angle(baseDir, dirTo);

            if (plane.GetSide(center + cross)){
				angle *= -1;
			}

            foot.SetBendAngle(baseBendAngle + angle);
		/*
            Ray ray = cam.ScreenPointToRay(data.position);
            float enter = 0.0f;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Debug.DrawLine(center, hitPoint, Color.green, 5.0f);

                Vector3 dirFrom = avatar.position - center;
                Vector3 dirTo = hitPoint - center;
                Vector3 cross = Vector3.Cross(dirFrom, dirTo);
	            float angle = Vector3.Angle(dirFrom, dirTo);

	            if (plane.GetSide(center + cross)){
    				angle *= -1;
    			}

                foot.AddBendAngle(angle);
            }*/

			if (OnPostDrag != null){
				OnPostDrag();
			}
		}
	}
}
