using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SEComponentBase : MonoBehaviour, ISEComponent, IUIComponent
{
    [SerializeField] private bool hasBot;
    [SerializeField] private RectTransform head;
    [SerializeField] private RectTransform foot;
	[SerializeField] private List<RectTransform> needMarginList;
	[SerializeField] private List<GameObject> InternalShowObjects;
	[SerializeField] private List<GameObject> InternalHideObjects;

    public bool HasBot(){
        return hasBot;
    }
    public float GetHeadHeight(){
        if (head == null){
            return 0.0f;
        }
        return head.rect.height;
    }

    public float GetFootHeight(){
        if (foot == null){
            return 0.0f;
        }
        return foot.rect.height;
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

    public void HideUI(){
        InitializeArrangement();
        gameObject.SetActive(false);       
    }

    public void Show(){
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

    public abstract void OnPreHide();

    public abstract void OnPreShow();
}
