using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class GymRouteScrollerData : GymRouteScrollerDataBase
{
    public string routeID;
    public List<string> wallImages;
    public List<string> tags;
    public int clearRate;
    public BNRoute.ClearStatus clearStatus;
    public bool isFavorite;
    public string period;
    public BNGradeMap.Grade grade;
    public bool isFinished;
    public RTape routeTape;
    
}
public class GymRouteScrollerDataBase{
}
}