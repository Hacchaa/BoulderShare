using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class GymRoutesFinishedToggleItem : MonoBehaviour
{

    [SerializeField] private Image flameImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isFinished;
    [SerializeField] private Color onBackColor;
    [SerializeField] private Color offBackColor;
    [SerializeField] private Color onTextColor;
    [SerializeField] private Color offTextColor;
    private bool completedInit = false;

    private GymRoutesFinishedToggleController controller;

    
    public void Init(GymRoutesFinishedToggleController cont){
        controller = cont;

        if (!completedInit){
            completedInit = true;
            BNManager.Instance.GetCornerPanelFill(OnLoadFill);
            BNManager.Instance.GetCornerPanelStroke(OnLoadFlame);
        }

    }
    private void OnLoadFlame(Sprite sprite){
        flameImage.sprite = sprite;
    }
    private void OnLoadFill(Sprite sprite){
        fillImage.sprite = sprite;
    }
    public bool IsFinished(){
        return isFinished;
    }
    public void FocusOn(){
        text.color = onTextColor;
        fillImage.color = onBackColor;
    }

    public void FocusOff(){
        text.color = offTextColor;
        fillImage.color = offBackColor;
    }

    public void SendClickInfo(){
        controller.Register(this);
    }
}
}