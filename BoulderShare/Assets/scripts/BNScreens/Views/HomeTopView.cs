using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class HomeTopView : BNScreenWithGyms
{
    [SerializeField] private GymScrollerController scroller;
    [SerializeField] private TextMeshProUGUI sortText;
 
    private BNScreenStackWithTargetGym stack;

    public override void InitForFirstTransition(){
        scroller.Init();
        SortToggle.SortType sortType = BNGymDataCenter.Instance.GetGymSortType();
        if (sortType == SortToggle.SortType.None){
            sortType = SortToggle.SortType.Latest;
        }
        stack = GetScreenStackWithGyms();
        stack.SetTargetSortType(sortType);
    }

    public override void UpdateScreen(){
        SortToggle.SortType sortType = BNGymDataCenter.Instance.GetGymSortType();
        SortToggle.SortType newSortType = stack.GetTargetSortType();
        if (sortType == SortToggle.SortType.None){
            sortType = SortToggle.SortType.Latest;
        }
        if (newSortType == SortToggle.SortType.None){
            newSortType = sortType;
        }
        scroller.FetchData(BNGymDataCenter.Instance.ReadGyms(), newSortType);

        if (stack != null){
            stack.ClearGym();
        }
        if (sortType != newSortType){
            sortText.text = SortToggle.GetSortTypeName(newSortType);
            BNGymDataCenter.Instance.SaveGymSortType(newSortType);
        }
        stack.ClearInputs();
    }
    public void ToGymView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymRoutesView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }
    public void ToInputViewSort(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.Sort);
        BNScreens.Instance.Transition(BNScreens.BNScreenType.InputItemsView, BNScreens.TransitionType.Push);
    }


    public void SaveTargetGymInStack(string gymID){
        if (string.IsNullOrEmpty(gymID)){
            return ;
        }
        if (stack != null){
            stack.ClearGym();
            stack.StoreTargetGym(gymID);
        }
    }
}
}