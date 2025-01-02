using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.XR;

namespace VSVRMod2.VRCamera
{
    class ControllerFlame
    {
        private GameObject handObject;
        public ControllerFlame(InputDeviceCharacteristics hand, MoveableVRCamera vrCamera) {
            VSVRMod.logger.LogInfo("Creating hand flame...");
            if ((hand & InputDeviceCharacteristics.Left) != 0)
            {
                handObject = GameObject.Instantiate(VSVRAssets.leftHandFlame);
            }
            else if ((hand & InputDeviceCharacteristics.Right) != 0)
            {
                handObject = GameObject.Instantiate(VSVRAssets.rightHandFlame);
            }
            handObject.transform.SetParent(vrCamera.vrCameraOffset.transform);
        }
    }
}
