using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{

public class SecondView : MonoBehaviour
{
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void ToFifthView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.FifthView, BNScreens.TransitionType.Modal);
    }
}
}