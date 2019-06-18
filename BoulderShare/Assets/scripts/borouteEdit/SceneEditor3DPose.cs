using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneEditor3DPose : SceneEditorComponent
{
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private Transform copyModel;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private ThreeDView threeDView;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private Image headCover;
	[SerializeField] private Image headSubmitImage;
	[SerializeField] private Image footCover;
	[SerializeField] private List<GameObject> headObjsInMenu;
	[SerializeField] private List<GameObject> footObjsInMenu;
	[SerializeField] private GameObject handEditorComponent;
	[SerializeField] private FBBIKController ikController;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private float dur = 0.15f;

	private bool isHandEnable = false;
	private bool isRightHand = false;
	private bool isShadowLoaded = false;

	public void CopyJustBeforeScene(){
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);

		if (scene != null){
			humanModel.SetModelPose(scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());
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
			//humanModel.CopyModelPose(copyModel, scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());	
			isShadowLoaded = true;
		}

		if(makeAT.IsPoseSet()){
			humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations(), makeAT.GetRightHandAnim(), makeAT.GetLeftHandAnim());
		}else{
			humanModel.CorrectModelPose();
		}
		humanModel.ShowMarks();
		humanModel.AddOnPostBeginDragActionWithMarks(sceneEditor.MoveHeadIconsWidthWithAnim);
		//humanModel.LookAtModel();
		copyModel.gameObject.SetActive(false);
		headCover.gameObject.SetActive(false);
		headSubmitImage.gameObject.SetActive(false);
	}
	
	public override void OnPreHide(){
		twoDWallMarks.ClearTouch();
		humanModel.InitModelPose();
		humanModel.InitHoldMarkInfo();
		humanModel.RemoveOnPostBeginDragActionWithMarks(sceneEditor.MoveHeadIconsWidthWithAnim);
		isShadowLoaded = false;
		copyModel.gameObject.SetActive(false);
		headCover.gameObject.SetActive(false);
		headSubmitImage.gameObject.SetActive(false);
	}

	public override void Regist(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation(), humanModel.GetRightHandAnim(), humanModel.GetLeftHandAnim());
	}

	public void SwitchShadow(){
		if(isShadowLoaded){
			copyModel.gameObject.SetActive(!copyModel.gameObject.activeSelf);
		}
	}

	public void EditLeftHand(){
		OpenHFEditField(MyUtility.FullBodyMark.LeftHand);
	}
	public void EditLeftFoot(){
		OpenHFEditField(MyUtility.FullBodyMark.LeftFoot);
	}
	public void EditRightHand(){
		OpenHFEditField(MyUtility.FullBodyMark.RightHand);
	}
	public void EditRightFoot(){
		OpenHFEditField(MyUtility.FullBodyMark.RightFoot);
	}
	public void EditLook(){
		OpenHFEditField(MyUtility.FullBodyMark.Head);
	}	

	public void OpenHFEditField(MyUtility.FullBodyMark mark){
		threeDView.SetDragType(ThreeDView.DragType.NoMove);

		ikController.DeactiveMarks();
		ikController.ActiveAimMark(mark);

		Quaternion rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Vector3 pos = ikController.GetAvatar(mark).position;
		float depth = 2.0f;

		cManager.Transform3DWithAnim(pos, rot, depth);

		if (mark == MyUtility.FullBodyMark.LeftHand){
			isHandEnable = true;
			isRightHand = false;
		}else if (mark == MyUtility.FullBodyMark.RightHand){
			isHandEnable = true;
			isRightHand = true; 	
		}else{
			isHandEnable = false;
		}
		PlayOpenHFEditFieldAnim();
	}

	public void CloseHFEditField(){
		threeDView.SetDragType(ThreeDView.DragType.Normal);

		ikController.ActiveMarks();
		ikController.DeactiveAimMarks();

		cManager.Reset3DCamPosAndDepthWithAnim();
		PlayCloseHFEditFieldAnim();
	}

	private void PlayOpenHFEditFieldAnim(){
		List<Image> fadeCoverList = new List<Image>();
		List<GameObject> menuObjs = new List<GameObject>();
		menuObjs.AddRange(footObjsInMenu);
		menuObjs.AddRange(headObjsInMenu);

		fadeCoverList.Add(headCover);
		fadeCoverList.Add(footCover);
		Sequence seq = DOTween.Sequence();
		seq.Append(MyTween.GetUIFadeOutSeq(menuObjs, fadeCoverList, dur));

		List<GameObject> fadeInList = new List<GameObject>();
		fadeInList.Add(headSubmitImage.gameObject);
		if (isHandEnable){
			fadeInList.Add(handEditorComponent);
		}
		seq.Append(MyTween.GetUIFadeInSeq(fadeInList, fadeCoverList, dur));

		seq.Play();
	}

	private void PlayCloseHFEditFieldAnim(){
		List<GameObject> menuObjs = new List<GameObject>();
		menuObjs.AddRange(footObjsInMenu);
		menuObjs.AddRange(headObjsInMenu);

		List<Image> fadeCoverList = new List<Image>();
		fadeCoverList.Add(headCover);
		fadeCoverList.Add(footCover);

		List<GameObject> fadeOutList = new List<GameObject>();
		fadeOutList.Add(headSubmitImage.gameObject);
		if (isHandEnable){
			fadeOutList.Add(handEditorComponent);
		}

		Sequence seq = DOTween.Sequence();
		seq.Append(MyTween.GetUIFadeOutSeq(fadeOutList, fadeCoverList, dur));

		seq.Append(MyTween.GetUIFadeInSeq(menuObjs, fadeCoverList, dur));

		seq.Play();

		isHandEnable = false;
	}

	public void MakeDefaultWithHand(){
		ikController.SetHandAnim(FBBIKController.HandAnim.Default, isRightHand);		
	}
	public void MakeKachiWithHand(){
		ikController.SetHandAnim(FBBIKController.HandAnim.Kachi, isRightHand);		
	}
	public void MakePinchWithHand(){
		ikController.SetHandAnim(FBBIKController.HandAnim.Pinch, isRightHand);		
	}
	public void MakePocketWithHand(){
		ikController.SetHandAnim(FBBIKController.HandAnim.Pocket, isRightHand);		
	}
	public void MakeSloperWithHand(){
		ikController.SetHandAnim(FBBIKController.HandAnim.Sloper, isRightHand);		
	}
}
