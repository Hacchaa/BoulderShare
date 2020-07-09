using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class EditWallImage_Undo : MonoBehaviour
{
    [SerializeField] private EditWallImageView view;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite enableSprite;
    [SerializeField] private Sprite disableSprite;
    
    public void Init(){
        icon.sprite = disableSprite;
    }

    public void ShowEnableIcon(){
        icon.sprite = enableSprite;
    }

    public void ShowDisableIcon(){
        icon.sprite = disableSprite;
    }

    public void Undo(){
        view.Undo();
    }

}
}