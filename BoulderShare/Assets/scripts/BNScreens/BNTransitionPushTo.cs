using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class BNTransitionPushTo : BNTransitionBase
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI backTitle;


    private RectTransform titleRect;
    private RectTransform backTitleRect;
    private Vector2 titleBase;
    private Vector2 backTitleBase;
    private Vector2 headBGBase;
    private Vector2 contentBase;
    

    public override void Init(BNScreen screen, RectTransform content, RectTransform head, RectTransform headBG, RectTransform tab){
        base.Init(screen, content, head, headBG, tab);
         if (title != null){
            titleRect = title.GetComponent<RectTransform>();
            titleBase = titleRect.anchoredPosition;
        }
        if (backTitle != null){
            backTitleRect = backTitle.GetComponent<RectTransform>();
            backTitleBase = backTitleRect.anchoredPosition;
        }       
        if (headBG != null){
            headBGBase = headBG.anchoredPosition;
        }
        if (content != null){
            contentBase = content.anchoredPosition;
        }
    }
    override public void TransitionLerp(float t){
		t = Mathf.Lerp(0.0f, 1.0f, t);

        //content
        float width = content.rect.width ;
        content.anchoredPosition = contentBase + new Vector2(width * (1.0f - t), 0.0f);
        if (HasHeadBG()){
            /*
            if (isAnotherWithHeadBG){
                //全体表示
                headBG.anchoredPosition = headBGBase;
            }else{
                //contentと一緒に動く
                headBG.anchoredPosition = headBGBase + new Vector2(width * (1.0f - t), 0.0f);
            }*/
            headBG.anchoredPosition = headBGBase + new Vector2(width * (1.0f - t), 0.0f);
        }
        //title and backTitle
        if (title != null){
            titleRect.anchoredPosition = titleBase + new Vector2(width * (1.0f - t), 0.0f);
        }

        if (backTitle != null){
            backTitleRect.anchoredPosition = backTitleBase + new Vector2(width * (1.0f - t), 0.0f);
        }

        //fade out head components
        if (headCG != null){
            headCG.alpha = t;
        }
        if (headBGCG != null){
            headBGCG.alpha = t;
        }
    }

    override public void Ready(){
        base.Ready();
/*
        if (HasHeadBG()){
            headBG.gameObject.SetActive(true);
        }*/
    }
    override public void Complete(bool isReverse){
        base.Complete(isReverse);
        if (isReverse){
            if (screen != null){
                screen.gameObject.SetActive(false);
            }
        }else{
            SetAllBlocksRaycasts(true);
        }
    }
}
}