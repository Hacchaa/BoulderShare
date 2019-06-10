using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkBounded : FBBIKMark
{
	[SerializeField] private ThreeDWall threeDWall;

    public override void Init(){
    	base.Init();
    	if (threeDWall != null){
    		OnPostDrag += BoundZ;
    	}
    }

    private void BoundZ(){
    	Vector3 v = threeDWall.CalcWallPoint(target.position);
    	if (v.z < target.position.z){
    		target.position = v;
    	}

    	foreach(Transform t in relativePosList){
    		v = threeDWall.CalcWallPoint(t.position);
    
	    	if (v.z < t.position.z){
	    		t.position = v;
	    	}
	    }
    }
}
