using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNTransitionFadeFrom : BNTransitionBase
{
    public override void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        base.Init(screen, content, head, headBG, tab);
    }
    override public void TransitionLerp(float t){

    }

    override public void Ready(){
        base.Ready();
    }
    override public void Complete(bool isReverse){
        base.Complete(isReverse);
        if (isReverse){
            SetAllBlocksRaycasts(true);
        }
    }
}
}