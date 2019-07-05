using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkChest : FBBIKBase
{
	[SerializeField] private float r = 1.0f;
    public override void Init(){
    	base.Init();
    	OnPostBeginDrag += LimitTargetPosition;
    	OnPostEndDrag += LimitTargetPosition;
    }

    protected void LimitTargetPosition(){
    	if (avatar != null){
    		target.position = avatar.position + (target.position - avatar.position).normalized * r;
    	}
    }
}
