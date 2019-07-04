using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkKeepOut : FBBIKBase
{
	[SerializeField] private Transform keepOut;
	[SerializeField] private float keepOutLength;

	public override void Init(){
		base.Init();

		OnPostDrag += KeepTargetOut;
	}

	private void KeepTargetOut(){
		Vector3 v = target.position - keepOut.position;
		float length = v.magnitude;

		if (length < keepOutLength){
			target.position = keepOut.position + v.normalized * keepOutLength;
		}
	}
}
