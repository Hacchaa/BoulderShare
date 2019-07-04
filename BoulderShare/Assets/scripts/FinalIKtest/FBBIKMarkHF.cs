using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkHF : FBBIKMarkBounded
{
	[SerializeField] private float holdMarkR;
	[SerializeField] private Vector3 holdMarkPos;
	[SerializeField] private LineRenderer line;

    public override void Init(){
    	base.Init();
    	InitHoldInfo();
    	OnPostDrag += LimitTargetPosition;
    	OnPostCorrectPosition += UpdateLine;
        moveType = FBBIKBase.MoveType.Point;
    }

    public void SetLine(LineRenderer l){
    	line = l;
    }

    //当たり判定半径
    public float GetR(){
        return transform.localScale.x / 2.0f;
    }

    private void LimitTargetPosition(){
    	if (!IsHoldMarkValid()){
    		return ;
    	}
    	Vector3 sub = (target.position - holdMarkPos);
    	float mag = sub.magnitude;

    	if(mag > holdMarkR){
    		target.position = holdMarkPos + sub / (mag / holdMarkR) ;
    	}
    }

    public void SetHoldInfo(float r, Vector3 p){
    	if (line != null){
    		holdMarkPos = p;
    		holdMarkR = r;
    		line.gameObject.SetActive(true);
    	}
    }

    public void InitHoldInfo(){
    	holdMarkR = -1;
    	holdMarkPos = Vector3.zero;
    	if (line != null){
    		line.gameObject.SetActive(false);
    	}
    }

    private void UpdateLine(){
    	if(IsHoldMarkValid()){
	    	Vector3[] p = new Vector3[2];
	    	p[0] = holdMarkPos;
	    	p[1] = GetAvatarOffsetPos();
	    	line.SetPositions(p);
    	}	
    }

    public bool IsHoldMarkValid(){
    	return line != null && holdMarkPos != Vector3.zero && holdMarkR > 0 ;
    }

}
