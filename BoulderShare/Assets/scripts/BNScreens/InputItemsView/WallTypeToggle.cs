using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class WallTypeToggle : MonoBehaviour
{
    [SerializeField] private WallTypeMap.Type type;
    [SerializeField] private WallTypeToggleGroup group;
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI nameText;
    private bool noProc;

    public void Init(WallTypeMap.Type target){
        noProc = false;
        nameText.text = WallTypeMap.Entity.GetWallTypeName(type);

        if (target == type){
            SetOnWithNoProc();
        }else{
            toggle.isOn = false;
        }
    }
    public void OnValueChanged(bool b){
        if (!noProc && b){
            group.SetWallType(type);
        }
        noProc = false;
    }

    private void SetOnWithNoProc(){
        noProc = true;
        toggle.isOn = true;
    }
}
}
