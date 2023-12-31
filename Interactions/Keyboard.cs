using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VSVRMod2;

class Keyboard
{
    public static void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            VRCamera.CenterCamera();
        }
    }
}
