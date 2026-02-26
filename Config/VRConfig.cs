using BepInEx.Configuration;
using UnityEngine;

namespace VSVRMod2;

public class VRConfig
{
    //UI settings
    public static ConfigEntry<bool> showButtonPrompts;
    public static ConfigEntry<float> uiDistance;
    public static ConfigEntry<bool> taskGradient;
    public static ConfigEntry<float> uiScale;
    public static ConfigEntry<float> uiHeightOffset;
    public static ConfigEntry<float> uiPosSmoothing;
    public static ConfigEntry<float> uiRotSmoothing;

    //Controls settings
    public static ConfigEntry<bool> useHeadMovement;
    public static ConfigEntry<int> yesThreshold;
    public static ConfigEntry<int> noThreshold;
    public static ConfigEntry<bool> automaticScreenSwap;
    public static ConfigEntry<bool> visibleControllers;

    //Camera settings
    public static ConfigEntry<float> vrCameraScale;
    public static ConfigEntry<bool> fixCameraHeight;
    public static ConfigEntry<bool> fixCameraAngle;
    public static ConfigEntry<bool> useMultipassRendering;

    //Greenscreen settings
    public static ConfigEntry<bool> greenscreenBackground;
    public static ConfigEntry<Color> greenscreenColor;
    public static ConfigEntry<bool> greenscreenUI;

    //Voice control settings
    public static ConfigEntry<bool> enableSpeechRecognition;
    public static ConfigEntry<bool> basicButtonSpeech;
    public static ConfigEntry<bool> urgeSpeech;
    public static ConfigEntry<bool> urgeAnswer;
    public static ConfigEntry<bool> yesAndNo;
    public static ConfigEntry<string> safeword;

    //Gameplay settings
    public static ConfigEntry<float> urgeSeconds;

    //Bugfixes
    public static ConfigEntry<bool> fixMissingPreText;

