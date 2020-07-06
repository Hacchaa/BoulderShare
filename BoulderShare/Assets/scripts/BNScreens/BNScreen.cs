using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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
    [SerializeField] private List<Behaviour> needEnabledList;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> fromMap;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> toMap;
    protected BNTStack belongingStack;

    private CanvasGroup contentCG;
    protected CanvasGroup headCG;
    protected CanvasGroup headBGCG;
    private GraphicRaycaster contentGR;
    private GraphicRaycaster headGR;
    protected bool processedInit = false;

    public virtual void SetBelongingStack(BNTStack s){
        belongingStack = s;
    }

    public virtual BNTStack GetBelongingStack(){
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

        if (content != null){
            contentCG = content.GetComponent<CanvasGroup>();
            contentGR = content.GetComponent<GraphicRaycaster>();
        }
        if (head != null){
            headCG = head.GetComponent<CanvasGroup>();
            headGR = head.GetComponent<GraphicRaycaster>();
        }
        if (headBG != null){
            headBGCG = headBG.GetComponent<CanvasGroup>();
        }

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
       processedInit = true;
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

    public bool IsShowedScreen(){
        return contentCG.blocksRaycasts;
    }

    public void ShowScreen(){
    //Debug.Log("showScreen");
       contentCG.blocksRaycasts = true;
       headCG.blocksRaycasts = true;
       headBGCG.blocksRaycasts = true;

       contentCG.alpha = 1f;
       headCG.alpha = 1f;
       headBGCG.alpha = 1f;

       //contentCG.interactable = true;
       //headCG.interactable = true;
       //headBGCG.interactable = true;

       contentGR.enabled = true;
       headGR.enabled = true;

       foreach(Behaviour b in needEnabledList){
           b.enabled = true;
       }

   }
    public void HideScreen(){
       contentCG.blocksRaycasts = false;
       headCG.blocksRaycasts = false;
       headBGCG.blocksRaycasts = false;

       contentCG.alpha = 0f;
       headCG.alpha = 0f;
       headBGCG.alpha = 0f;

       //contentCG.interactable = false;
       //headCG.interactable = false;
       //headBGCG.interactable = false;

       contentGR.enabled = false;
       headGR.enabled = false;


       foreach(Behaviour b in needEnabledList){
           b.enabled = false;
       }
   }
}

[System.Serializable]
public class Transition{
    public BNScreens.TransitionType type;
    public bool isFrom;
    public BNTransitionBase transition;
}
}