using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;

using SA.iOS.UIKit;
using SA.iOS.AVFoundation;
using SA.iOS.Photos;

namespace BoulderNotes {
public class SelecteWallImageView : BNScreenWithGyms
{
    [SerializeField] private Image image;
    private BNScreenStackWithTargetGym stack;

    public override void InitForFirstTransition(){
        stack = GetScreenStackWithGyms();
    }

    public override void UpdateScreen(){
        
    }

    public void OpenMediaActiveSheet(){
		#if UNITY_IPHONE
        //BNScreens.Instance.Interactive(false);
		if (Application.platform != RuntimePlatform.IPhonePlayer){
            #if UNITY_EDITOR
			LoadImageForEditor();
            #endif
			return ;
		}
			ISN_UIAlertController alert = new ISN_UIAlertController(null, null, ISN_UIAlertControllerStyle.ActionSheet);
			ISN_UIAlertAction cameraAction = new ISN_UIAlertAction("写真を撮る", ISN_UIAlertActionStyle.Default, PickImageFromCamera);
			ISN_UIAlertAction libAction = new ISN_UIAlertAction("アルバムから選ぶ", ISN_UIAlertActionStyle.Default, PickImageFromLibrary);
			ISN_UIAlertAction cancelAction = new ISN_UIAlertAction("キャンセル", ISN_UIAlertActionStyle.Cancel, ()=>{});

			alert.AddAction(cameraAction);
			alert.AddAction(libAction);
			alert.AddAction(cancelAction);
			alert.Present();
		#endif
	}

	public void PickImageFromLibrary(){
		#if UNITY_IPHONE
            ISN_PHAuthorizationStatus s = ISN_PHPhotoLibrary.AuthorizationStatus;
			if (s == ISN_PHAuthorizationStatus.Authorized){
				OpenPhotoLibrary();
				return ;
			}
			ISN_PHPhotoLibrary.RequestAuthorization((status) => {
				if (status == ISN_PHAuthorizationStatus.Authorized){
					OpenPhotoLibrary();
				}else if (status == ISN_PHAuthorizationStatus.StatusDenied){
					string title = "写真へのアクセスが拒否されています";
					string message = "写真アクセスの権限を許可してください。";

                    ISN_UIAlertController alert = new ISN_UIAlertController(title, message, ISN_UIAlertControllerStyle.ActionSheet);
					ISN_UIAlertAction okAction = new ISN_UIAlertAction("OK", ISN_UIAlertActionStyle.Default, () => {});
			        ISN_UIAlertAction setAction = new ISN_UIAlertAction("設定する", ISN_UIAlertActionStyle.Default, () => {});

                    setAction.MakePreferred();
                    alert.AddAction(setAction);
                    alert.AddAction(okAction);
                    alert.Present();
                    //BNScreens.Instance.Interactive(true);
				}else if (status == ISN_PHAuthorizationStatus.Restricted){
					string title = "写真へのアクセスが制限されています";
					string message = "";

                    ISN_UIAlertController alert = new ISN_UIAlertController(title, message, ISN_UIAlertControllerStyle.ActionSheet);
					ISN_UIAlertAction okAction = new ISN_UIAlertAction("OK", ISN_UIAlertActionStyle.Default, () => {});

                    alert.AddAction(okAction);
                    alert.Present();
                    //BNScreens.Instance.Interactive(true);
				}
			});
			return ;
		#endif
	}

	public void PickImageFromCamera(){
		#if UNITY_IPHONE
            ISN_AVAuthorizationStatus s = ISN_AVCaptureDevice.GetAuthorizationStatus(ISN_AVMediaType.Video);
			if (s == ISN_AVAuthorizationStatus.Authorized){
				OpenCamera();
				return ;
			}
			ISN_AVCaptureDevice.RequestAccess(ISN_AVMediaType.Video, (status) => {
				if (status == ISN_AVAuthorizationStatus.Authorized){
					OpenCamera();
				}else if (status == ISN_AVAuthorizationStatus.Denied){
					string title = "カメラへのアクセスが拒否されています";
					string message = "カメラアクセスの権限を許可してください。";

                    ISN_UIAlertController alert = new ISN_UIAlertController(title, message, ISN_UIAlertControllerStyle.ActionSheet);
					ISN_UIAlertAction okAction = new ISN_UIAlertAction("OK", ISN_UIAlertActionStyle.Default, () => {});
			        ISN_UIAlertAction setAction = new ISN_UIAlertAction("設定する", ISN_UIAlertActionStyle.Default, () => {});

                    setAction.MakePreferred();
                    alert.AddAction(setAction);
                    alert.AddAction(okAction);
                    alert.Present();
                    //BNScreens.Instance.Interactive(true);
				}else if (status == ISN_AVAuthorizationStatus.Restricted){
					string title = "カメラへのアクセスが制限されています";
					string message = "";

                    ISN_UIAlertController alert = new ISN_UIAlertController(title, message, ISN_UIAlertControllerStyle.ActionSheet);
					ISN_UIAlertAction okAction = new ISN_UIAlertAction("OK", ISN_UIAlertActionStyle.Default, () => {});

                    alert.AddAction(okAction);
                    alert.Present();
                    //BNScreens.Instance.Interactive(true);
				}
			});
			return ;
		#endif
	}

	private void OpenPhotoLibrary(){
        ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
        picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
        picker.MediaTypes = new List<string>() { ISN_UIMediaType.Image};
        picker.MaxImageSize = 1024;
        picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
        picker.ImageCompressionRate = 0.8f;

        picker.Present((result) => {
            if (result.IsSucceeded) {
                Debug.Log("IMAGE local path: " + result.ImageURL);
                stack.SetTargetSprite(result.Image.ToSprite());
                ReverseTransition();
            } else {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
                ReverseTransition();
            }
        });
	}

	private void OpenCamera(){
        ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
        picker.SourceType = ISN_UIImagePickerControllerSourceType.Camera;
        picker.CameraDevice = ISN_UIImagePickerControllerCameraDevice.Rear;
        picker.MediaTypes = new List<string>() { ISN_UIMediaType.Image};
        picker.MaxImageSize = 1024;
        picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
        picker.ImageCompressionRate = 0.8f;

        picker.Present((result) => {
            if (result.IsSucceeded) {
                Debug.Log("IMAGE local path: " + result.ImageURL);
                stack.SetTargetSprite(result.Image.ToSprite());
                ReverseTransition();
            } else {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
            }
        });
	}

    public void ReverseTransition(float t = 1.0f, int n = 1){
        BNScreens.Instance.ReverseTransition(t, n);
    }

    public void ReverseTransition(){
        ReverseTransition(1.0f, 1);
    }

    private void LoadImageForEditor(){
        #if UNITY_EDITOR
        var path = EditorUtility.OpenFilePanel("load image", "","png");
        if (path.Length != 0) {
            string destination = Application.persistentDataPath + "/" + "pickImage.png";
            if (File.Exists(destination))
                File.Delete(destination);
            File.Copy(path, destination);
            Debug.Log ("PickerOSX:" + destination);
     
            StartCoroutine(LoadImage(destination));
        }      
        #endif
    }


    private IEnumerator LoadImage(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                //Debug.Log("texture:"+texture.width);
                Sprite sprite = Sprite.Create(
                    texture, 
                    new Rect(0.0f, 0.0f, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f),
                    texture.height/4);
                image.sprite = sprite;
                stack.SetTargetSprite(sprite);
                ReverseTransition();
            }
        }
    }
}
}