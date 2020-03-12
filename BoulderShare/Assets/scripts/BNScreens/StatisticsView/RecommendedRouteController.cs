using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class RecommendedRouteController : MonoBehaviour
{
    [SerializeField] private RecommendedRouteView[] views;
    [SerializeField] private GameObject[] showObjs;
    public void SetData(BNPair[] arr){
        foreach(GameObject obj in showObjs){
            obj.SetActive(false);
        }

        for(int i = 0 ; i < views.Length ; i++){
            if (arr[i] != null){
                showObjs[i*2].SetActive(true);
                if (i != 0){
                    showObjs[i*2-1].SetActive(true);
                }
                views[i].SetData(arr[i]);
            }
        }
    }
}
}