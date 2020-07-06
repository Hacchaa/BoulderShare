using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using System.Linq;

namespace BoulderNotes{
public class RegisterView: BNScreenWithGyms
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private AdvancedInputField gymNameTextIF;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private IOSUISwitch kanteToggle;
    [SerializeField] private GameObject tapeSelectedObj;
    [SerializeField] private GameObject tapeNoSelectedObj;
    [SerializeField] private RouteTape routeTape;
    
    [SerializeField] private GameObject backButton;
    [SerializeField] private Color textColor_NoSelected;
    [SerializeField] private Color textColor_Selected;
    [SerializeField] private SelecteWallImages_RegisterView wallImages;
    [SerializeField] private GameObject wallImagesObj;
    private BNGradeMap.Grade grade;
    private WallTypeMap.Type wallType;
    private Sprite sprite;
    private RTape tape;   
    private enum ViewType{All, Gym, Route};
    private ViewType type ;
    private BNScreenStackWithTargetGym stack;
    public void ClearFields(){
        titleText.text = "";
        gymNameTextIF.Text = "";
        wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(WallTypeMap.Type.Slab);
        gradeText.text = "";
        kanteToggle.Init(false);
        stack = null;
        routeTape.LoadDefault();   

        grade = BNGradeMap.Grade.None;
        sprite = null;
        wallType = WallTypeMap.Type.Slab;
        tape = null;     
        wallImages.Init();
    }

    public override void InitForFirstTransition(){
        ClearFields();
        stack = GetScreenStackWithGyms();
        stack.ClearInputs();

        BNGym gym = stack.GetTargetGym();
        BNRoute route = stack.GetTargetRoute();
        if (gym == null){
            type = ViewType.Gym;
            titleText.text = "ジム登録";
        }else if (route == null){
            type = ViewType.Route;
            titleText.text = "課題登録";
        }
    }

    private void Show(){
        if (type == ViewType.All){
            BNManager.Instance.ActivateNecessary(gymInfo,true);
            BNManager.Instance.ActivateNecessary(routeInfo,true);
            BNManager.Instance.ActivateNecessary(wallImagesObj, true);
            BNManager.Instance.ActivateNecessary(backButton, false);
        }else if(type == ViewType.Gym){
            BNManager.Instance.ActivateNecessary(gymInfo,true);  
            BNManager.Instance.ActivateNecessary(routeInfo,false);
            BNManager.Instance.ActivateNecessary(wallImagesObj, false);
            BNManager.Instance.ActivateNecessary(backButton, true);         
        }else if(type == ViewType.Route){
            BNManager.Instance.ActivateNecessary(routeInfo,true);
            BNManager.Instance.ActivateNecessary(wallImagesObj, true);
            BNManager.Instance.ActivateNecessary(gymInfo,false);
            BNManager.Instance.ActivateNecessary(backButton, true);
        }
        if (tape != null && !tape.isDefault){
            BNManager.Instance.ActivateNecessary(tapeSelectedObj, true);
            BNManager.Instance.ActivateNecessary(tapeNoSelectedObj, false); 
        }else{
            BNManager.Instance.ActivateNecessary(tapeSelectedObj, false);
            BNManager.Instance.ActivateNecessary(tapeNoSelectedObj, true);             
        }  

        kanteToggle.SetIsOn(false);
    }

    public override void UpdateScreen(){
        if (stack.InputWallType()){
            wallType = stack.GetTargetWallType();
            wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
            if (wallType == WallTypeMap.Type.Slab){
                //未選択(未実装)本当はtextColor_NoSelectedを代入
                wallTypeText.color = textColor_Selected;
            }else{
                //選択
                wallTypeText.color = textColor_Selected;
            }
        }
        if (stack.InputGrade()){
            grade = stack.GetTargetGrade();
            gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
            if (grade == BNGradeMap.Grade.None){
                //未選択
                gradeText.color = textColor_NoSelected;
            }else{
                //選択
                gradeText.color = textColor_Selected;
            }
        }
        RTape t = stack.GetTargetTape();
        if (t != null){
            tape = t;
            routeTape.LoadTape(tape);
        }
        Show();
        stack.ClearInputs();
    }

    public void ReverseTransition(){
        stack.ClearInputs();
        BNScreens.Instance.ReverseTransition();
    }
    public void InputWallType(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.WallType);
        stack.SetTargetWallType(wallType);
        ToInputItemsView();
    }
    public void InputGrade(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.Grade);
        stack.SetTargetGrade(grade);
        ToInputItemsView();
    }
    public void InputSprite(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.Image);
        stack.SetTargetSprite(sprite);
        ToSelecteWallImageView();
    }
    public void InputTape(){
        stack.SetTargetItemToInput(InputItemsView.TargetItem.Tape);
        stack.SetTargetTape(tape);
        ToEditRouteTapeView();
    }
    private void ToEditRouteTapeView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.EditRouteTapeView, BNScreens.TransitionType.Push);
    }

    private void ToInputItemsView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.InputItemsView, BNScreens.TransitionType.Push);
    }
    private void ToSelecteWallImageView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SelecteWallImageView, BNScreens.TransitionType.Push);
    }

    public void Register(){
        if (type == ViewType.Gym){
            BNGym gym = new BNGym();
            gym.SetGymName(gymNameTextIF.Text);
            
            stack.WriteGym(gym);
            stack.StoreTargetGym(gym.GetID());
        }else if(type == ViewType.Route){
            BNRoute route = new BNRoute();
            route.SetGrade(grade);
            route.SetWallType(wallType);
            route.SetIsUsedKante(kanteToggle.IsOn());
            route.SetTape(tape);
            
            //画像の保存
            List<BNWallImage> wallImageList = new List<BNWallImage>();
            BNWallImage[] images = wallImages.GetImages();
            BNWallImageNames tmpNames = null;
            if (images != null){
                //i % 2 == 0は必ず存在すると仮定
                for(int i = 0 ; i < images.Length ; i++){
                    if (i % 2 == 0){
                        if(images[i] != null){
                            tmpNames = new BNWallImageNames();
                            tmpNames.fileName = images[i].fileName;
                            wallImageList.Add(images[i]);
                        }
                    }else{
                        if (images[i] != null){
                            tmpNames.editedFileName = images[i].fileName;
                            wallImageList.Add(images[i]);
                        }
                        if (tmpNames != null){
                            route.AddWallImageFileName(tmpNames);
                            tmpNames = null;
                        }
                    }
                }
            }

            stack.WriteRoute(route, wallImageList);
            stack.StoreTargetRoute(route.GetID());

            //gym boardimage にするかどうか
            if (wallImageList.Any()){
                BNGym target = stack.GetTargetGym();
                if (string.IsNullOrEmpty(target.GetBoardImagePath())){
                    target.SetBoardImagePath(wallImageList[0].fileName);
                    stack.ModifyGym(target);
                }
            }
        }
        ReverseTransition();
    }
}
}