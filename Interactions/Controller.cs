using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.UIElements.Collections;
using UnityEngine.XR;

namespace VSVRMod2;

class Controller
{
    private static InputDevice leftContoller;
    private static InputDevice rightContoller;
    public static void SetupControllers()
    {
        leftContoller = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightContoller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private static Dictionary<int, bool> triggerPresses = new Dictionary<int, bool>();

    public static bool WasATriggerClicked(int duplicateID)
    {
        bool clicked = false;
        bool pressed = IsATriggerPressed();
        if (pressed && !triggerPresses.Get(duplicateID))
        {
            clicked = true;
        }
        triggerPresses.Add(duplicateID, pressed);
        return clicked;
    }

    public static bool IsATriggerPressed()
    {
        
        float left = 0f;
        float right = 0f;

        if (leftContoller != null) {
            leftContoller.TryGetFeatureValue(CommonUsages.trigger, out left);
        }
        if (rightContoller != null)
        {
            rightContoller.TryGetFeatureValue(CommonUsages.trigger, out right);
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
        }
        triggerPresses.Add(duplicateID, pressed);
        return clicked;
    }

    public static bool IsAStickPressed()
    {
        bool left = false;
        bool right = false;

        if (leftContoller != null)
        {
            leftContoller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out left);
        }
        if (rightContoller != null)
        {
            rightContoller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out right);
        }

        return right  || left;
    }

    public static Vector2 GetMaximalJoystickValue()
    {
        Vector2 leftJoystickValue = Vector2.zeroVector;
        Vector2 rightJoystickValue = Vector2.zeroVector;

        if (leftContoller != null)
        {
            leftContoller.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftJoystickValue);
        }
        if (rightContoller != null)
        {
            leftContoller.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightJoystickValue);
        }

        return leftJoystickValue.magnitude > rightJoystickValue.magnitude ? leftJoystickValue : rightJoystickValue;
    }

    public static double GetMaximalJoystickAngle()
    {
        Vector2 maximal = GetMaximalJoystickValue();
        double angle = Vector2.Angle(maximal, Vector2.right);
        return maximal.y > 0 ? angle : 360 - angle;
    }

    public static double GetMaximalJoystickMagnitude()
    {
        Vector2 maximal = GetMaximalJoystickValue();
        return maximal.magnitude;
    }
}
