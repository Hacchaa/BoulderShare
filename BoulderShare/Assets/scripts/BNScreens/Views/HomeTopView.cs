using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoulderNotes{
public class HomeTopView : BNScreen
{
    [SerializeField] private GymScrollerController scroller;

    public override void InitForFirstTransition(){
        scroller.Init();
    }

    public override void UpdateScreen(){
        scroller.FetchData(BNGymDataCenter.Instance.ReadGyms());
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
        }
    }
    public void ToGymView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }

    public void SaveTargetGymInStack(BNGym gym){
        if (gym == null || string.IsNullOrEmpty(gym.GetID())){
            return ;
        }
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetGym(gym.GetID());
        }
    }
}
}