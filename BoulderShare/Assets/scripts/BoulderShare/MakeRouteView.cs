using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeRouteView : SEComponentBase
{
	private string ts;
	[SerializeField] private MyUtility.BSWall wall;
	[SerializeField] private Image image;
	public void ToRoutesView(){
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.RoutesView);
	}

	public void ToRouteDetailView(){
		ScreenTransitionManager.Instance.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

	public void SaveRoute(){
		ts = DateTime.Now.ToString(MyUtility.FORMAT_TIMESTAMP);
		BSRoute.Instance.SaveRoute(ts);
	}

	public void LoadRoute(){
		if (!String.IsNullOrEmpty(ts)){
			BSRoute.Instance.LoadRoute(ts);
			wall = BSRoute.Instance.GetWall();
			wall.grade = MyUtility.Grade.Q1;

			Texture2D tex = BSRoute.Instance.GetWallImageTexture();
			if (tex != null){
				image.sprite = Sprite.Create(
					tex, 
					new Rect(0.0f, 0.0f, tex.width, tex.height), 
					new Vector2(0.5f, 0.5f)
				);
			}
		}
	}

	public override void OnPreShow(){
        
	}

	public override void OnPreHide(){

	}
}
