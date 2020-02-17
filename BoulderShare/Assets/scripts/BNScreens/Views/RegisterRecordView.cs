using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;

namespace BoulderNotes {
public class RegisterRecordView : BNScreen
{
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI tryNumberText;
    [SerializeField] private Slider completeRateSlider;
    [SerializeField] private Slider conditionSlider;
    [SerializeField] private AdvancedInputField commentIF;

    [SerializeField] private Image selectedConditon;
    [SerializeField] private Sprite[] conditionImages;

    [SerializeField] private BNRecord record;
    [SerializeField] private BNRoute route;
  
    public override void InitForFirstTransition(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNGym gym = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym();
            BNWall wall = (belongingStack as BNScreenStackWithTargetGym).GetTargetWall();
            route = (belongingStack as BNScreenStackWithTargetGym).GetTargetRoute();

            if (route == null){
                //あるはずがない処理
                return ;
            }

            record = (belongingStack as BNScreenStackWithTargetGym).GetTargetRecord();

            if (record == null){
                //Debug.Log("new");
                //新規作成
                dayText.text = DateTime.Now.ToString(BNGymDataCenter.FORMAT_DATE);
                tryNumberText.text = route.GetNewTryNumber()+"";
                deleteButton.SetActive(false);
            }else{
                //Debug.Log("edit");
                //編集
                dayText.text = record.GetDate();
                tryNumberText.text = record.GetTryNumber()+"";
                completeRateSlider.value = 0f + record.GetCompleteRate();
                conditionSlider.value = 0.0f + (int)record.GetCondition();
                commentIF.Text = record.GetComment();

                deleteButton.SetActive(true);                
            }

        }
    }

    public void ClearFields(){
        dayText.text = "";
        completeRateSlider.value = 50f;
        conditionSlider.value = 2f;
        SetSelectedCondition(2);
        commentIF.Text = "";
        tryNumberText.text = "";
        record = null;
    }

    public override void UpdateScreen(){

    }

    public void Register(){
        if (route == null){
            return ;
        }
        if (record != null){
            //record.SetTime(time);
            record.SetTryNumber(int.Parse(tryNumberText.text));
            record.SetCompleteRate((int)completeRateSlider.value);
            record.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            record.SetComment(commentIF.Text);
            if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
                (belongingStack as BNScreenStackWithTargetGym).ModifyRecord(record);
            }
        }else{
            BNRecord rec = new BNRecord();
            rec.SetTryNumber(int.Parse(tryNumberText.text));
            rec.SetCompleteRate((int)completeRateSlider.value);
            rec.SetCondition((BNRecord.Condition)((int)conditionSlider.value));
            rec.SetComment(commentIF.Text);
            if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
                (belongingStack as BNScreenStackWithTargetGym).WriteRecord(rec);
            }               
        }
   
    }

    private void DeleteRecord(){
        if (route == null || record == null){
            return ;
        }
        
        route.DeleteRecord(record);
        record = null;
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).ModifyRoute(route);
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
        selectedConditon.sprite = conditionImages[index];
    }

}
}