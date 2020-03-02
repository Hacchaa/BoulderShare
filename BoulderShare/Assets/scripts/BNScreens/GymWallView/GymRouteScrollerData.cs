using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class GymRouteScrollerData : GymRouteScrollerDataBase
{
    public string routeID;
    public string markPath;
    public string period;
    public BNGradeMap.Grade grade;
    public bool isFinished;
    public RTape routeTape;
    
}
public class GymRoutePastScrollerData : GymRouteScrollerDataBase
{
}

public class GymRouteScrollerDataBase{
    public bool isRoute;
    public bool isAddButton;
}
}