using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Collections;
using UnityEngine.XR;
using System.Reflection;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace VSVRMod2;

class Controller
{
    public static int outputControllerDebug = 0;
    private static UnityEngine.XR.InputDevice leftController;
    private static UnityEngine.XR.InputDevice rightController;
    //public static void SetupControllers()
    //{
    //    var inputDevices = new List<UnityEngine.XR.InputDevice>();
    //    InputDevices.GetDevices(inputDevices);
    //    if (inputDevices.Count > 0)
    //    {
    //        foreach (var device in inputDevices)
    //        {
    //            VSVRMod.logger.LogInfo("Found controller: " + device.name + " Characteristics: " + device.characteristics);
    //        }
    //    }
    //    else
    //    {
    //        VSVRMod.logger.LogInfo("Could not find any controllers.");
    //    }
    //    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, inputDevices);
    //    if (inputDevices.Count > 0)
    //    {
    //        leftController = inputDevices[0];
    //    }
    //    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, inputDevices);
    //    if (inputDevices.Count > 0)
    //    {
    //        rightController = inputDevices[0];
    //    }
    //}

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

    public static void DeviceConnect(InputDevice device)
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

    private static Dictionary<int, bool> triggerPresses = new Dictionary<int, bool>();

    public static bool WasATriggerClicked(int duplicateID)
    {
        bool clicked = false;
        bool pressed = IsATriggerPressed();
        if (pressed && !triggerPresses.Get(duplicateID))
        {
            clicked = true;
            if (outputControllerDebug >= 1)
            {
                VSVRMod.logger.LogInfo("Trigger click!");
            }
        }
        triggerPresses[duplicateID] = pressed;

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

        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Trigger: Left: " + left + " Right: " + right);
        }

        return right > 0.5 || left > 0.5;
    }

    private static Dictionary<int, bool> stickPresses = new Dictionary<int, bool>();

    public static bool WasAStickClicked(int duplicateID)
    {
        bool clicked = false;
        bool pressed = IsAStickPressed();
        if (pressed && !stickPresses.Get(duplicateID))
        {
            clicked = true;
            if (outputControllerDebug >= 1)
            {
                VSVRMod.logger.LogInfo("Stick click!");
            }
        }
        stickPresses[duplicateID] = pressed;

        return clicked;
    }

    public static bool IsAStickPressed()
    {
        bool left = false;
        bool right = false;

        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out left);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out right);
        }

        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Stick: Left: " + left + " Right: " + right);
        }

        return right || left;
    }

    private static Dictionary<int, bool> facePresses = new Dictionary<int, bool>();
    public static bool WasAFaceButtonClicked(int duplicateID)
    {
        bool clicked = false;
        bool pressed = IsAFaceButtonPressed();
        if (pressed && !facePresses.Get(duplicateID))
        {
            clicked = true;
            if (outputControllerDebug >= 1)
            {
                VSVRMod.logger.LogInfo("Face click!");
            }
        }
        facePresses[duplicateID] = pressed;
        return clicked;
    }

    public static bool AreBothGripsPressed() 
    {
        bool left = false;
        bool right = false;
        if (leftController != null)
        {
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out left);
        }
        if (rightController != null)
        {
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out right);
        }

        return left && right;
    }

    public static bool IsAFaceButtonPressed()
    {
        bool left1 = false;
        bool left2 = false;
        bool right1 = false;
        bool right2 = false;

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

        if (outputControllerDebug >= 2)
        {
            VSVRMod.logger.LogInfo("Face Button: Left: " + (left1 || left2) + " Right: " + (right1 || right2));
        }

        return left1 || left2 || right1 || right2;
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

    public static void ControllerInteract()
    {
        //Ordered by priority
        if (Menus.SafewordMenuInteract())
        {
            return;
        }
        if (Buttons.TemporaryButtonInteract())
        {
            return;
        }
        if (Menus.FindomInputInteract())
        {
            return;
        }
        if (Buttons.RadialMenuInteract())
        {
            return;
        }
        if (Menus.StakesMenuInteract())
        {
            return;
        }
        if (Menus.ChoiceMenuInteract())
        {
            return;
        }
        if (Menus.IntInputInteract())
        {
            return;
        }
        if (Buttons.ChoiceButtonInteract())
        {
            return;
        }
    }
}
