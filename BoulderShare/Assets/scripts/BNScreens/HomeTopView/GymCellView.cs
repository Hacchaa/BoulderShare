using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI id;
    private string gymID;
    public OnButtonClickedDelegateWithString clickDel;

    public void SetData(GymScrollerData data){
        text.text = data.gymName;
        id.text = data.gymID;
        gymID = data.gymID;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(gymID);
        }
    }
}
}