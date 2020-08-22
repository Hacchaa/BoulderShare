using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoulderNotes{
public class EditWallImage_PenSizeController : MonoBehaviour
{
    [SerializeField] private MobilePaintController controller;
    [SerializeField] private PenSizeItem[] items;
    [SerializeField] private Color focusColor;
    [SerializeField] private Color deFocusColor;
    private int defaultItemIndex;
    private int cur;
    public void Init(){
        for(int i = 0 ; i < items.Length ; i++){
            items[i].Init(i, focusColor, deFocusColor);
        }
        defaultItemIndex = 2;

        items[defaultItemIndex].Focus();
        cur = 2;

        
    }

    public void ChangeFocusItem(int index){
        items[cur].DeFocus();
        items[index].Focus();

        cur = index;
        SendBrushSize();
    }

    public void SendBrushSize(){
        controller.SetBrushSize(items[cur].GetBrushSize());
    }

    public float GetDefaultBrushSize(){
        return items[defaultItemIndex].GetBrushSize();
    }
    public float GetBrushSize(){
        return items[cur].GetBrushSize();
    }
}
}