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
    protected string inputedText;
    protected Sprite inputedSprite;
    public virtual void ClearFields(){
        tape = null;
        inputedSprite = null;
        inputedText = "";
        wallType = WallTypeMap.Type.Slab;
        grade = BNGradeMap.Grade.None;
    }

    public void SetSprite(Sprite spr){
        inputedSprite = spr;
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

    public void SetText(string txt){
        inputedText = txt;
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

    public string GetText(){
        return inputedText;
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
    public void InputWallImage(){
        ToSelecteWallImageView();        
    }
    public void ToEditRouteTapeView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.EditRouteTapeView, BNScreens.TransitionType.Push);
    }

    public void ToInputItemsView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.InputItemsView, BNScreens.TransitionType.Push);
    }
    public void ToSelecteWallImageView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SelecteWallImageView, BNScreens.TransitionType.Push);
    }
}
public interface IBNScreenInputable{
    void SetWallType(WallTypeMap.Type t);
    WallTypeMap.Type GetWallType();
    InputItemsView.TargetItem GetCurrentTargetItem();
}
}