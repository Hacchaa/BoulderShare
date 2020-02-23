using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class WallTypeToggleGroup : MonoBehaviour
{
    [SerializeField] private List<WallTypeToggle> toggles;
    [SerializeField] private InputItemsView view;
    private WallTypeMap.Type selectedType;
    [SerializeField] private ScrollRect scroller;

    public void Init(WallTypeMap.Type t){
        scroller.verticalNormalizedPosition = 0.0f;
        selectedType = t;
        foreach(WallTypeToggle tog in toggles){
            tog.Init(t);
        }
    }

    public void SetWallType(WallTypeMap.Type t){
        selectedType = t;
    }
    public void Register(){
        view.Register();
    }
    public WallTypeMap.Type GetWallType(){
        return selectedType;
    }
}
}