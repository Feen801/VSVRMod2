using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

namespace VSVRMod2;
public class VRCameraManager
{
    private GameObject primaryCamera;
    private GameObject worldCamDefault;
    private Camera worldCamDefaultCamera;
    static GameObject vrCamera;
    static GameObject vrCameraDolly;
    static GameObject vrCameraParent;
    static GameObject vrCameraOffset;
    private GameObject headFollower;
    private Canvas uiCanvas;
    private Canvas overlayCanvas;
    private Canvas scoreCanvas;
    private Canvas fadeCanvas;
    private GameObject greenscreenUI;
    private GameObject overlay;
    private GameObject ui;

    private static bool isFirstUIAdjust = true;

    public VRCameraManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.SessionScene))
        {
            NewSetupCamera();
            UIReferences();

            //UI components may not properly be rescaled unless the VR camera is off at first.
            RevertUI();
            SetupUI();

            SetupGreenscreen();
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    private void NewSetupCamera()
    {
        worldCamDefault = GameObjectHelper.GetGameObjectCheckFound("WorldCamDefault");
        primaryCamera = GameObjectHelper.GetGameObjectCheckFound("PrimaryCamera");
        if( worldCamDefault == null ) 
        {
            VSVRMod.logger.LogInfo("WorldCamDefault may be disabled, doing fallback method.");
            worldCamDefault = primaryCamera.transform.Find("WorldCamDefault").gameObject;
        }
        worldCamDefaultCamera = worldCamDefault.GetComponent<Camera>();
        
        vrCameraParent = new GameObject("VRCameraParent");
        vrCameraParent.transform.SetParent(worldCamDefault.transform.root);
        vrCameraDolly = new GameObject("VRCameraParent");
        vrCameraDolly.transform.SetParent(vrCameraParent.transform);
        vrCameraOffset = new GameObject("VRCameraOffset");
        vrCameraOffset.transform.SetParent(vrCameraDolly.transform);
        

        headFollower = GameObjectHelper.GetGameObjectCheckFound("HeadTargetFollower");

        VSVRMod.logger.LogInfo("Creating VR camera...");
        vrCamera = new GameObject("VRCamera");
        VSVRMod.logger.LogInfo("Adding components to VR camera...");
        vrCamera.AddComponent<Camera>().nearClipPlane = 0.01f;
        vrCamera.AddComponent<TrackedPoseDriver>();
        float cameraScale = VRConfig.vrCameraScale.Value;
        vrCamera.transform.localScale = new Vector3(cameraScale, cameraScale, cameraScale);
        VSVRMod.logger.LogInfo("Reparenting VR camera...");
        vrCamera.transform.SetParent(vrCameraOffset.transform);

        PositionConstraint posConstraint = vrCameraParent.AddComponent<PositionConstraint>();
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = worldCamDefault.transform;
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
        GameObject cv = GameObjectHelper.GetGameObjectCheckFound("GeneralCanvas");
        if (VRConfig.greenscreenUI.Value)
        {
            greenscreenUI = new GameObject();
            greenscreenUI.AddComponent<RectTransform>();
            Image imageComponent = greenscreenUI.AddComponent<Image>();
            imageComponent.color = VRConfig.greenscreenColor.Value;
            greenscreenUI.transform.SetParent(cv.transform);
            greenscreenUI.transform.localScale = new Vector3(100, 100);
            greenscreenUI.transform.localPosition = Vector3.zero;
            greenscreenUI.SetActive(false);
            greenscreenUI.transform.SetAsFirstSibling();
        }
    }

    public void ToggleGreenscreenUI() {
        if(VRConfig.greenscreenUI.Value) {
            greenscreenUI.SetActive(!greenscreenUI.activeSelf);
        }
    }

    private void SetupHeadTargetFollower(Boolean revert)
    {
        if (revert)
        {
            headFollower.transform.SetParent(worldCamDefault.transform);
        }
        else
        {
            headFollower.transform.SetParent(vrCamera.transform);
        }
        PlayMakerFSM headResetter = headFollower.GetComponent<PlayMakerFSM>();
        headResetter.enabled = revert;
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

    private void UIReferences()
    {
        ui = GameObjectHelper.GetGameObjectCheckFound("GeneralCanvas");
        uiCanvas = ui.GetComponent<Canvas>();

        overlay = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas");
        overlayCanvas = overlay.GetComponent<Canvas>();

        GameObject score = GameObjectHelper.GetGameObjectCheckFound("ScoreCanvas");
        scoreCanvas = score.GetComponent<Canvas>();

        GameObject fade = GameObjectHelper.GetGameObjectCheckFound("FadeCanvas");
        fadeCanvas = fade.GetComponent<Canvas>();
    }

    public void SetupUI()
    {
        uiInVR = true;
        uiCanvas.sortingOrder = 400;
        uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        overlayCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        overlayCanvas.GetComponent<Canvas>().sortingOrder = 401;

        scoreCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        scoreCanvas.renderMode = RenderMode.ScreenSpaceCamera;

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

        /*        currentAdjust = ui.transform.Find("EventManager/ToyChecklist").gameObject;
                if (currentAdjust == null)
                {
                    VSVRMod.logger.LogError("ToyChecklist not found");
                }
                else
                {
                    VSVRMod.logger.LogInfo("ToyChecklist found");
                }
                currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 480, 0);
                currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 0.2f);*/

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
        if (isFirstUIAdjust) {
            currentAdjust.transform.localScale *= VRConfig.uiScale.Value;
            currentAdjust.transform.localPosition += new Vector3(0, VRConfig.uiHeightOffset.Value, 0);
        }

        currentAdjust = overlay.transform.Find("YourStatusMenuManager").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("YourStatusMenuManager not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("YourStatusMenuManager found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(340, -1100, 0);
        currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);

        currentAdjust = ui.transform.Find("EventManager/ASMROverlay").gameObject;
        if (currentAdjust == null)
        {
            VSVRMod.logger.LogError("ASMROverlay not found");
        }
        else
        {
            VSVRMod.logger.LogInfo("ASMROverlay found");
        }
        currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 750, 0);
        if(isFirstUIAdjust)
        {
            currentAdjust.GetComponent<RectTransform>().localScale = currentAdjust.GetComponent<RectTransform>().localScale * 1.2f;
        }

        vrCamera.SetActive(true);
        SetupHeadTargetFollower(false);
        worldCamDefaultCamera.enabled = false;
        isFirstUIAdjust = false;
        VSVRMod.logger.LogError("Adjusted UI for VR");
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

        worldCamDefaultCamera.enabled = true;
        vrCamera.SetActive(false);
        SetupHeadTargetFollower(true);

        VSVRMod.logger.LogInfo("Adjusted UI for monitor");
    }

    public void DisableTaskGradient()
    {
        GameObject eventManager = GameObjectHelper.GetGameObjectCheckFound("GeneralCanvas/EventManager");
        GameObject overlayCanvas = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas");

        List<GameObject> bgs= [];
        GameObjectHelper.AddChecked(bgs, "InstructionBorder/Background", eventManager);
        GameObjectHelper.AddChecked(bgs, "InstructionBorder/Background (1)", eventManager);
        GameObjectHelper.AddChecked(bgs, "TimedEvent/Background", eventManager);
        GameObjectHelper.AddChecked(bgs, "TimedEvent/Background (3)", eventManager);
        GameObjectHelper.AddChecked(bgs, "StrokeCount/Background", eventManager);
        GameObjectHelper.AddChecked(bgs, "StrokeCount/Background (1)", eventManager);
        GameObjectHelper.AddChecked(bgs, "TaskDisliked/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "Forgive/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "FavorCosts/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "EdgeLockout/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "IntensifyTask/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "ToggleDiffIncrease/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "ToggleDiffDecrease/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyTask/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyRefuse/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyEdgeLockout/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyEdgeCost/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyMood/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "MercyRefuseDispleased/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "TauntDispleased/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "TauntPunish/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "TauntDevious/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "BeatManager2/Background", eventManager);
        GameObjectHelper.AddChecked(bgs, "BeatManager2/Background", eventManager);

        GameObjectHelper.AddChecked(bgs, "FinDom/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "Chastity/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "FavorFail/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "FinDomFail/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "FinDomAccept/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "SettingsChange/Bubble", eventManager);
        GameObjectHelper.AddChecked(bgs, "HCChastityDouble/Bubble", eventManager);

        GameObjectHelper.AddChecked(bgs, "Urges/ActionTextContainer/Image (1)", eventManager);

        foreach (GameObject bg in bgs)
        {
            bg.SetActive(false);
            PlayMakerFSM fsm = bg.GetComponent<PlayMakerFSM>();
            if(fsm != null)
            {
                fsm.enabled = false;
            }
            Image img = bg.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false;
            }
        }
    }

    public void CenterCamera()
    {
        if (vrCamera == null || VRConfig.fixCameraHeight.Value)
        {
            return;
        }
        vrCameraOffset.transform.position = vrCamera.transform.position;
        vrCameraOffset.transform.localPosition = -vrCamera.transform.localPosition;
        vrCameraOffset.transform.localEulerAngles = new Vector3(0, -vrCamera.transform.localEulerAngles.y, 0);
    }

    private bool didRecenter = false;

    public void CenterCameraIfFar() 
    {
        if (didRecenter)
        {
            return;
        }
        Vector3 distanceVector = worldCamDefault.transform.position - vrCamera.transform.position;
        double distance = distanceVector.sqrMagnitude;
        //VSVRMod.logger.LogWarning(distance);
        if(distance > 0.1)
        {
            didRecenter = true;
            CenterCamera();
        }
    }

    public void CameraControls()
    {
        int gripCount = Controller.CountGripsPressed();
        if (gripCount == 2)
        {
            this.CenterCamera();
        }
        else if (gripCount == 1)
        {
            float speed = Controller.GetMaximalJoystickValue().y;
            vrCameraDolly.transform.localPosition += Vector3.forward * speed * Time.fixedDeltaTime;
        }
        if (Controller.WasAGripClickedQuickly())
        {
            this.ToggleGreenscreenUI();
        }
    }
}
