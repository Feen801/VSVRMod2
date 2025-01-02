using System;
using UnityEngine;

namespace VSVRMod2;

/**
 * Partially from https://github.com/korinVR/VRGestureRecognizer (Public Domain)
 */

public static class MathHelper
{
    public static float LinearMap(float value, float s0, float s1, float d0, float d1)
    {
        return d0 + (value - s0) * (d1 - d0) / (s1 - s0);
    }

    public static float WrapDegree(float degree)
    {
        if (degree > 180f)
        {
            return degree - 360f;
        }
        return degree;
    }

    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

    private static Vector3 ProjectPointOntoPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
    {
        Vector3 vectorToProject = point - planePoint;
        float distance = Vector3.Dot(vectorToProject, planeNormal) / planeNormal.sqrMagnitude;
        Vector3 projectedPoint = point - distance * planeNormal;
        return projectedPoint;
    }
}