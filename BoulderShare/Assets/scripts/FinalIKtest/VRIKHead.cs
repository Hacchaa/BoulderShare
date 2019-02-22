using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;

public class VRIKHead : VRIKComponent{
	private Vector3 basePos;
	[SerializeField] List<Transform> relativePosList;

	public override void Init(){
		basePos = target.position;
	}

	public override void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
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
			target.Translate(v, Space.World);

			foreach(Transform t in relativePosList){
				t.Translate(v, Space.World);
			}
			OnPostDrag();
		}
	}
}
