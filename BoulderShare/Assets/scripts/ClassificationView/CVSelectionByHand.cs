using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace BoulderNotes{
public class CVSelectionByHand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{	
    //指のx軸方向の速度の閾値
    [SerializeField] private float threshold_ScreensPerSecond = 4.0f;
    private int finger = MyUtility.FINGER_NONE;
    [SerializeField] private float width;
    [SerializeField] private float startWidth;
    private bool moveToRight;
    private bool determineFirstDir;
    private bool focusCurrent ;
    [SerializeField] private ClassificationView cv;
    


    public void OnBeginDrag(PointerEventData data){
        if (finger == MyUtility.FINGER_NONE){

            width = Screen.width;
            startWidth = data.position.x;                	
            determineFirstDir = false;
            focusCurrent = true;
            finger = data.pointerId;
        }
    }

    public void OnDrag(PointerEventData data){
        if (finger == data.pointerId){
            if (!determineFirstDir){
                if (data.delta.x > 0){
                    moveToRight = true;
                }else{
                    moveToRight = false;
                }
                cv.MoveStartByHand(moveToRight);
                determineFirstDir = true;
            }

            if (moveToRight && data.position.x - startWidth < 0.0f){
                moveToRight = false;
                cv.SelectTarget(moveToRight);
            }else if (!moveToRight && data.position.x - startWidth > 0.0f){
                moveToRight = true;
                cv.SelectTarget(moveToRight);
            }

            float t = CalcScreenRatio(data.position);
            if (cv.NeededOverShooting()){
                t /= 3.0f;
            }
            
            if (t > 0.5f){
                if (focusCurrent){
                    focusCurrent = false;
                    cv.SelectTabFocus(focusCurrent);
                }
            }else{
                if (!focusCurrent){
                    focusCurrent = true;
                    cv.SelectTabFocus(focusCurrent);
                }
            }
            cv.MoveByHand(t);
        }
    }

    public void OnEndDrag(PointerEventData data){
        if (finger == data.pointerId){
            finger = MyUtility.FINGER_NONE;
            
            float screensPerSecond = CalcScreensPerSecond(data.delta);
            float t = CalcScreenRatio(data.position);
            
            if (cv.NeededOverShooting()){
                t /= 3.0f;
                cv.MoveEndByHand(t, false);
                return ;
            }

            if (screensPerSecond > threshold_ScreensPerSecond){
                cv.MoveEndByHand(t, true);
                return ;
            }

            if (t > 0.5f){
                cv.MoveEndByHand(t, true);
            }else{
                cv.MoveEndByHand(t, false);
            }
        }
    }

    public float CalcScreenRatio(Vector2 pos){
        return Mathf.Abs((pos.x - startWidth) / width);
    }
    public float CalcScreensPerSecond(Vector2 delta){
        return Mathf.Abs((delta.x / Time.deltaTime) / width);
    }
}
}