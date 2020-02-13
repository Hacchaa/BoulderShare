using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymRouteCellView : EnhancedScrollerCellView
{

    public GymRouteRowCellView[] rowData;
    public OnButtonClickedDelegate onAddClicked;
    public GameObject addButton;

    public void SetData(ref List<GymRouteScrollerDataBase> data, int startingIndex, int dataCount, OnButtonClickedDelegateWithBNRoute routeDel, OnButtonClickedDelegate addDel){
        onAddClicked = addDel;
        addButton.SetActive(false);
        for(int i = 0 ; i < rowData.Length ; i++){
            if (startingIndex + i < dataCount && (data[startingIndex + i].isRoute || data[startingIndex + i].isAddButton)){
                rowData[i].SetData((data[startingIndex + i] as GymRouteScrollerData), routeDel);

                if (data[startingIndex + i].isAddButton){
                    addButton.SetActive(true);
                }
            }else{
                rowData[i].SetData(null, null);
            }
        }
    }

    public void OnAddClicked(){
        if (onAddClicked != null){
            onAddClicked();
        }
    }
}
}