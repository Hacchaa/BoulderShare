using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace BoulderNotes{
public class DisplayImageView: BNScreen
{
    public enum DisplayType{WallImages, GradeTable};
    [SerializeField] private DisplayImageControllerManager controllerManager;
    [SerializeField] private GameObject forWallImagesObj;
    [SerializeField] private GameObject forGradeTableObj;
    private BNWallImageNames imageNames;
    private bool showedEditedImage;
    private BNScreenStackWithTargetGym stack;
    [SerializeField] private Sprite[] wallChangeSprites;
    [SerializeField] private Image wallChangeIcon;
    public override void InitForFirstTransition(){
        stack = belongingStack as BNScreenStackWithTargetGym;
        if (stack == null){
            return ;
        }
        if (stack.GetTargetDisplayType() == DisplayType.WallImages){
            BNRoute route = stack.GetTargetRoute();
            List<BNWallImageNames> list = route.GetWallImageFileNames();
            Sprite[] spriteArr = new Sprite[list.Count*2];
            int i = 0;
            foreach(BNWallImageNames name in list){
                spriteArr[i] = stack.LoadImageByES3(name.fileName);
                i++;
                if(string.IsNullOrEmpty(name.editedFileName)){
                    spriteArr[i] = null;
                }else{
                    spriteArr[i] = stack.LoadImageByES3(name.editedFileName);
                }
                i++;
            }
            
            controllerManager.Init(spriteArr);
            DisplayImage di = controllerManager.GetCurrentDisplayImage();
            if (di.HasEditedSprite()){
                wallChangeIcon.sprite = wallChangeSprites[1];
            }else{
                wallChangeIcon.sprite = wallChangeSprites[0];
            }
            BNManager.Instance.ActivateNecessary(forGradeTableObj, false);
            BNManager.Instance.ActivateNecessary(forWallImagesObj, true);
        }else if(stack.GetTargetDisplayType() == DisplayType.GradeTable){
            BNWallImageNames n = stack.GetTargetImageNames();
            Sprite[] arr = new Sprite[2];
            arr[0] = stack.LoadImageByES3(n.fileName);
            arr[1] = null;
            controllerManager.Init(arr);
            BNManager.Instance.ActivateNecessary(forGradeTableObj, true);
            BNManager.Instance.ActivateNecessary(forWallImagesObj, false);
        }
/*
        if (TemporaryRepository_BNScreens.Instance.bNWallImageNames != null){
            imageNames = TemporaryRepository_BNScreens.Instance.bNWallImageNames;

            if (!string.IsNullOrEmpty(imageNames.editedFileName)){
                showedEditedImage = true;

            }
            controller.Init();
        }*/
    }

    public void ChangeWallImage(){
        DisplayImage di = controllerManager.GetCurrentDisplayImage();
        di.ChangeWallImage();

        if(di.ShowedOrigin()){
            wallChangeIcon.sprite = wallChangeSprites[0];
        }else{
            wallChangeIcon.sprite = wallChangeSprites[1];
        }
    }

    public void ChangeWallImageIcon(DisplayImage di){
        if(di.ShowedOrigin()){
            wallChangeIcon.sprite = wallChangeSprites[0];
        }else{
            wallChangeIcon.sprite = wallChangeSprites[1];
        }        
    }

    public override void UpdateScreen(){

    }    
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void EditGradeTable(){
        #if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.IPhonePlayer){
            #if UNITY_EDITOR
			    OnDeleteAction();
            #endif
			return ;
		}
        PickImageManager.Instance.OpenMediaActiveSheet(OnLoadGradeTable, null, OnSelecteDeleteAction);
        #endif
    }
    private void OnSelecteDeleteAction(){
        PickImageManager.Instance.ShowPopup("グレード表の削除", "このグレード表を削除します。よろしいですか？", "削除する", "キャンセル", OnDeleteAction);
    }
    private void OnDeleteAction(){
        stack.DeleteGradeTable();
        ReverseTransition();
    }
    private void OnLoadGradeTable(Texture2D texture){
        string fName = SaveGradeImage(texture);
        BNWallImageNames names = new BNWallImageNames();
        names.fileName = fName;
        stack.SetTargetImageNames(names);
        stack.SetTargetDisplayType(DisplayType.GradeTable);
        InitForFirstTransition();
    }

    private string SaveGradeImage(Texture2D texture){
        BNGym gym = stack.GetTargetGym();
        BNImage bni = new BNImage(texture, BNGymDataCenter.BNGYM_GRADETABLE);
        gym.SetGradeTableImagePath(bni.fileName);
        stack.ModifyGym(gym, bni);
        stack.StoreTargetGym(gym.GetID());    

        return bni.fileName;    
    }

    public void DeActivateHead(){
        headCG.blocksRaycasts = false;
        headBGCG.blocksRaycasts = false;
    }

    public void ActivateHead(){
        headCG.blocksRaycasts = true;
        headBGCG.blocksRaycasts = true;
    }

    public void SetHeadAlpha(float t){
        headCG.alpha = t;
        headBGCG.alpha = t;
    }

    public float GetHeadAlpha(){
        return headCG.alpha;
    }

}
}