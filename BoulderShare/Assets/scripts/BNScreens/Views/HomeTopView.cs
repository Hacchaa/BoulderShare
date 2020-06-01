using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class HomeTopView : BNScreenInput
{
    [SerializeField] private GymScrollerController scroller;
    public override void InitForFirstTransition(){
        scroller.Init();
        ClearFields();
    }

    public override void UpdateScreen(){
        scroller.FetchData(BNGymDataCenter.Instance.ReadGyms(), sortType);
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
        }
    }
    public void ToGymView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymRoutesView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }


    public void SaveTargetGymInStack(string gymID){
        if (string.IsNullOrEmpty(gymID)){
            return ;
        }
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetGym(gymID);
        }
    }
}
}