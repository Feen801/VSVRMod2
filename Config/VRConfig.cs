using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using BepInEx.Unity.Mono;
using UnityEngine;

namespace VSVRMod2;

public class VRConfig
{
    public static ConfigEntry<int> uiPosX;
    public static ConfigEntry<int> uiPosY;
    public static ConfigEntry<float> uiScale;
    public static ConfigEntry<float> uiDepth;

    public static void SetupConfig()
    {
        uiPosX = VSVRMod.config.Bind("UI", "UI X", 0, "X position of the UI.");
        uiPosY = VSVRMod.config.Bind("UI", "UI Y", 560, "Y position of the UI.");
        uiScale = VSVRMod.config.Bind("UI", "UI Scale", 0.7f, "Scale of the UI.");
        uiDepth = VSVRMod.config.Bind("UI", "UI Depth", 0.7f, "Distance from the camera to the UI.");
    }
}