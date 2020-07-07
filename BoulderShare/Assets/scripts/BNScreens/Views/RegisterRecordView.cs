using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using SA.iOS.UIKit;

namespace BoulderNotes {
public class RegisterRecordView : BNScreenWithGyms
{
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider completeRateSlider;
    [SerializeField] private Slider conditionSlider;
    [SerializeField] private TextMeshProUGUI commentText;

    [SerializeField] private Image selectedConditon;
    [SerializeField] private AssetReference[] conditionRef;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI clearRateText;
    [SerializeField] private BNRecord record;
    [SerializeField] private BNRoute route;
    [SerializeField] private ClearStatusToggle[] toggles;
    [SerializeField] private GameObject clearStatusObj;
    private BNScreenStackWithTargetGym stack;

    private bool hasInsight;

    private DateTime dateDT;
    private DateTime timeDT;
  
    public override void InitForFirstTransition(){
        ClearFields();
        stack = GetScreenStackWithGyms();
        if (stack != null){
            stack.ClearInputs();
            route = stack.GetTargetRoute();

            if (route == null){
                //あるはずがない処理
                return ;
            }
            BNRoute.ClearStatus status = route.GetTotalClearStatus();
            hasInsight = route.GetHasInsight();
            //toggleの初期値をrouteのhasInsightに依存させる
            if (status == BNRoute.ClearStatus.RP || status == BNRoute.ClearStatus.NoAchievement){
                if (hasInsight){
                    status = BNRoute.ClearStatus.Flash;
                }else{
                    status = BNRoute.ClearStatus.Onsight;
                }
            }
            for(int i = 0 ; i < toggles.Length ; i++){
                toggles[i].Init(status);
            }

            record = stack.GetTargetRecord();

            if (record == null){
                //Debug.Log("new");
                //新規作成
                DateTime now = DateTime.Now;
                dayText.text = now.ToString(BNGymDataCenter.FORMAT_DATE2);
                timeText.text = now.ToString(BNGymDataCenter.FORMAT_HM);
                dateDT = now;
                timeDT = now;
                deleteButton.SetActive(false);
                clearStatusObj.SetActive(false);
            }else{
                //Debug.Log("edit");
                //編集
                dayText.text = record.GetDate();
                timeText.text = record.GetDate3();
                DateTime t = record.GetFullTime();
                dateDT = t;
                timeDT = t;
                completeRateSlider.value = 0f + record.GetCompleteRate();
                conditionSlider.value = 0.0f + (int)record.GetCondition();
                stack.SetTargetString(record.GetComment());

                if (record.GetTryNumber() == 1 && record.GetCompleteRate() == 100){
                    clearStatusObj.SetActive(true);
                }else{
                    clearStatusObj.SetActive(false);
                }

                deleteButton.SetActive(true);
            }
        }
    }

    public void ClearFields(){
        dayText.text = "";
        completeRateSlider.value = 50f;
        clearRateText.text = "50%";
        conditionSlider.value = 2f;
        SetSelectedCondition(2);
        commentText.text = "";
        record = null;
        route = null;
        stack = null;
        hasInsight = true;
    }

    public override void UpdateScreen(){
        commentText.text = stack.GetTargetString();
    }

    public void Register(){
        if (route == null || stack == null){
            return ;
        }
        DateTime time = new DateTime(dateDT.Year, dateDT.Month, dateDT.Day, timeDT.Hour, timeDT.Minute, 0);
        route.SetHasInsight(hasInsight);
        if (record != null){
            record.SetTime(time);
            //record.SetTryNumber(route.GetNewTryNumber());
            record.SetCompleteRate((int)completeRateSlider.value);
            record.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            record.SetComment(stack.GetTargetString());

            BNRecord oldRecord = null;
            foreach(BNRecord r in route.GetRecords()){
                if (r.GetID().Equals(record.GetID())){
                    oldRecord = r;
                    break;
                }
            }

            if (oldRecord != null){
                route.DeleteRecord(oldRecord.GetID());
                route.AddRecord(record);
            }
        }else{
            record = new BNRecord();
            record.SetTime(time);
            record.SetTryNumber(route.GetNewTryNumber());
            record.SetCompleteRate((int)completeRateSlider.value);
            record.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            record.SetComment(stack.GetTargetString());

            route.AddRecord(record);           
        }
        stack.ModifyRoute(route);
        stack.StoreTargetRecord(record.GetID());  
    }

    private void DeleteRecord(){
        if (record == null){
            return ;
        }
        if (stack != null){
            route.DeleteRecord(record.GetID());
            stack.ModifyRoute(route);
        }
    }

    public void OnClickedDeleteRecordButton(){
        DeleteRecord();
        ReverseTransition(1.0f, 2);
    }

    public void OnClickedAddButton(){
        Register();
        ReverseTransition();
    }

    public void InputText(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.Texts);
        BNScreens.Instance.Transition(BNScreens.BNScreenType.InputItemsView, BNScreens.TransitionType.Push);
    }

    public void ReverseTransition(float t = 1.0f, int n = 1){
        route = null;
        record = null;
        stack.ClearInputs();
        BNScreens.Instance.ReverseTransition(t, n);
    }

    public void ClickedBackButton(){
        ReverseTransition();
    }

    public void OnConditionSliderValueChanged(float value){
        SetSelectedCondition((int)value);
        conditionText.text = BNGymDataCenter.Instance.ConditionNames[(int)value];
    }

    private void SetSelectedCondition(int index){
        BNManager.Instance.GetConditionSprite(index, OnLoadSprite);
    }

    private void OnLoadSprite(Sprite spr){
        selectedConditon.sprite = spr;
    }
    public void SetHasInsight(BNRoute.ClearStatus s){
        if (s == BNRoute.ClearStatus.Flash){
            hasInsight = true;
        }else if(s == BNRoute.ClearStatus.Onsight){
            hasInsight = false;
        }
    }

    public void OnClearSliderValueChanged(float v){
        clearRateText.text = (int)v + "%";
        if (v == 100f){
            if (record != null && record.GetTryNumber() == 1){
                clearStatusObj.SetActive(true);
            }else if(record == null && route.GetNewTryNumber() == 1){
                clearStatusObj.SetActive(true);
            }else{
                clearStatusObj.SetActive(false);
            }
        }else{
            clearStatusObj.SetActive(false);
        }
    }

    public void OnClickingDateButton(){
        ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
        picker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

        picker.Show((DateTime d) =>{
            dayText.text = d.ToString(BNGymDataCenter.FORMAT_DATE2);
            dateDT = d;
        });
    }
    public void OnClickingTimeButton(){
        ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
        picker.DatePickerMode = ISN_UIDateTimePickerMode.Time;

        picker.Show((DateTime d) =>{
            timeText.text = d.ToString(BNGymDataCenter.FORMAT_HM);
            timeDT = d;
        });
    }
    public void ShowCalendar(){
        ISN_UICalendar.PickDate((DateTime d) =>{
            dayText.text = d.ToString(BNGymDataCenter.FORMAT_TIME);
            dateDT = d;
        });
    }
}
}