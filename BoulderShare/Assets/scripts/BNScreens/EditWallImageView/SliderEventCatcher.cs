using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoulderNotes{
public class SliderEventCatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
{
    [SerializeField] private Slider reciever;
    [SerializeField] private MobilePaintController controller;

    public void OnPointerDown(PointerEventData data){
        reciever.OnPointerDown(data);
        controller.ShowStampPreview();
    }
    public void OnPointerUp(PointerEventData data){
        reciever.OnPointerUp(data);
        controller.HideStampPreview();
    }
    public void OnInitializePotentialDrag(PointerEventData data){
        reciever.OnInitializePotentialDrag(data);
    }   

    public void OnDrag(PointerEventData data){
        reciever.OnDrag(data);
    }
}
}