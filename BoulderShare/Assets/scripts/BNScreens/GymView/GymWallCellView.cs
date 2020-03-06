using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{

public class GymWallCellView : EnhancedScrollerCellView
{   
    public string wallID;
    public Image wallImage;
    private float fitWidth;
    private float fitHeight;
    [SerializeField] private RectTransform fitTarget;
    [SerializeField] private Sprite defaultSprite;

    public TextMeshProUGUI period;
    public OnButtonClickedDelegateWithString clickDel;

    private RectTransform rect;
    public void SetData(GymWallScrollerData data){
        rect = wallImage.GetComponent<RectTransform>();
        wallID = data.wallID;
        fitWidth = data.fitWidth;
        fitHeight = data.fitHeight;
        
        period.text = data.period;
        List<string> list = data.fileNames;

        if (list != null && list.Any()){
            data.stack.LoadImageAsync(list[0], OnLoadImage);
        }else{
            OnLoadImage(defaultSprite);
        }
    }
    public void OnClicked(){
        if (clickDel != null){
            clickDel(wallID);
        }
    }

    private void OnLoadImage(Sprite spr){
        wallImage.sprite = spr;

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
    }

}

}