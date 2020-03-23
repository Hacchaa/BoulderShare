using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class BNScreenViewTemplate: BNScreen
{

    public override void InitForFirstTransition(){
         
    }
    public override void UpdateScreen(){

    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

}
}