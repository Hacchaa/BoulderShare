using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class RouteTapeShapeDefault : MonoBehaviour
{
    private EditRouteTapeView view;
    public void Init(EditRouteTapeView v){
        view = v;
    }

    public void OnClicked(){
        view.ChangeAsDefault();
    }
}
}