using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
[RequireComponent(typeof(Image))]
public class CornerPanelStroke : MonoBehaviour
{
    private Image strokeImage;
    void Awake(){
        strokeImage = GetComponent<Image>();
        BNManager.Instance.GetCornerPanelStroke(OnLoad);
    }
    private void OnLoad(Sprite sprite){
        strokeImage.sprite = sprite;
    }
}
}