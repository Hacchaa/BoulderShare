using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class EditWallImage_PenTypeController : MonoBehaviour
{
    [SerializeField] private EditWallImage_PenTypeItem[] items;
    private int cur;
    public void Init(){
        for(int i = 0 ; i < items.Length ; i++){
            items[i].Init(i);
        }

        items[0].Focus();
        cur = 0;
    }

    public void ChangePenType(int index){
        items[cur].DeFocus();
        items[index].Focus();

        cur = index;
    }
}}