using UnityEngine;
using UnityEngine.SpatialTracking;

namespace VSVRMod2;
public class VRCamera
{
    //public static GameObject root;
    private static GameObject primaryCamera;
    static GameObject vrCamera;
    static GameObject vrCameraParent;
    //static GameObject vrUICamera;
    private static GameObject headFollower;
    private static GameObject eyeFollower;
    private static Canvas uiCanvas;
    private static Canvas overlayCanvas;
    private static Canvas scoreCanvas;
    private static Canvas fadeCanvas;

    public static void SetupCamera()
    {
        GameObject worldCamDefault = GetGameObjectCheckFound("WorldCamDefault");
        primaryCamera = GetGameObjectCheckFound("PrimaryCamera");
        vrCameraParent = new GameObject("VRCameraParent");
        vrCameraParent.transform.SetParent(worldCamDefault.transform);
        
        VSVRMod.logger.LogInfo("Creating VR camera...");
        vrCamera = new GameObject("VRCamera");
        VSVRMod.logger.LogInfo("Adding components to VR camera...");
        vrCamera.AddComponent<Camera>().nearClipPlane = 0.01f;
        vrCamera.AddComponent<TrackedPoseDriver>();
        float cameraScale = VRConfig.vrCameraScale.Value;
        vrCamera.transform.localScale = new Vector3(cameraScale, cameraScale, cameraScale);
        VSVRMod.logger.LogInfo("Reparenting VR camera...");
        vrCamera.transform.SetParent(vrCameraParent.transform);

        headFollower = GetGameObjectCheckFound("HeadTargetFollower");
        headFollower.transform.SetParent(vrCamera.transform);
        PlayMakerFSM headResetter = headFollower.GetComponent<PlayMakerFSM>();
        headResetter.enabled = false;
        headFollower.transform.localPosition = new Vector3(0, 0, 0);
        headFollower.transform.localRotation = new Quaternion(0, 0, 0, 0);

        //Tried to make eyes also follow the vr camera, but it is complicated. May revist
        /**
        eyeFollower = GameObject.Find("EyeTarget");
        //eyeFollower.transform.SetParent(vrCamera.transform);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(eyeFollower.transform);
        cube.transform.localScale = Vector3.one * 0.1f;
        PlayMakerFSM eyeResetter = eyeFollower.GetComponent<PlayMakerFSM>();
        PlayMakerLateUpdate eyeResetter2 = eyeFollower.GetComponent<PlayMakerLateUpdate>();
        eyeResetter.enabled = false;
        eyeResetter2.enabled = false;
        //eyeFollower.transform.localPosition = new Vector3(0, 0, 0);
        //eyeFollower.transform.localRotation = new Quaternion(0, 0, 0, 0);
        cube.transform.localPosition = new Vector3(0, 0, 0);
        cube.transform.localRotation = new Quaternion(0, 0, 0, 0);
        */
    }
    public static void MakeUIClose(bool close)
    {
        if(close)
        {
            uiCanvas.planeDistance = 0.2f;
            overlayCanvas.planeDistance = 0.18f;
            scoreCanvas.planeDistance = 0.16f;
            fadeCanvas.planeDistance = 0.1f;
        }
        else
        {
            uiCanvas.planeDistance = 0.5f;
            overlayCanvas.planeDistance = 0.45f;
            scoreCanvas.planeDistance = 0.4f;
            fadeCanvas.planeDistance = 0.1f;
        }
    }

    private static Vector3 ProjectPointOntoPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
    {
        Vector3 vectorToProject = point - planePoint;

        float distance = Vector3.Dot(vectorToProject, planeNormal) / planeNormal.sqrMagnitude;

        Vector3 projectedPoint = point - distance * planeNormal;

        return projectedPoint;
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
        GameObject ui = GetGameObjectCheckFound("GeneralCanvas");
        uiCanvas = ui.GetComponent<Canvas>();
        uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        GameObject overlay = GetGameObjectCheckFound("OverlayCanvas");
        overlayCanvas = overlay.GetComponent<Canvas>();
        overlayCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        overlayCanvas.GetComponent<Canvas>().sortingOrder = 9;

        GameObject score = GetGameObjectCheckFound("ScoreCanvas");
        scoreCanvas = score.GetComponent<Canvas>();
        scoreCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        scoreCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        GameObject fade = GetGameObjectCheckFound("FadeCanvas");
        fadeCanvas = fade.GetComponent<Canvas>();
        fadeCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        MakeUIClose(false);

        Transform hypnoSpinPlayer = overlay.transform.Find("HypnoSpinPlayer");
        hypnoSpinPlayer.rotation = new Quaternion(0, 0, 0, 0);

        GameObject currentAdjust = overlay.transform.Find("TributeMenu").gameObject;
        if(currentAdjust == null)
        {
            VSVRMod.logger.LogError("TributeMenu not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("TributeMenu found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 300, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);

        currentAdjust = ui.transform.Find("EventManager/SpinWheelUI").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("SpinWheelUI not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("SpinWheelUI found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 1000, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        currentAdjust = ui.transform.Find("EventManager/ToyChecklist").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("ToyChecklist not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("ToyChecklist found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 480, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 0.2f);

        currentAdjust = ui.transform.Find("EventManager/TradeOfferUI").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("TradeOfferUI not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("TradeOfferUI found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 1240, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);

        currentAdjust = ui.transform.Find("EventManager/Urges").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("Urges not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("Urges found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 1000, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        currentAdjust = ui.transform.Find("EventManager").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("EventManager not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("EventManager found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 560, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
        vrCamera.SetActive(true);
    }

    private static GameObject GetGameObjectCheckFound(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            VSVRMod.logger.LogError(path + " gameobject not found");
        }
        return go;
    }

    public static void RevertUI()
    {
        uiInVR = false;
        uiCanvas.worldCamera = primaryCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.planeDistance = 0.2f;

        overlayCanvas.worldCamera = primaryCamera.GetComponent<Camera>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        overlayCanvas.planeDistance = 0.2f;

        scoreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        fadeCanvas.worldCamera = null;
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

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
