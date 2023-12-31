using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace VSVRMod2;
public class VRCamera
{
    static GameObject vrCamera;
    static GameObject vrCameraParent;
    public static void SetupCamera()
    {
        GameObject worldCamDefault = GameObject.Find("WorldCamDefault");
        VSVRMod.logger.LogInfo("Creating VR camera...");
        vrCamera = new GameObject("VRCamera");
        vrCameraParent = new GameObject("VRCameraParent");
        VSVRMod.logger.LogInfo("Adding components to VR camera...");
        vrCamera.AddComponent<Camera>();
        vrCamera.AddComponent<TrackedPoseDriver>();
        VSVRMod.logger.LogInfo("Reparenting VR camera...");
        vrCamera.transform.SetParent(vrCameraParent.transform);
        vrCameraParent.transform.SetParent(worldCamDefault.transform);
    }

    public static void SetupUI()
    {

    }

    public static void CenterCamera()
    {
        if (vrCamera == null)
        {
            return;
        }
        vrCameraParent.transform.position = vrCamera.transform.position;
        vrCameraParent.transform.localPosition = -vrCamera.transform.localPosition;
        vrCameraParent.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
