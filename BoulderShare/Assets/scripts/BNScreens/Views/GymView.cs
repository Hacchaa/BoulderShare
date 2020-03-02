using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class GymView : BNScreen
{
    [SerializeField] private GymWallScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymNameText;
    public override void InitForFirstTransition(){
        scroller.Init();
    }

    public override void UpdateScreen(){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            //gymIDだけ記憶させる
            (belongingStack as BNScreenStackWithTargetGym).ClearWall();
            string name = "";
            if (gym != null){
                scroller.FetchData(gym.GetWalls());
                name = gym.GetGymName();
            }
            gymNameText.text = name;
        }
    }

    public void SaveTargetWallInStack(string wallID){
        (belongingStack as BNScreenStackWithTargetGym).StoreTargetWall(wallID);
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void ToGymWallView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymWallView, BNScreens.TransitionType.Push);
    }
    public void ToGymPastView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymPastView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }
    public void ToModifyView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ModifyView, BNScreens.TransitionType.Push);
    }
}
}