using System;
using UnityEngine.SceneManagement;
using VSVRMod2.VRCamera;

namespace VSVRMod2;
public class VRCameraManager
{
    readonly public MoveableVRCamera vrcamera;
    public VRCameraManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.SessionScene))
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
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public void Update()
    {
        vrcamera.CameraControls();
        vrcamera.CenterCameraIfFar();
    }
}