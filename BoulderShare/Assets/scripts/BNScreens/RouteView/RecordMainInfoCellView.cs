using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using TMPro;

namespace BoulderNotes{
public class RecordMainInfoCellView :  EnhancedScrollerCellView
{
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI kanteText;
    
    public void SetData(RecordMainInfoScrollerData data){
        if (data.tape != null){
            routeTape.LoadTape(data.tape);
        }else{
            routeTape.LoadDefault();
        }
        gymNameText.text = data.gymName;
        gradeText.text = data.grade;
        dateText.text = data.date;
        if (data.usedKante){
            kanteText.text = "あり";
        }else{
            kanteText.text = "なし";
        }
    }
}
}
