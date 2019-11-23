using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SEComponentBase : MonoBehaviour, ISEComponent, IUIComponent
{
    [SerializeField] private bool hasBot;
	[SerializeField] private List<RectTransform> needMarginList;
	[SerializeField] private List<GameObject> InternalShowObjects;
	[SerializeField] private List<GameObject> InternalHideObjects;

    public bool HasBot(){
        return hasBot;
    }

    public List<RectTransform> GetMarginList(){
    	return needMarginList;
    }
/*
    public void Hide(bool isPreProcExecution = true){
    	if (isPreProcExecution){
    		OnPreHide();    		
    	}
    	InitializeArrangement();
    	gameObject.SetActive(false);
    }*/

    public void HideScreen(){
        InitializeArrangement();
        gameObject.SetActive(false);       
    }

    public void ShowScreen(){
    	OnPreShow();
    	InitializeArrangement();
    	gameObject.SetActive(true);
    }

    private void InitializeArrangement(){
     	foreach(GameObject obj in InternalShowObjects){
    		obj.SetActive(true);
    	}
    	foreach(GameObject obj in InternalHideObjects){
    		obj.SetActive(false);
    	}
    }

    public abstract void OnPreShow();
    public abstract void OnPreHide();
    public void OnPreTransition(){

    }
    public void OnPostTransition(bool isShowed){

    }
}
