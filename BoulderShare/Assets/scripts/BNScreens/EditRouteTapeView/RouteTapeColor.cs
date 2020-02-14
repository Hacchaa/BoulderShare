using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{

public class RouteTapeColor : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Color color;
    [SerializeField] private EditRouteTapeView view;

    void Awake(){
        Init();
    }
    public void Init(){
        image.color = color;
    }

    public void OnClicked(){
        view.ChangeColor(color);
    }
}
}