using System;
using UnityEngine.SceneManagement;
using VSVRMod2.VRCamera;

namespace VSVRMod2;
public class VRCameraManager
{
    readonly public MoveableVRCamera vrcamera;
    private ControllerFlame leftHand;
    private ControllerFlame rightHand;
    public VRCameraManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.SessionStartScene))
        {
            vrcamera = new MoveableVRCamera();
            VRUI.Start(vrcamera);
            if (!VRConfig.taskGradient.Value)
            {
                VRUI.DisableTaskGradient();
                VSVRMod.logger.LogInfo("Session setup: Disabled task gradients");
            }
            VSVRMod.controllerHeadset.OnWorn += () => VRUI.SetupUI(vrcamera);
            VSVRMod.controllerHeadset.OnRemoved += () => VRUI.RevertUI(vrcamera);
            VSVRMod.logger.LogInfo("Session setup: OnWorn and OnRemoved");

            leftHand = new(UnityEngine.XR.InputDeviceCharacteristics.Left, vrcamera);
            rightHand = new(UnityEngine.XR.InputDeviceCharacteristics.Right, vrcamera);

        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public void Update()
    {
        vrcamera.CameraControls();
        /*vrcamera.CenterCameraIfFar();*/
    }

    bool alreadyCentered = false;
    public void LateUpdate()
    {
        if (!alreadyCentered)
        {
            alreadyCentered = vrcamera.CenterCamera(true);
        }
    }
}