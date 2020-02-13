using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{

public class GymWallCellView : EnhancedScrollerCellView
{   
    public BNWall wall;
    public TextMeshProUGUI wallTypeName;
    public TextMeshProUGUI period;
    public OnButtonClickedDelegateWithBNWall clickDel;
    public void SetData(GymWallScrollerData data){
        wallTypeName.text = "" + data.gymWallTypeName;
        period.text = data.gymWallPeriod;
        wall = data.wall;

    }
    public void OnClicked(){
        if (clickDel != null){
            clickDel(wall);
        }
    }
}

}