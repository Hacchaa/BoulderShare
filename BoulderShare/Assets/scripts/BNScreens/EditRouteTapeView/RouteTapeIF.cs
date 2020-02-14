using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class RouteTapeIF : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private EditRouteTapeView view;
    private bool noNeedChange;
    public void OnValueChanged(string str){
        if (noNeedChange){
            noNeedChange = false;
            return;
        }
        view.ChangeTapeText(str);    
    }

    public void Clear(){
        noNeedChange = true;
        inputField.text = "";
    }
}
}