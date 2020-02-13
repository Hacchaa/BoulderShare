using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes {
public class GymRouteRowCellView : MonoBehaviour
{
    public BNRoute route;
    public Image mark;
    public TextMeshProUGUI period;
    public TextMeshProUGUI grade;
    public GameObject container;
    public GameObject finishedObj;

    public OnButtonClickedDelegateWithBNRoute onRouteClicked;
    public void SetData(GymRouteScrollerData data, OnButtonClickedDelegateWithBNRoute routeDel){
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
                route = data.route;   
                onRouteClicked = routeDel;

                finishedObj.SetActive(data.isFinished);       
            }
        }
    }

    public void OnRouteClicked(){
        if (onRouteClicked != null){
            onRouteClicked(route);
        }
    }
}
}