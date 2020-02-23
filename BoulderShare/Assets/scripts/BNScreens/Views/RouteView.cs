using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace BoulderNotes{
public class RouteView : BNScreen
{
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI periodText; 
    [SerializeField] private TextMeshProUGUI kanteText;
    [SerializeField] private GameObject finishedObj;

    [SerializeField] private RecordScrollerController scroller;
    

    [SerializeField] private BNRoute route;

    [SerializeField] private Image favoriteImage;
    [SerializeField] private Sprite favoriteOn;
    [SerializeField] private Sprite favoriteOff;

    [SerializeField] private ClassificationView classView;

    private DateTime pushedTime;
    private bool isFavorite;

    public override void InitForFirstTransition(){
        scroller.Init();
        classView.Init();
    }

    public void ClearFields(){
        gradeText.text ="";
        periodText.text = "";
        kanteText.text = "";
        route = null;
    }

    public override void UpdateScreen(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            //recordIDを削除
            (belongingStack as BNScreenStackWithTargetGym).ClearRecord();
            route = (belongingStack as BNScreenStackWithTargetGym).GetTargetRoute();
           
            if (route != null){
                gradeText.text = route.GetGrade() + "級";
                periodText.text = route.GetPeriod();

                finishedObj.SetActive(route.IsFinished());
                if (route.IsFavorite()){
                    favoriteImage.sprite = favoriteOn;
                }else{
                    favoriteImage.sprite = favoriteOff;
                }
                isFavorite = route.IsFavorite();

                if (route.IsUsedKante()){
                    kanteText.text = "カンテあり";
                }else{
                    kanteText.text = "カンテなし";
                }

                scroller.FetchData(route.GetRecords());
            }
        }
    }

    public void SaveTargerRecordInStack(BNRecord rec){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetRecord(rec.GetID());
        }
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void ToRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RecordView, BNScreens.TransitionType.Push);
    }

    public void ToModifyView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ModifyView, BNScreens.TransitionType.Push);
    }

    public void ToRegisterRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterRecordView, BNScreens.TransitionType.Push);
    }
    public void OnFavoriteClicked(){
        if (pushedTime == null){
            pushedTime = DateTime.Now;
            return ;
        }

        TimeSpan ts = DateTime.Now - pushedTime;

        if (ts.TotalSeconds >= 1.5){
            SwitchFavoriteImage();
            pushedTime = DateTime.Now;
        }
    }

    private void SwitchFavoriteImage(){
        if (isFavorite){
            favoriteImage.sprite = favoriteOff;
        }else{
            favoriteImage.sprite = favoriteOn;
        }

        isFavorite = !isFavorite;

        //routeを保存
        if (route != null){
            route.SetIsFavorite(isFavorite);
            if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
                (belongingStack as BNScreenStackWithTargetGym).ModifyRoute(route);
            }
        }
    }
}
}