using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditInfoView: SEComponentBase
{
	[SerializeField] ScreenTransitionManager trans;
	
	public void ToMainView(){
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

	public void ToInputTextView(){
		trans.Transition(ScreenTransitionManager.Screen.InputTextView);
	}
	public override void OnPreShow(){

	}

	public override void OnPreHide(){

	}
}
