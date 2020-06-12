using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class RecordRouteWallImageTitleCellView : EnhancedScrollerCellView
{
    public OnButtonClickedDelegate addDel;
    [SerializeField] private Image frame;
    public bool proceedAtOnce = false;
    public void SetData(RecordRouteWallImageTitleScrollerData data, OnButtonClickedDelegate addButtonDel){
        addDel = addButtonDel;
        if (!proceedAtOnce){
            BNManager.Instance.GetCornerPanelStroke(OnLoadFrame);
            proceedAtOnce = true;
        }
    }

    private void OnLoadFrame(Sprite sprite){
        frame.sprite = sprite;
    }

    public void OnButtonPushed(){
        if (addDel != null){
            addDel();
        }
    }

}
}