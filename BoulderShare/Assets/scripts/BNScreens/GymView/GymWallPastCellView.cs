using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;


namespace BoulderNotes{
public class GymWallPastCellView : EnhancedScrollerCellView
{
    public OnButtonClickedDelegate clickDel;

    public void OnClicked(){
        if (clickDel != null){
            clickDel();
        }
    }
}
}