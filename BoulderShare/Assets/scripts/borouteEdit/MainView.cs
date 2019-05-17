using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.CrossPlatform.App;
using SA.iOS.Photos;
using SA.iOS.AVFoundation;
using SA.CrossPlatform.UI;

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
	[SerializeField] private EditorManager editorManager;

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
