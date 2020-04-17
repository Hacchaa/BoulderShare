using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

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
}
}