using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditInfoView: SEComponentBase
{
	[SerializeField] ScreenTransitionManager trans;
	
	public void ToRouteDetailView(){
		trans.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

	public void ToInputTextView(){
		trans.Transition(ScreenTransitionManager.Screen.InputTextView);
	}
	public override void OnPreShow(){

	}

	public override void OnPreHide(){

	}
}
