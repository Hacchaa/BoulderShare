using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;

namespace BoulderNotes {
public class GymRouteRowCellView : MonoBehaviour
{
    public string routeID;
    public RouteTape tape;
    public TextMeshProUGUI period;
    public TextMeshProUGUI grade;
    public GameObject container;
    public GameObject finishedObj;
    [SerializeField] private AssetReference defaultSprite;
    public OnButtonClickedDelegateWithString onRouteClicked;
    public void SetData(GymRouteScrollerData data, OnButtonClickedDelegateWithString routeDel){
        if (data == null){
            container.SetActive(false);
        }else{
            if (data.isAddButton){
                period.text = "";
                grade.text = "";
                container.SetActive(false);
            }else{
                container.SetActive(true);
                period.text = data.period;
                //とりあえず
                grade.text = BNGradeMap.Entity.GetGradeName(data.grade);  
                routeID = data.routeID;   
                onRouteClicked = routeDel;
                if (data.routeTape != null && !string.IsNullOrEmpty(data.routeTape.spriteName)){
                    tape.LoadTape(data.routeTape);
                }else{
                    Addressables.LoadAssetsAsync<Sprite>(defaultSprite, OnLoad);
                }

                finishedObj.SetActive(data.isFinished);       
            }
        }
    }

    private void OnLoad(Sprite sprite){
        tape.ChangeShape(sprite);
        tape.ChangeText("");
    }

    public void OnRouteClicked(){
        if (onRouteClicked != null){
            onRouteClicked(routeID);
        }
    }
}
}