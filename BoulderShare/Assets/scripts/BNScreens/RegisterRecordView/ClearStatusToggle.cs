using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class ClearStatusToggle : MonoBehaviour
{
    [SerializeField] private RegisterRecordView view;
    [SerializeField] private BNRoute.ClearStatus status;
    [SerializeField] private Toggle toggle;
    public void Init(BNRoute.ClearStatus s){
        if (status == s){
            toggle.isOn = true;
        }else{
            toggle.isOn = false;
        }
    }
    public void OnValueChanged(bool b){
        if (b){
            view.SetHasInsight(status);
        }
    }
}
}