using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes {
public class GymWallView : BNScreen
{
    [SerializeField] private GymRouteScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymWallText;
    public override void InitForFirstTransition(){
        scroller.Init();
    }

    public override void UpdateScreen(){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            //gymIDとwallIDだけ記憶
            (belongingStack as BNScreenStackWithTargetGym).ClearRoute();
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            BNWall wall = (belongingStack as BNScreenStackWithTargetGym).GetTargetWall();

            string name = "";
            if (wall != null){
                scroller.FetchData(BNGymDataCenter.Instance.ReadRoutes(wall, gym.GetID()));
                name = WallTypeMap.Entity.GetWallTypeName(wall.GetWallType());
            }

            gymWallText.text = name;
        }
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void SaveTargetRouteInStack(BNRoute route){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetRoute(route);
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

}
}