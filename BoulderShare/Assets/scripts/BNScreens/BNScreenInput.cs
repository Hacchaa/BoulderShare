using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoulderNotes{
public class BNScreenInput : BNScreen
{
    [SerializeField] protected InputItemsView.TargetItem currentTargetItem;
    [SerializeField] protected WallTypeMap.Type wallType;
    protected BNGradeMap.Grade grade;
    [SerializeField] protected RTape tape;

    public virtual void ClearFields(){
        tape = null;
    }
    public void SetWallType(WallTypeMap.Type t){
        wallType = t;
    }

    public WallTypeMap.Type GetWallType(){
        return wallType;
    }

    public void SetGrade(BNGradeMap.Grade g){
        grade = g;
    }

    public void SetTape(RTape t){
        if (t == null){
            tape = null;
            return;
        }
        tape = t.Clone();
        Debug.Log("z in screeninput :"+ tape.imageRot.eulerAngles.z);
    }

    public RTape GetTape(){
        if (tape == null){
            return null;
        }
        return tape.Clone();
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

    public void InputTape(){
        currentTargetItem = InputItemsView.TargetItem.Tape;
        ToEditRouteTapeView();        
    }
    public void ToEditRouteTapeView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.EditRouteTapeView, BNScreens.TransitionType.Push);
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