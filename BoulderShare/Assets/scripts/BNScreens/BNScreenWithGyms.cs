using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNScreenWithGyms : BNScreen
{
    public BNScreenStackWithTargetGym GetScreenStackWithGyms(){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            return belongingStack as BNScreenStackWithTargetGym;
        }
        return null;
    }
}
}