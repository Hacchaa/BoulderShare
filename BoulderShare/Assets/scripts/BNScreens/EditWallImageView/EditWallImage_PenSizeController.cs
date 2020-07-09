using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoulderNotes{
public class EditWallImage_PenSizeController : MonoBehaviour
{
    [SerializeField] private PenSizeItem[] items;
    [SerializeField] private Color focusColor;
    [SerializeField] private Color deFocusColor;

    private int cur;
    public void Init(){
        for(int i = 0 ; i < items.Length ; i++){
            items[i].Init(i, focusColor, deFocusColor);
        }
        items[2].Focus();
        cur = 2;
    }

    public void ChangeFocusItem(int index){
        items[cur].DeFocus();
        items[index].Focus();

        cur = index;
    }
}
}