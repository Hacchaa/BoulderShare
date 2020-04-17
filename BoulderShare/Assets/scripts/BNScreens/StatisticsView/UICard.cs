using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class UICard : MonoBehaviour
{
    [SerializeField] private Image frame;
    private bool procInit = false;

    public void TryInit(){
        if(procInit){
            return ;
        }

        BNManager.Instance.GetCornerPanelStroke(OnLoad);
        procInit = true;
    }

    private void OnLoad(Sprite sprite){
        frame.sprite = sprite;
    }

}
}