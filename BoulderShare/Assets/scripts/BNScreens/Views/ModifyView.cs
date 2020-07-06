using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using System.Threading.Tasks;
using System.Linq;

namespace BoulderNotes{
public class ModifyView: BNScreenWithGyms
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject gymInfo;
    [SerializeField] private GameObject routeInfo;

    [SerializeField] private AdvancedInputField gymNameText;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private IOSUISwitch finishedRouteToggle;
    [SerializeField] private IOSUISwitch kanteToggle;
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private GameObject tapeSelectedObj;
    [SerializeField] private GameObject tapeNoSelectedObj;

    [SerializeField] private TextMeshProUGUI deleteText;
    [SerializeField] private SelecteWallImages_RegisterView wallImages;
    [SerializeField] private GameObject wallImagesObj;

    private BNGym gym;

    private BNRoute route;
    private BNScreenStackWithTargetGym stack;
    private BNGradeMap.Grade grade;
    private WallTypeMap.Type wallType;
    private Sprite sprite;
    private RTape tape;

    private enum ViewType{Gym, Route};
    private ViewType type ;

    [SerializeField] private Color textColor_NoSelected;
    [SerializeField] private Color textColor_Selected;

    public void ClearFields(){
        gym = null;
        route = null;

        titleText.text = "";
        gymNameText.Text = "";
        wallTypeText.text = "";
        gradeText.text = "";     
        kanteToggle.Init(false);
        finishedRouteToggle.Init(false);
        grade = BNGradeMap.Grade.None;
        wallType = WallTypeMap.Type.Slab;
        sprite = null;
        tape = null;
        //routeTape.Init();   
        wallImages.Init();
    }

    public override void InitForFirstTransition(){
        ClearFields();
        stack = GetScreenStackWithGyms();
        if (stack != null){
            stack.ClearInputs();
            gym = stack.GetTargetGym();
            if (gym == null){
                //何もしない
                return ;
            }

            route = stack.GetTargetRoute();
            if (route == null){
                //gym編集
                gymNameText.Text = gym.GetGymName();
                type = ViewType.Gym;
                titleText.text = "ジム編集";
                deleteText.text = "ジム削除";
                ShowTargetObj();
                return ;
            }

            //route編集
            //grade = route.GetGrade();
            //wallType = route.GetWallType();
            grade = route.GetGrade();
            if (grade == BNGradeMap.Grade.None){
                //未選択
                gradeText.color = textColor_NoSelected;
            }else{
                //選択
                gradeText.color = textColor_Selected;
            }
            gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
            wallType = route.GetWallType();
            if (wallType == WallTypeMap.Type.Slab){
                //未選択(未実装)本当はtextColor_NoSelectedを代入
                wallTypeText.color = textColor_Selected;
            }else{
                //選択
                wallTypeText.color = textColor_Selected;
            }
            wallTypeText.text = WallTypeMap.Entity.GetWallTypeName(wallType);
            finishedRouteToggle.SetIsOn(route.IsFinished());
            kanteToggle.SetIsOn(route.IsUsedKante());
            tape = route.GetTape();
            if (tape == null){
                routeTape.LoadDefault();             
            }else{
                routeTape.LoadTape(tape);
            }

            //画像の読み込み
            wallImages.LoadImages(stack, route.GetWallImageFileNames());

            type = ViewType.Route;
            titleText.text = "課題編集";
            deleteText.text = "課題削除";
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
            BNManager.Instance.ActivateNecessary(gymInfo, true);
            BNManager.Instance.ActivateNecessary(routeInfo, false);    
            BNManager.Instance.ActivateNecessary(wallImagesObj, false);        
        }else if(type == ViewType.Route){
            BNManager.Instance.ActivateNecessary(gymInfo, false);
            BNManager.Instance.ActivateNecessary(routeInfo, true); 
            BNManager.Instance.ActivateNecessary(wallImagesObj, true);            
        }
        if (tape != null && !tape.isDefault){
            BNManager.Instance.ActivateNecessary(tapeSelectedObj, true);
            BNManager.Instance.ActivateNecessary(tapeNoSelectedObj, false); 
        }else{
            BNManager.Instance.ActivateNecessary(tapeSelectedObj, false);
            BNManager.Instance.ActivateNecessary(tapeNoSelectedObj, true);             
        }               
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

        ShowTargetObj();
        stack.ClearInputs();
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void Modify(){
        if (stack == null){
            return ;
        }
        if (type == ViewType.Gym){
            gym.SetGymName(gymNameText.Text);
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
            route.SetTape(routeTape.GetTape());
/*
            List<BNWallImage> wallImages = new List<BNWallImage>();
            List<BNWallImageNames> files = new List<BNWallImageNames>();
            if (sprite != null){
                route.SetWallImageFileNames(files);
                wallImage = new BNWallImage(sprite.texture);
                BNWallImageNames names = new BNWallImageNames();
                names.fileName = wallImage.fileName;
                route.AddWallImageFileName(names);
            }
            stack.ModifyRoute(route, wallImage);*/


            List<BNWallImage> wallImageList = new List<BNWallImage>();
            BNWallImage[] images = wallImages.GetImages();
            List<BNWallImageNames> files = new List<BNWallImageNames>();
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
                            files.Add(tmpNames);
                            tmpNames = null;
                        }
                    }
                }
            }

            List<string> oldNames = route.GetAllWallImageFileNames();
            route.SetWallImageFileNames(files);
            List<string> newNames = route.GetAllWallImageFileNames();
            List<string> sub = oldNames.Except(newNames).ToList();
            stack.ModifyRoute(route, wallImageList, sub);

            if (wallImageList.Any()){
                //gymboardの設定
                BNGym g = stack.GetTargetGym();
                if (string.IsNullOrEmpty(g.GetBoardImagePath())){
                    g.SetBoardImagePath(wallImageList[0].fileName);
                    stack.ModifyGym(g);  
                }
            }
            stack.ClearRecord();
            stack.StoreTargetRoute(route.GetID());
        }

        ReverseTransition();
    }

    public void Delete(){
        if (stack == null){
            return ;
        }
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
}
}