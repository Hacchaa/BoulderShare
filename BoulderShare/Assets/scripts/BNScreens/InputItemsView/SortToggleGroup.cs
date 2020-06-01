using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class SortToggleGroup : MonoBehaviour
{
    [SerializeField] private List<SortToggle> toggles;
    [SerializeField] private InputItemsView view;
    [SerializeField] private SortToggle.SortType selectedSortType;
    [SerializeField] private ScrollRect scroller;
    public void Init(SortToggle.SortType st){
        scroller.verticalNormalizedPosition = 1.0f;
        selectedSortType = st;
        foreach(SortToggle tog in toggles){
            tog.Init(st);
        }
    }

    public void SetSortType(SortToggle.SortType st){
        selectedSortType = st;
    }
    public void Register(){
        view.Register();
    }

    public SortToggle.SortType GetSortType(){
        return selectedSortType;
    }
}
}