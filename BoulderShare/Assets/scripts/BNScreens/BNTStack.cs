using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BNTStack : MonoBehaviour
{
    [SerializeField] private BNScreens.BNScreenType initialView;
    private Stack<BNScreen> screenStack;
    private Stack<BNTransitionController> controllers;

    private Stack<RectTransform> levels;

    public void Init(){
        screenStack = new Stack<BNScreen>();
        controllers = new Stack<BNTransitionController>();
        levels = new Stack<RectTransform>();

        GameObject obj = new GameObject("Level0");
        RectTransform rect = obj.AddComponent<RectTransform>();
        obj.transform.SetParent(this.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        levels.Push(rect);

        Transition(initialView, BNScreens.TransitionType.Push);
    }

    public void Transition(BNScreens.BNScreenType screenType, BNScreens.TransitionType t){
        Transform parent = null;
        if (t == BNScreens.TransitionType.Push){
            parent = levels.Peek();
        }else if(t == BNScreens.TransitionType.Modal){
            ToNextLevel();
            parent = levels.Peek();
        }

        BNScreen current = BNScreens.Instance.RecycleScreen(screenType, parent);
        if (current == null){
            current = BNScreens.Instance.MakeScreen(screenType, parent);
        }

        current.gameObject.SetActive(true);

        if (!screenStack.Any()){
            screenStack.Push(current);
            return ;
        }

        BNScreen prev = screenStack.Peek();
        BNTransitionBase curTrans = current.GetTransition(t, false);
        BNTransitionBase prevTrans = prev.GetTransition(t, true);

        //Debug.Log("current:"+current.gameObject.name);
        //Debug.Log("prev:"+prev.gameObject.name);
        BNTransitionController controller = new BNTransitionController(prevTrans, curTrans, t);

        controller.BNTransitionWithAnim();
        screenStack.Push(current);
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
            if (!screenStack.Any()){
                 return ;
            }
            screenStack.Peek().InitSO();
            return ;
        }

        controllers.Peek().UpdateSO();
    }

    public void ReverseTransition(float t = 1.0f){
        if (!controllers.Any()){
            return ;
        }
        BNTransitionController cont = controllers.Peek();
        cont.GetTo().SetOnCompleteAction(AfterReverseTransition);
        cont.ReverseBNTransitionWithAnim(t);
    }

    private void AfterReverseTransition(){
        BNTransitionController cont = controllers.Pop();
        cont.GetTo().RemoveOnCompleteAction(AfterReverseTransition);
        BNScreen screen = screenStack.Pop();
        BNScreens.Instance.AddUsedScreen(screen);
        if (cont.GetTransitionType() == BNScreens.TransitionType.Modal){
            ToPrevLevel();
        }
    }

    public BNTransitionController GetLatestController(){
        if (!controllers.Any()){
            return null;
        }
        return controllers.Peek();
    }
}
