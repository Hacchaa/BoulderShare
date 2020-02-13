using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class ModifyView: BNScreenInput
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject wallInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private TMP_InputField gymNameTextIF;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    [SerializeField] private TMP_InputField wallStartTextIF;
    [SerializeField] private Toggle finishedWallToggle;

    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private Toggle finishedRouteToggle;
    [SerializeField] private Toggle KanteToggle;

    [SerializeField] private TextMeshProUGUI deleteText;

    private BNGym gym;
    private BNWall wall;
    private BNRoute route;
    
    private enum ViewType{Gym, Wall, Route};
    private ViewType type ;

    public void ClearFields(){
        gym = null;
        wall = null;
        route = null;

        titleText.text = "";
        gymNameTextIF.text = "";
        wallTypeText.text = "";
        wallStartTextIF.text = "";
        gradeText.text = "";        
    }

    public override void InitForFirstTransition(){
        ClearFields();

        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
            gym = stack.GetTargetGym();
            if (gym == null){
                //何もしない
                return ;
            }

            wall = stack.GetTargetWall();
            if (wall == null){
                //gym編集
                gymNameTextIF.text = gym.GetGymName();
                type = ViewType.Gym;
                titleText.text = "ジム編集";
                deleteText.text = "ジム削除";
                ShowTargetObj();
                return ;
            }

            route = stack.GetTargetRoute();
            if (route == null){
                //wall編集
                wallType = wall.GetWallType();
                wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
                wallStartTextIF.text = "" + wall.GetStart();
                type = ViewType.Wall;
                titleText.text = "壁編集";
                deleteText.text = "壁削除";
                finishedWallToggle.isOn = wall.IsFinished();
                ShowTargetObj();
                return ;
            }

            //route編集
            grade = route.GetGrade();
            gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
            finishedRouteToggle.isOn = route.IsFinished();
            KanteToggle.isOn = route.IsUsedKante();
            type = ViewType.Route;
            titleText.text = "課題編集";
            deleteText.text = "課題削除";
            ShowTargetObj();
        }
    }

    private void ShowTargetObj(){
        if(type == ViewType.Gym){
            gymInfo.SetActive(true);
            wallInfo.SetActive(false);
            routeInfo.SetActive(false);            
        }else if(type == ViewType.Wall){
            gymInfo.SetActive(false);
            wallInfo.SetActive(true);
            routeInfo.SetActive(false);            
        }else if(type == ViewType.Route){
            gymInfo.SetActive(false);
            wallInfo.SetActive(false);
            routeInfo.SetActive(true);            
        }        
    }

    public override void UpdateScreen(){
        wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void Modify(){
        if (belongingStack == null || !(belongingStack is BNScreenStackWithTargetGym)){
            return ;
        }
        BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
        if (type == ViewType.Gym){
            BNGym newGym = new BNGym();
            newGym.SetID(gym.GetID());
            newGym.SetGymName(gymNameTextIF.text);
            newGym.SetWallIDs(gym.GetWallIDs());
            stack.ModifyGym(newGym);

        }else if(type == ViewType.Wall){
            BNWall newWall = new BNWall();
            newWall.SetID(wall.GetID());
            newWall.SetWallType(wallType);
            newWall.SetStart(wallStartTextIF.text);
            newWall.SetRouteIDs(wall.GetRouteIDs());
            newWall.SetIsFinished(finishedWallToggle.isOn);
            stack.ModifyWall(newWall);
        }else if(type == ViewType.Route){
            BNRoute newRoute = new BNRoute();
            newRoute.SetID(route.GetID());
            newRoute.SetGrade(grade);
            newRoute.SetRecords(route.GetRecords());
            newRoute.SetIsFinished(finishedRouteToggle.isOn);
            newRoute.SetIsUsedKante(KanteToggle.isOn);
            stack.ModifyRoute(newRoute);
        }

        ReverseTransition();
    }

    public void Delete(){
        if (belongingStack == null || !(belongingStack is BNScreenStackWithTargetGym)){
            return ;
        }
        BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
        if (type == ViewType.Gym){
            stack.DeleteGym();
        }else if(type == ViewType.Wall){
            stack.DeleteWall();
        }else if(type == ViewType.Route){
            stack.DeleteRoute();
        }

        ReverseTransitionDouble();
    }
    private void ReverseTransitionDouble(){
        BNScreens.Instance.ReverseTransition(1.0f, 2);
    }
}
}