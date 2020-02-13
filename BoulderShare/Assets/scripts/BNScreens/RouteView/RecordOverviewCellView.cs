using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class RecordOverviewCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI completeRateText;
    public TextMeshProUGUI tryCountText;
    public TextMeshProUGUI daysText;

    public void SetData(RecordOverviewScrollerData data){
        completeRateText.text = data.completeRate + "";
        tryCountText.text = data.tryCount + "";
        daysText.text = data.days + "";
    }
}
}