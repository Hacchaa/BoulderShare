using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BNScreens : SingletonMonoBehaviour<BNScreens>{

    public enum TransitionType {Push, Modal};
    public enum BNScreenType {FirstView, SecondView, ThirdView, FourthView, FifthView, SixthView};
    public enum BNTabName{Home=0, Favorite, Statistics, Other};
    [SerializeField] private List<BNTStack> stacks;
    [SerializeField] private List<BNScreenData> screenPrefabs;
    private Dictionary<BNScreenType, BNScreen> map;
    private int currentStackIndex;
    private List<BNScreen> usedScreens;
    [SerializeField] private Transform usedScreenRoot;

    void Awake(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; 
    }

    void Start(){
        map = new Dictionary<BNScreenType, BNScreen>();
        foreach(BNScreenData data in screenPrefabs){
            map.Add(data.t, data.screen);
        }

        usedScreens = new List<BNScreen>();

        foreach(BNTStack stack in stacks){
            stack.Init();
            stack.gameObject.SetActive(false);
        }
        stacks[0].gameObject.SetActive(true);
        currentStackIndex = 0;
    }

    public BNTStack GetCurrentStack(){
        return stacks[currentStackIndex];
    }

    public void ChangeScreenStack(BNTabName tab){
        stacks[currentStackIndex].gameObject.SetActive(false);
        currentStackIndex = (int)tab;
        stacks[currentStackIndex].gameObject.SetActive(true);

        stacks[currentStackIndex].UpdateLatestSO();

    }

    public void Transition(BNScreenType screen, TransitionType transition){
        stacks[currentStackIndex].Transition(screen, transition);
    }
    public void ReverseTransition(float t = 1.0f){
        stacks[currentStackIndex].ReverseTransition(t);
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
    public BNScreen RecycleScreen(BNScreens.BNScreenType screenType, Transform parent){
        BNScreen screen = null;
        foreach(BNScreen s in usedScreens){
            if (s.GetScreenType() == screenType){
                screen = s;
                break;
            }
        }
        if (screen != null){
            screen.transform.SetParent(parent, false);
            usedScreens.Remove(screen);            
        }
        return screen;
    }

    public void AddUsedScreen(BNScreen screen){
        usedScreens.Add(screen);
        screen.transform.SetParent(usedScreenRoot, false);
    }

}

[System.Serializable]
public class BNScreenData{
    public BNScreens.BNScreenType t;
    public BNScreen screen;
}
