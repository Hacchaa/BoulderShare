using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMark : FBBIKBase
{
    public override void Init(){
    	OnPostBeginDrag += MatchTargetPosition;
    	OnPostEndDrag += MatchTargetPosition;
    }

    protected void MatchTargetPosition(){
    	if (avatar != null){
    		target.position = avatar.position;
    	}
    }
}
