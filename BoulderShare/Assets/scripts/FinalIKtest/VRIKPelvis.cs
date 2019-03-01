using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRIKPelvis : VRIKComponent{
	[SerializeField] private float diff = 0.5f;
	[SerializeField] private VRIKPelvisRot rot;
	//targetの位置を修正
	public override void ModifyPosition(){
		Vector3 v = target.position - transform.position;
		if (v.magnitude > diff){
			//target.position = transform.position + v.normalized * diff;
		}
	}

	public override void ResetPos(){
		target.localPosition = defaultPos;
		rot.ResetPos();
	}

}
	
