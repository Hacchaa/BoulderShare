using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
[RequireComponent(typeof(Image))]
public class CornerPanelFill : MonoBehaviour
{
    private Image fillImage;
    void Awake(){
        fillImage = GetComponent<Image>();
        BNManager.Instance.GetCornerPanelFill(OnLoad);
    }
    private void OnLoad(Sprite sprite){
        fillImage.sprite = sprite;
    }
}
}