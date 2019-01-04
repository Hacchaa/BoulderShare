using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeDFirstSettingView : MonoBehaviour, IUIComponent
{
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager sManager;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private InclineSetter incline;
	[SerializeField]
	private ModelSizeSetter modelSize;

	public void ShowProc(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		gameObject.SetActive(true);
		threeD.InitModelPose();
		threeD.ResetCamPos();

		incline.SyncInclineValue();
		modelSize.SyncModelSize();
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	public void Submit(){
		sManager.Transition("AttemptTreeView");
	}
}
