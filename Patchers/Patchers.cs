using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEngine.XR.OpenXR.Features.OculusQuestSupport;

namespace VSVRMod2;

[HarmonyPatch]
public class Patchers
{
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(OpenXRSettings), "Awake")]
    //[HarmonyPatch(MethodType.Normal)]
    private static void AddFeatures(OpenXRFeature[] ___features)
    {
        ___features.AddItem(new OculusTouchControllerProfile());
        ___features.AddItem(new MetaQuestFeature());
        ___features.AddItem(new ValveIndexControllerProfile());
        ___features.AddItem(new HTCViveControllerProfile());
        ___features.AddItem(new MetaQuestTouchProControllerProfile());
        //___features.AddItem(ScriptableObject.CreateInstance("OculusTouchControllerProfile"));
        //___features.AddItem(ScriptableObject.CreateInstance("MetaQuestFeature"));
        //___features.AddItem(ScriptableObject.CreateInstance("ValveIndexControllerProfile"));
        //___features.AddItem(ScriptableObject.CreateInstance("HTCViveControllerProfile"));
        //___features.AddItem(ScriptableObject.CreateInstance("MetaQuestTouchProControllerProfile"));
    }
}
