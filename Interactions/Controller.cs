using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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

    public static void HandleControllerInput()
    {

    }

    private static Vector2 GetMaximalJoystickValue()
    {
        leftContoller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickValue);
        leftContoller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickValue);
        return leftJoystickValue.magnitude > rightJoystickValue.magnitude ? leftJoystickValue : rightJoystickValue;
    }

    private static double GetMaximalJoystickAngle()
    {
        Vector2 maximal = GetMaximalJoystickValue();
        double angle = Vector2.Angle(maximal, Vector2.right);
        return maximal.y > 0 ? angle : 360 - angle;
    }

    private static double GetMaximalJoystickMagnitude()
    {
        Vector2 maximal = GetMaximalJoystickValue();
        return maximal.magnitude;
    }
}
