using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;

namespace BoulderNotes{
public class RankWallType : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private AssetReference[] rankRef;

    public void SetData(string[] arr){
        for(int i = 0 ; i < texts.Length ; i++){
            texts[i].text = arr[i];
        }
    }
}
}