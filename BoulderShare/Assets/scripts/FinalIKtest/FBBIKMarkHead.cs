using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkHead : FBBIKMarkKeepOut
{
	[SerializeField] FBBIKMarkPelvis pelvis;

    public override void Init(){
    	base.Init();

		OnPostBeginDrag += CorrectPelvisTarget;
    }

    private void CorrectPelvisTarget(){
    	pelvis.MatchTargetPosition();
    }
}
