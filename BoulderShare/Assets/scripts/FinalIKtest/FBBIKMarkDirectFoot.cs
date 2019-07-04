using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FBBIKMarkDirectFoot : FBBIKMarkDirect
{
	[SerializeField] private FBBIKMarkFoot foot;

	public override void OnDrag(PointerEventData data){
		//DrawPlane(center, axis);
		if (finger == data.pointerId){
            Ray ray = cam.ScreenPointToRay(data.position);

            float enter = 0.0f;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 dirFrom = avatar.position - center;
                Vector3 dirTo = hitPoint - center;
                Vector3 cross = Vector3.Cross(dirFrom, dirTo);
	            float angle = Vector3.Angle(dirFrom, dirTo);

	            if (cross.y < 0){
	            	angle *= -1;
	            }

                foot.AddBendAngle(angle);
            }

			if (OnPostDrag != null){
				OnPostDrag();
			}
		}
	}
}
