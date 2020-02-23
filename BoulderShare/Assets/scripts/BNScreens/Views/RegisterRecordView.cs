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
    [SerializeField] private TextMeshProUGUI tryNumberText;
    [SerializeField] private Slider completeRateSlider;
    [SerializeField] private Slider conditionSlider;
    [SerializeField] private TextMeshProUGUI commentText;

    [SerializeField] private Image selectedConditon;
    [SerializeField] private AssetReference[] conditionRef;

    [SerializeField] private BNRecord record;
    [SerializeField] private BNRoute route;
    [SerializeField] private ClearStatusToggle[] toggles;
    [SerializeField] private GameObject clearStatusObj;
    private BNScreenStackWithTargetGym stack;
  
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
            for(int i = 0 ; i < toggles.Length ; i++){
                toggles[i].Init(status);
            }

            record = stack.GetTargetRecord();

            if (record == null){
                //Debug.Log("new");
                //新規作成
                dayText.text = DateTime.Now.ToString(BNGymDataCenter.FORMAT_DATE);
                tryNumberText.text = route.GetNewTryNumber()+"";
                deleteButton.SetActive(false);
                clearStatusObj.SetActive(false);
            }else{
                //Debug.Log("edit");
                //編集
                dayText.text = record.GetDate();
                tryNumberText.text = record.GetTryNumber()+"";
                completeRateSlider.value = 0f + record.GetCompleteRate();
                conditionSlider.value = 0.0f + (int)record.GetCondition();
                inputedText = record.GetComment();

                deleteButton.SetActive(true);
            }
        }
    }

    public override void ClearFields(){
        base.ClearFields();
        dayText.text = "";
        completeRateSlider.value = 50f;
        conditionSlider.value = 2f;
        SetSelectedCondition(2);
        commentText.text = "";
        tryNumberText.text = "";
        record = null;
        route = null;
        stack = null;
    }

    public override void UpdateScreen(){
        commentText.text = inputedText;
    }

    public void Register(){
        if (route == null || stack == null){
            return ;
        }

        if (record != null){
            //record.SetTime(time);
            record.SetTryNumber(int.Parse(tryNumberText.text));
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
                route.DeleteRecord(oldRecord);
                route.AddRecord(record);
            }
        }else{
            record = new BNRecord();
            record.SetTryNumber(int.Parse(tryNumberText.text));
            record.SetCompleteRate((int)completeRateSlider.value);
            record.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            record.SetComment(inputedText);

            route.AddRecord(record);           
        }
        route.ReCalculateClearStatus();
        stack.ModifyRoute(route);
        stack.StoreTargetRecord(record.GetID());  
    }

    private void DeleteRecord(){
        if (record == null){
            return ;
        }
        if (stack != null){
            route.DeleteRecord(record);
            route.ReCalculateClearStatus();
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
    }

    private void SetSelectedCondition(int index){
        Addressables.LoadAssetsAsync<Sprite>(conditionRef[index], OnLoadSprite);
    }

    private void OnLoadSprite(Sprite spr){
        selectedConditon.sprite = spr;
    }
    public void SetHasInsight(BNRoute.ClearStatus s){
        if (s == BNRoute.ClearStatus.Flash){
            route.SetHasInsight(true);
        }else if(s == BNRoute.ClearStatus.Onsight){
            route.SetHasInsight(false);
        }
    }

    public void OnClearSliderValueChanged(float v){
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