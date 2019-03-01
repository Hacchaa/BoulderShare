using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPickerComponent : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
    private Image image;
    [SerializeField]
    private ColorSetter3D colorSetter;

    public void OnPointerClick(PointerEventData data){
    	colorSetter.SetColor(image.color);
    }
}
