using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class GymRouteScrollerData : GymRouteScrollerDataBase
{
    public BNRoute route;
    public string markPath;
    public string period;
    public BNGradeMap.Grade grade;
    public bool isFinished;
    
}
public class GymRoutePastScrollerData : GymRouteScrollerDataBase
{
}

public class GymRouteScrollerDataBase{
    public bool isRoute;
    public bool isAddButton;
}
}