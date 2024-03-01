using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;

namespace VSVRMod2;
public class VRCameraManager
{
    //public static GameObject root;
    private GameObject primaryCamera;
    static GameObject vrCamera;
    static GameObject vrCameraParent;
    //static GameObject vrUICamera;
    private GameObject headFollower;
    //private GameObject eyeFollower;
    private Canvas uiCanvas;
    private Canvas overlayCanvas;
    private Canvas scoreCanvas;
    private Canvas fadeCanvas;

    public VRCameraManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            NewSetupCamera();
            SetupUI();
            SetupGreenscreen();
            SetupHeadTargetFollower();
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    private void NewSetupCamera()
    {
        GameObject worldCamDefault = GameObjectHelper.GetGameObjectCheckFound("WorldCamDefault");
        vrCameraParent = new GameObject("VRCameraParent");
        vrCameraParent.transform.SetParent(worldCamDefault.transform.root);

        VSVRMod.logger.LogInfo("Creating VR camera...");
        vrCamera = new GameObject("VRCamera");
        VSVRMod.logger.LogInfo("Adding components to VR camera...");
        vrCamera.AddComponent<Camera>().nearClipPlane = 0.01f;
        vrCamera.AddComponent<TrackedPoseDriver>();
        float cameraScale = VRConfig.vrCameraScale.Value;
        vrCamera.transform.localScale = new Vector3(cameraScale, cameraScale, cameraScale);
        VSVRMod.logger.LogInfo("Reparenting VR camera...");
        vrCamera.transform.SetParent(vrCameraParent.transform);

        PositionConstraint posConstraint = vrCameraParent.AddComponent<PositionConstraint>();
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = vrCameraParent.transform;
        constraintSource.weight = 1.0f;

        posConstraint.AddSource(constraintSource);

        VSVRMod.logger.LogInfo("Constrained VR camera position.");

        if (VRConfig.fixCameraHeight.Value)
        {
            posConstraint.translationAxis = Axis.X | Axis.Z;
            vrCameraParent.transform.position.Set(vrCameraParent.transform.position.x, 0f, vrCameraParent.transform.position.z);
        }
        else
        {
            posConstraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
        }
        posConstraint.translationOffset = Vector3.zero;
        posConstraint.constraintActive = true;

        RotationConstraint rotConstraint = vrCameraParent.AddComponent<RotationConstraint>();

        rotConstraint.AddSource(constraintSource);

        if (VRConfig.fixCameraAngle.Value)
        {
            rotConstraint.rotationAxis = Axis.Y;
        }
        else
        {
            rotConstraint.rotationAxis = Axis.X | Axis.Y | Axis.Z;
        }
        rotConstraint.rotationOffset = Vector3.zero;
        rotConstraint.constraintActive = true;

        VSVRMod.logger.LogInfo("Constrained VR camera rotation.");
    }

    private void SetupGreenscreen()
    {
        if (VRConfig.greenscreenBackground.Value)
        {
            vrCamera.GetComponent<Camera>().backgroundColor = VRConfig.greenscreenColor.Value;
        }
        GameObject fade = GameObjectHelper.GetGameObjectCheckFound("FadeCanvas");
        if (VRConfig.greenscreenUI.Value)
        {
            GameObject greenscreenUI = new GameObject();
            SpriteRenderer spriteRenderer = greenscreenUI.AddComponent<SpriteRenderer>();
            spriteRenderer.color = VRConfig.greenscreenColor.Value;
            spriteRenderer.sortingOrder = 99;
            greenscreenUI.transform.SetParent(fade.transform);
            greenscreenUI.transform.localScale = new Vector3(100, 100);
        }
    }
    private void SetupHeadTargetFollower()
    {
        headFollower = GameObjectHelper.GetGameObjectCheckFound("HeadTargetFollower");
        headFollower.transform.SetParent(vrCamera.transform);
        PlayMakerFSM headResetter = headFollower.GetComponent<PlayMakerFSM>();
        headResetter.enabled = false;
        headFollower.transform.localPosition = new Vector3(0, 0, 0);
        headFollower.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public void MakeUIClose()
    {
        uiCanvas.planeDistance = VRConfig.uiDistance.Value;
        overlayCanvas.planeDistance = VRConfig.uiDistance.Value;
        scoreCanvas.planeDistance = VRConfig.uiDistance.Value;
        fadeCanvas.planeDistance = VRConfig.uiDistance.Value;
    }

    private static Vector3 ProjectPointOntoPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
    {
        Vector3 vectorToProject = point - planePoint;

        float distance = Vector3.Dot(vectorToProject, planeNormal) / planeNormal.sqrMagnitude;

        Vector3 projectedPoint = point - distance * planeNormal;

        return projectedPoint;
    }

    private bool uiInVR = false;
    public void ToggleUIVR()
    {
        if (uiInVR)
        {
            RevertUI();
        }
        else
        {
            SetupUI();
        }
    }

    private void SetupUI()
    {
        uiInVR = true;
        GameObject ui = GameObjectHelper.GetGameObjectCheckFound("GeneralCanvas");
        uiCanvas = ui.GetComponent<Canvas>();
        uiCanvas.sortingOrder = 400;
        uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        GameObject overlay = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas");
        overlayCanvas = overlay.GetComponent<Canvas>();
        overlayCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        overlayCanvas.GetComponent<Canvas>().sortingOrder = 401;

        GameObject score = GameObjectHelper.GetGameObjectCheckFound("ScoreCanvas");
        scoreCanvas = score.GetComponent<Canvas>();
        scoreCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        scoreCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        GameObject fade = GameObjectHelper.GetGameObjectCheckFound("FadeCanvas");
        fadeCanvas = fade.GetComponent<Canvas>();
        fadeCanvas.sortingOrder = 399;
        fadeCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        MakeUIClose();

        Transform hypnoSpinPlayer = overlay.transform.Find("HypnoSpinPlayer");
        hypnoSpinPlayer.rotation = new Quaternion(0, 0, 0, 0);

        GameObject currentAdjust = overlay.transform.Find("TributeMenu").gameObject;
        if (currentAdjust == null)
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

    public void RevertUI()
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

    public void CenterCamera()
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
