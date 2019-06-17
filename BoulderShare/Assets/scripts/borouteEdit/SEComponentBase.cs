using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SEComponentBase : MonoBehaviour, ISEComponent, IUIComponent
{
    [SerializeField] private bool needBot = true;
	[SerializeField] private Image topSERef;
	[SerializeField] private Image botSERef;
	[SerializeField] private List<RectTransform> needMarginList;
	[SerializeField] private List<GameObject> InternalShowObjects;
	[SerializeField] private List<GameObject> InternalHideObjects;
	[SerializeField] private List<GameObject> ExternalShowObjects;
	[SerializeField] private List<GameObject> ExternalHideObjects;

    public bool IsBotNeed(){
        return needBot;
    }
    public Color GetTopSEColor(){
        if (topSERef == null){
            return new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    	return topSERef.color;
    }
    public Color GetBotSEColor(){
        if (botSERef == null){
            return new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    	return botSERef.color;
    }

    public List<RectTransform> GetMarginList(){
    	return needMarginList;
    }

    public void Hide(bool isPreProcExecution = true){
    	if (isPreProcExecution){
    		OnPreHide();    		
    	}
    	InitializeArrangement();
    	foreach(GameObject obj in ExternalHideObjects){
    		obj.SetActive(true);
    	}
    	gameObject.SetActive(false);
    }

    public void Show(){
    	OnPreShow();
    	InitializeArrangement();
    	foreach(GameObject obj in ExternalHideObjects){
    		obj.SetActive(false);
    	}
    	gameObject.SetActive(true);
    }

    private void InitializeArrangement(){
     	foreach(GameObject obj in InternalShowObjects){
    		obj.SetActive(true);
    	}
    	foreach(GameObject obj in InternalHideObjects){
    		obj.SetActive(false);
    	}
    	foreach(GameObject obj in ExternalShowObjects){
    		obj.SetActive(true);
    	}
    }

    public abstract void OnPreHide();

    public abstract void OnPreShow();
}
