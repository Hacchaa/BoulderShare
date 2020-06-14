using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNScreens : SingletonMonoBehaviour<BNScreens>{

    public enum TransitionType {Push, Modal, Fade};
    public enum BNScreenType {
        FirstView, SecondView, ThirdView, FourthView, FifthView, SixthView, HomeTopView, GymView, 
        RegisterView, RouteView, ModifyView, RegisterRecordView, RecordView,
        InputItemsView, EditRouteTapeView, SelecteWallImageView, StatisticsView, SelectRouteTagView,
        GymRoutesView, EditWallImageView, DisplayImageView, GymSortView};
    public enum BNTabName{Home=0, Favorite, Add, Statistics, Other};
    [SerializeField] private CanvasGroup blockTouchCG;  
    [SerializeField] private List<BNTStack> stacks;
    [SerializeField] private List<BNScreenData> screenPrefabs;
    [SerializeField] private BNTab bnTab;  

    private Dictionary<BNScreenType, BNScreen> map;
    private int currentStackIndex;

    public void Init(){
        map = new Dictionary<BNScreenType, BNScreen>();
        foreach(BNScreenData data in screenPrefabs){
            map.Add(data.t, data.screen);
        }

        foreach(BNTStack stack in stacks){
            stack.Init();
            stack.DeactivateStack();
        }
        stacks[0].ActivateStack();
        currentStackIndex = 0;
        bnTab.Init();
    }

    public void Interactive(bool b){
        blockTouchCG.blocksRaycasts = !b;
    }

    public BNTStack GetCurrentStack(){
        return stacks[currentStackIndex];
    }

    public void ChangeScreenStack(BNTabName tab){
        stacks[currentStackIndex].DeactivateStack();
        currentStackIndex = (int)tab;
        stacks[currentStackIndex].ActivateStack();
    }

    public void Transition(BNScreenType screen, TransitionType transition){
        stacks[currentStackIndex].Transition(screen, transition);
    }
    public void ReverseTransition(float t = 1.0f, int rtTimes = 1){
        stacks[currentStackIndex].ReverseTransition(t, rtTimes);
    }

    public BNScreen MakeScreen(BNScreenType t, Transform parent){
        if (map.ContainsKey(t)){
            BNScreen obj = Instantiate(map[t]) as BNScreen;
            obj.transform.SetParent(parent, false);
            obj.Init();
            return obj;
        }
        return null;
    }

}

[System.Serializable]
public class BNScreenData{
    public BNScreens.BNScreenType t;
    public BNScreen screen;
}
}