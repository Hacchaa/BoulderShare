using System.Collections;
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
    
    [SerializeField] private GameObject backButton;

    
    private enum ViewType{All, Gym, Wall, Route};
    private ViewType type ;
    private BNScreenStackWithTargetGym stack;
    public override void ClearFields(){
        base.ClearFields();
        titleText.text = "";
        gymNameTextIF.Text = "";
        wallType = WallTypeMap.Type.Slab;
        wallTypeIF.text = WallTypeMap.Entity.GetWallTypeName(wallType);
        gradeIF.text = "";
        kanteToggle.isOn = false;
        stack = null;
        routeTape.LoadDefault();        
    }

    public override void InitForFirstTransition(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            stack = belongingStack as BNScreenStackWithTargetGym;
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
        gymInfo.SetActive(false);
        wallInfo.SetActive(false);
        routeInfo.SetActive(false);
        backButton.SetActive(true);
        if (type == ViewType.All){
            gymInfo.SetActive(true);
            wallInfo.SetActive(true); 
            routeInfo.SetActive(true);
            backButton.SetActive(false);
        }else if(type == ViewType.Gym){
            gymInfo.SetActive(true);           
        }else if(type == ViewType.Wall){
            wallInfo.SetActive(true);          
        }else if(type == ViewType.Route){
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
        if (stack != null){
            if (type == ViewType.Gym){
                BNGym gym = new BNGym();
                gym.SetGymName(gymNameTextIF.Text);
                stack.WriteGym(gym);
                stack.StoreTargetGym(gym.GetID());
            }else if(type == ViewType.Wall){
                BNWall wall = new BNWall();
                //Debug.Log(wallTypeIFIF.text);
                wall.SetWallType(wallType);

                List<BNWallImage> list = new List<BNWallImage>();
                if (inputedSprite != null){
                    BNWallImage wallImage = new BNWallImage(inputedSprite.texture);
                    list.Add(wallImage);
                    wall.AddWallImageFileName(wallImage.fileName);
                }
                stack.WriteWall(wall, list);
                stack.StoreTargetWall(wall.GetID());
            }else if(type == ViewType.Route){
                BNRoute route = new BNRoute();
                route.SetGrade(grade);
                route.SetIsUsedKante(kanteToggle.isOn);
                route.SetTape(tape);
                stack.WriteRoute(route);
                stack.StoreTargetRoute(route.GetID());
            }
            ReverseTransition();
        }else{
            if (type == ViewType.All){
                BNGym gym = new BNGym();
                gym.SetGymName(gymNameTextIF.Text);

                BNWall wall = new BNWall();
                wall.SetWallType(wallType);

                List<BNWallImage> list = new List<BNWallImage>();
                if (inputedSprite != null){
                    BNWallImage wallImage = new BNWallImage(inputedSprite.texture);
                    list.Add(wallImage);
                    wall.AddWallImageFileName(wallImage.fileName);
                }
                gym.AddWall(wall);

                BNRoute route = new BNRoute();
                route.SetGrade(grade);
                route.SetIsUsedKante(kanteToggle.isOn);
                route.SetTape(tape);
                wall.AddRoute(route);

                BNGymDataCenter.Instance.WriteGym(gym);
                BNGymDataCenter.Instance.SaveWallImages(gym, list);
            }
            ClearFields();
            BNTab.Instance.ToHomeTab();
        }
    }
}
}