    public static void SetupConfig()
    {
        showButtonPrompts = VSVRMod.config.Bind("UI", "Show Button Prompts", true, "Show images beside various UI elements demonstrating how to interact with them using VR controllers.");
        uiDistance = VSVRMod.config.Bind("UI", "UI Distance", 0.5f, "How far away the UI appears from your face in meters.\n" +
            "The relative size will not change, i.e. if you make it closer to you it will also become smaller such that it takes up the same FOV.");
        taskGradient = VSVRMod.config.Bind("UI", "Task Gradient", true, "Enable or disable the semitransparent gradient behind the task text. May not work in all circumstances.");
        uiScale = VSVRMod.config.Bind("UI", "UI Scale", 1.0f, "Scale the overall size of the UI up or down." +
            "\nLowering this could be helpful if some UI elements are difficult to see in your headset due to limited FOV.");
        uiHeightOffset = VSVRMod.config.Bind("UI", "UI Height Offset", 0f, "Vertically moves the UI. 10 units corresponds to about 1 degree in VR." +
            "\nChanging this could be helpful if some UI elements are difficult to see in your headset due to limited FOV.");
        uiPosSmoothing = VSVRMod.config.Bind("UI", "UI Position Smoothing", 0.08f, "How slowly the UI snaps to the position you are looking. LOWER is snappier.");
        uiRotSmoothing = VSVRMod.config.Bind("UI", "UI Rotation Smoothing", 10.0f, "How slowly the UI snaps to the rotation you are looking. HIGHER is snappier.");

        useHeadMovement = VSVRMod.config.Bind("Controls", "Use Head Movement", true, "Enable or disable the ability to press buttons by nodding or shaking your head.");
        yesThreshold = VSVRMod.config.Bind("Controls", "Yes Threshold", 2, "How many vertical movements of your head trigger the left option. 2 means up then down (or vice versa)");
        noThreshold = VSVRMod.config.Bind("Controls", "No Threshold", 3, "How many horizontal movements of your head trigger the right option. 3 is recommended since headshakes are more senestive than nods.");
        automaticScreenSwap = VSVRMod.config.Bind("Controls", "Automatic Screen Swap", true, "Automatically swap the game output to your monitor when your headset is removed. Only works on Oculus and WMR headsets.");
        visibleControllers = VSVRMod.config.Bind("Controls", "Visible Controllers", true, "Places a particle effect and L/R indicatiors where your VR controllers are. Makes it easier to find them if you set them down.");

        vrCameraScale = VSVRMod.config.Bind("Camera", "VR Camera Scale", 1f, "Scale of the VR camera. A value of 0.5 would make the camera half size, making everything else appear twice as large.");
        fixCameraHeight = VSVRMod.config.Bind("Camera", "Fix Camera Height", false, "Makes the VR camera respect the actual distance from your head to the floor, instead of being scene dependent. Not recommended.");
        fixCameraAngle = VSVRMod.config.Bind("Camera", "Fix Camera Rotation", false, "Makes the VR camera respect the actual rotation of your head relative to the floor, instead of being scene dependent. Not recommended.");
        // Single pass does NOT work
        // useMultipassRendering = VSVRMod.config.Bind("Camera", "Use Multipass Rendering", false, "Multipass rendering is slower, but more compatable with shaders and other effects. Turn this on if you encounter graphical bugs.");

        greenscreenBackground = VSVRMod.config.Bind("Greenscreen", "Use Greenscreen Background", false, "If true, the session background will be set to the greenscreen color background.\n" +
            "IMPORTANT: For this to have an effect, turn off the ingame environment in Options -> General -> Graphics Settings -> No Background.\n" +
            "If you enable this setting, you should probably also enable \"Fix Camera Height,\" \"Fix Camera Angle,\" and disable \"Task Gradient\" in this config.\n" +
            "I don't really recommend using full passthrough, as many poses will look very strange.");
        greenscreenColor = VSVRMod.config.Bind("Greenscreen", "Greenscreen Color", Color.blue, "Color of the greenscreen in RGBA format. Default is R:00 G:00 B:FF A:FF for full opacity blue.");
        greenscreenUI = VSVRMod.config.Bind("Greenscreen", "UI Greenscreen", false, "Enables a toggleable greenscreen that covers everything except the UI, making it possible to find things without removing your headset.");

        enableSpeechRecognition = VSVRMod.config.Bind("Speech Recongition", "Enable Speech Recongition", false, "Enables local speech reconition with Vosk.");
        basicButtonSpeech = VSVRMod.config.Bind("Speech Recongition", "Press Buttons With Voice", true, "The words on the two main buttons can be said aloud to press them. Speech recognition must be enabled for this do to anything.");
        urgeSpeech = VSVRMod.config.Bind("Speech Recongition", "Speak Urges With Voice", true, "Speak urges will automatically be accepted if the given phrase is said. Speech recognition must be enabled for this do to anything.");
        urgeAnswer = VSVRMod.config.Bind("Speech Recongition", "Urge Give in/Resist With Voice", true, "All urges can be accepted or declined by saying 'give in' or 'resist'. Speech recognition must be enabled for this do to anything.");
        yesAndNo = VSVRMod.config.Bind("Speech Recongition", "Yes and No", true, "Saying 'yes' and 'no' will always press the left and right buttons, respectively. Speech recognition must be enabled for this do to anything.");
        safeword = VSVRMod.config.Bind("Speech Recongition", "Safeword", "Red Light", "The phrase that will activate the safeword feature when said. Speech recognition must be enabled for this do to anything.");

        urgeSeconds = VSVRMod.config.Bind("Gameplay", "Extra Urge Timeout", 0.0f, "Addional time to accept or reject urges. If you find yourself struggling to say phrases on time with voice control, increasing this should help.");

        fixMissingPreText = VSVRMod.config.Bind("Base Game Bugfixes", "Fix Missing Pretask Text", true, "Fixes the base game bug where the text that tells you to get something before a task does not show. May break some events, not sure yet. Disable if something else breaks.");
    }
}