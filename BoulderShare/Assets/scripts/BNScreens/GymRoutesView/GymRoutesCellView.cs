using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class GymRoutesCellView : EnhancedScrollerCellView
{
    [SerializeField] private GymRoutesRowCellView[] rows;
 
    public void SetData(ref List<GymRoutesScrollerData> _data, int index, int numOfData, BNScreenStackWithTargetGym stack, OnButtonClickedDelegateWithString onButtonClicked){
        for(int i = 0 ; i < rows.Length ; i++){
            if (index + i < numOfData){
                rows[i].gameObject.SetActive(true);
                rows[i].SetData(_data[index+i], stack, onButtonClicked);
            }else{
                rows[i].gameObject.SetActive(false);
            }
        }
    }
}
}