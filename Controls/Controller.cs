﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace VSVRMod2;

class Controller
{
    public static int outputControllerDebug = 0;
    private static UnityEngine.XR.InputDevice leftController;
    private static UnityEngine.XR.InputDevice rightController;
    private static UnityEngine.XR.InputDevice headset;

    public static void CheckControllers()
    {
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevices(inputDevices);
        if (inputDevices.Count > 0)
        {
            foreach (var device in inputDevices)
            {
                VSVRMod.logger.LogInfo("Found controller: " + device.name + " Characteristics: " + device.characteristics);
            }
        }
        VSVRMod.logger.LogInfo("Current Left Controller: " + leftController.name);
        VSVRMod.logger.LogInfo("Current Right Controller: " + rightController.name);
    }

    public static void DeviceConnect(UnityEngine.XR.InputDevice device)
    {
        VSVRMod.logger.LogInfo("Device Connected: " + device.name);
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
        {
            leftController = device;
            VSVRMod.logger.LogInfo("Left Controller!");
        }
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
        {
            rightController = device;
            VSVRMod.logger.LogInfo("Right Controller!");
        }
        else if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
        {
            headset = device;
            VSVRMod.logger.LogInfo("Headset found!");
        }
    }

    /**
    * From https://github.com/DaXcess/LCVR (GPL-3.0 license)
    */
    public static void EnableControllerProfiles()
    {
        var valveIndex = ScriptableObject.CreateInstance<ValveIndexControllerProfile>();
        var hpReverb = ScriptableObject.CreateInstance<HPReverbG2ControllerProfile>();
        var htcVive = ScriptableObject.CreateInstance<HTCViveControllerProfile>();
        var mmController = ScriptableObject.CreateInstance<MicrosoftMotionControllerProfile>();
        var khrSimple = ScriptableObject.CreateInstance<KHRSimpleControllerProfile>();
        var metaQuestTouch = ScriptableObject.CreateInstance<MetaQuestTouchProControllerProfile>();
        var oculusTouch = ScriptableObject.CreateInstance<OculusTouchControllerProfile>();

        valveIndex.enabled = true;
        hpReverb.enabled = true;
        htcVive.enabled = true;
        mmController.enabled = true;
        khrSimple.enabled = true;
        metaQuestTouch.enabled = true;
        oculusTouch.enabled = true;

        // Patch the OpenXRSettings.features field to include controller profiles
        // This feature list is empty by default if the game isn't a VR game

        var featList = new List<OpenXRFeature>()
            {
                valveIndex,
                hpReverb,
                htcVive,
                mmController,
                khrSimple,
                metaQuestTouch,
                oculusTouch
            };
        typeof(OpenXRSettings).GetField("features", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(OpenXRSettings.Instance, featList.ToArray());

        VSVRMod.logger.LogInfo("Enabled XR Controller Profiles");
    }

    private static int lastTriggerFrame = 0;

    public static bool WasATriggerClicked()
    {
        int frame = Time.frameCount;
        bool clicked = false;
        bool pressed = IsATriggerPressed();
        if (pressed)
        {
            if (frame != lastTriggerFrame + 1)
            {
                clicked = true;
                if (outputControllerDebug >= 1)
                {
                    VSVRMod.logger.LogInfo("Trigger click!");
                }
            }
        }

        return clicked;
    }

    public static bool IsATriggerPressed()
    {
        float left = 0f;
        float right = 0f;
        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out left);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out right);
        }
        bool joystick = Input.GetAxis("Fire1") > 0.5f;
        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Trigger: Left: " + left + " Right: " + right);
        }

        return right > 0.5 || left > 0.5 || joystick;
    }

    private static int lastStickFrame = 0;

    public static bool WasAStickClicked()
    {
        int frame = Time.frameCount;
        bool clicked = false;
        bool pressed = IsAStickPressed();
        if (pressed)
        {
            if (frame != lastStickFrame + 1)
            {
                clicked = true;
                if (outputControllerDebug >= 1)
                {
                    VSVRMod.logger.LogInfo("Stick click!");
                }
            }
        }

        return clicked;
    }

    public static bool IsAStickPressed()
    {
        bool left = false;
        bool right = false;
        bool joystick = false;

        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out left);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out right);
        }
        joystick = Input.GetAxis("Fire2") > 0.5f;

        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Stick: Left: " + left + " Right: " + right);
        }

        return right || left || joystick;
    }

    private static int lastFaceFrame = 0;
    public static bool WasAFaceButtonClicked()
    {
        int frame = Time.frameCount;
        bool clicked = false;
        bool pressed = IsAFaceButtonPressed();
        if (pressed)
        {
            if (frame != lastFaceFrame + 1)
            {
                clicked = true;
                if (outputControllerDebug >= 1)
                {
                    VSVRMod.logger.LogInfo("Face button click!");
                }
            }
        }

        return clicked;
    }

    public static int CountGripsPressed()
    {
        bool left = false;
        bool right = false;
        bool joystick = false;
        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out left);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out right);
        }
        joystick = Input.GetAxis("Fire3") > 0.5f;

        return Math.Clamp(Convert.ToInt32(left) + Convert.ToInt32(right) + Convert.ToInt32(joystick), 0, 2);
    }

    public static bool IsAFaceButtonPressed()
    {
        bool left1 = false;
        bool left2 = false;
        bool right1 = false;
        bool right2 = false;
        bool joystick = false;

        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out left1);
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out left2);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out right1);
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out right2);
        }
        joystick = Input.GetAxis("Jump") > 0.5f;

        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Face Button: Left: " + (left1 || left2) + " Right: " + (right1 || right2));
        }

        return left1 || left2 || right1 || right2 || joystick;
    }

    public static bool IsHeadsetWorn()
    {
        headset.TryGetFeatureValue(UnityEngine.XR.CommonUsages.userPresence, out bool worn);
        return worn;
    }

    public class Headset
    {
        public event Action OnWorn;
        public event Action OnRemoved;

        private bool lastState = true;

        public void Update()
        {
            bool worn = IsHeadsetWorn();
            if (worn != lastState)
            {
                if(worn)
                {
                    OnWorn?.Invoke();
                }
                else
                { 
                    OnRemoved?.Invoke();
                }
                lastState = worn;
            }
        }
    }

    public static Vector2 GetMaximalJoystickValue()
    {
        Vector2 leftJoystickValue = Vector2.zeroVector;
        Vector2 rightJoystickValue = Vector2.zeroVector;

        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftJoystickValue);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightJoystickValue);
        }
        Vector2 joystick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        leftJoystickValue = leftJoystickValue.magnitude > joystick.magnitude ? leftJoystickValue : joystick;

        if (outputControllerDebug >= 3)
        {
            VSVRMod.logger.LogInfo("Joystick right: x: " + rightJoystickValue.x + " y: " + rightJoystickValue.y);
            VSVRMod.logger.LogInfo("Joystick left: x: " + leftJoystickValue.x + " y: " + leftJoystickValue.y);
        }

        return leftJoystickValue.magnitude > rightJoystickValue.magnitude ? leftJoystickValue : rightJoystickValue;
    }

    public static double GetMaximalJoystickAngle()
    {
        Vector2 maximal = GetMaximalJoystickValue();
        double angle = Vector2.Angle(maximal, Vector2.right);
        angle = maximal.y > 0 ? angle : 360 - angle;

        if (outputControllerDebug >= 4)
        {
            VSVRMod.logger.LogInfo("Joystick angle: " + angle);
        }

        return angle;
    }

    public static double GetMaximalJoystickMagnitude()
    {
        Vector2 maximal = GetMaximalJoystickValue();

        if (outputControllerDebug >= 4)
        {
            VSVRMod.logger.LogInfo("Joystick magnitude: " + maximal.magnitude);
        }

        return maximal.magnitude;
    }

    public static void endFrame()
    {
        int frame = Time.frameCount;
        lastStickFrame = frame;
        lastFaceFrame = frame;
        lastTriggerFrame = frame;
    }
}
