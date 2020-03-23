using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoulderNotes{
public class SliderEventCatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
{
    [SerializeField] private Slider reciever;
    [SerializeField] private EditWallImageView view;

    public void OnPointerDown(PointerEventData data){
        view.SetBrushSize();
        view.ShowBrush();
        reciever.OnPointerDown(data);
    }
    public void OnPointerUp(PointerEventData data){
        view.HideBrush();
        reciever.OnPointerUp(data);
    }
    public void OnInitializePotentialDrag(PointerEventData data){
        reciever.OnInitializePotentialDrag(data);
    }   

    public void OnDrag(PointerEventData data){
        reciever.OnDrag(data);
    }
}
}