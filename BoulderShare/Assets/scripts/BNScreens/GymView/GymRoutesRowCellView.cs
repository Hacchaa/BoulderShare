using System.Collections;
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
    public void SetData(GymRoutesScrollerData data, BNScreenStackWithTargetGym stack, OnButtonClickedDelegateWithString onButtonClicked){
        del = onButtonClicked;
        if(data.routeTape != null){
            tape.LoadTape(data.routeTape);
        }else{
            tape.LoadDefault();
        }
        infoText.text = data.gradeName + "・" + data.wallTypeName;
        dateText.text = data.period;
        routeID = data.routeID;
        if (!string.IsNullOrEmpty(data.wallImagePath)){
            stack.LoadImageAsync(data.wallImagePath, FitImage);
        }else{
            FitImage(defaultSprite);
        }
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

    public void OnClick(){
        if (del != null){
            del(routeID);
        }
    }
}
}