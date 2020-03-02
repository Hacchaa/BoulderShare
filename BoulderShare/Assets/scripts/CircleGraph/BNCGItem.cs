using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
[RequireComponent(typeof(Image))]    
public class BNCGItem : MonoBehaviour
{
    private Image image;
    private float amount;
    private int priority;
    public void Init(float size, Color c, int pri){
        image = GetComponent<Image>();
        image.color = c;

        RectTransform r = GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(size, size);

        amount = 0.0f;
        priority = pri;
        DeActive();
    }

    public void SetAmount(float f){
        amount = f;
    }

    public float GetAmount(){
        return amount;
    }
    public int GetPriority(){
        return priority;
    }

    public void Arrenge(float degreeRate){
        image.fillAmount = amount;
        transform.localRotation = Quaternion.Euler(0f, 0f, degreeRate*360f);
    }

    public void Active(){
        gameObject.SetActive(true);
    }
    public void DeActive(){
        gameObject.SetActive(false);
    }
}
}