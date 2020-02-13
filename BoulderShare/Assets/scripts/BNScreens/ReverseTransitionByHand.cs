using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BoulderNotes{
public class ReverseTransitionByHand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{	
    //指のx軸方向の速度の閾値
    [SerializeField] private float threshold_ScreensPerSecond = 4.0f;
    [SerializeField] private bool isHorizontal;
    private int finger = MyUtility.FINGER_NONE;
    private float whole;
    private float startPos;
    private BNTransitionController controller;

    public void OnBeginDrag(PointerEventData data){
        if (finger == MyUtility.FINGER_NONE){
            if (isHorizontal){
                whole = Screen.width;
                startPos = data.position.x;                
            }else{
                whole = Screen.height;
                startPos = data.position.y;
            }

            controller = BNScreens.Instance.GetCurrentStack().GetLatestController();
            controller.Ready(true);	
            
            finger = data.pointerId;
        }
    }

    public void OnDrag(PointerEventData data){
        if (finger == data.pointerId){
            float t = CalcScreenRatio(data.position);
            controller.BNTransitionLerp(t);
        }
    }

    public void OnEndDrag(PointerEventData data){
        if (finger == data.pointerId){
            finger = MyUtility.FINGER_NONE;
            
            float screensPerSecond = CalcScreensPerSecond(data.delta);
            float t = CalcScreenRatio(data.position);

            if (screensPerSecond > threshold_ScreensPerSecond){
                BNScreens.Instance.ReverseTransition(t);
                return ;
            }

            if (t > 0.5f){
                controller.SwitchTransitionDirection();
                controller.BNTransitionWithAnim(t);
            }else{
                //画面遷移する
                BNScreens.Instance.ReverseTransition(t);
            }
        }
    }

    public float CalcScreenRatio(Vector2 pos){
        float diff;
        if (isHorizontal){
            diff = pos.x - startPos;
        }else{
            //y軸は逆
            diff = (pos.y - startPos) * -1.0f;
        }
        return (whole - diff) / whole;
    }
    public float CalcScreensPerSecond(Vector2 delta){
        if (isHorizontal)
            return (delta.x / Time.deltaTime) / whole;

        return (-delta.y / Time.deltaTime) / whole;
    }
}
}