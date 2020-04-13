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
    [SerializeField] private Image fillImage;
    [SerializeField] private Image flameImage;
    [SerializeField] private Color onBackColor;
    [SerializeField] private Color offBackColor;
    [SerializeField] private Color onTextColor;
    [SerializeField] private Color offTextColor;

    private ScrollGradeController controller;
    private BNGradeMap.Grade grade;
    private bool completedInit = false;
    public void Init(ScrollGradeController cont){
        controller = cont;

        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanelFill(OnLoadFill);
            BNManager.Instance.GetCornerPanelStroke(OnLoadFlame);
        }
    }
    private void OnLoadFlame(Sprite sprite){
        flameImage.sprite = sprite;
    }
    private void OnLoadFill(Sprite sprite){
        fillImage.sprite = sprite;
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
        fillImage.color = onBackColor;
    }

    public void FocusOff(){
        gradeText.color = offTextColor;
        fillImage.color = offBackColor;
    }

    public void SendClickInfo(){
        controller.Register(this);
    }
}
}