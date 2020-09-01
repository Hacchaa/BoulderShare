using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class EditWallImage_PenTypeController : MonoBehaviour
{
    [SerializeField] private EditWallImage_PenTypeItem[] items;
    [SerializeField] private GameObject[] typeObjects;
    [SerializeField] private MobilePaintController controller;
    private Dictionary<int, int> modeMap;

    private int cur;
    public void Init(){
        modeMap = new Dictionary<int, int>();
        for(int i = 0 ; i < items.Length ; i++){
            items[i].Init(i);
            modeMap.Add((int)items[i].GetDrawMode(), i);
            typeObjects[i].SetActive(false);
        }

        items[0].Focus();
        typeObjects[0].SetActive(true);
        cur = 0;
    }

    public void ChangePenType(int index){
        controller.SetBrushMode(items[index].GetDrawMode());
    }

    public void Focus(MobilePaintUGUI.DrawMode mode){
        if(!modeMap.ContainsKey((int)mode)){
            return;
        }
        items[cur].DeFocus();
        typeObjects[cur].SetActive(false);

        int index = modeMap[(int)mode];
        items[index].Focus();
        typeObjects[index].SetActive(true);

        cur = index;
    }
}}