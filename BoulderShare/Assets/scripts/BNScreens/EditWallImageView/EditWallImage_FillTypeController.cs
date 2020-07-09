using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class EditWallImage_FillTypeController : MonoBehaviour
{
    [SerializeField] private EditWallImageView view;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    private bool isOn;
    public void Init(){
        icon.sprite = offSprite;
        isOn = false;
    }

    public void SwitchOn(){
        icon.sprite = onSprite;
    }

    public void SwitchOff(){
        icon.sprite = offSprite;
    }

    public void SwitchFillType(){
        view.ChangeFillType();
    }
}
}
