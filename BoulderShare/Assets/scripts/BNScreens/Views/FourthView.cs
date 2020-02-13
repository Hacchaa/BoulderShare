using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class FourthView : MonoBehaviour
{
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void ToSixthView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SixthView, BNScreens.TransitionType.Push);
    }
}
}