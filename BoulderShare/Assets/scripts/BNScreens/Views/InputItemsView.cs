using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class InputItemsView: BNScreen
{
    public enum TargetItem{None, WallType, Grade, Texts};
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TargetItem currentTargetItem;
    private BNScreenInput screen;

    [SerializeField] private WallTypeToggleGroup wallTypeGroup;
    [SerializeField] private GradeToggleGroup gradeGroup;
    [SerializeField] private GameObject textObj;
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
                
                }
            }
        }
    }
    private void ShowObj(){
        wallTypeGroup.gameObject.SetActive(false);
        gradeGroup.gameObject.SetActive(false);
        textObj.SetActive(false);
        if (currentTargetItem == TargetItem.WallType){
            wallTypeGroup.gameObject.SetActive(true);
        }else if(currentTargetItem == TargetItem.Grade){
            gradeGroup.gameObject.SetActive(true);
        }else if (currentTargetItem == TargetItem.Texts){
            textObj.SetActive(true);
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
            }
        }

        ReverseTransition();
    }
}
}