using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRouteView : SEComponentBase
{

	public void ToRoutesView(){
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.RoutesView);
	}

	public void ToRouteDetailView(){
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

	public void SaveRoute(){
		BSRoute.Instance.SaveRoute(DateTime.Now.ToString(MyUtility.FORMAT_TIMESTAMP));
	}

	public override void OnPreShow(){
        
	}

	public override void OnPreHide(){

	}
}
