using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation;

public static class SA_Extensions_Bounds
{
    public static Vector3 GetVertex(this Bounds bounds, SA_VertexX x, SA_VertexY y, SA_VertexZ z)
    {
        var center = bounds.center;

        switch (x)
        {
            case SA_VertexX.Right:
                center.x -= bounds.extents.x;
                break;
            case SA_VertexX.Left:
                center.x += bounds.extents.x;
                break;
        }

        switch (y)
        {
            case SA_VertexY.Bottom:
                center.y -= bounds.extents.y;
                break;

            case SA_VertexY.Top:
                center.y += bounds.extents.y;
                break;
        }

        switch (z)
        {
            case SA_VertexZ.Back:
                center.z -= bounds.extents.z;
                break;

            case SA_VertexZ.Front:
                center.z += bounds.extents.z;
                break;
        }

        return center;
    }

    public static Bounds CalculateBounds(GameObject obj)
    {
        var hasBounds = false;
        var Bounds = new Bounds(Vector3.zero, Vector3.zero);
        var ChildrenRenderer = obj.GetComponentsInChildren<Renderer>();

        //Quaternion oldRotation = obj.transform.rotation;
        //obj.transform.rotation = Quaternion.identity;

        var rnd = obj.GetComponent<Renderer>();
        if (rnd != null)
        {
            Bounds = rnd.bounds;
            hasBounds = true;
        }

        foreach (var child in ChildrenRenderer)
            if (!hasBounds)
            {
                Bounds = child.bounds;
                hasBounds = true;
            }
            else
            {
                Bounds.Encapsulate(child.bounds);
            }

        //obj.transform.rotation = oldRotation;

        return Bounds;
    }
}
