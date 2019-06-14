﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.CrossPlatform.App;
using SA.iOS.Photos;
using SA.iOS.AVFoundation;
using SA.CrossPlatform.UI;

public class MainView: SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private HScenes2 hScenes2;
	[SerializeField] private ThreeDFirstSettingView threeDSettingView;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private EditorManager editorManager;
	[SerializeField] private Image wallImageOnUI;
	[SerializeField] private GameObject noImageContent;
	[SerializeField] private Button makeButton;

	public override void OnPreShow(){
		cameraManager.Active2D();
		cameraManager.Reset2DCamPosAndDepth();

		if (wallManager.IsWallImagePrepared()){
			wallImageOnUI.gameObject.SetActive(true);
			noImageContent.SetActive(false);
			makeButton.interactable = true;
		}else{
			wallImageOnUI.gameObject.SetActive(false);
			noImageContent.SetActive(true);
			makeButton.interactable = false;
		}

		wallManager.HideTranslucentWall();
	}

	public override void OnPreHide(){

	}

	public void OpenPhotoLibrary(){
		#if UNITY_IPHONE
			ISN_PHAuthorizationStatus s = ISN_PHPhotoLibrary.AuthorizationStatus;
			if (s == ISN_PHAuthorizationStatus.Authorized){
				PickImageFromLibrary();
				return ;
			}
			ISN_PHPhotoLibrary.RequestAuthorization((status) =>{
				if (status == ISN_PHAuthorizationStatus.Authorized){
					PickImageFromLibrary();
				}else if (status == ISN_PHAuthorizationStatus.StatusDenied){
					string title = "写真へのアクセスが拒否されています";
					string message = "写真アクセスの権限を許可してください。";
					var builder = new UM_NativeDialogBuilder(title, message);
					builder.SetPositiveButton("OK", () =>{
						editorManager.ExitImmediately();
					});
					var dialog = builder.Build();
					dialog.Show();
				}else if (status == ISN_PHAuthorizationStatus.Restricted){
					string title = "写真へのアクセスが制限されています";
					string message = "";
					var builder = new UM_NativeDialogBuilder(title, message);
					builder.SetPositiveButton("OK", () =>{
						editorManager.ExitImmediately();
					});
					var dialog = builder.Build();
					dialog.Show();
				}
			});
			return ;
		#endif
		PickImageFromLibrary();
	}

	public void TakePictureFromNativeCamera(){
		#if UNITY_IPHONE
			ISN_AVAuthorizationStatus s = ISN_AVCaptureDevice.GetAuthorizationStatus(ISN_AVMediaType.Video);
			if (s == ISN_AVAuthorizationStatus.Authorized){
				PickImageFromCamera();
				return ;
			}

			ISN_AVCaptureDevice.RequestAccess(ISN_AVMediaType.Video, (status) => {
				if (status == ISN_AVAuthorizationStatus.Authorized){
					PickImageFromCamera();
				}else if (status == ISN_AVAuthorizationStatus.Denied){
					string title = "カメラへのアクセスが拒否されています";
					string message = "カメラアクセスの権限を許可してください。";
					var builder = new UM_NativeDialogBuilder(title, message);
					builder.SetPositiveButton("OK", () =>{
						editorManager.ExitImmediately();
					});
					var dialog = builder.Build();
					dialog.Show();
				}else if (status == ISN_AVAuthorizationStatus.Restricted){
					string title = "カメラへのアクセスが制限されています";
					string message = "";
					var builder = new UM_NativeDialogBuilder(title, message);
					builder.SetPositiveButton("OK", () =>{
						editorManager.ExitImmediately();
					});
					var dialog = builder.Build();
					dialog.Show();					
				}
			});
			return ;
		#endif
		PickImageFromCamera();
	}


	private void PickImageFromLibrary(){
		var gallery = UM_Application.GalleryService;

		int maxThumbnailSize = 1024;
		gallery.PickImage(maxThumbnailSize, (result) => {
		    if (result.IsSucceeded) {
		        UM_Media media = result.Media;
		        
		        Texture2D image = media.Thumbnail;
		        Debug.Log("Thumbnail width: " + image.width + " / height: " + image.height);
		        Debug.Log("mdeia.Type: " + media.Type);
		        Debug.Log("mdeia.Path: " + media.Path);

		        ApplyWallImage(image);
		        trans.Transition(ScreenTransitionManager.Screen.MainView);
		    } else {
		        Debug.Log("failed to pick an image: " + result.Error.FullMessage);
		    }
		});
	}

	private void PickImageFromCamera(){
		//Debug.Log("PickImage");
		var camera = UM_Application.CameraService;

		int maxThumbnailSize = 1024;
		camera.TakePicture(maxThumbnailSize, (result) => {
		   if(result.IsSucceeded) {
		        UM_Media mdeia = result.Media;

		        Texture2D image = mdeia.Thumbnail;
		        Debug.Log("Thumbnail width: " + image.width + " / height: " + image.height);
		        Debug.Log("mdeia.Type: " + mdeia.Type);
		        Debug.Log("mdeia.Path: " + mdeia.Path);

		       	ApplyWallImage(image);
		        trans.Transition(ScreenTransitionManager.Screen.MainView);
		    } else {
		        Debug.Log("failed to take a picture: " + result.Error.FullMessage);
		    }
		});
	}

	private void ApplyWallImage(Texture2D texture){
		wallManager.CommitWallImage(texture);

		wallImageOnUI.sprite = 
			Sprite.Create(
		        texture, 
		        new Rect(0.0f, 0.0f, texture.width, texture.height), 
		        new Vector2(0.5f, 0.5f)
		    );
	}

	public void ToLayerGraph(){
		trans.Transition(ScreenTransitionManager.Screen.LayerGraphView);
	}

	public void ToMakeAT(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Loop);

		if (threeDSettingView.IsInit()){
			if (hScenes2.IsATListEmpty()){
				trans.Transition(ScreenTransitionManager.Screen.SceneEditor);
			}else{
				hScenes2.LoadLatestAT();
				AttemptTreeMenu.mode = AttemptTreeMenu.Mode.Menu;
				trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
			}
		}else{
			trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);
		}
	}

	public void ToPost(){
		trans.Transition(ScreenTransitionManager.Screen.Post);
	}

	public void To3DSetting(){
		trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);
	}
}
