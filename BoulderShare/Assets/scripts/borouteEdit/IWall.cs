using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IWall
{
    void SetWallImage(Texture2D tex);
    void SetIncline(int incline);
    void SetWallMarks(GameObject rootMarks, int n);
}
