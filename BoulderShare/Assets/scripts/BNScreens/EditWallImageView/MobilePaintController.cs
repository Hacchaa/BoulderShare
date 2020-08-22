using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace BoulderNotes{
public class MobilePaintController : MonoBehaviour, IInitializePotentialDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private EditWallImageView view;
    [SerializeField] private MoveImageByDoubleFingerController moveImageController;
	[SerializeField] private MobilePaintUGUI mobilePaint;
	[SerializeField] private Image image;
	[SerializeField] private RectTransform displayRect;
    [SerializeField] private EditWallImage_PenSizeController penSizeController;
    [SerializeField] private EditWallImage_PenTypeController penTypeController;
    [SerializeField] private EditWallImage_FillTypeController fillTypeController;
    [SerializeField] private EditWallImage_Undo undoController;
    private bool canMove;
    private bool isDeterminedEvent;
	private int[] eTouches;
	public static int FINGER_NONE = -100;

    public void Init(Texture2D texture){
        if (eTouches == null){
			eTouches = new int[] {FINGER_NONE, FINGER_NONE};
		}
        image.sprite = BNManager.Instance.CreateSprite(texture);
        moveImageController.Init(texture, image.GetComponent<RectTransform>(), displayRect);
		mobilePaint.Init(texture);
		penSizeController.Init();
        penTypeController.Init();
        fillTypeController.Init();
        undoController.Init();
		SetBrushSize();
        canMove = false;
        isDeterminedEvent = false;
    }
    public void LateUpdate(){
        if(canMove){
            moveImageController.OnLateUpdate();
        }
    }
    public void OnInitializePotentialDrag(PointerEventData data){
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
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
		}
    }
    public void OnPointerUp(PointerEventData data){
        if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;
            }
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
    }

    public void OnDrag(PointerEventData ped)
    {   
        if(canMove){
            moveImageController.OnDrag(ped);
        }else{
            mobilePaint.OnDrag(ped);
        }
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        //イベントを選択
        if(!isDeterminedEvent){
			if (HasFinger(1)){
				//二本指ならmoveImageController
				canMove = true;
			}else{
				//一本指
				canMove = false;
			}
            isDeterminedEvent = true;
        }

        if (canMove){
            moveImageController.OnBeginDrag(ped);
        }else{
            mobilePaint.OnBeginDrag(ped);
        }
    }
    public void OnEndDrag(PointerEventData ped)
    {
        if (canMove){
            moveImageController.OnEndDrag(ped);
        }else{
            mobilePaint.OnEndDrag(ped);
			undoController.EnableUndo();
        }

        if(!HasFinger(0)){
            isDeterminedEvent = false;
        }
    }  

	public void OnZoomAction(){
		SetBrushSize();
	}

	public void SetBrushSize(float v = -1f){
        if (v < 0){
            v = penSizeController.GetBrushSize();
        }
        int val = (int)v;
        mobilePaint.SetBrushSizeByPt(val/2f);
    }
    public void SetBrushMode(){
        mobilePaint.SetDrawMode(MobilePaintUGUI.DrawMode.Default);
    }
    public void SetFillMode(){
        mobilePaint.SetDrawMode(MobilePaintUGUI.DrawMode.FloodFill);
    }

    public void Undo(){
        mobilePaint.DoUndo();

        if(mobilePaint.CanDoUndo()){
			undoController.EnableUndo();
		}else{
			undoController.DisableUndo();
		}
    }

    private bool fillSwitch = false;

    public void ChangeFillType(){
        if (fillSwitch){
            fillTypeController.SwitchOff();
        }else{
            fillTypeController.SwitchOn();
        }
        fillSwitch = !fillSwitch;
    }
}
}