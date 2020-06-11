using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class HomeTopView : BNScreenInput
{
    [SerializeField] private GymScrollerController scroller;
    [SerializeField] private TextMeshProUGUI sortText;
    public override void InitForFirstTransition(){
        scroller.Init();
        ClearFields();
        sortType = BNGymDataCenter.Instance.GetGymSortType();
    }

    public override void UpdateScreen(){
        scroller.FetchData(BNGymDataCenter.Instance.ReadGyms(), sortType);
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
        }
        sortText.text = SortToggle.GetSortTypeName(sortType);
        BNGymDataCenter.Instance.SaveGymSortType(sortType);
    }
    public void ToGymView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.GymRoutesView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }


    public void SaveTargetGymInStack(string gymID){
        if (string.IsNullOrEmpty(gymID)){
            return ;
        }
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ClearGym();
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetGym(gymID);
        }
    }
}
}