using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class ScrollGradeItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private Image image;
    [SerializeField] private Color onBackColor;
    [SerializeField] private Color offBackColor;
    [SerializeField] private Color onTextColor;
    [SerializeField] private Color offTextColor;

    private ScrollGradeController controller;
    private BNGradeMap.Grade grade;
    
    public void Init(ScrollGradeController cont){
        controller = cont;
    }
    public void SetNum(int n){
        numText.text = n +"件";
    }
    public void SetGrade(BNGradeMap.Grade g){
        if (g == BNGradeMap.Grade.None){
            gradeText.text = "全部";
        }else{
            gradeText.text = BNGradeMap.Entity.GetGradeName(g);
        }
        grade = g;
    }

    public BNGradeMap.Grade GetGrade(){
        return grade;
    }

    public void FocusOn(){
        gradeText.color = onTextColor;
        image.color = onBackColor;
    }

    public void FocusOff(){
        gradeText.color = offTextColor;
        image.color = offBackColor;
    }

    public void SendClickInfo(){
        controller.Register(this);
    }
}
}