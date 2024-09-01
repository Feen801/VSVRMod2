using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using VSVRMod2.Helper;
using VSVRMod2.UI;

namespace VSVRMod2;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public class VSVRMod : BaseUnityPlugin
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public bool inSession = false;
    private static readonly VRGestureRecognizer vrGestureRecognizer = new();
    public static ManualLogSource logger;
    public static ConfigFile config;

    private static VRCameraManager vrCameraManager;
    private static UIContainer uiContainer;
    private static StartUIManager beginUiManager;

    public static readonly Controller.Headset controllerHeadset = new();

    private static bool noVR = false;

    public static VSVRMod instance;

    private static Scene sessionScene;

    private void Awake()
    {
        instance = this;
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        logger = Logger;
        config = Config;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        VRConfig.SetupConfig();

        ShortcutHelper.CreateShortcut();

        SceneManager.sceneLoaded += OnSceneLoaded;

        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Contains<string>("-novr")) {
            logger.LogWarning("VR disabled!");
            noVR = true;
            return;
        }

        Application.runInBackground = true;

        Controller.EnableControllerProfiles();
        InitializeXRRuntime();
        StartDisplay();

        InputDevices.deviceConnected += Controller.DeviceConnect;

        VSVRAssets.LoadAssets();

        beginUiManager = new StartUIManager();

        logger.LogInfo("Reached end of Plugin.Awake()");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddToDebugDisplay(noVR);
        if (noVR)
        {
            return;
        }
        Logger.LogInfo("A scene was loaded: " + scene.name);
        if (Equals(scene.name, Constants.SessionScene))
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            sessionScene = scene;
            InitialSessionSetup();
        }
        else
        {
            inSession = false;
            XRGeneralSettings.Instance.Manager.StopSubsystems();
        }
    }

    public void InitialSessionSetup()
    {
        if (vrCameraManager != null)
        {
            controllerHeadset.OnWorn -= vrCameraManager.SetupUI;
            controllerHeadset.OnRemoved -= vrCameraManager.RevertUI;
        }
        if (uiContainer != null)
        {
            vrGestureRecognizer.Nodded -= uiContainer.basicUIManager.headMovementTracker.Nod;
            vrGestureRecognizer.HeadShaken -= uiContainer.basicUIManager.headMovementTracker.Headshake;
        }
        logger.LogInfo("Starting session setup");
        vrCameraManager = new(sessionScene);
        logger.LogInfo("Session setup: created camera manager");
        uiContainer = new(sessionScene);
        logger.LogInfo("Session setup: created ui container");
        VSVRAssets.ApplyUIShader();
        logger.LogInfo("Session setup: applied ui shaders");

        vrGestureRecognizer.Nodded += uiContainer.basicUIManager.headMovementTracker.Nod;
        vrGestureRecognizer.HeadShaken += uiContainer.basicUIManager.headMovementTracker.Headshake;
        logger.LogInfo("Session setup: setup gestures");

        controllerHeadset.OnWorn += vrCameraManager.SetupUI;
        controllerHeadset.OnRemoved += vrCameraManager.RevertUI;
        logger.LogInfo("Session setup: OnWorn and OnRemoved");
        if (!VRConfig.taskGradient.Value)
        {
            vrCameraManager.DisableTaskGradient();
            logger.LogInfo("Session setup: Disabled task gradients");
        }
        inSession = true;
    }

    void FixedUpdate()
    {
        if (noVR)
        {
            return;
        }
        if (inSession)
        {
            if (VRConfig.useHeadMovement.Value)
            {
                vrGestureRecognizer.Update();
            }
            if (VRConfig.automaticScreenSwap.Value)
            {
                controllerHeadset.Update();
            }
            uiContainer.Interact();
            vrCameraManager.CameraControls();
            vrCameraManager.CenterCameraIfFar();
        }
        else
        {
            beginUiManager.Interact();
        }
        Controller.EndFrame();
    }

    void Update()
    {
        Keyboard.HandleKeyboardInput();
        if (noVR)
        {
            return;
        }
        if (inSession)
        {
            Keyboard.HandleKeyboardInputSession(vrCameraManager);
        }
    }

    void AddToDebugDisplay(bool isModDisabled)
    {
        if (GameObject.Find("VR Mod Version") != null)
        {
            return;
        }
        string fullString = Constants.VersionStringPrefix + " " + Constants.CurrentVersionString + (isModDisabled ? "\n(Disabled)" : "");
        GameObject version = GameObjectHelper.GetGameObjectCheckFound("DebugCanvas/Version");
        GameObject VRModversion = Instantiate(version);
        VRModversion.GetComponent<PlayMakerFSM>().enabled = false;
        VRModversion.name = "VR Mod Version";
        VRModversion.transform.position = version.transform.position + Vector3.down * 30;
        VRModversion.transform.parent = GameObjectHelper.GetGameObjectCheckFound("DebugCanvas").transform;
        VRModversion.GetComponent<TextMeshProUGUI>().SetText(fullString);
        logger.LogInfo("Added VR mod version to debug overlay");
    }

    /**
    * From https://github.com/DaXcess/LCVR (GPL-3.0 license)
    */
    private void InitializeXRRuntime()
    {
        // Set up the OpenXR loader
        var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        var xrLoader = ScriptableObject.CreateInstance<OpenXRLoader>();

        generalSettings.Manager = managerSettings;

        ((List<XRLoader>)managerSettings.activeLoaders).Clear();
        ((List<XRLoader>)managerSettings.activeLoaders).Add(xrLoader);

        OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.MultiPass;
        OpenXRSettings.Instance.depthSubmissionMode = OpenXRSettings.DepthSubmissionMode.Depth24Bit;

        typeof(XRGeneralSettings).GetMethod("InitXRSDK", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(generalSettings, []);
        typeof(XRGeneralSettings).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(generalSettings, []);

        logger.LogInfo("Initialized OpenXR Runtime");
    }

    /**
    * From https://github.com/DaXcess/LCVR (GPL-3.0 license)
    */
    private bool StartDisplay()
    {
        var displays = new List<XRDisplaySubsystem>();

#pragma warning disable CS0618 // Type or member is obsolete
        SubsystemManager.GetInstances(displays);
#pragma warning restore CS0618 // Type or member is obsolete

        if (displays.Count < 1)
        {
            return false;
        }

        displays[0].Start();

        logger.LogInfo("Started XR Display subsystem, welcome to VR!");

        return true;
    }
}
