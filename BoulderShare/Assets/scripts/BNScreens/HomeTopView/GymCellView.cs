using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI text;
    //public TextMeshProUGUI id;
    public Image maskImage;
    public Image boardImage;
    [SerializeField] private Sprite firstBoardSprite;
    private string gymID;
    public OnButtonClickedDelegateWithString clickDel;

    private bool completedInit = false;
    private bool needLateUpdate = false;
    private Sprite boardSprite;
    void LateUpdate(){
        if (needLateUpdate){
            FitImage(boardSprite);
            boardSprite = null;
            needLateUpdate = false;
        }
    }
    public void SetData(GymScrollerData data){
        text.text = data.gymName;
        //id.text = data.gymID;
        gymID = data.gymID;

        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanelFill(OnLoad);
        }

        if (!string.IsNullOrEmpty(data.boardImagePath)){
            BNGymDataCenter.Instance.LoadImageAsync(BNGymDataCenter.Instance.GetWallImagePath(data.gymID)+data.boardImagePath, PrepareUpdate);
        }else{
            PrepareUpdate(firstBoardSprite);
        }
    }

    private void OnLoad(Sprite sprite){
        maskImage.sprite = sprite;
    }

    public void OnClicked(){
        if (clickDel != null){
            clickDel(gymID);
        }
    }

    private void PrepareUpdate(Sprite spr){
        needLateUpdate = true;
        boardSprite = spr;
    }

    private void FitImage(Sprite spr){
        RectTransform rect = boardImage.GetComponent<RectTransform>();
        RectTransform parent = boardImage.transform.parent.GetComponent<RectTransform>();

        //Debug.Log("parent w:"+parent.rect.width+", h:"+parent.rect.height);
        float fitHeight = parent.rect.height;
        float fitWidth = parent.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h;

        if (fitHeight / fitWidth >= texHeight / texWidth){
            h = fitHeight;
            w = texWidth * (fitHeight / texHeight); 
        }else{
            w = fitWidth;
            h = texHeight * (fitWidth / texWidth);
        } 
        //Debug.Log("w="+w+" h="+h);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(w, h);
        boardImage.sprite = spr;
    }
}
}