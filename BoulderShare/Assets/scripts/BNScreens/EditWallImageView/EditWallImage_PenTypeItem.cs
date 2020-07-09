using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class EditWallImage_PenTypeItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private EditWallImage_PenTypeController controller;
    [SerializeField] private Sprite focusSprite;
    [SerializeField] private Sprite deFocusSprite;
    [SerializeField] private Sprite disableSprite;
    private int index;
    public void Init(int ind){
        this.index = ind;

        icon.sprite = deFocusSprite;
    }

    public void Focus(){
        icon.sprite = focusSprite;
    }

    public void DeFocus(){
        icon.sprite = deFocusSprite;
    }

    public void Disable(){
        icon.sprite = disableSprite;
    }

    public void OnPushingButton(){
        controller.ChangePenType(index);
    }
}
}