using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BoulderNotes{
public class EditWallImage_ShowOrigin : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private MobilePaintController controller;
    private int fingerID ;
    public void Init(){
        fingerID = -100;
    }
    public void OnPointerDown(PointerEventData data){
        if (fingerID == -100){
            controller.ShowOriginalImage();
            fingerID = data.pointerId;
        }
    }
    public void OnPointerUp(PointerEventData data){
        if (fingerID != data.pointerId){
            return ;
        }
        controller.HideOriginalImage();

        fingerID = -100;
    }
}
}