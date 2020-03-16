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
    public override void InitForFirstTransition(){
        scroller.Init();
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
            }
            gymNameText.text = name;
        }
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