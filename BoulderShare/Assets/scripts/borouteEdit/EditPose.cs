using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPose : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private AttemptTreeView atv;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private EditScene es;
	[SerializeField] private SceneComments3D comments;


	public void LookButton(){
		threeD.SwitchLookingActive();
	}
	public void ToEditComment(){
		trans.Transition("Edit3DSceneComment");
	}
	public void ShowNextComment(){
		comments.Next();
	}

	public void CopyJustBeforeScene(){
		int index = atv.GetCurSceneIndex();
		if (AttemptTreeView.GetSceneType() == (int)AttemptTreeView.SCENETYPE.EDIT){
			index--;
		}
		HScene2 scene = atv.GetScene(index);

		if (scene != null){
			threeD.SetModelPose(scene.GetPose(), scene.GetRots());
		}
	}

	public void Submit(){
		es.SetPose(threeD.GetModelPosition(), threeD.GetModelRotation());
		es.SetIsCurLookingActivate(threeD.IsLookingActive());
		Close();
	}

	public void Close(){
		trans.Transition("EditScene");
	}

	public void ShowProc(){
		if(es.IsPoseDetermined()){
			threeD.SetModelPose(es.GetPose(), es.GetRots());
			threeD.SetIsLookingActive(es.IsCurLookingActivate());
			if(es.IsCurLookingActivate()){
				threeD.SetIsLookingActive(true);
			}
		}else{
			threeD.CorrectModelPose();
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		threeD.LookAtModel();

		gameObject.SetActive(true);
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_VIEW);
		//Debug.Break();
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
	}
}
