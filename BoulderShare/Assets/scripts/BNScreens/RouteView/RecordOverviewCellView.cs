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
        completeRateText.SetText("<b>{0}</b><size=50%><color=#212121>%",data.completeRate);
        tryCountText.SetText("<b>{0}</b><size=50%><color=#212121>回",data.tryCount);
        daysText.SetText("<b>{0}</b><size=50%><color=#212121>日",data.days);
    }
}
}