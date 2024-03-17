using BepInEx.Configuration;
using UnityEngine;

namespace VSVRMod2;

public class VRConfig
{
    //UI settings
    public static ConfigEntry<float> uiDistance;

    //Controls settings
    public static ConfigEntry<bool> useHeadMovement;
    public static ConfigEntry<bool> automaticScreenSwap;

    //Camera settings
    public static ConfigEntry<float> vrCameraScale;
    public static ConfigEntry<bool> fixCameraHeight;
    public static ConfigEntry<bool> fixCameraAngle;

    //Greenscreen settings
    public static ConfigEntry<bool> greenscreenBackground;
    public static ConfigEntry<Color> greenscreenColor;
    public static ConfigEntry<bool> greenscreenUI;

    public static void SetupConfig()
    {
        uiDistance = VSVRMod.config.Bind("UI", "UI Distance", 0.5f, "How far away the UI appears from your face in meters.\n" +
            "The relative size will not change, i.e. if you make it closer to you it will also become smaller such that it takes up the same FOV.");

        useHeadMovement = VSVRMod.config.Bind("Controls", "Use Head Movement", true, "Enable or disable the ability to press buttons by nodding or shaking your head.");
        automaticScreenSwap = VSVRMod.config.Bind("Controls", "Automatic Screen Swap", true, "Automatically swap the game output to your monitor when your headset is removed. Only works on Oculus and WMR headsets.");

        vrCameraScale = VSVRMod.config.Bind("Camera", "VR Camera Scale", 1f, "Scale of the VR camera. A value of 0.5 would make the camera half size, making everything else appear twice as large.");
        fixCameraHeight = VSVRMod.config.Bind("Camera", "Fix Camera Height", false, "Makes the VR camera respect the actual distance from your head to the floor, instead of being scene dependent. Not recommended.");
        fixCameraAngle = VSVRMod.config.Bind("Camera", "Fix Camera Rotation", false, "Makes the VR camera respect the actual rotation of your head relative to the floor, instead of being scene dependent. Not recommended.");

        greenscreenBackground = VSVRMod.config.Bind("Greenscreen", "Use Greenscreen Background", false, "If true, the session background will be set to the greenscreen color background.\n" +
            "IMPORTANT: For this to have an effect, turn off the ingame environment in Options -> General -> Graphics Settings -> No Background.\n" +
            "If you enable this setting, you should probably also enable \"Fix Camera Height\" and \"Fix Camera Angle\" in this config.\n" +
            "I don't really recommend using full passthrough, as many poses will look very strange.");
        greenscreenColor = VSVRMod.config.Bind("Greenscreen", "Greenscreen Color", Color.green, "Color of the greenscreen in RGBA format. Default is R:00 G:FF B:00 A:FF for full opacity green.");
        greenscreenUI = VSVRMod.config.Bind("Greenscreen", "UI Greenscreen", false, "Enables a toggleable greenscreen that covers everything except the UI, making it possible to find things without removing your headset.");
    }
}