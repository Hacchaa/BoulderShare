using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace BoulderNotes{
public class DisplayImageView: BNScreen
{
    private BNScreenInput screenInput;
    [SerializeField] private MoveImageController2 controller;

    public override void InitForFirstTransition(){
        screenInput = null;
        if (belongingStack == null || !(belongingStack is BNScreenStackWithTargetGym)){
            return ;
        }
        BNScreen screen = (belongingStack as BNScreenStackWithTargetGym).GetPreviousScreen(1);
        if (screen is BNScreenInput){
            screenInput = screen as BNScreenInput;
            controller.Init(screenInput.GetSprite());
        }

    }

    public override void UpdateScreen(){

    }    
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
}
}