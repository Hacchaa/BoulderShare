using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class GradeToggleGroup : MonoBehaviour
{
    [SerializeField] private List<GradeToggle> toggles;
    [SerializeField] private InputItemsView view;
    [SerializeField] private BNGradeMap.Grade selectedGrade;

    public void Init(BNGradeMap.Grade g){
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