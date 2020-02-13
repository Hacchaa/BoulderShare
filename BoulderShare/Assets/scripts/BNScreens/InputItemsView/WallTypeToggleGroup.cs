using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class WallTypeToggleGroup : MonoBehaviour
{
    [SerializeField] private List<WallTypeToggle> toggles;
    [SerializeField] private InputItemsView view;
    private WallTypeMap.Type selectedType;

    public void Init(WallTypeMap.Type t){
        selectedType = t;
        foreach(WallTypeToggle tog in toggles){
            tog.Init(t);
        }
    }

    public void SetWallType(WallTypeMap.Type t){
        selectedType = t;
    }

    public WallTypeMap.Type GetWallType(){
        return selectedType;
    }
}
}