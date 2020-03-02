using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using System.Threading.Tasks;

namespace BoulderNotes{
public class ModifyView: BNScreenInput
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject wallInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private AdvancedInputField gymNameTextIF;
    [SerializeField] private TMP_InputField wallTypeText;
    [SerializeField] private Toggle finishedWallToggle;
    [SerializeField] private Image wallImage;
    [SerializeField] private GameObject wallImageNoSelectedObj;
    [SerializeField] private GameObject wallImageSelectedObj;

    [SerializeField] private TMP_InputField gradeText;
    [SerializeField] private Toggle finishedRouteToggle;
    [SerializeField] private Toggle KanteToggle;
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private GameObject tapeSelectedObj;
    [SerializeField] private GameObject tapeNoSelectedObj;

    [SerializeField] private TextMeshProUGUI deleteText;

    private BNGym gym;
    private BNWall wall;
    private BNRoute route;
    
    private enum ViewType{Gym, Wall, Route};
    private ViewType type ;

    public override void ClearFields(){
        base.ClearFields();
        gym = null;
        wall = null;
        route = null;

        titleText.text = "";
        gymNameTextIF.Text = "";
        wallTypeText.text = "";
        gradeText.text = "";     

        //routeTape.Init();   
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
                gymNameTextIF.Text = gym.GetGymName();
                type = ViewType.Gym;
                titleText.text = "ジム編集";
                deleteText.text = "ジム削除";
                ShowTargetObj();
                return ;
            }

            route = stack.GetTargetRoute();
            if (route == null){
                //wall編集
                wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
                type = ViewType.Wall;
                titleText.text = "壁編集";
                deleteText.text = "壁削除";
                finishedWallToggle.isOn = wall.IsFinished();
                StartCoroutine(LoadWallImage(wall.GetWallImageFileNames()));
                ShowTargetObj();
                return ;
            }

            //route編集
            grade = route.GetGrade();
            wallType = route.GetWallType();
            gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
            finishedRouteToggle.isOn = route.IsFinished();
            KanteToggle.isOn = route.IsUsedKante();
            if (route.GetTape() != null){
                SetTape(route.GetTape());

                //updatescreenで行う
                //routeTape.LoadTape(tape);              
            }else{
                routeTape.LoadDefault();
            }

            type = ViewType.Route;
            titleText.text = "課題編集";
            deleteText.text = "課題削除";
            ShowTargetObj();
        }
    }

    private IEnumerator LoadWallImage(List<string> nameList){
        if (belongingStack == null || !(belongingStack is BNScreenStackWithTargetGym)){
            yield break;
        }
        BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
        foreach(string str in nameList){
            Sprite spr = stack.LoadWallImage(str);
            wallImage.sprite = spr;
            inputedSprite = spr;
            yield return null;
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
        if (tape != null){
            tapeSelectedObj.SetActive(true);
            tapeNoSelectedObj.SetActive(false); 
        }else{
            tapeSelectedObj.SetActive(false);
            tapeNoSelectedObj.SetActive(true);             
        }

        if (inputedSprite != null){
            wallImageNoSelectedObj.SetActive(false);
            wallImageSelectedObj.SetActive(true);
        }else{
            wallImageNoSelectedObj.SetActive(true);
            wallImageSelectedObj.SetActive(false);            
        }                    
    }

    public override void UpdateScreen(){
        wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
        if (tape != null){
            routeTape.LoadTape(tape);
        }
        if (inputedSprite != null){
            wallImage.sprite = inputedSprite;
        }

        ShowTargetObj();
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
            gym.SetGymName(gymNameTextIF.Text);
            stack.ModifyGym(gym);
            stack.ClearWall();
            stack.StoreTargetGym(gym.GetID());
        }else if(type == ViewType.Wall){
            wall.SetIsFinished(finishedWallToggle.isOn);
            if (finishedWallToggle.isOn){
                wall.SetEnd(DateTime.Now);
            }else{
                wall.ClearEnd();
            }
            
            List<BNWallImage> list = new List<BNWallImage>();
            List<string> files = new List<string>();
            if (inputedSprite != null){
                wall.SetWallImageFileNames(files);
                BNWallImage wallImage = new BNWallImage(inputedSprite.texture);
                list.Add(wallImage);
                wall.AddWallImageFileName(wallImage.fileName);
            }
            stack.ModifyWall(wall, list);
            stack.ClearRoute();
            stack.StoreTargetWall(wall.GetID());

        }else if(type == ViewType.Route){
            route.SetGrade(grade);
            route.SetWallType(wallType);
            route.SetIsFinished(finishedRouteToggle.isOn);
            if (finishedRouteToggle.isOn){
                route.SetEnd(DateTime.Now);
            }else{
                route.ClearEnd();
            }
            route.SetIsUsedKante(KanteToggle.isOn);
            route.SetTape(tape);
            stack.ModifyRoute(route);

            stack.ClearRecord();
            stack.StoreTargetRoute(route.GetID());
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