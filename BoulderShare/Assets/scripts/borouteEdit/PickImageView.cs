using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kakera;

public class PickImageView : SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallImage twoDWallImage;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private BorouteLSManager2 bManager;

	public override void OnPreShow(){
		cameraManager.Active2D();
	}

	public override void OnPreHide(){
	}

	public void ToRouteDetailView(){
		twoDWallImage.RotateWallTexture();
		//ファイルに書き込まないように仕様変更
		//bManager.WriteWallImage();
		trans.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

}
