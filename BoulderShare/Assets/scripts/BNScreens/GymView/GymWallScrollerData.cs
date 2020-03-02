using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoulderNotes{
public class GymWallScrollerData : GymWallScrollerDataBase
{
    public string wallID;
    public string period;
    public List<string> fileNames;
    public BNScreenStackWithTargetGym stack;
    public float fitWidth;
    public float fitHeight;
    
}

public class GymWallAddScrollerData : GymWallScrollerDataBase
{
}

public class GymWallPastScrollerData : GymWallScrollerDataBase
{
}

public class GymWallScrollerDataBase{

}


}