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
    private BNGym gym;
    public OnButtonClickedDelegateWithBNGym clickDel;

    public void SetData(GymScrollerData data){
        text.text = data.gymName;
        id.text = data.gym.GetID();
        gym = data.gym;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(gym);
        }
    }
}
}