using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using AdvancedInputFieldPlugin;
namespace BoulderNotes{
public class EditRouteTapeView: BNScreenWithGyms
{
    [SerializeField] private AssetLabelReference label;
    [SerializeField] private Transform routeTapeRoot;
    [SerializeField] private RouteTapeShape shapePrefab;
    [SerializeField] private RouteTape routeTape;
    [SerializeField] private ClassificationView classView;
    [SerializeField] private InputItemsView.TargetItem currentTargetItem;
    private RouteTapeShape[] shapes;
    [SerializeField] private RouteTapeShapeDefault shapeDefaultPrefab;
    [SerializeField] private Color firstColor;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private AdvancedInputField tapeTextIF;
    private bool selectedDefault;
    private BNScreenStackWithTargetGym stack;

    public override void InitForFirstTransition(){
        classView.Init();
        selectedDefault = true;
        if (shapes == null){
            foreach(Transform child in routeTapeRoot){
                Destroy(child.gameObject);
            }
            shapes = new RouteTapeShape[RouteTapePriority.Entity.GetTapesNumber()];

            for(int i = 0 ; i < shapes.Length ; i++){
                shapes[i] = Instantiate(shapePrefab, routeTapeRoot);
            }         

            RouteTapeShapeDefault def = Instantiate<RouteTapeShapeDefault>(shapeDefaultPrefab, routeTapeRoot);
            def.Init(this);

            //sprite読み込み
            Addressables.LoadAssetsAsync<Sprite>(label, OnLoadHoldMarkSprite);
        }

        //初期値取得
        stack = GetScreenStackWithGyms();
        currentTargetItem = stack.GetTargetItemToInput();
        if (currentTargetItem == InputItemsView.TargetItem.Tape){
            RTape t = stack.GetTargetTape();
            if (t == null){
                routeTape.LoadDefault();
                tapeTextIF.Text = "";
            }else{
                routeTape.LoadTape(t);
                if (!t.isDefault){
                    selectedDefault = false;
                }
                tapeTextIF.Text = t.tapeText;
            }
        }
    }
    public override void UpdateScreen(){

    }
    private void OnLoadHoldMarkSprite(Sprite sprite){
        //Debug.Log("onload :"+sprite.name);
        //ホールドマークを参照させる
        int priority = RouteTapePriority.Entity.GetPriority(sprite.name);
        if (priority == -1){
            return ;
        }
        shapes[priority].Init(sprite, this);
    }

    public void ChangeAsDefault(){
        routeTape.LoadDefault();
        tapeTextIF.Text = "";
        selectedDefault = true;
    }

    public void ChangeShape(Sprite sprite){
        routeTape.ChangeShape(sprite);
        if (selectedDefault){
            selectedDefault = false;
            ChangeColor(firstColor);
        }
        selectedDefault = false;
    }
    public void ChangeColor(Color c){
        if (!selectedDefault){
            routeTape.ChangeColor(c);
        }
    }
    public void AddRot(){
        if (!selectedDefault){
            routeTape.AddRot(45f);
        }
    }

    public void SubRot(){
        if (!selectedDefault){
            routeTape.AddRot(-45f);
        }
    }
    
    public void ChangeTapeText(string txt){
        if (!selectedDefault){
            routeTape.ChangeText(txt);
        }else{
            tapeTextIF.Text = "";
        }
    }

    public void Register(){
        if (currentTargetItem == InputItemsView.TargetItem.Tape){
            stack.SetTargetTape(routeTape.GetTape());
        }
        ReverseTransition();
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

}
}