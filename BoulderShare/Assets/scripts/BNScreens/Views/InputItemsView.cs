using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AdvancedInputFieldPlugin;

namespace BoulderNotes{
public class InputItemsView: BNScreen
{
    public enum TargetItem{None, WallType, Grade, Texts, Tape, Image, Sort};
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TargetItem currentTargetItem;
    [SerializeField] private BNScreenInput screen;

    [SerializeField] private WallTypeToggleGroup wallTypeGroup;
    [SerializeField] private GradeToggleGroup gradeGroup;
    [SerializeField] private SortToggleGroup sortToggleGroup;
    [SerializeField] private GameObject textObj;
    [SerializeField] private GameObject walltypeObj;
    [SerializeField] private GameObject gradeObj;
    [SerializeField] private GameObject sortObj;
    [SerializeField] private AdvancedInputField advancedIF;

    public override void InitForFirstTransition(){
        screen = null;
        
        if (belongingStack != null){
            BNScreen s = belongingStack.GetPreviousScreen(1);
            if (s is BNScreenInput){
                screen = s as BNScreenInput;
                currentTargetItem = screen.GetCurrentTargetItem();

                ShowObj();

                if (currentTargetItem == TargetItem.WallType){
                    titleText.text = "壁の種類";
                    wallTypeGroup.Init(screen.GetWallType());
                }else if (currentTargetItem == TargetItem.Grade){
                    titleText.text = "グレード";
                    gradeGroup.Init(screen.GetGrade());
                }else if (currentTargetItem == TargetItem.Texts){
                    titleText.text = "テキスト";
                    advancedIF.Text = screen.GetText();
                }else if (currentTargetItem == TargetItem.Sort){
                    titleText.text = "";
                    sortToggleGroup.Init(screen.GetSortType());

                }
            }
        }
    }
    private void ShowObj(){
        walltypeObj.SetActive(false);
        gradeObj.SetActive(false);
        textObj.SetActive(false);
        sortObj.SetActive(false);
        if (currentTargetItem == TargetItem.WallType){
            walltypeObj.SetActive(true);
        }else if(currentTargetItem == TargetItem.Grade){
            gradeObj.SetActive(true);
        }else if (currentTargetItem == TargetItem.Texts){
            textObj.SetActive(true);
        }else if (currentTargetItem == TargetItem.Sort){
            sortObj.SetActive(true);
        }
    }

    public override void UpdateScreen(){

    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void Register(){
        if (screen != null){
            if (currentTargetItem == TargetItem.WallType){
                screen.SetWallType(wallTypeGroup.GetWallType());
            }else if(currentTargetItem == TargetItem.Grade){
                screen.SetGrade(gradeGroup.GetGrade());
            }else if(currentTargetItem == TargetItem.Texts){
                screen.SetText(advancedIF.Text);
            }else if(currentTargetItem == TargetItem.Sort){
                screen.SetSortType(sortToggleGroup.GetSortType());
            }
        }

        ReverseTransition();
    }
}
}