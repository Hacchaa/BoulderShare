using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace BoulderNotes{
public class DisplayImageView: BNScreen
{
    
    public override void InitForFirstTransition(){
    }

    public override void UpdateScreen(){

    }    
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
}
}