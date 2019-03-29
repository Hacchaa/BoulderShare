using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FailureComment : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string txt){
    	text.text = txt;
    }
}
