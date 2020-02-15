using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class ThirdView : BNScreen
{
    [SerializeField] private ClassificationView view;

    public override void InitForFirstTransition(){
        if (view != null){
            view.Init();
        }
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
     public void ToSecondView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SecondView, BNScreens.TransitionType.Push);
    }
}
}