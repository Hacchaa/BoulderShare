using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class RecordMainInfoCellView :  EnhancedScrollerCellView
{
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI kanteText;
    [SerializeField] private Image frame;
    [SerializeField] private TextMeshProUGUI clearStatusText;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    private bool procInit = false;
    public void Init(){
        if (procInit){
            return;
        }

        procInit = true;
        BNManager.Instance.GetCornerPanelFill(OnLoad);
    }
    private void OnLoad(Sprite sprite){
        frame.sprite = sprite;
    }
    
    public void SetData(RecordMainInfoScrollerData data){
        if (data.tape != null){
            routeTape.LoadTape(data.tape);
        }else{
            routeTape.LoadDefault();
        }
        gymNameText.text = data.gymName;
        gradeText.text = data.grade;
        dateText.text = data.date;
        clearStatusText.text = data.completeStatusName;
        wallTypeText.text = data.wallTypeName;
        
        if (data.usedKante){
            kanteText.text = "あり";
        }else{
            kanteText.text = "なし";
        }
    }
}
}
