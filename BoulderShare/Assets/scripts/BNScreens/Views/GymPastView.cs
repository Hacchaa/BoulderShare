using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class GymPastView : BNScreen
{
    [SerializeField] private GymPastScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymNameText;
    public override void InitForFirstTransition(){
        scroller.Init();
    }

    public override void UpdateScreen(){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            //壁だけ記憶させる
            (belongingStack as BNScreenStackWithTargetGym).ClearWall();
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            string name = "";
            if (gym != null){
                scroller.FetchData(gym.GetWalls());
                name = gym.GetGymName();
            }

            gymNameText.text = name;
        }
    }

    public void SaveTargetWallInStack(BNWall wall){
        (belongingStack as BNScreenStackWithTargetGym).StoreTargetWall(wall.GetID());
    }
    public void ToGymWallView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymWallView, BNScreens.TransitionType.Push);
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
}
}