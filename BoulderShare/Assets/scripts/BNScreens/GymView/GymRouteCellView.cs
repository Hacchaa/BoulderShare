using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Linq;
using System;

namespace BoulderNotes{
public class GymRouteCellView : EnhancedScrollerCellView
{
    public string routeID;
    public RouteTape tape;
    public TextMeshProUGUI period;
    public TextMeshProUGUI grade;
    [SerializeField] private Image wallImage;
    [SerializeField] private TextMeshProUGUI tagText;
    [SerializeField] private TextMeshProUGUI clearRateText;

    [SerializeField] private Sprite defaultSprite;
    public OnButtonClickedDelegateWithString onClicked;
    private bool completedInit = false;
    [SerializeField] private Image maskImage;
    public void SetData(GymRouteScrollerData data, BNScreenStackWithTargetGym stack, OnButtonClickedDelegateWithString routeDel){
        onClicked = routeDel;
        period.text = data.period;
        //とりあえず
        grade.text = BNGradeMap.Entity.GetGradeName(data.grade);  
        routeID = data.routeID;   
        if (data.routeTape != null && !string.IsNullOrEmpty(data.routeTape.spriteName)){
            tape.gameObject.SetActive(true);
            tape.LoadTape(data.routeTape);
        }else{
            tape.gameObject.SetActive(false);
        }     
        tagText.text = string.Join(Environment.NewLine, data.tags);
        clearRateText.text = data.clearRate + "%";
        if (data.wallImages.Any()){
            stack.LoadImageAsync(data.wallImages[0], FitImage);
        }else{
            FitImage(defaultSprite);
        }

        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanelFill(OnLoadMask);
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
        
        float w, h;
        //texのwidthかheightがfitTargetのwidhtかheightより大きいか小さいか
        if (fitWidth > texWidth || fitHeight > texHeight){
            //小さい場合、textureのwidthかheightどちらかを拡大する
            //差の大きいほうをfitTargetに合わせて拡大
            if (difW < difH){
                h = fitHeight;
                w = texWidth * (fitHeight / texHeight); 
            }else{
                w = fitWidth;
                h = texHeight * (fitWidth / texWidth);
            }
        }else{
            //大きい場合、textureのwidthかheightどちらかを縮小する
            //差の小さいほうをfitTargetに合わせて縮小
            if (difW < difH){
                w = fitWidth;
                h = texHeight * (fitWidth / texWidth);
            }else{
                h = fitHeight;
                w = texWidth * (fitHeight / texHeight);                 
            }
        }   
        //Debug.Log("w="+w+" h="+h);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(w, h);
        wallImage.sprite = spr;
    }
    private void OnLoad(Sprite sprite){
        tape.ChangeShape(sprite);
        tape.ChangeText("");
    }

    public void OnClicked(){
        if (onClicked != null){
            onClicked(routeID);
        }
    }
}
public class GymRouteLineCellView : EnhancedScrollerCellView{

}
}