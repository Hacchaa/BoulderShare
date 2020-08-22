using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class PenSizeItem : MonoBehaviour
{
    [SerializeField] private EditWallImage_PenSizeController controller;
    [SerializeField] private Image outerCircle;
    [SerializeField] private RectTransform innnerCircleRect;
    [SerializeField] private float size;
    private RectTransform outerCircleRect;
    private Color focusColor;
    private Color deFocusColor;
    private int index;
    public void Init(int ind, Color focus, Color deFocus){
        index = ind;
        focusColor = focus;
        deFocusColor = deFocus;

        if (outerCircleRect == null){
            outerCircleRect = outerCircle.GetComponent<RectTransform>();
        }

        outerCircleRect.sizeDelta = new Vector2(size, size);
        DeFocus();
    }

    public void Focus(){
        outerCircle.color = focusColor;
        innnerCircleRect.sizeDelta = new Vector2(size-3, size-3);
    }

    public void DeFocus(){
        outerCircle.color = deFocusColor;
        innnerCircleRect.sizeDelta = new Vector2(size-1, size-1);        
    }

    public void OnPushingButton(){
        controller.ChangeFocusItem(index);
    }

    public float GetBrushSize(){
        return size;
    }
}
}