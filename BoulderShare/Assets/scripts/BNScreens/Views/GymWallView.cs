using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq; 
namespace BoulderNotes {
public class GymWallView : BNScreen
{
    [SerializeField] private GymRouteScrollerController scroller;
    [SerializeField] private TextMeshProUGUI gymWallText;
    [SerializeField] private Image wallImage;
    [SerializeField] private Sprite defaultWallImage;
    public override void InitForFirstTransition(){
        scroller.Init();
    }

    public override void UpdateScreen(){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            BNScreenStackWithTargetGym stack = belongingStack as BNScreenStackWithTargetGym;
            //gymIDとwallIDだけ記憶
            stack.ClearRoute();
            BNGym gym = stack.GetTargetGym();
            BNWall wall = stack.GetTargetWall();
            wallImage.sprite = defaultWallImage;

            string name = "";
            if (wall != null){
                scroller.FetchData(wall.GetRoutes());
                name = WallTypeMap.Entity.GetWallTypeName(wall.GetWallType());
                List<string> list = wall.GetWallImageFileNames();

                if (list != null && list.Any()){
                    wallImage.sprite = stack.LoadWallImage(list[0]);
                }
            }

            gymWallText.text = name;
        }
    }

    private void OnLoadImage(Sprite spr){
        wallImage.sprite = spr;
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void SaveTargetRouteInStack(BNRoute route){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetRoute(route.GetID());
        }
    }
    public void ToRouteView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RouteView, BNScreens.TransitionType.Push);
    }
    public void ToRegisterView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterView, BNScreens.TransitionType.Push);
    }
    public void ToModifyView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ModifyView, BNScreens.TransitionType.Push);
    }

}
}