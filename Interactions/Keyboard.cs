﻿using UnityEngine;

namespace VSVRMod2;

class Keyboard
{
    public static void HandleKeyboardInputSession(VRCameraManager vrCameraManager)
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            vrCameraManager.CenterCamera();
        }
        
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            vrCameraManager.ToggleUIVR();
        }
    }

    public static void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            Controller.outputControllerDebug = (Controller.outputControllerDebug + 1) % 5;
            VSVRMod.logger.LogInfo("Controller debug level is now " + Controller.outputControllerDebug);
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Controller.CheckControllers();
        }
    }
}
