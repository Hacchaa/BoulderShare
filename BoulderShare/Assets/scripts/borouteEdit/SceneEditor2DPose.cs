using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEditor2DPose : SceneEditorComponent
{
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private HScenes2 hScenes;

	public override void OnPreShow(){
		twoDWallMarks.ClearTouch();
		string[] str = makeAT.Get2DTouchMarks();
		if (str != null && str.Length > 0){
			twoDWallMarks.SetTouchInfo(str);
		}
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);
		if (scene != null){
			twoDWallMarks.SetDummyTouchInfo(scene.GetOnHolds());
		}
		twoDWallMarks.IgnoreEvents();
	}

	public override void OnPreHide(){
		twoDWallMarks.ClearTouch();
	}

	public override void Regist(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
	}
}
