﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;

namespace BoulderNotes{
public class RegisterView: BNScreenInput
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject wallInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private AdvancedInputField gymNameTextIF;
    [SerializeField] private TMP_InputField wallTypeIF;
    [SerializeField] private Image wallImage;
    [SerializeField] private GameObject wallImageNoSelectedObj;
    [SerializeField] private GameObject wallImageSelectedObj;

    [SerializeField] private TMP_InputField gradeIF;
    [SerializeField] private Toggle kanteToggle;
    [SerializeField] private GameObject tapeSelectedObj;
    [SerializeField] private GameObject tapeNoSelectedObj;
    [SerializeField] private RouteTape routeTape;


    
    private enum ViewType{All, Gym, Wall, Route};
    private ViewType type ;

    public override void ClearFields(){
        base.ClearFields();
        titleText.text = "";
        gymNameTextIF.Text = "";
        wallType = WallTypeMap.Type.Slab;
        wallTypeIF.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        gradeIF.text = "";
        kanteToggle.isOn = false;

        routeTape.LoadDefault();        
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
        Show();
    }

    private void Show(){
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
        wallTypeIF.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        gradeIF.text = BNGradeMap.Entity.GetGradeName(grade);
        Show();
        if (tape != null){
            routeTape.LoadTape(tape);
        }
        if (inputedSprite != null){
            wallImage.sprite = inputedSprite;
        }
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
            gym.SetGymName(gymNameTextIF.Text);
            s.WriteGym(gym);
            s.StoreTargetGym(gym.GetID());
        }

        if(type == ViewType.All || type == ViewType.Wall){
            BNWall wall = new BNWall();
            //Debug.Log(wallTypeIFIF.text);
            wall.SetWallType(wallType);

            List<BNWallImage> list = new List<BNWallImage>();
            if (inputedSprite != null){
                BNWallImage wallImage = new BNWallImage(inputedSprite.texture);
                list.Add(wallImage);
                wall.AddWallImageFileName(wallImage.fileName);
            }
            s.WriteWall(wall, list);
            s.StoreTargetWall(wall.GetID());
        }
        
        if(type == ViewType.All || type == ViewType.Route){
            BNRoute route = new BNRoute();
            route.SetGrade(grade);
            route.SetIsUsedKante(kanteToggle.isOn);
            route.SetTape(tape);
            s.WriteRoute(route);
            s.StoreTargetRoute(route.GetID());
        }

        ReverseTransition();
    }
}
}