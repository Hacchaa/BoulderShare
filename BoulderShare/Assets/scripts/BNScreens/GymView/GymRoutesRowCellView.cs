﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class GymRoutesRowCellView : EnhancedScrollerCellView
{
    [SerializeField] private GymRoutesCellView cellView;
    [SerializeField] private RouteTape tape;
    [SerializeField] private Image wallImage;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Sprite defaultSprite;

    private string routeID;
    private OnButtonClickedDelegateWithString del;
    private bool completedInit = false;
    [SerializeField] private Image maskImage;

    public void SetData(GymRoutesScrollerData data, BNScreenStackWithTargetGym stack, OnButtonClickedDelegateWithString onButtonClicked){
        del = onButtonClicked;
        if(data.routeTape != null){
            tape.LoadTape(data.routeTape);
        }else{
            tape.LoadDefault();
        }
        infoText.text = BNGradeMap.Entity.GetGradeName(data.grade) + "・" + BNRoute.GetClearStatusName(data.clearStatus);
        if (data.clearStatus == BNRoute.ClearStatus.NoAchievement){
            infoText.text += "("+data.clearRate+"%)";
        }

        dateText.text = data.period;
        routeID = data.routeID;
        if (!string.IsNullOrEmpty(data.wallImagePath)){
            stack.LoadImageAsync(data.wallImagePath, FitImage);
        }else{
            FitImage(defaultSprite);
        }
        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanel(OnLoadMask);
        }
    }

    private void OnLoadMask(Sprite sprite){
        maskImage.sprite = sprite;
    }

    private void FitImage(Sprite spr){
        RectTransform rect = wallImage.GetComponent<RectTransform>();
        RectTransform parent = wallImage.transform.parent.GetComponent<RectTransform>();
        float fitHeight = parent.rect.height;
        float fitWidth = parent.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h, r;/*
        //texのwidthかheightがfitTargetのwidhtかheightより大きいか小さいか
        if (fitWidth > texWidth || fitHeight > texHeight){
            //小さい場合、textureのwidthかheightどちらかを拡大する
            //差の大きいほうをfitTargetに合わせて拡大
            if (difW < difH){
                r = fitHeight / texHeight; 
                h = fitHeight;
                w = texWidth * r; 
            }else{
                r = fitWidth / texWidth; 
                w = fitWidth;
                h = texHeight * r;
            }
        }else{
            //大きい場合、textureのwidthかheightどちらかを縮小する
            //差の小さいほうをfitTargetに合わせて縮小
            if (difW < difH){
                r = fitWidth / texWidth; 
                w = fitWidth;
                h = texHeight * r;
            }else{
                r = fitHeight / texHeight; 
                h = fitHeight;
                w = texWidth * r;                  
            }
        } */
        if (fitHeight / fitWidth > texHeight / texWidth){
            r = fitHeight / texHeight; 
            h = fitHeight;
            w = texWidth * r; 
        }else{
            r = fitWidth / texWidth; 
            w = fitWidth;
            h = texHeight * r;   
        }  
        //Debug.Log("texW:"+texWidth+ " texH:"+texHeight);
        //Debug.Log("fitW:"+fitWidth+ " fitH:"+fitHeight);
        //Debug.Log("_width:"+fitWidth/r+ " _height:"+fitHeight/r);
        //Debug.Log("w="+w+" h="+h+" r="+r);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(w, h);

        wallImage.sprite = spr;
        /*
        wallImage.material.SetFloat("_TexWidth", w);
        wallImage.material.SetFloat("_TexHeight", h);
        wallImage.material.SetFloat("_Width", fitWidth);
        wallImage.material.SetFloat("_Height", fitHeight);  
        //runtimeMaterial.SetFloat("_Radius", runtimeMaterial.GetFloat("_Radius") / r);      */
    }

    public void OnClick(){
        if (del != null){
            del(routeID);
        }
    }
}
}