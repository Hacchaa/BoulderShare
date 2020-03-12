using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BoulderNotes{
public class BNScreen : MonoBehaviour
{
    [SerializeField] private BNScreens.BNScreenType screenType;
    [SerializeField] private float headHeight;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected RectTransform head;
    [SerializeField] protected RectTransform headBG;
    [SerializeField] protected RectTransform tab;
    [SerializeField] private RectTransform safeAreaInContent;
    [SerializeField] private RectTransform botSafeAreaInset;
    [SerializeField] private RectTransform statusBar;
    [SerializeField] private RectTransform safeAreaInHeadBG;
    [SerializeField] private bool stretchContentToTop = false;
  
    [SerializeField] private List<Transition> transitions;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> fromMap;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> toMap;
    protected BNTStack belongingStack;


    public void SetBelongingStack(BNTStack s){
        belongingStack = s;
    }

    public BNTStack GetBelongingStack(){
        return belongingStack;
    }

    //新しい画面として設定されてから始めてTransitionする時呼び出される
    public virtual void InitForFirstTransition(){
 
    }

    //Transition時に遷移する側であるときと、stackが変更されて表示されたときに呼び出される
    public virtual void UpdateScreen(){

    }

    //
   public void Init(){
       fromMap = new Dictionary<BNScreens.TransitionType, BNTransitionBase>();
       toMap = new Dictionary<BNScreens.TransitionType, BNTransitionBase>();

       float topHeight = CanvasResolutionManager.Instance.GetStatusBarHeight();
       float botHeight = CanvasResolutionManager.Instance.GetBotSafeAreaInset();

        if (content != null){
            content.anchorMin = new Vector2(0.0f, 0.0f);
            content.anchorMax = new Vector2(1.0f, 1.0f);
            float h = 0f;

            if (tab != null){
                h = tab.rect.height;
            }
            content.offsetMin = new Vector2(0.0f, h);                

            if (stretchContentToTop){
                content.offsetMax = new Vector2(0f, 0f);
            }else{
                h = 0f;
                if (safeAreaInHeadBG != null){
                    h = topHeight + headHeight;
                }else if (statusBar != null){
                    h = topHeight;
                }
                content.offsetMax = new Vector2(0.0f, -h);      
            }     
        }

        if (botSafeAreaInset != null){
            botSafeAreaInset.anchorMin = new Vector2(0.0f, 0.0f);
            botSafeAreaInset.anchorMax = new Vector2(1.0f, 0.0f);
            botSafeAreaInset.anchoredPosition = new Vector2(0.0f, botHeight/2.0f);
            botSafeAreaInset.sizeDelta = new Vector2(0.0f, botHeight);

            if (safeAreaInContent != null){
                safeAreaInContent.anchorMin = new Vector2(0.0f, 0.0f);
                safeAreaInContent.anchorMax = new Vector2(1.0f, 1.0f);
                safeAreaInContent.offsetMin = new Vector2(0.0f, botHeight);
                safeAreaInContent.offsetMax = new Vector2(0.0f, 0.0f);
            }
        }else{
            if (safeAreaInContent != null){
                safeAreaInContent.anchorMin = new Vector2(0.0f, 0.0f);
                safeAreaInContent.anchorMax = new Vector2(1.0f, 1.0f);
                safeAreaInContent.offsetMin = new Vector2(0.0f, 0.0f);
                safeAreaInContent.offsetMax = new Vector2(0.0f, 0.0f);
            }            
        }

        if (head != null){
            head.anchorMin = new Vector2(0.0f, 1.0f);
            head.anchorMax = new Vector2(1.0f, 1.0f);
            head.anchoredPosition = new Vector2(0.0f, -(topHeight + headHeight/2.0f));
            head.sizeDelta = new Vector2(0.0f, headHeight);           
        }

        if (headBG != null){
            headBG.anchorMin = new Vector2(0.0f, 1.0f);
            headBG.anchorMax = new Vector2(1.0f, 1.0f);
            float h ;
            if (safeAreaInHeadBG != null){
                h = topHeight + headHeight;
            }else{
                h = topHeight;
            }
            headBG.anchoredPosition = new Vector2(0.0f, -h / 2.0f);
            headBG.sizeDelta = new Vector2(0.0f, h);
        }
        if (statusBar != null){
            statusBar.anchorMin = new Vector2(0.0f, 1.0f);
            statusBar.anchorMax = new Vector2(1.0f, 1.0f);
            statusBar.anchoredPosition = new Vector2(0.0f, -topHeight/2.0f);
            statusBar.sizeDelta = new Vector2(0.0f, topHeight);

            if (safeAreaInHeadBG != null){
                safeAreaInHeadBG.anchorMin = new Vector2(0.0f, 0.0f);
                safeAreaInHeadBG.anchorMax = new Vector2(1.0f, 1.0f);
                safeAreaInHeadBG.offsetMin = new Vector2(0.0f, 0.0f);
                safeAreaInHeadBG.offsetMax = new Vector2(0.0f, -topHeight);
            }
        }

        foreach(Transition t in transitions){
            t.transition.Init(this, content, head, headBG, tab);
            if (t.isFrom){
                fromMap.Add(t.type, t.transition);
            }else{
                toMap.Add(t.type, t.transition);
            }
       }
   }

   public void InitSO(){
       if (transitions.Any()){
           transitions[0].transition.InitSO();
       }
   }

   public BNScreens.BNScreenType GetScreenType(){
       return screenType;
   }

   public BNTransitionBase GetTransition(BNScreens.TransitionType type, bool isFrom){
       Dictionary<BNScreens.TransitionType, BNTransitionBase> map;
       if (isFrom){
           map = fromMap;
       }else{
           map = toMap;
       }
       if (map.ContainsKey(type)){
           return map[type];
       }
       return null;
   }
}

[System.Serializable]
public class Transition{
    public BNScreens.TransitionType type;
    public bool isFrom;
    public BNTransitionBase transition;
}
}