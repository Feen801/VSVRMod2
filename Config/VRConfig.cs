using BepInEx.Configuration;

namespace VSVRMod2;

public class VRConfig
{
    public static ConfigEntry<bool> useHeadMovement;

    public static void SetupConfig()
    {
        useHeadMovement = VSVRMod.config.Bind("Controls", "Use Head Movement", true, "Enable or disable the ability to press buttons by nodding or shaking your head.");
    }
}