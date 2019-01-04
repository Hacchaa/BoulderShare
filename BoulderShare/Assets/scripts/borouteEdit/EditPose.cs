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
	private GameObject threeDCamera;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private EditScene es;
	[SerializeField]
	private IKLookAt ikLookAt;


	public void LookButton(){
		ikLookAt.ActivateLooking();
	}

	public void CopyJustBeforeScene(){
		int index = atv.GetCurSceneIndex();
		if (AttemptTreeView.GetSceneType() == (int)AttemptTreeView.SCENETYPE.EDIT){
			index--;
		}
		HScene2 scene = atv.GetScene(index);

		if (scene != null){
			threeD.SetModelPose(scene.GetPose(), scene.GetPRotate());
		}
	}

	public void Submit(){
		es.SetPose(threeD.GetModelPosition());
		es.SetRotate(threeD.GetModelRotation());
		es.SetIsCurLookingActivate(threeD.IsLookingActivate());
		Close();
	}

	public void Close(){
		trans.Transition("EditScene");
	}

	public void ShowProc(){
		if(es.IsPoseDetermined()){
			threeD.SetModelPose(es.GetPose(), es.GetRotate());
			threeD.SetIsLookingActivate(es.IsCurLookingActivate());
			if(es.IsCurLookingActivate()){
				ikLookAt.gameObject.SetActive(true);
			}
		}else{
			threeD.CorrectModelPose();
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		threeD.LookAtModel();

		gameObject.SetActive(true);
		threeDCamera.SetActive(true);
	}

	public void HideProc(){
		ikLookAt.gameObject.SetActive(false);
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		threeDCamera.SetActive(false);
	}
}
