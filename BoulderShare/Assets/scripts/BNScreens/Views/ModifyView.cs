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
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private AdvancedInputField gymNameTextIF;
    [SerializeField] private TMP_InputField wallTypeText;
    [SerializeField] private Image wallImage;
    [SerializeField] private GameObject wallImageNoSelectedObj;
    [SerializeField] private GameObject wallImageSelectedObj;

    [SerializeField] private TMP_InputField gradeText;
    [SerializeField] private IOSUISwitch finishedRouteToggle;
    [SerializeField] private IOSUISwitch kanteToggle;
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private GameObject tapeSelectedObj;
    [SerializeField] private GameObject tapeNoSelectedObj;

    [SerializeField] private TextMeshProUGUI deleteText;

    private BNGym gym;

    private BNRoute route;
    
    private enum ViewType{Gym, Route};
    private ViewType type ;

    public override void ClearFields(){
        base.ClearFields();
        gym = null;
        route = null;

        titleText.text = "";
        gymNameTextIF.Text = "";
        wallTypeText.text = "";
        gradeText.text = "";     
        kanteToggle.Init(false);
        finishedRouteToggle.Init(false);
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

            route = stack.GetTargetRoute();
            if (route == null){
                //gym編集
                gymNameTextIF.Text = gym.GetGymName();
                type = ViewType.Gym;
                titleText.text = "ジム編集";
                deleteText.text = "ジム削除";
                ShowTargetObj();
                return ;
            }

            //route編集
            grade = route.GetGrade();
            wallType = route.GetWallType();
            gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
            finishedRouteToggle.SetIsOn(route.IsFinished());
            kanteToggle.SetIsOn(route.IsUsedKante());
            tape = route.GetTape();
            if (tape != null){
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
/*
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
    }*/

    private void ShowTargetObj(){
        if(type == ViewType.Gym){
            gymInfo.SetActive(true);
            routeInfo.SetActive(false);            
        }else if(type == ViewType.Route){
            gymInfo.SetActive(false);
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
        }else{
            routeTape.LoadDefault();
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
            stack.ClearRoute();
            stack.StoreTargetGym(gym.GetID());
        }else if(type == ViewType.Route){
            route.SetGrade(grade);
            route.SetWallType(wallType);
            route.SetIsFinished(finishedRouteToggle.IsOn());
            if (finishedRouteToggle.IsOn()){
                route.SetEnd(DateTime.Now);
            }else{
                route.ClearEnd();
            }
            route.SetIsUsedKante(kanteToggle.IsOn());
            route.SetTape(tape);

            BNWallImage wallImage = null;
            List<BNWallImageNames> files = new List<BNWallImageNames>();
            if (inputedSprite != null){
                route.SetWallImageFileNames(files);
                wallImage = new BNWallImage(inputedSprite.texture);
                BNWallImageNames names = new BNWallImageNames();
                names.fileName = wallImage.fileName;
                route.AddWallImageFileName(names);
            }
            stack.ModifyRoute(route, wallImage);

            if (wallImage != null){
                //gymboardの設定
                BNGym g = stack.GetTargetGym();
                if (string.IsNullOrEmpty(g.GetBoardImagePath())){
                    g.SetBoardImagePath(wallImage.fileName);
                    stack.ModifyGym(g);  
                }
            }
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
        }else if(type == ViewType.Route){
            stack.DeleteRoute();

            //ジムボード画像の変更が必要かどうか
            BNGym g = stack.GetTargetGym();
            if (!stack.HasWallImage(g.GetBoardImagePath())){
                g.SetBoardImagePath(stack.FindOldestWallImageName());
                stack.ModifyGym(g);
            }
        }

        ReverseTransitionDouble();
    }
    private void ReverseTransitionDouble(){
        BNScreens.Instance.ReverseTransition(1.0f, 2);
    }
}
}