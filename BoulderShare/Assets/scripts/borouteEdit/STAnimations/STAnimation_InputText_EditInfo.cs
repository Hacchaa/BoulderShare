﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAnimation_InputText_EditInfo : STAnimationFromRightToLeftReverse
{
    public override void Play(){
    	OnPostStartAction = StartAction;
    	OnPostCompleteAction = CompleteAction;
    	Animate();
    }

    private void StartAction(){
    	to.Show();
    }

    private void CompleteAction(){
    	from.Hide();
    }
}