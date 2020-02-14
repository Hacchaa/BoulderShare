using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{

public class RouteTapeShape : MonoBehaviour
{
    [SerializeField] Image image;
    private EditRouteTapeView view;
    public void Init(Sprite sprite, EditRouteTapeView v){
        view = v;
        image.sprite = sprite;
    }

    public void OnClicked(){
        view.ChangeShape(image.sprite);
    }
}
}