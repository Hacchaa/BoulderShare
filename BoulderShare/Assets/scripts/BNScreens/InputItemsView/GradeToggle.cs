using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class GradeToggle : MonoBehaviour
{
    [SerializeField] private BNGradeMap.Grade grade;
    [SerializeField] private GradeToggleGroup group;
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI nameText;
    private bool noProc;

    public void Init(BNGradeMap.Grade target){
        noProc = false;
        nameText.text = BNGradeMap.Entity.GetGradeName(grade);

        if (target == grade){
            SetOnWithNoProc();
        }else{
            toggle.isOn = false;
        }
    }
    public void OnValueChanged(bool b){
        if (!noProc && b){
            group.SetGrade(grade);
            group.Register();
        }
        noProc = false;
    }

    private void SetOnWithNoProc(){
        noProc = true;
        toggle.isOn = true;
    }
}
}
