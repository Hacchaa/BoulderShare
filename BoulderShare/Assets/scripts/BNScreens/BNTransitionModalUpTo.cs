using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BNTransitionModalUpTo : BNTransitionBase
{
    private RectTransform screenRect;
    public override void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        base.Init(screen, content, head, headBG, tab);
 
        screenRect = screen.GetComponent<RectTransform>();
    }
    override public void TransitionLerp(float t){
		t = Mathf.Lerp(0.0f, 1.0f, t);

        //screenRect
        float height = screenRect.rect.height;
        screenRect.anchoredPosition = new Vector2(0.0f, -height * (1.0f - t));
    }

    override public void Ready(){
        base.Ready();
    }
    override public void Complete(bool isReverse){
        base.Complete(isReverse);
        if (isReverse){
            if (screen != null){
                screen.gameObject.SetActive(false);
            }
        }else{
            SetAllBlocksRaycasts(true);
        }
    }
}
