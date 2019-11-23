using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAnimation_Info_Main :STAnimationFromBotToTopReverse
{
    public override void Play(){
    	OnPostStartAction = StartAction;
    	OnPostCompleteAction = CompleteAction;
    	Animate();
    }

    private void StartAction(){
        from.OnPreHide();
    	to.ShowScreen();
    }

    private void CompleteAction(){
    	from.HideScreen();
    }
}
