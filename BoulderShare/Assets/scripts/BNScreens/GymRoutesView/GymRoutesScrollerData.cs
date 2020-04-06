using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class GymRoutesScrollerData
{
    public string routeID;
    public string wallImagePath;
    public bool isFavorite;
    public bool isFinished;
    public string period;
    public BNGradeMap.Grade grade;
    public int clearRate;
    public BNRoute.ClearStatus clearStatus;
    public string wallTypeName;
    public RTape routeTape;
}
}