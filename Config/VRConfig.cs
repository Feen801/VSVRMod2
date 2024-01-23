using BepInEx.Configuration;

namespace VSVRMod2;

public class VRConfig
{
    public static ConfigEntry<bool> useHeadMovement;
    public static ConfigEntry<float> vrCameraScale;

    public static void SetupConfig()
    {
        useHeadMovement = VSVRMod.config.Bind("Controls", "Use Head Movement", true, "Enable or disable the ability to press buttons by nodding or shaking your head.");

        vrCameraScale = VSVRMod.config.Bind("Camera", "VR Camera Scale", 1f, "Scale of the VR camera. A value of 0.5 would make the camera half size, making everything else appear twice as large.");
    }
}