using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class RegisterView: BNScreenInput
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject wallInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private TMP_InputField gymNameTextIF;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    [SerializeField] private TMP_InputField wallStartTextIF;

    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private Toggle kanteToggle;



    
    private enum ViewType{All, Gym, Wall, Route};
    private ViewType type ;

    public void ClearFields(){
        titleText.text = "";
        gymNameTextIF.text = "";
        wallType = WallTypeMap.Type.Slab;
        wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        wallStartTextIF.text = "";
        gradeText.text = "";
        kanteToggle.isOn = false;        
    }

    public override void InitForFirstTransition(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
            BNGym gym = stack.GetTargetGym();
            BNWall wall = stack.GetTargetWall();
            BNRoute route = stack.GetTargetRoute();
            if (gym == null){
                type = ViewType.Gym;
                titleText.text = "ジム登録";
            }else if (wall == null){
                type = ViewType.Wall;
                titleText.text = "壁登録";
            }else if (route == null){
                type = ViewType.Route;
                titleText.text = "課題登録";
            }
        }else{
            type = ViewType.All;
            titleText.text = "新規登録";
        }

        if (type == ViewType.All){
            gymInfo.SetActive(true);
            wallInfo.SetActive(true);
            routeInfo.SetActive(true);
        }else if(type == ViewType.Gym){
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

    public void Register(){
        BNScreenStackWithTargetGym s = null;
        if (belongingStack == null || !(belongingStack is BNScreenStackWithTargetGym)){
            return ;
        }
        s = belongingStack as BNScreenStackWithTargetGym;

        if (type == ViewType.All || type == ViewType.Gym){
            BNGym gym = new BNGym();
            gym.SetGymName(gymNameTextIF.text);
            s.WriteGym(gym);
        }

        if(type == ViewType.All || type == ViewType.Wall){
            BNWall wall = new BNWall();
            //Debug.Log(wallTypeTextIF.text);
            wall.SetWallType(wallType);
            wall.SetStart(wallStartTextIF.text);
            s.WriteWall(wall);
        }
        
        if(type == ViewType.All || type == ViewType.Route){
            BNRoute route = new BNRoute();
            route.SetGrade(grade);
            route.SetIsUsedKante(kanteToggle.isOn);
            s.WriteRoute(route);
        }

        ReverseTransition();
    }
}
}