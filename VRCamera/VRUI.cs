using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using UnityEngine.UIElements.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VSVRMod2.VRCamera
{
    public class VRUI
    {
        private static Canvas uiCanvas;
        private static Canvas overlayCanvas;
        private static Canvas scoreCanvas;
        private static Canvas fadeCanvas;
        private static GameObject greenscreenUI;
        private static GameObject overlay;
        private static GameObject ui;
        private static GameObject leftHand;
        private static GameObject rightHand;

        public static void Start(MoveableVRCamera vrcamera)
        {
            UIReferences();

            if (Controller.IsHeadsetWorn())
            {
                SetupUI(vrcamera);
            }

            SetupGreenscreen(vrcamera);
        }

        public static void MakeUIClose()
        {
            uiCanvas.planeDistance = VRConfig.uiDistance.Value;
            overlayCanvas.planeDistance = VRConfig.uiDistance.Value;
            scoreCanvas.planeDistance = VRConfig.uiDistance.Value;
            fadeCanvas.planeDistance = VRConfig.uiDistance.Value;
        }

        private static bool uiInVR = false;
        public static void ToggleUIVR(MoveableVRCamera vrcamera)
        {
            if (uiInVR)
            {
                RevertUI(vrcamera);
            }
            else
            {
                SetupUI(vrcamera);
            }
        }

        private static void UIReferences()
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

        private static List<GameObject> gameObjectsToUndo = new List<GameObject>();
        private static Dictionary<GameObject, Vector3> pastPositions = new Dictionary<GameObject, Vector3>();
        private static Dictionary<GameObject, Vector3> pastScales = new Dictionary<GameObject, Vector3>();
        private static float oldFavorHeight = 0;

        private static void SavePastPositionAndScale(GameObject gameObject)
        {
            gameObjectsToUndo.Add(gameObject);
            if (!pastPositions.TryAdd(gameObject, gameObject.transform.GetComponent<RectTransform>().anchoredPosition3D))
            {
                pastPositions[gameObject] = gameObject.transform.GetComponent<RectTransform>().anchoredPosition3D;
            }
            if (!pastScales.TryAdd(gameObject, gameObject.transform.GetComponent<RectTransform>().localScale))
            {
                pastScales[gameObject] = gameObject.transform.GetComponent<RectTransform>().localScale;
            }
        }
        public static void SetupUI(MoveableVRCamera vrcamera)
        {
            //UIReferences();
            gameObjectsToUndo.Clear();
            vrcamera.vrCamera.SetActive(true);
            vrcamera.SetupHeadTargetFollower(false);
            vrcamera.worldCamDefaultCamera.enabled = false;

            uiInVR = true;
            uiCanvas.sortingOrder = 400;
            uiCanvas.worldCamera = vrcamera.vrCamera.GetComponent<Camera>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            overlayCanvas.worldCamera = vrcamera.vrCamera.GetComponent<Camera>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            overlayCanvas.GetComponent<Canvas>().sortingOrder = 401;

            scoreCanvas.worldCamera = vrcamera.vrCamera.GetComponent<Camera>();
            scoreCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            fadeCanvas.sortingOrder = 399;
            fadeCanvas.worldCamera = vrcamera.vrCamera.GetComponent<Camera>();
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
            SavePastPositionAndScale(currentAdjust);
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
            SavePastPositionAndScale(currentAdjust);
            currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 1000, 0);
            currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            currentAdjust = ui.transform.Find("EventManager/TradeOfferUI").gameObject;
            if (currentAdjust == null)
            {
                VSVRMod.logger.LogError("TradeOfferUI not found");
            }
            else
            {
                VSVRMod.logger.LogInfo("TradeOfferUI found");
            }
            SavePastPositionAndScale(currentAdjust);
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
            SavePastPositionAndScale(currentAdjust);
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
            SavePastPositionAndScale(currentAdjust);
            currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 560, 0);
            currentAdjust.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
            currentAdjust.transform.localScale *= VRConfig.uiScale.Value;
            currentAdjust.transform.localPosition += new Vector3(0, VRConfig.uiHeightOffset.Value, 0);

            currentAdjust = overlay.transform.Find("YourStatusMenuManager").gameObject;
            if (currentAdjust == null)
            {
                VSVRMod.logger.LogError("YourStatusMenuManager not found");
            }
            else
            {
                VSVRMod.logger.LogInfo("YourStatusMenuManager found");
            }
            SavePastPositionAndScale(currentAdjust);
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
            SavePastPositionAndScale(currentAdjust);
            currentAdjust.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 750, 0);
            currentAdjust.GetComponent<RectTransform>().localScale = currentAdjust.GetComponent<RectTransform>().localScale * 1.2f;

            //Specific fix to favor
            currentAdjust = overlay.transform.Find("Favor").gameObject;
            if (currentAdjust == null)
            {
                VSVRMod.logger.LogError("Favor not found");
            }
            else
            {
                VSVRMod.logger.LogInfo("Favor found");
            }
            SetFloatValue setFloatValue = (SetFloatValue)(currentAdjust.gameObject.GetComponent<PlayMakerFSM>().FsmStates[1].Actions[0]);
            oldFavorHeight = setFloatValue.floatValue.ToFloat();
            setFloatValue.floatValue.Value = 1060;

            if (leftHand != null && rightHand != null)
            {
                leftHand.gameObject.SetActive(true);
                rightHand.gameObject.SetActive(true);
            }

            VSVRMod.logger.LogInfo("Adjusted UI for VR");
        }

        private static void UndoSetupPositioning()
        {
            UIReferences();
            for (int i = gameObjectsToUndo.Count - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjectsToUndo[i];
                if (Equals(gameObject.name, "EventManager"))
                {
                    gameObject.transform.localPosition -= new Vector3(0, VRConfig.uiHeightOffset.Value, 0);
                }
                gameObject.GetComponent<RectTransform>().localScale = pastScales.Get(gameObject);
                gameObject.GetComponent<RectTransform>().anchoredPosition3D = pastPositions.Get(gameObject);
            }
            SetFloatValue setFloatValue = (SetFloatValue)(overlay.transform.Find("Favor").gameObject.gameObject.GetComponent<PlayMakerFSM>().FsmStates[1].Actions[0]);
            setFloatValue.floatValue.Value = oldFavorHeight;
        }

        public static void RevertUI(MoveableVRCamera vrcamera)
        {
            UndoSetupPositioning();

            vrcamera.worldCamDefaultCamera.enabled = true;
            vrcamera.vrCamera.SetActive(false);
            vrcamera.SetupHeadTargetFollower(true);

            uiInVR = false;
            uiCanvas.worldCamera = vrcamera.primaryCamera.GetComponent<Camera>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            uiCanvas.planeDistance = 0.2f;

            overlayCanvas.worldCamera = vrcamera.primaryCamera.GetComponent<Camera>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            overlayCanvas.planeDistance = 0.2f;

            scoreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            fadeCanvas.worldCamera = null;
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (leftHand != null && rightHand != null)
            {
                leftHand.gameObject.SetActive(false);
                rightHand.gameObject.SetActive(false);
            }

            VSVRMod.logger.LogInfo("Adjusted UI for monitor");
        }

        public static void DisableTaskGradient()
        {
            GameObject eventManager = GameObjectHelper.GetGameObjectCheckFound("GeneralCanvas/EventManager");
            GameObject overlayCanvas = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas");

            List<GameObject> bgs = [];
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
                if (fsm != null)
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
        private static void SetupGreenscreen(MoveableVRCamera vrcamera)
        {
            if (VRConfig.greenscreenBackground.Value)
            {
                vrcamera.vrCamera.GetComponent<Camera>().backgroundColor = VRConfig.greenscreenColor.Value;
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

        public static void ToggleGreenscreenUI()
        {
            if (VRConfig.greenscreenUI.Value)
            {
                greenscreenUI.SetActive(!greenscreenUI.activeSelf);
            }
        }
    }
}
