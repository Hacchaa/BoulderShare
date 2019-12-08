using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
    abstract public class BarLabelsBase : ScriptableObject,IBarLabels
    {
        abstract public string GetLabelName(int index);
    }
}
