using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace BoulderNotes{
public class RecordCellView : EnhancedScrollerCellView
{
    [SerializeField] private Color low;
    [SerializeField] private Color middle;
    [SerializeField] private Color high;
    [SerializeField] private TextMeshProUGUI completeRateText;
    [SerializeField] private TextMeshProUGUI tryNumberAndTimeText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private Image condition;
    [SerializeField] private BNRecord record;
    [SerializeField] private OnButtonClickedDelegateWithBNRecord clickDel;

    public void SetData(RecordScrollerData data, OnButtonClickedDelegateWithBNRecord onClickDel){
        clickDel = onClickDel;
        completeRateText.SetText("{0}<size=50%>%", data.completeRate);
        //色設定
        if (data.completeRate <= 25){
            completeRateText.color = low;
        }else if(data.completeRate <= 74){
            completeRateText.color = middle;
        }else{
            completeRateText.color = high;
        }
        tryNumberAndTimeText.SetText("{0}回目  "+data.date, data.tryNumber);
        commentText.text = data.comment;
        BNManager.Instance.GetConditionSprite(data.condition, OnLoadSprite);
        record = data.record;
    }

    private void OnLoadSprite(Sprite spr){
        condition.sprite = spr;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(record);
        }
    }
}

}