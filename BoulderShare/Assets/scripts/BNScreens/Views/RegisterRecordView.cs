using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace BoulderNotes {
public class RegisterRecordView : BNScreenInput
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
    private string recordTime;
    private bool hasInsight;
  
    public override void InitForFirstTransition(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            stack = (belongingStack as BNScreenStackWithTargetGym);
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
                dayText.text = DateTime.Now.ToString(BNGymDataCenter.FORMAT_DATE2);
                timeText.text = DateTime.Now.ToString(BNGymDataCenter.FORMAT_HM);
                recordTime = DateTime.Now.ToString(BNGymDataCenter.FORMAT_TIME);
                deleteButton.SetActive(false);
                clearStatusObj.SetActive(false);
            }else{
                //Debug.Log("edit");
                //編集
                dayText.text = record.GetDate();
                timeText.text = record.GetDate3();
                recordTime = record.GetTime();
                completeRateSlider.value = 0f + record.GetCompleteRate();
                conditionSlider.value = 0.0f + (int)record.GetCondition();
                inputedText = record.GetComment();

                if (record.GetTryNumber() == 1 && record.GetCompleteRate() == 100){
                    clearStatusObj.SetActive(true);
                }else{
                    clearStatusObj.SetActive(false);
                }

                deleteButton.SetActive(true);
            }
        }
    }

    public override void ClearFields(){
        base.ClearFields();
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
        commentText.text = inputedText;
    }

    public void Register(){
        if (route == null || stack == null){
            return ;
        }
        DateTime time = DateTime.ParseExact(recordTime,BNGymDataCenter.FORMAT_TIME, null);
        route.SetHasInsight(hasInsight);
        if (record != null){
            record.SetTime(time);
            //record.SetTryNumber(route.GetNewTryNumber());
            record.SetCompleteRate((int)completeRateSlider.value);
            record.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            record.SetComment(inputedText);

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
            record.SetComment(inputedText);

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

    public void ReverseTransition(float t = 1.0f, int n = 1){
        route = null;
        record = null;
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
        Addressables.LoadAssetsAsync<Sprite>(conditionRef[index], OnLoadSprite);
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
}
}