using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class RecordRouteWallImageCellView : EnhancedScrollerCellView
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite defaultSprite; 
    [SerializeField] private Image topIcon;
    [SerializeField] private Image switchImageIcon;
    
    private BNWallImageNames imageNames;
    private OnButtonClickedDelegateWithImageNames onButtonClicked;

    public void SetData(RecordRouteWallImageScrollerData data, BNScreenStackWithTargetGym stack, OnButtonClickedDelegateWithImageNames clicked){
        imageNames = data.names;

        image.sprite = defaultSprite;

        if (imageNames != null){
            if(!string.IsNullOrEmpty(imageNames.editedFileName)){
                stack.LoadImageAsync(imageNames.editedFileName, FitImage);
                switchImageIcon.gameObject.SetActive(true);
            }else if(!string.IsNullOrEmpty(imageNames.fileName)){
                stack.LoadImageAsync(imageNames.fileName, FitImage);
                switchImageIcon.gameObject.SetActive(false);
            }       
        }else{
            switchImageIcon.gameObject.SetActive(false);
        }

        topIcon.gameObject.SetActive(data.isTopImage);
    }

    public void OnButtonClicked(){
        if (onButtonClicked != null){
            onButtonClicked(imageNames);
        }
    }
    private void FitImage(Sprite spr){
        RectTransform rect = image.GetComponent<RectTransform>();
        RectTransform parent = image.transform.parent.GetComponent<RectTransform>();
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

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(w, h);

        image.sprite = spr;
    }
}
}