using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkKeepOut : FBBIKBase
{
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private Transform keepOut;
	[SerializeField] private float keepOutLength;

	public override void Init(){
		base.Init();

		OnPostDrag += KeepTargetOut;
	}

	private void KeepTargetOut(){
		Vector3 v = target.position - keepOut.position;
		float length = v.magnitude;
		float minLength = keepOutLength * humanModel.GetModelSize();

		if (length < minLength){
			target.position = keepOut.position + v.normalized * minLength;
		}
	}
}
