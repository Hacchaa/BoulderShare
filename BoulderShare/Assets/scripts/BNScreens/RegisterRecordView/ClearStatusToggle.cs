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
        //Debug.Log(s+ " "+ status);
        if (status == s){
            //Debug.Log("ture");
            toggle.isOn = true;
        }else{
            //Debug.Log("false");
            toggle.isOn = false;
        }
    }
    public void OnValueChanged(bool b){
        //Debug.Log(status+" "+b);
        if (b){
            view.SetHasInsight(status);
        }
    }
}
}