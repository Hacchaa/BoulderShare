using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class TapeShapeCVC : CVContent
{
    [SerializeField] ScrollRect scroller;

    public override void Init(float w){
        base.Init(w);
        //Debug.Log("init tapechapecvc");
        scroller.verticalNormalizedPosition = 1.0f;
    }
}
}