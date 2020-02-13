using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes {
public class RecordView : BNScreen
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI tryNumberText;
    [SerializeField] private TextMeshProUGUI completeRateText;
    [SerializeField] private TextMeshProUGUI commentText; 
    [SerializeField] private Image condition;

    [SerializeField] private Sprite[] conditionImages;

    [SerializeField] private BNRoute route;
    [SerializeField] private BNRecord record;
    public override void InitForFirstTransition(){
    }

    public void ClearFields(){
        dayText.text ="";
        tryNumberText.text ="";
        completeRateText.text = "";
        commentText.text = "";
        route = null;
        record = null;
    }

    public override void UpdateScreen(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            route = (belongingStack as BNScreenStackWithTargetGym).GetTargetRoute();
 
            if (route == null){
                return ;
            }
            record = (belongingStack as BNScreenStackWithTargetGym).GetTargetRecord();
            if (record == null){
                return ;
            }
            dayText.text = record.GetDate();
            tryNumberText.text = "" + record.GetTryNumber();
            completeRateText.text = record.GetCompleteRate() + "%";
            condition.sprite = conditionImages[(int)record.GetCondition()];
            commentText.text = record.GetComment();
        }
    }
/*
    public void SaveTargerRecordInStack(string id){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).SetTargetRecordID(id);
        }
    }*/
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void ToRegisterRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterRecordView, BNScreens.TransitionType.Push);
    }

}
}