using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKPoleAimed : FBBIKBase
{    
	[SerializeField] private float planeDist = 0.5f;
	[SerializeField] private Transform aimedTarget;
	private Plane plane ;

	public override void Init(){
        base.Init();
		OnPostBeginDrag += DeterminePlane;
		OnPostBeginDrag += HideAimTarget;
    	OnPostDrag += LimitTargetPosition;
    	OnPostEndDrag += ShowAimTarget;
    	moveType = FBBIKBase.MoveType.Point;
    }

    private void HideAimTarget(){
    	aimedTarget.gameObject.SetActive(false);
    }

    private void ShowAimTarget(){
    	aimedTarget.gameObject.SetActive(true);
    }

    public void Activate(){
    	gameObject.SetActive(true);
    }

    public void Deactivate(){
    	gameObject.SetActive(false);
    }

    public float GetPlaneDist(){
    	return planeDist;
    }

    private void DeterminePlane(){
    	Vector3 normal = (aimedTarget.position - avatar.position).normalized;

    	plane = new Plane(normal, avatar.position);
    }
    public void DeterminePosition(Vector3 axis){
		target.position = avatar.TransformPoint(axis);
		target.position = avatar.position + (target.position - avatar.position).normalized * planeDist;
    }

    private void LimitTargetPosition(){
    	Vector3 v = plane.ClosestPointOnPlane(target.position);
    	target.position = avatar.position + (v - avatar.position).normalized * planeDist;
    }
}
