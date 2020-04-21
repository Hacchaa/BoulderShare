﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace BoulderNotes{
public class MoveImageController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -100;
	private float prevLength;
	private bool isUpdate = false;
	private const float WEIGHT = 0.2f;
	[SerializeField] private RectTransform boundsRect;
	private const float BOUNDSDELTARATE = 0.5f;
    [SerializeField] private RectTransform moveRect;
	[SerializeField] private Image displayImage;
	[SerializeField] private RectTransform displayArea;
 	public void Init (Sprite sprite) {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};

		FitImage(sprite);
		///Debug.Log("moveRect:"+moveRect.sizeDelta.x + " "+moveRect.sizeDelta.y);
		boundsRect.sizeDelta = moveRect.sizeDelta;
		boundsRect.anchoredPosition = moveRect.anchoredPosition;
	}	

	void LateUpdate(){
		isUpdate = false;
	}

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
		}
	}
    public void OnBeginDrag(PointerEventData data){
		
    }

	public void OnDrag(PointerEventData data){
		//Debug.Log("drag "+data.pointerId);
        Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;
		//data.pointerIdが現在扱っている指かどうか
		if (isUpdate || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}       
		//Debug.Log("pass");
		p1 = data.position;
		dP1 = data.delta;
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
				dP2 = touch.deltaPosition;
			}
		} 

        float length = Vector2.Distance(p1, p2);


        //一本指の場合
        if (eTouches[1] == FINGER_NONE){
			Vector2 del = dP1 / CanvasResolutionManager.Instance.GetRatioOfPtToPx();

			moveRect.anchoredPosition += CalcBoundsDelta(del);
            
            isUpdate = true;
            return ;
        }


        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = p1 - dP1;
        Vector2 touchOnePrevPos = p2 - dP2;
        
        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (p1 - p2).magnitude;
        
        // Find the difference in the distances between each frame.
        float rate = touchDeltaMag / prevTouchDeltaMag;
		Vector2 diff = moveRect.sizeDelta * (rate - 1f);
		//画面中心にある点の、moverectから見た座標
		Vector2 center = -moveRect.anchoredPosition;
		moveRect.sizeDelta += diff;
		CalculateBoundsArea(moveRect.sizeDelta);

		moveRect.anchoredPosition -= center * (rate - 1f);

		Vector2 old = new Vector2((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f);
        Vector2 cur = new Vector2((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f);

        diff = (cur - old) / CanvasResolutionManager.Instance.GetRatioOfPtToPx();;
        moveRect.anchoredPosition += CalcBoundsDelta(diff);

		isUpdate = true;
	}
	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;
            }
			Bounds();
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
			Bounds();
		}
    }

	public void CalculateBoundsArea(Vector2 size){
		float x, y;
		x = Mathf.Min(size.x, displayArea.rect.width);
		y = Mathf.Min(size.y, displayArea.rect.height);
		boundsRect.sizeDelta = new Vector2(x, y);
	}

	public Vector2 CalcBoundsDelta(Vector2 delta){
		float x, y;
		if (IsOutBoundX()){
			//Debug.Log("boudsX");
			x = delta.x * BOUNDSDELTARATE;
		}else{
			x = delta.x;
		}

		if (IsOutBoundY()){
			//Debug.Log("BoundsY");
			y = delta.y * BOUNDSDELTARATE;
		}else{
			y = delta.y;
		}
		//Debug.Log("delta"+delta.x +" "+delta.y);
		//Debug.Log("newDelta"+x+" "+y);
		return new Vector2(x, y);
		
	}

	public bool IsOutBoundX(){
		//左側
		//Debug.Log("boundsRect.rect.x < (moveRect.anchoredPosition.x - moveRect.rect.width/2f) " + boundsRect.rect.x + " < "+(moveRect.anchoredPosition.x - moveRect.rect.width/2f));
		if (boundsRect.rect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			return true;
		}
		//右側
		if (-boundsRect.rect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			return true;
		}

		return false;
	}
	public bool IsOutBoundY(){
		//Debug.Log("-boundsRect.rect.y > (moveRect.anchoredPosition.y + moveRect.rect.height/2f) " + -boundsRect.rect.y + " > "+(moveRect.anchoredPosition.y + moveRect.rect.height/2f));
		//上側
		if (-boundsRect.rect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			return true;
		}
		//下側
		if (boundsRect.rect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			return true;
		}

		return false;
	}

	public void Bounds(){
		float x = 0;
		float y = 0;

		if (boundsRect.rect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			x = boundsRect.rect.x - (moveRect.anchoredPosition.x - moveRect.rect.width/2f);
		}else if (-boundsRect.rect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			x = -boundsRect.rect.x - (moveRect.anchoredPosition.x + moveRect.rect.width/2f);
		}		

		if (-boundsRect.rect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			y = -boundsRect.rect.y - (moveRect.anchoredPosition.y + moveRect.rect.height/2f);
		}else if (boundsRect.rect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			y = boundsRect.rect.y - (moveRect.anchoredPosition.y - moveRect.rect.height/2f);
		}

		moveRect.anchoredPosition += new Vector2(x, y);
	}

	private void FitImage(Sprite spr){
        float fitHeight = displayArea.rect.height;
        float fitWidth = displayArea.rect.width;

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

        moveRect.anchorMin = new Vector2(0.5f, 0.5f);
        moveRect.anchorMax = new Vector2(0.5f, 0.5f);
        moveRect.sizeDelta = new Vector2(w, h);

        displayImage.sprite = spr;
    }
}
}