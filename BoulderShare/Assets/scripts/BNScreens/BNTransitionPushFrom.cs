using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BNTransitionPushFrom : BNTransitionBase
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI backTitle;

    private RectTransform titleRect;
    private RectTransform backTitleRect;
    private Vector2 titleBase;
    private Vector2 backTitleBase;
    private Vector2 headBGBase;
    private Vector2 tabBase;
    private Vector2 contentBase;
    private bool isNeedHeadBG ;


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
        if (tab != null){
            tabBase = tab.anchoredPosition;
        }
    }
    override public void TransitionLerp(float t){
		t = Mathf.Lerp(0.0f, 1.0f, t);

        //content
        float width = content.rect.width / 4.0f;
        content.anchoredPosition = contentBase + new Vector2(-width * t, 0.0f);

        //headBG
        if (isNeedHeadBG){
            headBG.anchoredPosition = headBGBase + new Vector2(-width * t, 0.0f);
        }
        //tab
        if (HasTab() && !isAnotherWithTab){
            tab.anchoredPosition = tabBase + new Vector2(-width * t, 0.0f);
        }

        //title and backTitle
        if (title != null){
            titleRect.anchoredPosition = titleBase + new Vector2(-width * t, 0.0f);
        }
        if (backTitle != null){
            backTitleRect.anchoredPosition = backTitleBase + new Vector2(-width * t, 0.0f);
        }

        //fadeout head component
        if (headCG != null){
            headCG.alpha = 1.0f - t;
        } 
    }

    override public void Ready(){
        base.Ready();

        isNeedHeadBG = false;
        if (HasHeadBG()){
            if(!isAnotherWithHeadBG){
                isNeedHeadBG = true;
            }
            headBG.gameObject.SetActive(isNeedHeadBG);
        }
    }
    override public void Complete(bool isReverse){
        base.Complete(isReverse);
        if (isReverse){
            SetAllBlocksRaycasts(true);

            if (HasHeadBG()){
                headBG.gameObject.SetActive(true);
            }
        }else{
            screen.gameObject.SetActive(isActiveWhenHiding);
        }
    }
}
