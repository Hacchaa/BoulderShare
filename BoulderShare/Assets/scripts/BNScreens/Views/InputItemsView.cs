using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AdvancedInputFieldPlugin;

namespace BoulderNotes{
public class InputItemsView: BNScreenWithGyms
{
    public enum TargetItem{None, WallType, Grade, Texts, Tape, Image, Sort};
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TargetItem currentTargetItem;

    [SerializeField] private WallTypeToggleGroup wallTypeGroup;
    [SerializeField] private GradeToggleGroup gradeGroup;
    [SerializeField] private SortToggleGroup sortToggleGroup;
    [SerializeField] private GameObject textObj;
    [SerializeField] private GameObject walltypeObj;
    [SerializeField] private GameObject gradeObj;
    [SerializeField] private GameObject sortObj;
    [SerializeField] private AdvancedInputField advancedIF;
    private BNScreenStackWithTargetGym stack;
    public override void InitForFirstTransition(){
        stack = GetScreenStackWithGyms();
        if (stack != null){
            currentTargetItem = stack.GetTargetItemToInput();
            
            ShowObj();

            if (currentTargetItem == TargetItem.WallType){
                titleText.text = "壁の種類";
                wallTypeGroup.Init(stack.GetTargetWallType());
            }else if (currentTargetItem == TargetItem.Grade){
                titleText.text = "グレード";
                gradeGroup.Init(stack.GetTargetGrade());
            }else if (currentTargetItem == TargetItem.Texts){
                titleText.text = "テキスト";
                advancedIF.Text = stack.GetTargetString();
            }else if (currentTargetItem == TargetItem.Sort){
                titleText.text = "";
                sortToggleGroup.Init(stack.GetTargetSortType());

            }
        }
    }
    private void ShowObj(){
        if (currentTargetItem == TargetItem.WallType){
            BNManager.Instance.ActivateNecessary(walltypeObj, true);
            BNManager.Instance.ActivateNecessary(gradeObj, false);
            BNManager.Instance.ActivateNecessary(textObj, false);
            BNManager.Instance.ActivateNecessary(sortObj, false);
        }else if(currentTargetItem == TargetItem.Grade){
            BNManager.Instance.ActivateNecessary(gradeObj, true);
            BNManager.Instance.ActivateNecessary(walltypeObj, false);
            BNManager.Instance.ActivateNecessary(textObj, false);
            BNManager.Instance.ActivateNecessary(sortObj, false);
        }else if (currentTargetItem == TargetItem.Texts){
            BNManager.Instance.ActivateNecessary(textObj, true);
            BNManager.Instance.ActivateNecessary(walltypeObj, false);
            BNManager.Instance.ActivateNecessary(gradeObj, false);
            BNManager.Instance.ActivateNecessary(sortObj, false);
        }else if (currentTargetItem == TargetItem.Sort){
            BNManager.Instance.ActivateNecessary(sortObj, true);
            BNManager.Instance.ActivateNecessary(walltypeObj, false);
            BNManager.Instance.ActivateNecessary(gradeObj, false);
            BNManager.Instance.ActivateNecessary(textObj, false);
        }
    }

    public override void UpdateScreen(){

    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void Register(){
        if (stack != null){
            if (currentTargetItem == TargetItem.WallType){
                stack.SetTargetWallType(wallTypeGroup.GetWallType());
            }else if(currentTargetItem == TargetItem.Grade){
                stack.SetTargetGrade(gradeGroup.GetGrade());
            }else if(currentTargetItem == TargetItem.Texts){
                stack.SetTargetString(advancedIF.Text);
            }else if(currentTargetItem == TargetItem.Sort){
                stack.SetTargetSortType(sortToggleGroup.GetSortType());
            }
        }

        ReverseTransition();
    }
}
}