using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;


public abstract class STAnimationBase : MonoBehaviour, ISTAnimation
{
    protected CanvasGroup canvasGroup;
    [SerializeField] private ScreenTransitionManager.Screen fromScreen;
    [SerializeField] private ScreenTransitionManager.Screen toScreen;
    protected RectTransform fromRect;
    protected RectTransform toRect;
    protected SEComponentBase from;
    protected SEComponentBase to;

    protected Action OnPostStartAction = null;
    protected Action OnPostCompleteAction = null;

    public abstract void Animate();
    public abstract void Play();

    public void Init(SEComponentBase f, SEComponentBase t, CanvasGroup cg){
        from = f;
        fromRect = f.transform.GetComponent<RectTransform>();
        to = t;
        toRect = t.transform.GetComponent<RectTransform>();
        canvasGroup = cg;        
    }

    protected void PrioritizeTo(){
    	toRect.SetAsLastSibling();
    }

    protected void PrioritizeFrom(){
    	fromRect.SetAsLastSibling();
    }
    public ScreenTransitionManager.Screen GetFrom(){
        return fromScreen;
    }

    public ScreenTransitionManager.Screen GetTo(){
        return toScreen;
    }

    public void ResetRectPos(RectTransform r){
        r.offsetMin = new Vector2(0.0f, 0.0f);
        r.offsetMax = new Vector2(0.0f, 0.0f);
    }
}
