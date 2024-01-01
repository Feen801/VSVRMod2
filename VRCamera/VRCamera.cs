using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace VSVRMod2;
public class VRCamera
{
    private static GameObject primaryCamera;
    static GameObject vrCamera;
    static GameObject vrCameraParent;
    public static void SetupCamera()
    {
        primaryCamera = GameObject.Find("PrimaryCamera");

        GameObject worldCamDefault = GameObject.Find("WorldCamDefault");
        VSVRMod.logger.LogInfo("Creating VR camera...");
        vrCamera = new GameObject("VRCamera");
        vrCameraParent = new GameObject("VRCameraParent");
        VSVRMod.logger.LogInfo("Adding components to VR camera...");
        vrCamera.AddComponent<Camera>().nearClipPlane = 0.01f;
        vrCamera.AddComponent<TrackedPoseDriver>();
        VSVRMod.logger.LogInfo("Reparenting VR camera...");
        vrCamera.transform.SetParent(vrCameraParent.transform);
        vrCameraParent.transform.SetParent(worldCamDefault.transform);

        GameObject headFollower = GameObject.Find("HeadTargetFollower");
        headFollower.transform.SetParent(vrCamera.transform);
        PlayMakerFSM headResetter = headFollower.GetComponent<PlayMakerFSM>();
        headResetter.enabled = false;
        headFollower.transform.localPosition = new Vector3(0, 0, 0);
        headFollower.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    private static bool uiInVR = false;

    public static void ToggleUIVR()
    {
        if (uiInVR) {
            RevertUI();
        }
        else
        {
            SetupUI();
        }
    }

    public static void SetupUI()
    {
        uiInVR = true;
        GameObject ui = GameObject.Find("GeneralCanvas");
        Canvas uiCanvas = ui.GetComponent<Canvas>();
        uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.planeDistance = 0.5f;

        GameObject uiEvent = GameObject.Find("GeneralCanvas/EventManager");
        uiEvent.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 560, 0);
        uiEvent.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
        vrCamera.SetActive(true);
    }

    public static void RevertUI()
    {
        uiInVR = false;
        GameObject ui = GameObject.Find("GeneralCanvas");
        Canvas uiCanvas = ui.GetComponent<Canvas>();
        uiCanvas.worldCamera = primaryCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.planeDistance = 0.2f;

        GameObject uiEvent = GameObject.Find("GeneralCanvas/EventManager");
        uiEvent.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 100, 0);
        uiEvent.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
        vrCamera.SetActive(false);
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
