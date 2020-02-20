using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class GradeToggleGroup : MonoBehaviour
{
    [SerializeField] private List<GradeToggle> toggles;
    [SerializeField] private InputItemsView view;
    [SerializeField] private BNGradeMap.Grade selectedGrade;
    [SerializeField] private ScrollRect scroller;
    public void Init(BNGradeMap.Grade g){
        scroller.verticalNormalizedPosition = 0.0f;
        selectedGrade = g;
        foreach(GradeToggle tog in toggles){
            tog.Init(g);
        }
    }

    public void SetGrade(BNGradeMap.Grade g){
        //Debug.Log("setGrade "+g.ToString());
        selectedGrade = g;
    }

    public BNGradeMap.Grade GetGrade(){
        return selectedGrade;
    }
}
}