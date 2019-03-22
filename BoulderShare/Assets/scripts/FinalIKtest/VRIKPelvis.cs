using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRIKPelvis : VRIKComponent{
	[SerializeField] private float diff = 0.5f;
	[SerializeField] private VRIKPelvisRot rot;
	[SerializeField] private Transform headTarget;
	private Vector3 fromDir = Vector3.up;

	//targetの位置を修正
	public override void ModifyPosition(){
		Vector3 v = target.position - transform.position;
		if (v.magnitude > diff){
			//target.position = transform.position + v.normalized * diff;
		}
	}

	public override void OnDrag(PointerEventData data){
		base.OnDrag(data);
		Vector3 toDir = headTarget.position - target.position;

		//model.localRotation = Quaternion.identity * Quaternion.FromToRotation(fromDir, toDir);
		//Debug.DrawRay(target.position, model.localRotation * Vector3.up, Color.red);
	}

	public override void ResetPos(){
		target.localPosition = defaultPos;
		rot.ResetPos();
	}

}
	
