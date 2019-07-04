using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkPelvis : FBBIKMarkKeepOut
{
	[SerializeField] private Transform avatarLeftLeg;
	[SerializeField] private Transform avatarRightLeg;

	public override void Init(){
		base.Init();

		OnPostBeginDrag += MatchTargetPosition;
	}

    public void MatchTargetPosition(){
    	if (avatar != null && avatar.position != target.position){
    		target.position = (avatarLeftLeg.position + avatarRightLeg.position) / 2.0f;
    	}
    }
}
