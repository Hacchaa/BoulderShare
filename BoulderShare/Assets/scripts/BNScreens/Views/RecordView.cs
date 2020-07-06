using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;

namespace BoulderNotes {
public class RecordView : BNScreenWithGyms
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI tryNumberText;
    [SerializeField] private TextMeshProUGUI completeRateText;
    [SerializeField] private Image completeRateCircle;
    [SerializeField] private TextMeshProUGUI commentText; 
    [SerializeField] private Image condition;

    [SerializeField] private BNRoute route;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Image prevButton;
    [SerializeField] private Image nextButton;
    private int currentTryNumber;
    private BNScreenStackWithTargetGym stack;
    public override void InitForFirstTransition(){
    }

    public void ClearFields(){
        dayText.text ="";
        tryNumberText.text ="";
        completeRateText.text = "";
        commentText.text = "";
        route = null;
        stack = null;
    }

    public override void UpdateScreen(){
        ClearFields();
        stack = GetScreenStackWithGyms();
        if (stack != null){
            route = stack.GetTargetRoute();
 
            if (route == null){
                return ;
            }
            BNRecord record = stack.GetTargetRecord();
            if (record == null){
                return ;
            }
            LoadRecord(record);
        }
    }

    public void PrevRecord(){
        if (currentTryNumber == 1){
            return ;
        }
        BNRecord rec = route.FindRecord(currentTryNumber-1);
        if (rec != null){
            LoadRecord(rec);
        }
    }

    public void NextRecord(){
        if (currentTryNumber == route.GetNewTryNumber() - 1){
            return ;
        }
        BNRecord rec = route.FindRecord(currentTryNumber+1);
        if (rec != null){
            LoadRecord(rec);
        }
    }

    private void LoadRecord(BNRecord record){
        dayText.text = record.GetDate() + Environment.NewLine + record.GetDate3();
        tryNumberText.text = "" + record.GetTryNumber() + "回目";
        completeRateText.text = record.GetCompleteRate() + "<size=50%>%";
        completeRateCircle.fillAmount = (record.GetCompleteRate()+0f) / 100f;
        BNManager.Instance.GetConditionSprite((int)record.GetCondition(), OnLoadSprite);
        commentText.text = record.GetComment();    

        currentTryNumber = record.GetTryNumber();  


        prevButton.color = Color.black;
        nextButton.color = Color.black;  

        if (currentTryNumber == 1){
            prevButton.color = disabledColor;
        }
        if (currentTryNumber == route.GetNewTryNumber() - 1){
            nextButton.color = disabledColor;
        }

        SaveTargerRecordInStack(record.GetID());
    }

    private void OnLoadSprite(Sprite sprite){
        condition.sprite = sprite;
    }

    public void SaveTargerRecordInStack(string id){
        if (stack != null){
            stack.StoreTargetRecord(id);
        }
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void ToRegisterRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterRecordView, BNScreens.TransitionType.Push);
    }

}
}