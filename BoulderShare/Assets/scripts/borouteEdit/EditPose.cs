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
	private Incline2 inclineUI;
	[SerializeField]
	private Slider modelSizeSlider;
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

	public void ModelSizeButton(){
		modelSizeSlider.transform.parent.gameObject.SetActive(true);
		inclineUI.gameObject.SetActive(false);
	}

	public void InclineButton(){
		inclineUI.gameObject.SetActive(true);
		modelSizeSlider.transform.parent.gameObject.SetActive(false);
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
		SyncIncline();
		SyncModelSize();

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

	public void SyncIncline(){
		inclineUI.SetValue(threeD.GetWallIncline());
	}

	public void SetIncline(int value){
		threeD.SetWallIncline(value);
	}

	public void SyncModelSize(){
		modelSizeSlider.value = threeD.GetModelSize();
	}

	public void SetModelSize(){
		threeD.SetModelSize(modelSizeSlider.value);

	}

	public void OpenIncline(){
		inclineUI.gameObject.SetActive(true);
		modelSizeSlider.transform.parent.gameObject.SetActive(false);
	}

	public void CloseIncline(){
		inclineUI.gameObject.SetActive(false);
	}

	public void OpenModelSize(){
		modelSizeSlider.transform.parent.gameObject.SetActive(true);
		inclineUI.gameObject.SetActive(false);
	}

	public void CloseModelSize(){
		modelSizeSlider.gameObject.SetActive(false);
	}
}
