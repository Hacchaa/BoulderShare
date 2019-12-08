using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BNTransitionController
{
    [SerializeField] private BNTransitionBase from;
    [SerializeField] private BNTransitionBase to;
    private BNScreens.TransitionType type;

    [SerializeField] private float duration = 0.5f;
    private bool isReverse ;

    public BNTransitionController(BNTransitionBase from, BNTransitionBase to, BNScreens.TransitionType type){
        this.from = from;
        this.to = to;
        this.type = type;
    }

    public BNScreens.TransitionType GetTransitionType(){
        return type;
    }


    public BNTransitionBase GetFrom(){
    	return from;
    }

    public BNTransitionBase GetTo(){
    	return to;
    }

    public void BNTransitionLerp(float t){
        t = Mathf.Lerp(0.0f, 1.0f, t);
    	from.TransitionLerp(t);
    	to.TransitionLerp(t);
    }

    public void BNTransitionWithAnim(float t){
        float dist ;
    	t = Mathf.Lerp(0.0f, 1.0f, t);
    	if (!isReverse){
            dist = 1.0f;
        }else{
            dist = 0.0f;
        }

    	DOVirtual.Float(t, dist, duration, value => {
    		BNTransitionLerp(value);
    	}).SetEase(Ease.OutQuart).OnComplete(() =>{
            Complete();
        });
    }

    public void BNTransitionWithAnim(){
        Ready(false);
    	BNTransitionWithAnim(0.0f);
    }

    public void ReverseBNTransitionWithAnim(float t = 1.0f){
        Ready(true);
        BNTransitionWithAnim(t);
    }

    public void UpdateSO(){
        bool fromHasHead = from.HasHeadBG();
        bool toHasHead = to.HasHeadBG();
        int baseSO = from.GetSortingOrderWithContent();
        int so ;

        if (type == BNScreens.TransitionType.Push){

            if ((fromHasHead && toHasHead) || (!fromHasHead && toHasHead)){
                so = baseSO+2;
                to.SetSortingOrderWithContent(so);

                so = baseSO+3;
                from.SetSortingOrderWithHeadBG(so);
                to.SetSortingOrderWithHeadBG(so);
                
                so = baseSO+4;
                from.SetSortingOrderWithHead(so);
                to.SetSortingOrderWithHead(so);

                if (from.HasTab() && !to.HasTab()){
                    so = baseSO+1;
                }else{
                    so = baseSO+3;
                }
                from.SetSortingOrderWithTab(so);
                to.SetSortingOrderWithTab(so);
            
            }else if(fromHasHead && !toHasHead){
                so = baseSO+1;
                from.SetSortingOrderWithHeadBG(so);

                so = baseSO+2;
                from.SetSortingOrderWithHead(so);

                so = baseSO+3;
                to.SetSortingOrderWithContent(so);

                so = baseSO+4;
                to.SetSortingOrderWithHeadBG(so);

                so = baseSO+5;
                to.SetSortingOrderWithHead(so);

                if (from.HasTab() && !to.HasTab()){
                    so = baseSO+2;
                }else{
                    so = baseSO+4;
                }
                from.SetSortingOrderWithTab(so);
                to.SetSortingOrderWithTab(so);
            
            }else{
                if (from.HasTab() && !to.HasTab()){
                    so = baseSO+1;
                    from.SetSortingOrderWithTab(so);  
                    
                    so = baseSO+2;  
                    to.SetSortingOrderWithContent(so);          
                }else{
                    so = baseSO+1;
                    to.SetSortingOrderWithContent(so);
                    
                    so = baseSO+2;
                    from.SetSortingOrderWithTab(so);
                    to.SetSortingOrderWithTab(so);
                }
            }

        }else if(type == BNScreens.TransitionType.Modal){
            //toのcontentがfromのどの要素より前に来るようにする
            so = from.GetBiggestSO();
            to.SetSortingOrderWithContent(so+1);
            to.SetSortingOrderWithHeadBG(so+2);
            to.SetSortingOrderWithHead(so+3);
            to.SetSortingOrderWithTab(so+4);
        }        
    }

    //transition なのかreverseTransitionなのかを入れ替える
    public void SwitchTransitionDirection(){
        isReverse = !isReverse;
    }

    public void Ready(bool isReverse){
        this.isReverse = isReverse;

        from.SetIsAnotherWithHeadBG(to.HasHeadBG());
        to.SetIsAnotherWithHeadBG(from.HasHeadBG());

        from.SetIsAnotherWithTab(to.HasTab());
        to.SetIsAnotherWithTab(from.HasTab());
        
        //determine sorting order. 
        UpdateSO();

        from.Ready();
        to.Ready();
    }

    public void Complete(){
        from.Complete(isReverse);
        to.Complete(isReverse);
    }
}
