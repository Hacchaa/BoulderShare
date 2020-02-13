using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class RecordDateTitleCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI dateText;

    public void SetData(RecordDateTitleScrollerData data){
       dateText.text = data.date; 
    }
}
}