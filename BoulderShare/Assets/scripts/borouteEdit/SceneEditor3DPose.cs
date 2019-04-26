using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEditor3DPose : SceneEditorComponent
{
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private Transform copyModel;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	private bool isShadowLoaded = false;

	public void CopyJustBeforeScene(){
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);

		if (scene != null){
			humanModel.SetModelPose(scene.GetPose(), scene.GetRots());
		}
	}

	public void CorrectModelPose(){
		humanModel.CorrectModelPose();
	}

	public override void OnPreShow(){
		string[] holds = makeAT.Get2DTouchMarks();
		twoDWallMarks.SetTouchInfo(holds);
		for(int i = (int)TwoDMark.HFType.RH ; i <= (int)TwoDMark.HFType.LF ; i++){
			humanModel.SetHoldMarkInfo((TwoDMark.HFType)i, threeDWallMarks.GetMarkR(holds[i]), threeDWallMarks.GetMarkWorldPos(holds[i]));
		}

		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);
		isShadowLoaded = false;
		if (scene != null){
			humanModel.CopyModelPose(copyModel, scene.GetPose(), scene.GetRots());	
			isShadowLoaded = true;
		}

		if(makeAT.IsPoseSet()){
			humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		}else{
			humanModel.CorrectModelPose();
		}
		humanModel.ShowMarks();
		//humanModel.LookAtModel();
		copyModel.gameObject.SetActive(false);
	}
	
	public override void OnPreHide(){
		twoDWallMarks.ClearTouch();
		humanModel.InitModelPose();
		humanModel.InitHoldMarkInfo();
		isShadowLoaded = false;
		copyModel.gameObject.SetActive(false);
	}

	public override void Regist(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation());
	}

	public void SwitchShadow(){
		if(isShadowLoaded){
			copyModel.gameObject.SetActive(!copyModel.gameObject.activeSelf);
		}
	}
}
