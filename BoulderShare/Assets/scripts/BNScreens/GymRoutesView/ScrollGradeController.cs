using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
namespace BoulderNotes{
public class ScrollGradeController : MonoBehaviour
{
    [SerializeField] private ScrollGradeItem[] items;
    [SerializeField] private ScrollGradeItem itemPrefab;
    [SerializeField] private GymRoutesView view;
    [SerializeField] private ScrollRect scrollRect;
    private ScrollGradeItem currentItem;

    public BNGradeMap.Grade GetCurrentGrade(){
        if (currentItem == null){
            return BNGradeMap.Grade.None;
        }

        return currentItem.GetGrade();
    }
    public void Init(){
        foreach(Transform t in transform){
            Destroy(t.gameObject);
        }
        int n = BNGradeMap.Entity.GetSize();
        items = new ScrollGradeItem[n];
        for(int i = 0 ; i < n ; i++){
            ScrollGradeItem item = Instantiate<ScrollGradeItem>(itemPrefab, transform);
            item.Init(this);
            item.FocusOff();
            item.SetGrade((BNGradeMap.Grade)i);

            items[i] = item;
        }
        currentItem = items[0];
        currentItem.FocusOn();
        scrollRect.horizontalNormalizedPosition = 0.0f;
    }

    public void SetRouteNum(int[] arr){
        int total = 0;
        for(int i = 0 ; i < items.Length ; i++){
            if (i != 0){
                items[i].SetNum(arr[i]);
            }
            total += arr[i];
        }
        items[0].SetNum(total);
    }

    public void Register(ScrollGradeItem item){
        if(currentItem != null){
            currentItem.FocusOff();
        }
        currentItem = item;   
        currentItem.FocusOn();
        view.LookUpRoutes(currentItem.GetGrade());

    }
}
}