using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
namespace BoulderNotes{
public class ScrollGradeController : MonoBehaviour
{
    private ScrollGradeItem[] items;
    [SerializeField] private ScrollGradeItem itemPrefab;
    [SerializeField] private GymRoutesView view;
    [SerializeField] private ScrollRect scrollRect;
    private ScrollGradeItem currentItem;
    [SerializeField] private RectTransform movedContent;
    [SerializeField] private Transform contentRoot;

    public BNGradeMap.Grade GetCurrentGrade(){
        if (currentItem == null){
            return BNGradeMap.Grade.None;
        }

        return currentItem.GetGrade();
    }
    public void Init(){
        foreach(Transform t in contentRoot){
            Destroy(t.gameObject);
        }
        int n = BNGradeMap.Entity.GetSize();
        items = new ScrollGradeItem[n];
        for(int i = 0 ; i < n ; i++){
            ScrollGradeItem item = Instantiate<ScrollGradeItem>(itemPrefab, contentRoot);
            item.Init(this);
            item.FocusOff();
            item.SetGrade((BNGradeMap.Grade)i);

            items[i] = item;
        }
        currentItem = items[0];
        currentItem.FocusOn();
        scrollRect.horizontalNormalizedPosition = 0.0f;
        RectTransform rect = GetComponent<RectTransform>();
        movedContent.anchorMin = Vector2.one * 0.5f;
        movedContent.anchorMax = Vector2.one * 0.5f;
        movedContent.sizeDelta = new Vector2(rect.rect.width, rect.rect.height);
        movedContent.anchoredPosition = Vector2.zero;
    }

    public void MoveContentByGymRoutesScroller(Vector2 p){
        movedContent.anchoredPosition = p;
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