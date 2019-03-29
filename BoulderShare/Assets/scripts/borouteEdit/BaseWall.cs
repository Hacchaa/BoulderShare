using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWall : MonoBehaviour, IWall
{
    public abstract void SetWallImage(Texture2D tex);
    public abstract void SetIncline(int incline);
    public abstract void SetWallMarks(GameObject rootMarks, int n);
}
