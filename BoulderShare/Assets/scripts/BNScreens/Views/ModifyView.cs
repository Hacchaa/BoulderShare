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
                wallType = wall.GetWallType();
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
            BNGym newGym = gym.Clone();
            newGym.SetGymName(gymNameTextIF.Text);
            stack.ModifyGym(newGym);

        }else if(type == ViewType.Wall){
            BNWall newWall = wall.Clone();
            newWall.SetWallType(wallType);
            newWall.SetIsFinished(finishedWallToggle.isOn);
            if (finishedWallToggle.isOn){
                newWall.SetEnd(DateTime.Now);
            }else{
                newWall.ClearEnd();
            }
            
            List<BNWallImage> list = new List<BNWallImage>();
            List<string> removeList = null;
            if (inputedSprite != null){
                BNWallImage wallImage = new BNWallImage(inputedSprite.texture);
                list.Add(wallImage);
                removeList = wall.GetWallImageFileNames();
                wall.AddWallImageFileName(wallImage.fileName);
            }
            stack.ModifyWall(wall, list, removeList);

        }else if(type == ViewType.Route){
            BNRoute newRoute = route.Clone();
            newRoute.SetGrade(grade);
            newRoute.SetIsFinished(finishedRouteToggle.isOn);
            if (finishedRouteToggle.isOn){
                newRoute.SetEnd(DateTime.Now);
            }else{
                newRoute.ClearEnd();
            }
            newRoute.SetIsUsedKante(KanteToggle.isOn);
            newRoute.SetTape(tape);
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