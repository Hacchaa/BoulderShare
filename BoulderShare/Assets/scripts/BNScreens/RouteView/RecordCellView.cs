using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

namespace BoulderNotes{
public class RecordCellView : EnhancedScrollerCellView
{
    public Color low;
    public Color middle;
    public Color high;
    public TextMeshProUGUI completeRatePercentText;
    public TextMeshProUGUI completeRateText;
    public TextMeshProUGUI tryNumberText;
    public TextMeshProUGUI commentText;
    public Image condition;
    public BNRecord record;
    public string date;
    public OnButtonClickedDelegateWithBNRecord clickDel;

    public void SetData(RecordScrollerData data, Sprite conditionImage){
        completeRateText.text = data.completeRate + "";
        //色設定
        if (data.completeRate <= 25){
            completeRateText.color = low;
            completeRatePercentText.color = low;
        }else if(data.completeRate <= 74){
            completeRateText.color = middle;
            completeRatePercentText.color = middle;
        }else{
            completeRateText.color = high;
            completeRatePercentText.color = high;
        }
        tryNumberText.text = data.tryNumber + "回目";
        commentText.text = data.comment;
        condition.sprite = conditionImage;
        record = data.record;
        date = data.date;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(record);
        }
    }
}

}