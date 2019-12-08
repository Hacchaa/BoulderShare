using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstView : MonoBehaviour
{
    public void ToSecondView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SecondView, BNScreens.TransitionType.Push);
    }
    public void ToThirdView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ThirdView, BNScreens.TransitionType.Push);
    }
    public void ToFourthView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.FourthView, BNScreens.TransitionType.Push);
    }
}
