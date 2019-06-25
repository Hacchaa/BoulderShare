using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTextView : SEComponentBase
{
	[SerializeField] private ScreenTransitionManager trans;

	public override void OnPreShow(){
	}
	public override void OnPreHide(){
	}

	public void ToEditInfo(){
		trans.Transition(ScreenTransitionManager.Screen.EditInfoView);
	}
}
