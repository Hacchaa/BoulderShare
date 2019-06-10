using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class FBBIKMarkSP : FBBIKMarkBounded
{
	[SerializeField] private Transform limitAvatar;
	[SerializeField] private float limitLength = 1.0f;
    [SerializeField] private FBBIKMarkSP another;
    [SerializeField] private RotationLimit limit;


    public override void Init(){
        base.Init();
    	OnPostDrag += LimitTargetPosition;
        OnPostEndDrag += FitPositions;
    }

    public void ApplyRotationLimit(){
        if (limit != null){
            //Debug.Log("apply:"+gameObject.name);
            limit.Apply();
        }
    }

    private void LimitTargetPosition(){
    	Vector3 v = target.position - limitAvatar.position;

    	if (v.magnitude > limitLength){
    		target.position = limitAvatar.position + v.normalized * limitLength;
    	}
    }

    private void FitPositions(){
        MatchTargetPosition();
        if (another != null){
            another.MatchTargetPosition();
        }
    }
}
