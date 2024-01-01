using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VSVRMod2;

class Keyboard
{
    public static void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            VRCamera.CenterCamera();
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Controller.SetupControllers();
        }
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            VRCamera.ToggleUIVR();
        }
    }
}
