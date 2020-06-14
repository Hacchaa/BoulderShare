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
    private List<BNScreen> usedScreens;
    private int restRTCount;
    [SerializeField] List<BNScreens.BNScreenType> mightUsedScreens;
    public virtual void Init(){
        screenList = new List<BNScreen>();
        controllers = new Stack<BNTransitionController>();
        usedScreens = new List<BNScreen>();
        restRTCount = 0;

        PrepareScreens();
    }

    private void PrepareScreens(){
        foreach(BNScreens.BNScreenType type in mightUsedScreens){
            BNScreen screen = BNScreens.Instance.MakeScreen(type, transform);
            if (screen != null){
                screen.gameObject.SetActive(true);
                screen.HideScreen();
                usedScreens.Add(screen);
            }
        }
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
        BNScreen current = RecycleScreen(screenType);
        if (current == null){
            current = BNScreens.Instance.MakeScreen(screenType, transform);
            current.gameObject.SetActive(true);
        }
        screenList.Add(current);
        current.ShowScreen();
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
        AddUsedScreen(screen);
 
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
    
    public BNScreen RecycleScreen(BNScreens.BNScreenType screenType){
        BNScreen screen = null;
        //Debug.Log("screenType:"+screenType.ToString());
        foreach(BNScreen s in usedScreens){
            //Debug.Log("s.getscreentype():"+s.GetScreenType().ToString());
            if (s.GetScreenType() == screenType){
                screen = s;
                break;
            }
        }
        if (screen != null){
            //Debug.Log("found:"+screen.GetScreenType().ToString());
            usedScreens.Remove(screen);            
        }
        return screen;
    }
    public void AddUsedScreen(BNScreen screen){
        //Debug.Log("addusedScreen:"+screen.GetScreenType().ToString());
        usedScreens.Add(screen);
    }
}
}