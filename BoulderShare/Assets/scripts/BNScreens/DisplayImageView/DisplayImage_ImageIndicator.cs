using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class DisplayImage_ImageIndicator : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] Color focusColor;
    [SerializeField] Color deFocusColor;
    private int focusIndex;
    public void Init(int n){
        for(int i = 0 ; i < images.Length ; i++){
            if (i < n){
                BNManager.Instance.ActivateNecessary(images[i].gameObject, true);
                images[i].color = deFocusColor;
            }else{
                BNManager.Instance.ActivateNecessary(images[i].gameObject, false);
            }
        }
        images[0].color = focusColor;
        SetAlpha(1.0f);
        focusIndex = 0;
    }

    public void SetAlpha(float t){
        foreach(Image img in images){
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, t);
        }
    }

    public void Focus(int index){
        images[focusIndex].color = deFocusColor;
        focusIndex = index;
        images[focusIndex].color = focusColor;
    }
}
}