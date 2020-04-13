using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class BNTransitionModalUpFrom : BNTransitionBase
{
    private float screenDownRatio = 0.0f;
    private float screenShrinkRatio = 0.1f;
    private RectTransform screenRect;
    public override void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        base.Init(screen, content, head, headBG, tab);
 
        screenRect = screen.GetComponent<RectTransform>();
    }
    override public void TransitionLerp(float t){
		t = Mathf.Lerp(0.0f, 1.0f, t);

        //screenRect
        float height = screenRect.rect.height * screenDownRatio;
        screenRect.anchoredPosition = new Vector2(0.0f, -height * t);
        screenRect.transform.localScale = Vector3.one * (1.0f - screenShrinkRatio * t);
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