using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkSP : FBBIKBase
{
	[SerializeField] private Transform limitAvatar;
	[SerializeField] private float limitLength = 1.0f;

    public override void Init(){
    	OnPostDrag += LimitTargetPosition;
    }

    private void LimitTargetPosition(){
    	Vector3 v = target.position - limitAvatar.position;

    	if (v.magnitude > limitLength){
    		target.position = limitAvatar.position + v.normalized * limitLength;
    	}
    }
}
