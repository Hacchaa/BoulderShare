using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SEComponentBase : MonoBehaviour, ISEComponent, IUIComponent
{
	[SerializeField] private Image topSERef;
	[SerializeField] private Image botSERef;
	[SerializeField] private List<RectTransform> needMarginList;

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

    public abstract void Hide();

    public abstract void ShowProc();

    public abstract void HideProc();
}
