using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPoseDetails : SEComponentBase {
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private ThreeD threeD;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private HScenes2 hScenes;

	void Awake(){

	}

	public void ToEditPose(){
		trans.Transition(ScreenTransitionManager.Screen.EditPose);
	}

	public override void OnPreShow(){
	}

	public override void OnPreHide(){

	}
}
