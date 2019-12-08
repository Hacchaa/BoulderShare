using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdView : MonoBehaviour
{
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
     public void ToSecondView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SecondView, BNScreens.TransitionType.Push);
    }
}
