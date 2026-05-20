using HarmonyLib;
using UniGLTF;
using UnityEngine;
using VRM;

namespace VSVRMod2.VRCamera
{
    [HarmonyPatch]
    public static class EyeTrackingPatches
    {
        public static bool Enabled = false;

        public const float YawGain = 0.2f;
        public const float PitchGain = 0.2f;
        public const float MaxYawOuter  = 19f;
        public const float MaxYawInner  = 13f;
        public const float MaxPitchUp   = 7f;
        public const float MaxPitchDown = 17f;
        public const float PitchBias    = -2f;

        private static int _debugFrame;

        [HarmonyPatch(typeof(VRMLookAtBoneApplyer), "ApplyRotations")]
        [HarmonyPrefix]
        static bool ApplyRotations_Prefix(VRMLookAtBoneApplyer __instance, float yaw, float pitch)
        {
            if (!Enabled) return true;
            if (__instance.LeftEye.Transform == null || __instance.RightEye.Transform == null) return false;

            if ((_debugFrame++ % 60) == 0)
                VSVRMod.logger.LogInfo($"EyePatch raw yaw={yaw:F1} pitch={pitch:F1}");

            float y = yaw * YawGain;
            float p = pitch * PitchGain + PitchBias;

            float absYaw = Mathf.Abs(y);
            float signYaw = y < 0 ? -1f : 1f;
            float outerYaw = signYaw * Mathf.Min(absYaw, MaxYawOuter);
            float innerYaw = signYaw * Mathf.Min(absYaw, MaxYawInner);
            float leftYaw  = y < 0 ? outerYaw : innerYaw;
            float rightYaw = y < 0 ? innerYaw : outerYaw;

            p = Mathf.Clamp(p, -MaxPitchDown, MaxPitchUp);

            Quaternion leftRot  = Quaternion.AngleAxis(leftYaw,  Vector3.up) * Quaternion.AngleAxis(-p, Vector3.right);
            Quaternion rightRot = Quaternion.AngleAxis(rightYaw, Vector3.up) * Quaternion.AngleAxis(-p, Vector3.right);

            __instance.LeftEye.Transform.rotation  = __instance.LeftEye.InitialWorldMatrix.ExtractRotation()  * leftRot;
            __instance.RightEye.Transform.rotation = __instance.RightEye.InitialWorldMatrix.ExtractRotation() * rightRot;
            return false;
        }
    }
}
