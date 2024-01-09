using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using UnityEngine.XR.Management;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;
using UnityEngine.Events;
using BepInEx.Logging;
using UnityEngineInternal.Input;
using BepInEx.Configuration;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.InputSystem.Layouts;
using Unity.XR.Oculus.Input;
using UnityEngine.Analytics;

namespace VSVRMod2;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class VSVRMod : BaseUnityPlugin
{
    public const string sessionScene = "ExtraLoadScene";
    public bool inSession = false;
    private VRGestureRecognizer vrGestureRecognizer = new VRGestureRecognizer();
    public static ManualLogSource logger;
    public static ConfigFile config;

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        logger = Logger;
        config = Config;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        
        VRConfig.SetupConfig();
        Controller.EnableControllerProfiles();
        InitializeXRRuntime();
        StartDisplay();

        InputDevices.deviceConnected += DevConnect;

        SceneManager.sceneLoaded += OnSceneLoaded;

        Logger.LogInfo("Reached end of Plugin.Awake()");
    }

    void DevConnect(InputDevice dev)
    {
        Logger.LogInfo("Device Connected: " + dev);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Controller.SetupControllers();
        Logger.LogInfo("A scene was loaded: " + scene.name);
        if (Equals(scene.name, sessionScene))
        {
            VRCamera.SetupCamera();
            VRCamera.SetupUI();
            Buttons.SetupChoiceButtons();
            Buttons.SetupRadialButtons();
            VRCamera.CenterCamera();

            vrGestureRecognizer.Nodded += Buttons.HeadMovementTracker.Nod;
            vrGestureRecognizer.HeadShaken += Buttons.HeadMovementTracker.Headshake;

            inSession = true;
        }
        else
        {
            inSession = false;
        }
    }

    void Update()
    {
        if (inSession)
        {
            vrGestureRecognizer.Update();
            Keyboard.HandleKeyboardInput();
            Buttons.RadialMenuInteract();
            VRCamera.ProcessHeadMovement();
        }
    }

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

        Logger.LogInfo("Initialized OpenXR Runtime");
    }

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

        Logger.LogInfo("Started XR Display subsystem, welcome to VR!");

        return true;
    }
}
