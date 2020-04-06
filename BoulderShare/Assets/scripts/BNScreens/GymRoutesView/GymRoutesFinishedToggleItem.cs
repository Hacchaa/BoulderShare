using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class GymRoutesFinishedToggleItem : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isFinished;
    [SerializeField] private Color onBackColor;
    [SerializeField] private Color offBackColor;
    [SerializeField] private Color onTextColor;
    [SerializeField] private Color offTextColor;

    private GymRoutesFinishedToggleController controller;

    
    public void Init(GymRoutesFinishedToggleController cont){
        controller = cont;
    }
    public bool IsFinished(){
        return isFinished;
    }
    public void FocusOn(){
        text.color = onTextColor;
        image.color = onBackColor;
    }

    public void FocusOff(){
        text.color = offTextColor;
        image.color = offBackColor;
    }

    public void SendClickInfo(){
        controller.Register(this);
    }
}
}