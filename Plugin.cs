﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
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

        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Contains<string>("-novr")) {
            logger.LogWarning("VR disabled!");
            noVR = true;
            return;
        }

        Controller.EnableControllerProfiles();
        InitializeXRRuntime();
        StartDisplay();

        InputDevices.deviceConnected += Controller.DeviceConnect;

        SceneManager.sceneLoaded += OnSceneLoaded;

        VSVRAssets.LoadAssets();

        beginUiManager = new StartUIManager();

        logger.LogInfo("Reached end of Plugin.Awake()");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (noVR)
        {
            return;
        }
        Logger.LogInfo("A scene was loaded: " + scene.name);
        if (Equals(scene.name, Constants.SessionScene))
        {
            sessionScene = scene;
            InitialSessionSetup();
        }
        else
        {
            inSession = false;
        }
    }

    public void InitialSessionSetup()
    {
        vrCameraManager = new(sessionScene);
        uiContainer = new(sessionScene);
        VSVRAssets.ApplyUIShader();

        vrGestureRecognizer.Nodded += uiContainer.basicUIManager.headMovementTracker.Nod;
        vrGestureRecognizer.HeadShaken += uiContainer.basicUIManager.headMovementTracker.Headshake;

        controllerHeadset.OnWorn += vrCameraManager.SetupUI;
        controllerHeadset.OnRemoved += vrCameraManager.RevertUI;
        if (!VRConfig.taskGradient.Value)
        {
            vrCameraManager.DisableTaskGradient();
        }
        inSession = true;
    }

    void FixedUpdate()
    {
        if (noVR)
        {
            return;
        }
        Keyboard.HandleKeyboardInput();
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
            Keyboard.HandleKeyboardInputSession(vrCameraManager);
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
