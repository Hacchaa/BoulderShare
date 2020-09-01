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
    [SerializeField] private Image originalImage;
	[SerializeField] private RectTransform displayRect;
    [SerializeField] private EditWallImage_PenSizeController penSizeController;
    [SerializeField] private EditWallImage_PenTypeController penTypeController;
    [SerializeField] private EditWallImage_FillTypeController fillTypeController;
    [SerializeField] private EditWallImage_ShowOrigin showOrigin;
    [SerializeField] private EditWallImage_Undo undoController;
    [SerializeField] private Slider StampSizeSlider;
    [SerializeField] private float minStampSize = 10;
    [SerializeField] private float maxStampSize = 100;
    [SerializeField] private EditWallImage_SaveButton saveButton;
    private bool canMove;
    private bool isDeterminedEvent;
	private int[] eTouches;
	public static int FINGER_NONE = -100;

    public void Init(Sprite mainImage, Sprite maskImage){
        Texture2D maskTex = (maskImage == null)? null : maskImage.texture;

        if (eTouches == null){
			eTouches = new int[] {FINGER_NONE, FINGER_NONE};
		}
        image.sprite = mainImage;
        moveImageController.Init(mainImage.texture, image.GetComponent<RectTransform>(), displayRect);
		mobilePaint.Init(mainImage.texture, maskTex);
		penSizeController.Init();
        penTypeController.Init();
        fillTypeController.Init();
        undoController.Init();
        showOrigin.Init();
		SetBrushSize();
        canMove = false;
        isDeterminedEvent = false;

        originalImage.sprite = mainImage;
        originalImage.gameObject.SetActive(false);

        saveButton.Init();
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
            int currentUndoSize = mobilePaint.GetUndoSize();
            if (currentUndoSize != 0){
                undoController.EnableUndo();

                saveButton.EnableButton();
            }else{
                saveButton.DisableButton();
            }
        }

        if(!HasFinger(0)){
            isDeterminedEvent = false;
        }
    }  

	public void OnZoomAction(){
        MobilePaintUGUI.DrawMode mode = mobilePaint.GetDrawMode();
		if (mode == MobilePaintUGUI.DrawMode.Default){
            SetBrushSize(penSizeController.GetBrushSize());
        }else if(mode == MobilePaintUGUI.DrawMode.Circle){
            SetBrushSizeByStampSizeSlider(StampSizeSlider.value);
            HideStampPreview();
        }else if (mode == MobilePaintUGUI.DrawMode.Eraser){
            SetBrushSize(penSizeController.GetBrushSize());
        }
	}

	public void SetBrushSize(float v = -1f){
        if (v < 0){
            v = penSizeController.GetDefaultBrushSize();
        }

        mobilePaint.SetBrushSizeByPt(v/2f);
    }
    public void SetBrushSizeByStampSizeSlider(float v){
        float val = v * (maxStampSize - minStampSize) + minStampSize;
        mobilePaint.SetBrushSizeByPt(val/2f);
        ShowStampPreview();
    }
    public void SetBrushMode(MobilePaintUGUI.DrawMode mode){
        mobilePaint.SetDrawMode(mode);
        penTypeController.Focus(mode);

        if (mode == MobilePaintUGUI.DrawMode.Default){
            penSizeController.ChangeFocusItemByDefault();
        }else if(mode == MobilePaintUGUI.DrawMode.Circle){
            StampSizeSlider.value = 0f;
            StampSizeSlider.value = 0.5f;
            HideStampPreview();
        }else if (mode == MobilePaintUGUI.DrawMode.Eraser){
            penSizeController.ChangeFocusItemByDefault();
        }
    }

    public void Undo(){
        mobilePaint.DoUndo();

        if(mobilePaint.CanDoUndo()){
			undoController.EnableUndo();
            saveButton.EnableButton();
		}else{
			undoController.DisableUndo();
            saveButton.DisableButton();
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

    public void ShowOriginalImage(){
        originalImage.gameObject.SetActive(true);
    }
    public void HideOriginalImage(){
        originalImage.gameObject.SetActive(false);
    }
    public void ShowStampPreview(){
        mobilePaint.ShowPreviewCircle(new Vector2(Screen.width/2f, Screen.height/2f));
    }
    public void HideStampPreview(){
        mobilePaint.HidePreviewCircle();
    }

    public bool HasUpdate(){
        return mobilePaint.CanDoUndo();
    }
}
}