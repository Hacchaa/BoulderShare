using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace BoulderNotes{
public class GymRoutesView : BNScreen
{
    [SerializeField] private GymRoutesScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private ScrollGradeController scrollGrade;
    [SerializeField] private GymRoutesFinishedToggleController finishedController;

    public override void InitForFirstTransition(){
        scroller.Init();
        scrollGrade.Init();
        finishedController.Init();
    }

    public override void UpdateScreen(){
        
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            //gymIDだけ記憶させる
            (belongingStack as BNScreenStackWithTargetGym).ClearRoute();
            string name = "";
            if (gym != null){
                scroller.FetchData(gym.GetRoutes());
                name = gym.GetGymName();
                scroller.SetFinishedRoutes(finishedController.IsFinished());
                scroller.LookUp(scrollGrade.GetCurrentGrade());
                scrollGrade.SetRouteNum(scroller.GetNumSplitedByGrade());
            }
            gymNameText.text = name;
        }
    }
    public void LookUpRoutes(BNGradeMap.Grade grade){
        scroller.LookUp(grade);
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
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
}
}