using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymWallAddCellView : EnhancedScrollerCellView
{
    public OnButtonClickedDelegate clickDel;
    public void OnClicked(){
        if(clickDel != null){
            clickDel();
        }
    }
}

}