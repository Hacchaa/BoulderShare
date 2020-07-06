using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoulderNotes{
public class DisplayImageScroller : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private DisplayImageControllerManager manager;
    private static float THRESHOLD_FRAMEPOSRATE = 0.3f;
    //単位時間当たりの移動量をframeSize.x単位で表したもの
    private static float THRESHOLD_FRAMEPERSEC = 4f;
    private ScrollRect scrollRect;
    private float space;
    private Vector2 frameSize;
    private Vector2 contentSize;
    private int n;
    private int curFrame;
    private bool isAlreadyUpdate = false;
    public void Init(DisplayImageControllerManager m, ScrollRect sr, Vector2 fSize, int frames, float spa){
        manager = m;
        scrollRect = sr;
        frameSize = fSize;
        n = frames;
        space = spa;
        curFrame = 0;

        contentSize = new Vector2((n-1) * (frameSize.x + space) + frameSize.x, frameSize.y);
        Vector2 setting = new Vector2(0f, 0.5f);
        scrollRect.viewport.pivot = setting;
        scrollRect.viewport.anchorMax = setting;
        scrollRect.viewport.anchorMin = setting;
        scrollRect.viewport.sizeDelta = contentSize;
        scrollRect.viewport.anchoredPosition = Vector2.zero;

        scrollRect.content.pivot = setting;
        scrollRect.content.anchorMax = setting;
        scrollRect.content.anchorMin = setting;
        scrollRect.content.sizeDelta = contentSize;
        scrollRect.content.anchoredPosition = Vector2.zero;
    }

    public int GetCurrentFrameIndex(){
        return curFrame;
    }

    public void OnInitializePotentialDrag(PointerEventData data){
        scrollRect.OnInitializePotentialDrag(data);
    }
    public void OnDrag(PointerEventData ped)
    {   
        if(!isAlreadyUpdate){
            scrollRect.OnDrag(ped);
            isAlreadyUpdate = true;
        }
    }
    public void ResetUpdateNeeded(){
        isAlreadyUpdate = false;
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        scrollRect.viewport.sizeDelta = frameSize;
        scrollRect.content.anchoredPosition += new Vector2(scrollRect.viewport.anchoredPosition.x, 0f);
        scrollRect.viewport.anchoredPosition = Vector2.zero;
        scrollRect.OnBeginDrag(ped);

        if (curFrame > 0){
            manager.ClearFrame(curFrame-1);
        }
        if (curFrame < n - 1){
            manager.ClearFrame(curFrame+1);
        }
    }
    public void OnEndDrag(PointerEventData ped)
    {
        float x = -scrollRect.content.anchoredPosition.x;
        //int target = Mathf.Clamp(Mathf.FloorToInt((x + (frameSize.x + space)/2f) / (frameSize.x + space)), 0, n-1);
        int target;
        float curX = curFrame * (frameSize.x + space);
        //現在の位置から決める
        if (x - curX > frameSize.x * THRESHOLD_FRAMEPOSRATE){
            target = curFrame + 1;
        }else if(x - curX < -frameSize.x * THRESHOLD_FRAMEPOSRATE){
            target = curFrame - 1;
        }else{
            //さらに速度で決定
            float dx = (ped.delta.x / Time.deltaTime) / frameSize.x;
            if (dx > THRESHOLD_FRAMEPERSEC){
                target = curFrame - 1;
            }else if(dx < -THRESHOLD_FRAMEPERSEC){
                target = curFrame + 1;
            }else{
                target = curFrame;
            }
        }
        target = Mathf.Clamp(target, 0, n-1);
        float targetX = target * (frameSize.x + space);
        /*
        Debug.Log("x:"+x);
        Debug.Log("floor"+ (x + (frameSize.x + space)/2f) / (frameSize.x + space));
        Debug.Log("target:"+target);*/
        scrollRect.viewport.anchoredPosition = new Vector2(-targetX, 0f);
        scrollRect.viewport.sizeDelta = contentSize;
        scrollRect.content.anchoredPosition = new Vector2(targetX - x, 0f);
        scrollRect.OnEndDrag(ped);

        curFrame = target;
        manager.ChangeDisplayImageInMoveImageController(curFrame);
    }
}
}