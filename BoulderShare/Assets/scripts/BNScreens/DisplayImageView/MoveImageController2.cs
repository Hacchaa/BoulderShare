using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace BoulderNotes{
public class MoveImageController2 : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -100;
	private bool isUpdate = false;
	[SerializeField] private Image displayImage;
	private RectTransform displayImageRect;
	[SerializeField] private ScrollRect scrollRect;
	private Vector2 centerPointOffset;
 	public void Init (Sprite sprite) {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};

		displayImageRect = displayImage.GetComponent<RectTransform>();
		FitImage(sprite);

		scrollRect.content.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.content.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.content.sizeDelta = new Vector2(scrollRect.viewport.rect.width, scrollRect.viewport.rect.height);

		centerPointOffset = Vector2.zero;
	}	
	void LateUpdate(){
		isUpdate = false;
	}	
	public void OnInitializePotentialDrag(PointerEventData data){
		if (data.pointerId == eTouches[0]){
			scrollRect.OnInitializePotentialDrag(data);
		}
	}
	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
			centerPointOffset = Vector2.zero;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
			//calculate new offset
			//指のpositionの取得
			Vector2 p1 = Vector2.zero;
			foreach(Touch touch in Input.touches){
				if (touch.fingerId == eTouches[0]){
					p1 = touch.position;
				}
			} 
			//scrollRectに渡すpositionの計算
			Vector2 p = p1 + centerPointOffset;
			//新しいcenterの計算
			Vector2 center = (data.position + p1) / 2f;
			//centerからpに向かうベクトルをcenterpointoffsetにする
			centerPointOffset = p - center;
		}
	}
	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] != data.pointerId && eTouches[1] != data.pointerId){
			return ;
		}
		if (eTouches[1] == FINGER_NONE){
			return ;
		}

		//calculate new offset
		Vector2 p1, p2;
		p1 = p2 = Vector2.zero;

		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
			}
		} 
		//scrollRectに渡すpositionの計算
		Vector2 p = (p1 + p2) / 2f + centerPointOffset;
		//新しいcenterの計算
		Vector2 center;
		if (data.pointerId == eTouches[0]){
			center = p2;
		}else{
			center = p1;
		}
		//centerからpに向かうベクトルをcenterpointoffsetにする
		centerPointOffset = p - center;
	}
	public void OnBeginDrag(PointerEventData data){
		if (data.pointerId == eTouches[0]){
			scrollRect.OnBeginDrag(data);
		}
	}
	public void OnDrag(PointerEventData data){
        Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;
		//data.pointerIdが現在扱っている指かどうか
		if (isUpdate || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}       

		//一本指の場合
        if (eTouches[1] == FINGER_NONE){
            isUpdate = true;
			data.position = data.position + centerPointOffset;
			scrollRect.OnDrag(data);
            return ;
        }
		//二本指の場合
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

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = p1 - dP1;
        Vector2 touchOnePrevPos = p2 - dP2;
        
        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (p1 - p2).magnitude;
        
        // Find the difference in the distances between each frame.
        float rate = touchDeltaMag / prevTouchDeltaMag;
		Vector2 diff = displayImageRect.sizeDelta * (rate - 1f);
		//画面中心にある点の、moverectから見た座標
		Vector2 center = -scrollRect.content.anchoredPosition;
		displayImageRect.sizeDelta += diff;
		scrollRect.content.sizeDelta += diff;
		scrollRect.content.anchoredPosition -= center * (rate - 1f);

        Vector2 cur = new Vector2((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f);
		data.position = cur + centerPointOffset;
		scrollRect.OnDrag(data);
		isUpdate = true;
	}
	public void OnEndDrag(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			//二本の指が離された場合
			if (eTouches[1] == FINGER_NONE){
				scrollRect.OnEndDrag(data);
			}
			eTouches[0] = eTouches[1];
			eTouches[1] = FINGER_NONE;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
	}
	public void OnScroll(PointerEventData data){
		scrollRect.OnScroll(data);
	}
	private void FitImage(Sprite spr){
        float fitHeight = scrollRect.viewport.rect.height;
        float fitWidth = scrollRect.viewport.rect.width;

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

		displayImage.sprite = spr;

        displayImageRect.anchorMin = new Vector2(0.5f, 0.5f);
        displayImageRect.anchorMax = new Vector2(0.5f, 0.5f);
        displayImageRect.sizeDelta = new Vector2(w, h);
    }
 /*
	public void OnDrag(PointerEventData data){
		//Debug.Log("drag "+data.pointerId);
        Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;
		//data.pointerIdが現在扱っている指かどうか
		if (isUpdate || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}       

		//一本指の場合
        if (eTouches[1] == FINGER_NONE){
            
            isUpdate = true;
			scrollRect.OnDrag(data);
            return ;
        }
		//二本指の場合
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

        Vector2 old = new Vector2((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f);
        Vector2 cur = new Vector2((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f);

        Vector2 diff = (cur - old) / CanvasResolutionManager.Instance.GetRatioOfPtToPx();;
        moveRect.anchoredPosition += CalcBoundsDelta(diff);


        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = p1 - dP1;
        Vector2 touchOnePrevPos = p2 - dP2;
        
        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (p1 - p2).magnitude;
        
        // Find the difference in the distances between each frame.
        float rate = touchDeltaMag / prevTouchDeltaMag;
		diff = moveRect.sizeDelta * (rate - 1f);
		//画面中心にある点の、moverectから見た座標
		Vector2 center = -moveRect.anchoredPosition;
		moveRect.sizeDelta += diff;

		moveRect.anchoredPosition -= center * (rate - 1f);

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
		//Debug.Log("boundsRect.x < (moveRect.anchoredPosition.x - moveRect.rect.width/2f) " + boundsRect.x + " < "+(moveRect.anchoredPosition.x - moveRect.rect.width/2f));
		if (boundsRect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			return true;
		}
		//右側
		if (-boundsRect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			return true;
		}

		return false;
	}
	public bool IsOutBoundY(){
		//Debug.Log("-boundsRect.y > (moveRect.anchoredPosition.y + moveRect.rect.height/2f) " + -boundsRect.y + " > "+(moveRect.anchoredPosition.y + moveRect.rect.height/2f));
		//上側
		if (-boundsRect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			return true;
		}
		//下側
		if (boundsRect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			return true;
		}

		return false;
	}

	public void Bounds(){
		float x = 0;
		float y = 0;

		if (boundsRect.x < moveRect.anchoredPosition.x - moveRect.rect.width/2f){
			x = boundsRect.x - (moveRect.anchoredPosition.x - moveRect.rect.width/2f);
		}else if (-boundsRect.x > moveRect.anchoredPosition.x + moveRect.rect.width/2f){
			x = -boundsRect.x - (moveRect.anchoredPosition.x + moveRect.rect.width/2f);
		}		

		if (-boundsRect.y > moveRect.anchoredPosition.y + moveRect.rect.height/2f){
			y = -boundsRect.y - (moveRect.anchoredPosition.y + moveRect.rect.height/2f);
		}else if (boundsRect.y < moveRect.anchoredPosition.y - moveRect.rect.height/2f){
			y = boundsRect.y - (moveRect.anchoredPosition.y - moveRect.rect.height/2f);
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

		displayImage.sprite = spr;

		RectTransform imageRect = displayImage.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.sizeDelta = new Vector2(w, h);
    }*/
}
}