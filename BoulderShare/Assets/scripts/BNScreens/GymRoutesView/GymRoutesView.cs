using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;
using SA.iOS.UIKit;
using SA.iOS.AVFoundation;
using SA.iOS.Photos;

namespace BoulderNotes{
public class GymRoutesView : BNScreenInput
{
    [SerializeField] private GymRoutesScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private ScrollGradeController scrollGrade;
    [SerializeField] private GymRoutesFinishedToggleController finishedController;
    [SerializeField] private bool showedWithTab ;
    private CVContent_CanTryRoutes[] cvScrollers;
    [SerializeField] private ClassificationView classificationView;
    [SerializeField] private Sprite[] gradeSprites;
    [SerializeField] private Image gradeIcon;
    public override void InitForFirstTransition(){
        if (!showedWithTab){
            scroller.Init();
            scrollGrade.Init();
            finishedController.Init();            
        }else{
            classificationView.Init();
            classificationView.SetonActivateContentAction(UpdateScreen);

            cvScrollers = new CVContent_CanTryRoutes[2];
            for(int i = 0 ; i < cvScrollers.Length ; i++){
                cvScrollers[i] = classificationView.GetContent(i) as CVContent_CanTryRoutes;  
                cvScrollers[i].GetGradeScrollerController().Init();
                cvScrollers[i].GetRoutesScrollerController().Init();      
            }
        }
    }

    public override void UpdateScreen(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            //gymIDだけ記憶させる
            (belongingStack as BNScreenStackWithTargetGym).ClearRoute();
            string name = "";
            if (gym != null){
                if (!showedWithTab){
                    scroller.FetchData(gym.GetRoutes());
                    name = gym.GetGymName();
                    scroller.SetFinishedRoutes(finishedController.IsFinished());
                    scroller.LookUp(scrollGrade.GetCurrentGrade());
                    scrollGrade.SetRouteNum(scroller.GetNumSplitedByGrade());   

                }else{
                    int ind = classificationView.GetCurrentIndex();
                    ScrollGradeController gymScr = cvScrollers[ind].GetGradeScrollerController();
                    GymRoutesScrollerController routeScr = cvScrollers[ind].GetRoutesScrollerController();
                    routeScr.FetchData(gym.GetRoutes());
                    name = gym.GetGymName();
                    routeScr.SetFinishedRoutes(ind == 1);
                    routeScr.LookUp(gymScr.GetCurrentGrade());
                    gymScr.SetRouteNum(routeScr.GetNumSplitedByGrade());
                }
            }
            gymNameText.text = name;

            //gradeIconの設定
            if (gym != null && !string.IsNullOrEmpty(gym.GetGradeTableImagePath())){
                gradeIcon.sprite = gradeSprites[0];
            }else{
                gradeIcon.sprite = gradeSprites[1];
            }
        }
    }
    public void PushGradeIconButton(){
        BNScreenStackWithTargetGym stack = (belongingStack as BNScreenStackWithTargetGym);
        BNGym gym = stack.GetTargetGym();

        if (gym != null && !string.IsNullOrEmpty(gym.GetGradeTableImagePath())){
            BNWallImageNames names = new BNWallImageNames();
            names.fileName = gym.GetGradeTableImagePath();
            stack.SetTargetImageNames(names);
            ToDisplayImageView();
        }else{
            OpenMediaActiveSheet();
        }
    }

    public void LookUpRoutes(BNGradeMap.Grade grade){
        if (!showedWithTab){
            scroller.LookUp(grade);
        }else{
            cvScrollers[classificationView.GetCurrentIndex()].GetRoutesScrollerController().LookUp(grade);
        }
    }

    public void ChangeFinished(bool isFinished){
        scroller.SetFinishedRoutes(isFinished);
        scrollGrade.SetRouteNum(scroller.GetNumSplitedByGrade());
        LookUpRoutes(scrollGrade.GetCurrentGrade());
    }


    public void SaveTargetRouteInStack(string routeID){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetRoute(routeID);
        }
    }

    public void ToRouteView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RouteView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }
    public void ToModifyView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ModifyView, BNScreens.TransitionType.Push);
    }
    public void ToDisplayImageView(){

        BNScreens.Instance.Transition(BNScreens.BNScreenType.DisplayImageView, BNScreens.TransitionType.Fade);
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
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
			ISN_UIAlertController alert = new ISN_UIAlertController("グレード表を選択", null, ISN_UIAlertControllerStyle.ActionSheet);
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
                string fName = SaveGradeImage(result.Image);
                BNWallImageNames names = new BNWallImageNames();
                names.fileName = fName;
                (belongingStack as BNScreenStackWithTargetGym).SetTargetImageNames(names);
                ToDisplayImageView();
            } else {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
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
                string fName = SaveGradeImage(result.Image);
                BNWallImageNames names = new BNWallImageNames();
                names.fileName = fName;
                (belongingStack as BNScreenStackWithTargetGym).SetTargetImageNames(names);
                ToDisplayImageView();
            } else {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
            }
        });
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
                string fName = SaveGradeImage(texture);
                BNWallImageNames names = new BNWallImageNames();
                names.fileName = fName;
                (belongingStack as BNScreenStackWithTargetGym).SetTargetImageNames(names);
                ToDisplayImageView();
                //ReverseTransition();
            }
        }
    }

    private string SaveGradeImage(Texture2D texture){
        BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
        BNGym gym = stack.GetTargetGym();
        BNImage bni = new BNImage(texture, BNGymDataCenter.BNGYM_GRADETABLE);
        gym.SetGradeTableImagePath(bni.fileName);
        stack.ModifyGym(gym, bni);
        stack.StoreTargetGym(gym.GetID());    

        return bni.fileName;    
    }
}
}