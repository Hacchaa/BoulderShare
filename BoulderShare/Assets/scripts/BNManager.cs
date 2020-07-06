using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;

namespace BoulderNotes{
public class BNManager : SingletonMonoBehaviour<BNManager>
{
    [SerializeField] private AssetReference[] cornerPanelFill;
    [SerializeField] private AssetReference[] cornerPanelStroke;
    [SerializeField] private Sprite[] cornerPanelFillSprites;
    [SerializeField] private Sprite[] cornerPanelStrokeSprites;
    [SerializeField] private Sprite[] conditionSprites;
    [SerializeField] private AssetReference[] conditionRef;
    protected override void Awake(){
        base.Awake();
        //cornerPanelFillSprites = new Sprite[cornerPanelFill.Length];
        //cornerPanelStrokeSprites = new Sprite[cornerPanelStroke.Length];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; 

        StartCoroutine(Setup());

        //Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[0],OnLoadCornerPanel);
    }

    IEnumerator Setup(){
        CanvasResolutionManager.Instance.Init();
        BNGymDataCenter.Instance.Init();
        yield return null;
        BNScreens.Instance.Init();  
    }

/*
    private void OnLoadCornerPanel(Sprite spr){
        cornerPanelFillSprites[index] = spr;
        index++;
        if(index < cornerPanelFill.Length){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[index],OnLoadCornerPanel);
        }
    }*/

    public void GetCornerPanelFill(Action<Sprite> loadAction){
        int retina = (int)CanvasResolutionManager.Instance.GetRatioOfPtToPx() - 1;
        if (retina < 0 || retina > cornerPanelFill.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }
            return ;
        }
        if (cornerPanelFillSprites[retina] == null){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[retina], loadAction);
        }else{
            loadAction(cornerPanelFillSprites[retina]);
        }
    }
    public void GetCornerPanelStroke(Action<Sprite> loadAction){
        int retina = (int)CanvasResolutionManager.Instance.GetRatioOfPtToPx() - 1;
        if (retina < 0 || retina > cornerPanelStroke.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }
            return ;
        }
        if (cornerPanelStrokeSprites[retina] == null){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelStroke[retina], loadAction);
        }else{
            loadAction(cornerPanelStrokeSprites[retina]);
        }
    }

    public void GetConditionSprite(int index, Action<Sprite> loadAction){
        GetConditionSprite((BNRecord.Condition)index, loadAction);
    }

    public void GetConditionSprite(BNRecord.Condition condition, Action<Sprite> loadAction){
        int index = (int)condition;
        if (index < 0 || index > conditionSprites.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }

        }
        if (conditionSprites[index] == null){
            Addressables.LoadAssetsAsync<Sprite>(conditionRef[index], loadAction);
        }else{
            loadAction(conditionSprites[index]);
        }
    }  
    public void ActivateNecessary(GameObject obj, bool b){
        if (obj.activeSelf != b){
            obj.SetActive(b);
        }
    }     
    public void FillImageToParent(Image wallImage, RectTransform fillRect, RectTransform parent, Sprite spr){
        if (parent == null){
            parent = fillRect.transform.parent.GetComponent<RectTransform>();
        }

        float fitHeight = parent.rect.height;
        float fitWidth = parent.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h, r;
        if (fitHeight / fitWidth > texHeight / texWidth){
            r = fitHeight / texHeight; 
            h = fitHeight;
            w = texWidth * r; 
        }else{
            r = fitWidth / texWidth; 
            w = fitWidth;
            h = texHeight * r;   
        }  

        fillRect.anchorMin = new Vector2(0.5f, 0.5f);
        fillRect.anchorMax = new Vector2(0.5f, 0.5f);
        fillRect.sizeDelta = new Vector2(w, h);

        wallImage.sprite = spr;
    }
	public void FitImageToParent(Image image, RectTransform fitRect, RectTransform parent, Sprite spr){
        if (parent == null){
            parent = fitRect.transform.parent.GetComponent<RectTransform>();
        }
        float fitHeight = parent.rect.height;
        float fitWidth = parent.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h, r;

        if (fitHeight / fitWidth < texHeight / texWidth){
            r = fitHeight / texHeight; 
            h = fitHeight;
            w = texWidth * r; 
        }else{
            r = fitWidth / texWidth; 
            w = fitWidth;
            h = texHeight * r;   
        }  

        fitRect.anchorMin = new Vector2(0.5f, 0.5f);
        fitRect.anchorMax = new Vector2(0.5f, 0.5f);
        fitRect.sizeDelta = new Vector2(w, h);
		fitRect.anchoredPosition = Vector2.zero;

        image.sprite = spr;
    }

    public Sprite CreateSprite(Texture2D tex){
        return Sprite.Create(
            tex, 
            new Rect(0.0f, 0.0f, tex.width, tex.height), 
            new Vector2(0.5f, 0.5f),
            tex.height/4);
    }
}
}