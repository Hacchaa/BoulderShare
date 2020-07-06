using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace BoulderNotes{
public class DisplayImageControllerManager : MonoBehaviour, IInitializePotentialDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private DisplayImageView view;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private MoveImageController moveImageController;
    [SerializeField] private DisplayImageScroller displayImageController;
    [SerializeField] private DisplayImage[] displayImages;
    [SerializeField] private DisplayImage_ImageIndicator indicator;
    [SerializeField] private float space;
    private Vector2 frameSize;
    private bool canMove;
    private bool isDeterminedEvent;
	private int[] eTouches;
	private const int FINGER_NONE = -100;
    private int imageNum;
    private bool needClickEvent;
    private bool isAnimationing;
    public void Init(Sprite[] sprites){
        if (eTouches == null){
			eTouches = new int[] {FINGER_NONE, FINGER_NONE};
		}
        frameSize = new Vector2(Screen.width/CanvasResolutionManager.Instance.GetRatioOfPtToPx(), Screen.height/CanvasResolutionManager.Instance.GetRatioOfPtToPx());

        int n = Mathf.Min(displayImages.Length, sprites.Length/2);
        imageNum = n;
        //Debug.Log("n="+n);
        float x;
        for (int i = 0 ; i < displayImages.Length ; i++){
            if (i < n){
                x = i * (frameSize.x + space);
                BNManager.Instance.ActivateNecessary(displayImages[i].gameObject, true);
                displayImages[i].Init(frameSize.x, frameSize.y, x, 0f, sprites[i*2], sprites[i*2+1]);
            }else{
                BNManager.Instance.ActivateNecessary(displayImages[i].gameObject, false);
            }
        }
        displayImageController.Init(this, scrollRect, frameSize, n, space);
        moveImageController.Init(displayImages[0]);
        canMove = false;
        isDeterminedEvent = false;
        needClickEvent = false;
        isAnimationing = false;

        //indicatorを表示するかどうか
        if (n == 1){
            BNManager.Instance.ActivateNecessary(indicator.gameObject, false);
        }else{
            BNManager.Instance.ActivateNecessary(indicator.gameObject, true);
            indicator.Init(n);
        }
    }
    public void LateUpdate(){
        if(canMove){
            moveImageController.OnLateUpdate();
        }else{
            displayImageController.ResetUpdateNeeded();
        }
    }
    public void OnInitializePotentialDrag(PointerEventData data){
        displayImageController.OnInitializePotentialDrag(data);
    }

    public bool HasFinger(int index){
        if (index < 0 || index > eTouches.Length - 1){
            return false;
        }
        return eTouches[index] != FINGER_NONE;
    }

    public int GetFinger(int index){
        if (index < 0 || index > eTouches.Length - 1){
            return FINGER_NONE;
        }
        return eTouches[index];
    }

    public void OnPointerDown(PointerEventData data){

		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
            needClickEvent = true;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
            needClickEvent = false;
		}
    }
    public void OnPointerUp(PointerEventData data){
        if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;

                if (!isAnimationing && needClickEvent){
                    OnClick();
                }
            }
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
    }
    private void OnClick(){
        SwitchShowingWithAnim();
    }

    public void OnDrag(PointerEventData ped)
    {   
        if(canMove){
            moveImageController.OnDrag(ped);
        }else{
            displayImageController.OnDrag(ped);
        }
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        needClickEvent = false;
        //イベントを選択
        if(!isDeterminedEvent){
            //Debug.Log("chooseEvent");
            if (imageNum == 1){
                //表示画像が1枚しかない場合
                canMove = true;
            }else if (!moveImageController.IsAlreadyMoved()){
                //画像を動かしているかどうか
                //Debug.Log("!isAlreadyMoved");
                if (HasFinger(1)){
                   // Debug.Log("has two fingers");
                    //二本指ならmoveImageController
                    canMove = true;
                }else{
                    //Debug.Log("has one finger");
                    //一本指ならDisplayImageScroller
                    canMove = false;
                }
            }else{
                //Debug.Log("isAlreadyMoved");
                if (HasFinger(1)){
                    //Debug.Log("has two fingers");
                    //二本指ならmoveImageController
                    canMove = true;
                }else{
                    //Debug.Log("has one finger");
                    //一本指
                    //画像の両端に達していて、さらに端に動かそうとしているかどうか
                    if (moveImageController.IsAttemptingToGoOver(ped.delta)){
                        //Debug.Log("isAttemptingToGoOver");
                        canMove = false;
                    }else{
                        //Debug.Log("!isAttemptingToGoOver");
                        canMove = true;
                    }
                }
            }
            isDeterminedEvent = true;
        }

        if (canMove){
            moveImageController.OnBeginDrag(ped);
        }else{
            displayImageController.OnBeginDrag(ped);
        }
    }
    public void OnEndDrag(PointerEventData ped)
    {
        if (canMove){
            moveImageController.OnEndDrag(ped);
        }else{
            displayImageController.OnEndDrag(ped);
        }

        if(!HasFinger(0)){
            isDeterminedEvent = false;
        }
    }  

    public void ChangeDisplayImageInMoveImageController(int index){
        moveImageController.Init(displayImages[index]);
        view.ChangeWallImageIcon(displayImages[index]);
        indicator.Focus(index);
    }  
    public void ClearFrame(int index){
        displayImages[index].Clear();
    }
    public void DisableScroller(){
        scrollRect.enabled = false;
    }
    public void EnableScroller(){
        scrollRect.enabled = true;
    }

    public void AddScrollPositionX(float x){
        scrollRect.content.anchoredPosition += new Vector2(x, 0f);
    }

    public void ResetScrollPosition(){
        scrollRect.content.anchoredPosition = Vector2.zero;
    }
    public Vector2 GetScrollContentPosition(){
        return scrollRect.content.anchoredPosition;
    }
    public void AddScrollVelocityX(float v){
        scrollRect.velocity += new Vector2(v, 0f);
    }

    public DisplayImage GetCurrentDisplayImage(){
        return displayImages[displayImageController.GetCurrentFrameIndex()];
    }

    public void SwitchShowingWithAnim(){
        float duration = 0.2f;
        float t, dist;
        if (view.GetHeadAlpha() == 0f){
            t = 0f;
            dist = 1f;
        }else{
            t = 1f;
            dist = 0f;
        }

        DOVirtual.Float(t, dist, duration, value => {
    		view.SetHeadAlpha(value);
            indicator.SetAlpha(value);
    	}).SetEase(Ease.OutQuart)
        //onstartは上の処理と同じフレームで呼ばれる
        .OnStart(() =>{
            view.DeActivateHead();
            isAnimationing = true;
        })
        .OnComplete(() =>{
            view.ActivateHead();
            isAnimationing = false;
        });
    }
}
}