using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class BNTabItem : MonoBehaviour
{
    [SerializeField] private BNScreens.BNTabName bnTabName;
    [SerializeField] private GameObject focusImage;
    [SerializeField] private GameObject defocusImage;
    [SerializeField] private TextMeshProUGUI text;
    
    public BNScreens.BNTabName GetTabName(){
        return bnTabName;
    }

    public void Focus(Color textColor){
        focusImage.SetActive(true);
        defocusImage.SetActive(false);

        SetTextColor(textColor);
    }

    public void DeFocus(Color textColor){
        focusImage.SetActive(false);
        defocusImage.SetActive(true);  

        SetTextColor(textColor);      
    }

    public void SetTextColor(Color c){
        text.color = c;
    }

}
}