using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using VSVRMod2.Helper;
using VSVRMod2.UI;
using VSVRMod2.UI.SpecifcUI;
using Newtonsoft.Json;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace VSVRMod2;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public class VSVRMod : BaseUnityPlugin
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public volatile bool inSession = false;
    private static readonly VRGestureRecognizer vrGestureRecognizer = new();
    public static ManualLogSource logger;
    public static ConfigFile config;

    private static VRCameraManager vrCameraManager;
    private static UIContainer uiContainer;
    private static StartUIManager beginUiManager;

    public static readonly Controller.Headset controllerHeadset = new();

    public static bool noVR = false;

    public static VSVRMod instance;

    private static Scene sessionScene;

    private VoiceProcessor voiceProcessor;
    public static VoskSpeechRecognizer speechRecognizer;

    private bool voskLoaded = false;

    async private void Awake()
    {
        instance = this;
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        logger = Logger;
        config = Config;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        VRConfig.SetupConfig();

        if (VRConfig.enableSpeechRecognition.Value)
        {
            voiceProcessor = new VoiceProcessor(
                microphoneIndex: 0,
                minimumSpeakingSampleValue: 0.03f,
                silenceTimer: 1.0f,
                autoDetect: false
            );
            speechRecognizer = new VoskSpeechRecognizer(
                modelPath: "vosk-model-small-en-us-0.15.zip",
                voiceProcessor: voiceProcessor,
                keyPhrases: null,
                maxAlternatives: 3
            );

            speechRecognizer.OnStatusUpdated += (status) => logger.LogInfo($"VOSK: {status}");

            speechRecognizer.OnTranscriptionResult += (result) =>
            {
                try
                {
                    if (inSession)
                    {
                        var json = JsonConvert.DeserializeObject<TranscriptionResult>(result);
                        foreach (var alt in json.alternatives)
                        {
                            string alttext = alt.text.Trim();
                            if (alttext.Length > 0)
                            {
                                logger.LogMessage("Speech Detected: " + alttext);
                                if (uiContainer.VoiceInteract(alttext))
                                    break;
                            }
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    logger.LogError($"Voice Error: {ex.Message}\n{ex.StackTrace}");
                }
            };
        }

        ShortcutHelper.CreateShortcut();

        SceneManager.sceneLoaded += OnSceneLoaded;

        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Contains<string>("-novr")) {
            logger.LogWarning("VR disabled!");
            noVR = true;
        }
        else
        {
            Application.runInBackground = true;

            Controller.EnableControllerProfiles();
            InitializeXRRuntime();
            StartDisplay();

            InputDevices.deviceConnected += Controller.DeviceConnect;

            VSVRAssets.LoadAssets();

            beginUiManager = new StartUIManager();

            logger.LogInfo("Reached end of Plugin.Awake()");
        }

        if (VRConfig.enableSpeechRecognition.Value) 
        {
            await speechRecognizer.InitializeAsync();
            speechRecognizer.StartRecording();
            voskLoaded = true;

            logger.LogInfo("Finished loading VOSK");
        }
    }

    void OnDestroy()
    {
        CleanupSpeech();
    }

    void OnEmergencyStop(string detectedText)
    {
        Debug.Log("EMERGENCY: " + detectedText);
    }

    void OnPauseGame(string detectedText)
    {
        Debug.Log("Pausing: " + detectedText);
    }

    void OnOpenMenu(string detectedText)
    {
        Debug.Log("Menu: " + detectedText);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddToDebugDisplay(noVR);
        if (noVR)
        {
            if (Equals(scene.name, Constants.SessionStartScene))
            {
                sessionScene = scene;
                inSession = true;
                InitialSessionSetup();
            }
            return;
        }
        Logger.LogInfo("A scene was loaded: " + scene.name);
        if (Equals(scene.name, Constants.SessionStartScene))
        {
            sessionScene = scene;
            inSession = true;
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            InitialSessionSetupVR();
        }
        else
        {
            inSession = false;
            XRGeneralSettings.Instance.Manager.StopSubsystems();
        }
    }

    public void InitialSessionSetupVR()
    {
        logger.LogInfo("Starting session setup");
        SessionBugfixes.FixAll();
        //Do this before the camera manager!
        VSVRAssets.ApplyUIShader();
        logger.LogInfo("Session setup: applied ui shaders");
        uiContainer = new(sessionScene);
        logger.LogInfo("Session setup: created ui container");
        vrCameraManager = new(sessionScene);
        logger.LogInfo("Session setup: created camera manager");
        vrGestureRecognizer.Nodded += uiContainer.basicUIManager.headMovementTracker.Nod;
        vrGestureRecognizer.HeadShaken += uiContainer.basicUIManager.headMovementTracker.Headshake;
        logger.LogInfo("Session setup: setup gestures");
    }

    public void InitialSessionSetup()
    {
        logger.LogInfo("Starting session setup");
        SessionBugfixes.FixAll();
        uiContainer = new(sessionScene);
        logger.LogInfo("Session setup: created ui container");
    }

    void FixedUpdate()
    {
        if (voskLoaded)
        {
            try
            {
                voiceProcessor?.ProcessAudioFrame();
                speechRecognizer?.Update();
            }
            catch (Exception ex)
            {
                logger?.LogError($"Voice processing error: {ex.Message}");
            }
        }
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
            vrCameraManager.Update();
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
            vrCameraManager.OnPreRender();
        }
    }

    void LateUpdate()
    {
        if (noVR)
        {
            return;
        }
        if (inSession)
        {
            vrCameraManager.LateUpdate();
        }
    }

    void OnPreRender()
    {
        
    }

    void OnApplicationQuit()
    {
        CleanupSpeech();
    }

    void CleanupSpeech()
    {
        if (speechRecognizer != null) speechRecognizer?.Dispose();
        if (voiceProcessor != null) voiceProcessor?.Dispose();
    }
       

    void AddToDebugDisplay(bool isModDisabled)
    {
        if (GameObject.Find("VR Mod Version") != null)
        {
            return;
        }
        string fullString = Constants.VersionStringPrefix + " " + Constants.CurrentVersionString + (isModDisabled ? "\n(VR Disabled)" : "");
        GameObject version = GameObjectHelper.GetGameObjectCheckFound("DebugCanvas/Version");
        if (version == null)
        {
            logger.LogInfo("Did not add VR mod version to debug overlay?");
            return;
        }
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

        //SinglePassInstanced does not work with this game, I may try to fix it one day though.
        if (true)
        {
            OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.MultiPass;
        }
        else
        {
            OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
        }
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

[Serializable]
public class TranscriptionResult
{
    [SerializeField]
    public Alternative[] alternatives;
}

[Serializable]
public class Alternative
{
    [SerializeField]
    public float confidence;
    [SerializeField]
    public string text;
}
