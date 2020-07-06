using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNTransitionFadeTo : BNTransitionBase
{
    public override void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        base.Init(screen, content, head, headBG, tab);
    }
    override public void TransitionLerp(float t){
        t = Mathf.Lerp(0.0f, 1.0f, t);

        contentCG.alpha = t;
        headCG.alpha = t;
        headBGCG.alpha = t;
    }

    override public void Ready(){
        base.Ready();
    }
    override public void Complete(bool isReverse){
        base.Complete(isReverse);
        if (isReverse){
            if (screen != null){
                screen.HideScreen();
            }
        }
    }
}
}