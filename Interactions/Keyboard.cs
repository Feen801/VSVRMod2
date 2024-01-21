using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VSVRMod2;

class Keyboard
{
    public static void HandleKeyboardInputSession()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            VRCamera.CenterCamera();
        }
        
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            VRCamera.ToggleUIVR();
        }
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            Controller.outputControllerDebug = (Controller.outputControllerDebug + 1) % 5;
            VSVRMod.logger.LogInfo("Controller debug level is now " + Controller.outputControllerDebug);
        }

        //VSVRMod.logger.LogInfo(Gamepad.current.aButton);
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
