using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoulderNotes{
public class GymWallScrollerData : GymWallScrollerDataBase
{
    public BNWall wall;
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