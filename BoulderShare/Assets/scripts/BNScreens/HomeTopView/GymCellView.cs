using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI text;
    //public TextMeshProUGUI id;
    public Image maskImage;
    private string gymID;
    public OnButtonClickedDelegateWithString clickDel;

    private bool completedInit = false;

    public void SetData(GymScrollerData data){
        text.text = data.gymName;
        //id.text = data.gymID;
        gymID = data.gymID;

        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanelFill(OnLoad);
        }
    }

    private void OnLoad(Sprite sprite){
        maskImage.sprite = sprite;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(gymID);
        }
    }
}
}