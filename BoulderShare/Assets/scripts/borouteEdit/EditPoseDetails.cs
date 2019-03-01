using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPoseDetails : SEComponentBase {
	[SerializeField] private List<GameObject> externalUIComponents;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private ThreeD threeD;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private HScenes2 hScenes;

	void Awake(){

	}

	public void ToEditPose(){
		trans.Transition("EditPose");
	}

	public override void ShowProc(){
		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
	}

	public override void HideProc(){
		Hide();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
	}

}
