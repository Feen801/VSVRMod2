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

    //Controls settings
    public static ConfigEntry<bool> useHeadMovement;
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

        useHeadMovement = VSVRMod.config.Bind("Controls", "Use Head Movement", true, "Enable or disable the ability to press buttons by nodding or shaking your head.");
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
    }
}