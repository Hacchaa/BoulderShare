using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BSScreenTransition : MonoBehaviour
{
	public enum TransitionType {Go, Peer}

    [SerializeField] ScreenTransitionManager.Screen fromScreen;
    [SerializeField] ScreenTransitionManager.Screen toScreen;
    [SerializeField] private BSTransitionBase fromTrans;
    [SerializeField] private BSTransitionBase toTrans;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private TransitionType transitionType;
    private float latestT;


    public void Init(){
        fromTrans.Init();
        toTrans.Init();
        latestT = -1.0f;
    }

    public TransitionType GetTransitionType(){
        return transitionType;
    }

    public ScreenTransitionManager.Screen GetFrom(){
    	return fromScreen;
    }

    public ScreenTransitionManager.Screen GetTo(){
    	return toScreen;
    }

    public void BSTransitionLerp(float t){
        t = Mathf.Lerp(0.0f, 1.0f, t);
        latestT = t;
    	fromTrans.TransitionLerp(t);
    	toTrans.TransitionLerp(t);
    }

    public void BSTransitionWithAnim(float t, bool isReverse = false){
        float dist ;
    	t = Mathf.Lerp(0.0f, 1.0f, t);
    	if (!isReverse){
            dist = 1.0f;
        }else{
            dist = 0.0f;
        }

    	DOVirtual.Float(t, dist, duration, value => {
    		BSTransitionLerp(value);
    	}).SetEase(Ease.OutQuart).OnComplete(() =>{
            Complete(isReverse);
        });
    }

    public void BSTransitionWithAnim(){
        Ready();
        if (latestT < 0.0f){
            latestT = 0.0f;
        }
    	BSTransitionWithAnim(latestT);
    }

    public void ReverseBSTransitionWithAnim(){
        toTrans.UpdateScreen();
        Ready(true);
        if (latestT < 0.0f){
            latestT = 1.0f;
        }
        BSTransitionWithAnim(latestT, true);
    }

    public void Ready(bool isReverse = false){
        fromTrans.Activate(false);
        toTrans.Activate(false);
        fromTrans.Ready(isReverse);
        toTrans.Ready(isReverse);
        CorrectSortingOrder();
    }

    public void Complete(bool isReverse){
        bool isFromShowed = isReverse;

        fromTrans.Complete(isFromShowed);
        toTrans.Complete(!isFromShowed);

        if (isReverse){
            fromTrans.Activate(true);
            toTrans.DeActivate(false);
        }else{
            fromTrans.DeActivate(false);
            toTrans.Activate(true);
        }
/*
        switch(transitionType){
            case TransitionType.Go:
                if (isReverse){
                    fromTrans.Activate(true);
                    toTrans.DeActivate(false);
                }else{
                    fromTrans.Activate(false);
                    toTrans.Activate(true);
                }
                break;

            case TransitionType.Peer:
                if (isReverse){
                    fromTrans.Activate(true);
                    toTrans.DeActivate(false);
                }else{
                    fromTrans.DeActivate(false);
                    toTrans.Activate(true);
                }
                break;                           
        }*/
        latestT = -1.0f;
    }

    private void CorrectSortingOrder(){
        int fromIndex = fromTrans.GetSortingOrder();
        int toIndex = toTrans.GetSortingOrder();

        if (fromIndex > toIndex){
            toTrans.SetSortingOrder(fromIndex);
            fromTrans.SetSortingOrder(toIndex);
        }
    }

    public void UpdateScreenStack(Stack<ScreenTransitionManager.Screen> stack, bool isReverse){
        switch(transitionType){
            case TransitionType.Go:
                if (isReverse){
                    stack.Pop();
                    if (stack.Any() && stack.Peek() != fromScreen){
                        stack.Push(fromScreen);
                    }
                }else{
                    stack.Push(toScreen);
                }
                break;

            case TransitionType.Peer:
                if (isReverse){
                    stack.Pop();
                    stack.Push(fromScreen);
                }else{
                    stack.Pop();
                    stack.Push(toScreen);
                }
                break;                           
        }        
    }
}
