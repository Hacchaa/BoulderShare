using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class EditWallImage_SaveButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    [SerializeField] private EditWallImageView view;
    private bool isEnable;

    public void Init(){
        DisableButton();
    }

    public void EnableButton(){
        isEnable = true;
        image.color = enableColor;
    }

    public void DisableButton(){
        isEnable = false;
        image.color = disableColor;
    }
    
    public void OnPushButton(){
        if (!isEnable){
            return ;
        }

        view.SaveWallImage();
        view.ReverseTransition();
    }
}
}