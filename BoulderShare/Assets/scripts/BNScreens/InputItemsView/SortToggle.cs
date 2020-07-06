using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
public class SortToggle : MonoBehaviour
{
    public enum SortType{Latest, Name, More, None};
    [SerializeField] private SortType sortType;
    [SerializeField] private SortToggleGroup group;
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI nameText;
    private bool noProc;

    public void Init(SortType target){
        noProc = false;
        nameText.text = GetSortTypeName(sortType);

        if (target == sortType){
            SetOnWithNoProc();
        }else{
            toggle.isOn = false;
        }
    }
    public void OnValueChanged(bool b){
        if (!noProc && b){
            group.SetSortType(sortType);
            group.Register();
        }
        noProc = false;
    }

    private void SetOnWithNoProc(){
        noProc = true;
        toggle.isOn = true;
    }

    public static string GetSortTypeName(SortType type){
        if (type == SortType.Latest){
            return "最後に挑戦した順";
        }
        if (type == SortType.Name){
            return "ジム名（昇順）";
        }
        if (type == SortType.More){
            return "課題が多い順";
        }
        return "";
    }
}
}