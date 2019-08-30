using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutesView : SEComponentBase
{
	public void ToRouteDetailView(){
		//BSRoute.Instance.LoadRoute(System.DateTime.Now.ToString("yyyyMMddHHmmss"));
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

	public void MakeRoute(){
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.MakeRouteView);
	}

	public override void OnPreShow(){
        
	}

	public override void OnPreHide(){

	}
}
