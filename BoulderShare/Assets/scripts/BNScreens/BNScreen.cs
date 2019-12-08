using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BNScreen : MonoBehaviour
{
    [SerializeField] private BNScreens.BNScreenType screenType;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected RectTransform head;
    [SerializeField] protected RectTransform headBG;
    [SerializeField] protected RectTransform tab;
    [SerializeField] private List<Transition> transitions;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> fromMap;
    private Dictionary<BNScreens.TransitionType, BNTransitionBase> toMap;

   public void Init(){
       fromMap = new Dictionary<BNScreens.TransitionType, BNTransitionBase>();
       toMap = new Dictionary<BNScreens.TransitionType, BNTransitionBase>();

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
       transitions[0].transition.InitSO();
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
