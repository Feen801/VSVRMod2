using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace VSVRMod2;
public class VRCamera
{
    public static void SetupCamera()
    {
        GameObject worldCamDefault = GameObject.Find("WorldCamDefault");
        GameObject vrCamera = new GameObject("VRCamera");
        vrCamera.AddComponent<Camera>();
        vrCamera.AddComponent<TrackedPoseDriver>();
        vrCamera.transform.SetParent(worldCamDefault.transform);
    }
}
