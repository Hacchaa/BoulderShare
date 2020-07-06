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
public class GymRoutesView : BNScreenWithGyms
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
    private BNScreenStackWithTargetGym stackWithTargetGym;
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

        stackWithTargetGym = GetScreenStackWithGyms();
    }

    public override void UpdateScreen(){
        if (stackWithTargetGym != null){
            BNGym gym = stackWithTargetGym.GetTargetGym();
            //gymIDだけ記憶させる
            stackWithTargetGym.ClearRoute();
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
        BNGym gym = stackWithTargetGym.GetTargetGym();

        if (gym != null && !string.IsNullOrEmpty(gym.GetGradeTableImagePath())){
            BNWallImageNames names = new BNWallImageNames();
            names.fileName = gym.GetGradeTableImagePath();
            stackWithTargetGym.SetTargetImageNames(names);
            ToDisplayImageViewForGradeTable();
        }else{
            PickImageManager.Instance.OpenMediaActiveSheet(OnLoadGradeTable, "グレード表を選択", null);
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
        if (stackWithTargetGym != null){
            stackWithTargetGym.StoreTargetRoute(routeID);
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
    public void ToDisplayImageViewForImages(){
        stackWithTargetGym.SetTargetDisplayType(DisplayImageView.DisplayType.WallImages);
        BNScreens.Instance.Transition(BNScreens.BNScreenType.DisplayImageView, BNScreens.TransitionType.Fade);
    }
    public void ToDisplayImageViewForGradeTable(){
        stackWithTargetGym.SetTargetDisplayType(DisplayImageView.DisplayType.GradeTable);
        BNScreens.Instance.Transition(BNScreens.BNScreenType.DisplayImageView, BNScreens.TransitionType.Fade);
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    private void OnLoadGradeTable(Texture2D texture){
        string fName = SaveGradeImage(texture);
        BNWallImageNames names = new BNWallImageNames();
        names.fileName = fName;
        stackWithTargetGym.SetTargetImageNames(names);
        ToDisplayImageViewForGradeTable();
    }

    private string SaveGradeImage(Texture2D texture){
        BNGym gym = stackWithTargetGym.GetTargetGym();
        BNImage bni = new BNImage(texture, BNGymDataCenter.BNGYM_GRADETABLE);
        gym.SetGradeTableImagePath(bni.fileName);
        stackWithTargetGym.ModifyGym(gym, bni);
        stackWithTargetGym.StoreTargetGym(gym.GetID());    

        return bni.fileName;    
    }
}
}