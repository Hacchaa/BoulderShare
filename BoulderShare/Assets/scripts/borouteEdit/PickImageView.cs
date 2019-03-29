using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kakera;

public class PickImageView : SEComponentBase{
	[SerializeField] private PickerController2 pc;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallImage twoDWallImage;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private BorouteLSManager2 bManager;

	public override void OnPreShow(){
		cameraManager.Active2D();
	}

	public override void OnPreHide(){
	}

	public void ToMainView(){
		twoDWallImage.RotateWallTexture();
		bManager.WriteWallImage();
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

}
