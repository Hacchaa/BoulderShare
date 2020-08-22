using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class EditWallImage_Undo : MonoBehaviour
{
    [SerializeField] private MobilePaintController controller;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite enableSprite;
    [SerializeField] private Sprite disableSprite;
    private bool enableUndo;
    public void Init(){
        icon.sprite = disableSprite;
        enableUndo = false;
    }

    public void EnableUndo(){
        icon.sprite = enableSprite;
        enableUndo = true;
    }

    public void DisableUndo(){
        icon.sprite = disableSprite;
        enableUndo = false;
    }

    public void Undo(){
        if (enableUndo){
            controller.Undo();
        }
    }

}
}