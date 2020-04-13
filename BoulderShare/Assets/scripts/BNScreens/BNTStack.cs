using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BoulderNotes{
public class BNTStack : MonoBehaviour
{
    [SerializeField] private BNScreens.BNScreenType initialView;
    private List<BNScreen> screenList;
    private Stack<BNTransitionController> controllers;

    private Stack<RectTransform> levels;

    private int restRTCount;
    public virtual void Init(){
        screenList = new List<BNScreen>();
        controllers = new Stack<BNTransitionController>();
        levels = new Stack<RectTransform>();
        restRTCount = 0;

        GameObject obj = new GameObject("Level0");
        RectTransform rect = obj.AddComponent<RectTransform>();
        obj.transform.SetParent(this.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        levels.Push(rect);
    }

    public void ActivateStack(){
        gameObject.SetActive(true);
        if (!screenList.Any()){
            Transition(initialView, BNScreens.TransitionType.Push);
        }else{
            screenList[screenList.Count-1].UpdateScreen();            
        }
    }

    public void DeactivateStack(){
        gameObject.SetActive(false);
    }

    public BNScreen GetPreviousScreen(int prevCount){
        return screenList[screenList.Count - 1 - prevCount];
    }

    public void Transition(BNScreens.BNScreenType screenType, BNScreens.TransitionType t){
        Transform parent = null;
        if (t == BNScreens.TransitionType.Push){
            parent = levels.Peek();
        }else if(t == BNScreens.TransitionType.Modal || t == BNScreens.TransitionType.Fade){
            ToNextLevel();
            parent = levels.Peek();
        }

        BNScreen current = BNScreens.Instance.RecycleScreen(screenType, parent);
        if (current == null){
            current = BNScreens.Instance.MakeScreen(screenType, parent);
        }
        screenList.Add(current);

        current.gameObject.SetActive(true);
        current.SetBelongingStack(this);
        current.InitForFirstTransition();
        current.UpdateScreen();
        if (screenList.Count == 1){
            return ;
        }

        BNScreen prev = screenList[screenList.Count-2];
        BNTransitionBase curTrans = current.GetTransition(t, false);
        BNTransitionBase prevTrans = prev.GetTransition(t, true);

        //Debug.Log("current:"+curTrans.gameObject.name);
        //Debug.Log("prev:"+prevTrans.gameObject.name);
        BNTransitionController controller = new BNTransitionController(prevTrans, curTrans, t);
        //controller.SetDuration(10.0f);
        controller.BNTransitionWithAnim();
        controllers.Push(controller);

    }

    private void ToNextLevel(){
        GameObject obj = new GameObject("Level" + levels.Count());
        RectTransform rect = obj.AddComponent<RectTransform>();
        obj.transform.SetParent(this.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        levels.Push(rect);        
    }

    private void ToPrevLevel(){
        Destroy(levels.Pop().gameObject);
    }

    public void UpdateLatestSO(){
        if (!controllers.Any()){
            if (!screenList.Any()){
                 return ;
            }
            screenList[screenList.Count-1].InitSO();
            return ;
        }

        controllers.Peek().UpdateSO();
    }

    public void ReverseTransition(float t = 1.0f, int rtTimes = 1){
        if (!controllers.Any()){
            restRTCount = 0;
            return ;
        }
        restRTCount = rtTimes-1;
        BNTransitionController cont = controllers.Peek();
        BNScreen screen = cont.GetFrom().GetScreen();

        if (rtTimes == 1){
            //screen.InitForTargetTransition();
            screen.UpdateScreen();
        }
        cont.AddOnCompleteAction(AfterReverseTransition);
        cont.ReverseBNTransitionWithAnim(t);
    }

    private void AfterReverseTransition(){
        BNTransitionController cont = controllers.Pop();
        cont.RemoveOnCompleteAction(AfterReverseTransition);
        BNScreen screen = screenList[screenList.Count-1];
        screenList.RemoveAt(screenList.Count-1);
        BNScreens.Instance.AddUsedScreen(screen);
        if (cont.GetTransitionType() == BNScreens.TransitionType.Modal || cont.GetTransitionType() == BNScreens.TransitionType.Fade){
            ToPrevLevel();
        }
        if (restRTCount > 0){
            ReverseTransition(1.0f, restRTCount);
        }
    }

    public BNTransitionController GetLatestController(){
        if (!controllers.Any()){
            return null;
        }
        return controllers.Peek();
    }
}
}