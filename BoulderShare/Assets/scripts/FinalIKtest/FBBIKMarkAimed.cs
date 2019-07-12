using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkAimed : FBBIKBase
{    
	[SerializeField] private float limitR = 0.5f;
	[SerializeField] private FBBIKPoleAimed poleTarget;
	[SerializeField] private FBBAimIKComponent aimIKCom;
    private float modelSize;

	public override void Init(){
        base.Init();
    	OnPostDrag += LimitTargetPosition;
    	OnPostBeginDrag += HidePole;
    	OnPostEndDrag += ShowPole;
    }
    public void StoreModelSize(float size){
        modelSize = size;
    }

    private void HidePole(){
    	poleTarget.gameObject.SetActive(false);
    }
    private void ShowPole(){
    	aimIKCom.DeterminePolePosition();
    	poleTarget.gameObject.SetActive(true);
    }
    public void Activate(){
    	gameObject.SetActive(true);
    }

    public void Deactivate(){
    	gameObject.SetActive(false);
    }

    public float GetLimitR(){
    	return limitR;
    }

    public void DeterminePosition(Vector3 axis){
		target.position = avatar.TransformPoint(axis);
		target.position = avatar.position + (target.position - avatar.position).normalized * limitR * modelSize;
    }

    private void LimitTargetPosition(){
    	//Debug.Log("avatar:"+avatar.localRotation.eulerAngles);
    	Vector3 sub = (target.position - avatar.position);

    	target.position = avatar.position + sub.normalized * limitR * modelSize;

    }
}
