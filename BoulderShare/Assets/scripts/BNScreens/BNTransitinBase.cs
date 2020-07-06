using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoulderNotes{

public abstract class BNTransitionBase : MonoBehaviour, ITransitionable
{
    protected BNScreen screen;
    protected RectTransform content;
    protected CanvasGroup contentCG;
    protected Canvas contentCanvas;
    protected RectTransform head;
    protected CanvasGroup headCG;
    protected Canvas headCanvas;

    protected RectTransform headBG;
    protected CanvasGroup headBGCG;
    protected Canvas headBGCanvas;

    protected RectTransform tab;
    protected CanvasGroup tabCG;
    protected Canvas tabCanvas;
    protected bool isAnotherWithHeadBG;
    protected bool isAnotherWithTab;

    protected Action OnComplete;

    public virtual void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        this.screen = screen;
        this.content = content;
        this.head = head;
        this.headBG = headBG;
        this.tab = tab;

        if (content != null){
            content.gameObject.SetActive(true);
            contentCG = content.GetComponent<CanvasGroup>();
            contentCanvas = content.GetComponent<Canvas>();
        }
        if (head != null){
            head.gameObject.SetActive(true);
            headCG = head.GetComponent<CanvasGroup>();
            headCanvas = head.GetComponent<Canvas>();
        }
        if (headBG != null){
            headBG.gameObject.SetActive(true);
            headBGCG = headBG.GetComponent<CanvasGroup>();
            headBGCanvas = headBG.GetComponent<Canvas>();
        }
        if (tab != null){
            tab.gameObject.SetActive(true);
            tabCG = tab.GetComponent<CanvasGroup>();
            tabCanvas = tab.GetComponent<Canvas>();
        }
    }

    public void InitSO(){
        SetSortingOrderWithContent(1);
        SetSortingOrderWithHeadBG(2);
        SetSortingOrderWithHead(3);
        SetSortingOrderWithTab(4);
    }

    public BNScreen GetScreen(){
        return screen;
    }

    public void SetOnCompleteAction(Action act){
        OnComplete += act;
    }
    public void RemoveOnCompleteAction(Action act){
        OnComplete -= act;
    }
    public int GetBiggestSO(){
        int so = 0;
        int biggest = -1;

        so = GetSortingOrderWithContent();
        if (biggest < so){
            biggest = so;
        }

        so = GetSortingOrderWithHead();
        if (biggest < so){
            biggest = so;
        }

        so = GetSortingOrderWithHeadBG();
        if (biggest < so){
            biggest = so;
        }

        so = GetSortingOrderWithTab();
        if (biggest < so){
            biggest = so;
        }

        return biggest;
    }

    public int GetSortingOrderWithContent(){
        if (contentCanvas != null){
            return contentCanvas.sortingOrder;
        }
        return -1;
    }
    public void SetSortingOrderWithContent(int order){
        if (contentCanvas != null){
            contentCanvas.sortingOrder = order;
        }       
    }

    public int GetSortingOrderWithHead(){
        if (headCanvas != null){
            return headCanvas.sortingOrder;
        }
        return -1;
    }
    public void SetSortingOrderWithHead(int order){
        if (headCanvas != null){
            headCanvas.sortingOrder = order;
        }       
    }

    public int GetSortingOrderWithHeadBG(){
        if (headBGCanvas != null){
            return headBGCanvas.sortingOrder;
        }
        return -1;
    }
    public void SetSortingOrderWithHeadBG(int order){
        if (headBGCanvas != null){
            headBGCanvas.sortingOrder = order;
        }       
    }

    public int GetSortingOrderWithTab(){
        if (tabCanvas != null){
            return tabCanvas.sortingOrder;
        }
        return -1;
    }
    public void SetSortingOrderWithTab(int order){
        if (tabCanvas != null){
            tabCanvas.sortingOrder = order;
        }       
    }
    public bool HasHeadBG(){
        return headBG != null;
    }
    public void SetIsAnotherWithHeadBG(bool b){
        isAnotherWithHeadBG = b;
    }

    public bool HasTab(){
        return tab != null;
    }

    public void SetIsAnotherWithTab(bool b){
        isAnotherWithTab = b;
    }

	public abstract void TransitionLerp(float t);
    public virtual void Ready(){
        //screen.ShowScreen();
        //SetAllBlocksRaycasts(false);
        //BNScreens.Instance.Interactive(false);
    }
    public virtual void Complete(bool isReverse){
        BNScreens.Instance.Interactive(true);
        if (OnComplete != null){
            OnComplete();
            OnComplete = null;
        }
    }

    public void SetAllBlocksRaycasts(bool b){
        if (contentCG != null)
            contentCG.blocksRaycasts = b;
        if (headCG != null)
            headCG.blocksRaycasts = b;
        if (tabCG != null)
            tabCG.blocksRaycasts = b;
    }

}
}