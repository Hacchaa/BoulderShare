using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoulderNotes{
public class BNScreenInput : BNScreen
{
    [SerializeField] protected InputItemsView.TargetItem currentTargetItem;
    [SerializeField] protected WallTypeMap.Type wallType;
    protected BNGradeMap.Grade grade;
    public void SetWallType(WallTypeMap.Type t){
        wallType = t;
    }

    public WallTypeMap.Type GetWallType(){
        return wallType;
    }

    public void SetGrade(BNGradeMap.Grade g){
        grade = g;
    }

    public BNGradeMap.Grade GetGrade(){
        return grade;
    }    

    public InputItemsView.TargetItem GetCurrentTargetItem(){
        return currentTargetItem;
    }

    public void InputWallType(){
        currentTargetItem = InputItemsView.TargetItem.WallType;
        ToInputItemsView();
    }

    public void InputGrade(){
        currentTargetItem = InputItemsView.TargetItem.Grade;
        ToInputItemsView();        
    }

    public void InputTexts(){
        currentTargetItem = InputItemsView.TargetItem.Texts;
        ToInputItemsView();
    }

    public void ToInputItemsView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.InputItemsView, BNScreens.TransitionType.Push);
    }
}
public interface IBNScreenInputable{
    void SetWallType(WallTypeMap.Type t);
    WallTypeMap.Type GetWallType();
    InputItemsView.TargetItem GetCurrentTargetItem();
}
}