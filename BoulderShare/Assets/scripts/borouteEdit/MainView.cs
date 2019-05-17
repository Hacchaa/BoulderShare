using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.CrossPlatform.App;
using SA.iOS.Photos;
using SA.iOS.AVFoundation;

public class MainView: SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private HScenes2 hScenes2;
	[SerializeField] private GameObject forEdit;
	[SerializeField] private SceneSelectView ssView;
	[SerializeField] private ThreeDFirstSettingView threeDSettingView;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private GameObject pickImageFrame;
	[SerializeField] private TwoDWallImage twoDWallImage;

	public override void OnPreShow(){
		cameraManager.Active2D();
		cameraManager.Reset2DCamPosAndDepth();
		if (hScenes2.GetNum() == 0){
			forEdit.SetActive(false);
		}else{
			forEdit.SetActive(true);
		}

		pickImageFrame.SetActive(!wallManager.IsWallImagePrepared());
		twoDWallImage.HideTranslucentWall();
	}

	public override void OnPreHide(){

	}

	public void PrintDevice(){
		#if UNITY_IPHONE
		Debug.Log("unity_iphone");
		#else
		Debug.Log("another");
		#endif

		Debug.Log("application.isEditor:"+Application.isEditor);
	}

	public void OpenPhotoLibrary(){
		#if UNITY_IPHONE
			ISN_PHAuthorizationStatus authoStatus = ISN_PHPhotoLibrary.AuthorizationStatus;

			if (authoStatus == ISN_PHAuthorizationStatus.Authorized){
				PickImageFromLibrary();
				return ;
			}


			ISN_PHPhotoLibrary.RequestAuthorization((status) =>{
				if (status == ISN_PHAuthorizationStatus.Authorized){
					PickImageFromLibrary();
				}
			});
			return ;
		#endif
		PickImageFromLibrary();
	}

	public void TakePictureFromNativeCamera(){
		#if UNITY_IPHONE
			ISN_AVAuthorizationStatus authoStatus = ISN_AVCaptureDevice.GetAuthorizationStatus(ISN_AVMediaType.Video);

			if (authoStatus == ISN_AVAuthorizationStatus.Authorized){
				PickImageFromCamera();
				return ;
			}


			ISN_AVCaptureDevice.RequestAccess(ISN_AVMediaType.Video, (status) => {
				if (status == ISN_AVAuthorizationStatus.Authorized){
					PickImageFromCamera();
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

		        wallManager.CommitWallImage(image);
		        trans.Transition(ScreenTransitionManager.Screen.MainView);
		    } else {
		        Debug.Log("failed to pick an image: " + result.Error.FullMessage);
		    }
		});
	}

	private void PickImageFromCamera(){
		var camera = UM_Application.CameraService;

		int maxThumbnailSize = 1024;
		camera.TakePicture(maxThumbnailSize, (result) => {
		   if(result.IsSucceeded) {
		        UM_Media mdeia = result.Media;

		        Texture2D image = mdeia.Thumbnail;
		        Debug.Log("Thumbnail width: " + image.width + " / height: " + image.height);
		        Debug.Log("mdeia.Type: " + mdeia.Type);
		        Debug.Log("mdeia.Path: " + mdeia.Path);
		       	wallManager.CommitWallImage(image);
		        trans.Transition(ScreenTransitionManager.Screen.MainView);
		    } else {
		        Debug.Log("failed to take a picture: " + result.Error.FullMessage);
		    }
		});
	}

	public void ToLayerGraph(){
		trans.Transition(ScreenTransitionManager.Screen.LayerGraphView);
	}

	public void ToMakeAT(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Loop);

		if (threeDSettingView.IsInit()){
			trans.Transition(ScreenTransitionManager.Screen.EditWallMark);
		}else{
			trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);
		}
	}

	public void ToPost(){
		trans.Transition(ScreenTransitionManager.Screen.Post);
	}

	public void ToTry(){
		trans.Transition(ScreenTransitionManager.Screen.TryView);
	}

	public void To3DSetting(){
		trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);
	}
}
