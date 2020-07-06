using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoulderNotes{
public class BNTabItem : MonoBehaviour
{
    [SerializeField] private BNScreens.BNTabName bnTabName;
    [SerializeField] private Sprite focusSprite;
    [SerializeField] private Sprite defocusSprite;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    
    public BNScreens.BNTabName GetTabName(){
        return bnTabName;
    }

    public void Focus(Color textColor){
        icon.sprite = focusSprite;

        SetTextColor(textColor);
    }

    public void DeFocus(Color textColor){
        icon.sprite = defocusSprite;  

        SetTextColor(textColor);      
    }

    public void SetTextColor(Color c){
        text.color = c;
    }

}
}