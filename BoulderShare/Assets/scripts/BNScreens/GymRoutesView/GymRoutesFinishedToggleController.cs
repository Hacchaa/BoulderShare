using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoulderNotes{
public class GymRoutesFinishedToggleController : MonoBehaviour
{
    [SerializeField] private GymRoutesFinishedToggleItem[] items;
    [SerializeField] private GymRoutesView view;
    private GymRoutesFinishedToggleItem currentItem;

    public void Init(){
        foreach(GymRoutesFinishedToggleItem item in items){
            item.Init(this);
            item.FocusOff();
        }
        currentItem = items[0];
        currentItem.FocusOn();
    }

    public bool IsFinished(){
        if (currentItem == null){
            return false;
        }
        return currentItem.IsFinished();
    }

    public void Register(GymRoutesFinishedToggleItem item){
        if (currentItem != null && currentItem.IsFinished() == item.IsFinished()){
            return ;
        }
        if(currentItem != null){
            currentItem.FocusOff();
        }
        currentItem = item;   
        currentItem.FocusOn();
        view.ChangeFinished(currentItem.IsFinished());
    }
}
}