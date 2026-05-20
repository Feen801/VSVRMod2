using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SpatialTracking;
using VRM;

namespace VSVRMod2.VRCamera
{
    public class MoveableVRCamera
    {
        public GameObject primaryCamera;
        public GameObject worldCamDefault;
        public Camera worldCamDefaultCamera;
        public GameObject vrCamera;
        public GameObject vrCameraDolly;
        public GameObject vrCameraParent;
        public GameObject vrCameraOffset;

        private GameObject headFollower;
        private HeadLookController headLookController;

        private VRMLookAtHead eyeLookAt;
        private bool eyeLookAtOriginalEnabled;
        private Transform originalEyeTarget;
        private PlayMakerFSM lookAtManagerFsm;

        private GameObject headBone;
        private GameObject leftHandBone;
        private GameObject rightHandBone;
        private float headTrackingBlend;
        private const float HEAD_TRACKING_BASE = 0.2f;
        private const float HEAD_TRACKING_RAMP_START_DEG = 20f;
        private const float HEAD_TRACKING_RAMP_END_DEG = 30f;
        private const float LOOK_AWAY_DEADZONE_DEG = 10f;
        private const float LOOK_AWAY_FULL_DEG = 25f;
        private const float HEAD_TRACKING_BLEND_SMOOTHING = 5f;
        private const float HAND_NEAR_FACE_FULL_M = 0.20f;
        private const float HAND_NEAR_FACE_FADE_M = 0.32f;

        public MoveableVRCamera()
        {
            InitializeCameras();
            CreateVRCameraHierarchy();
            ConfigureVRCamera();
            SetupConstraints();
            vrCamera.SetActive(false);
            VSVRMod.logger.LogInfo("VR camera setup complete.");
        }

        private void InitializeCameras()
        {
            worldCamDefault = GameObjectHelper.GetGameObjectCheckFound("WorldCamDefault");
            primaryCamera = GameObjectHelper.GetGameObjectCheckFound("PrimaryCamera");

            if (worldCamDefault == null)
            {
                VSVRMod.logger.LogInfo("WorldCamDefault may be disabled, using fallback method.");
                worldCamDefault = primaryCamera.transform.Find("WorldCamDefault").gameObject;
            }

            worldCamDefaultCamera = worldCamDefault.GetComponent<Camera>();
            headFollower = GameObjectHelper.GetGameObjectCheckFound("HeadTargetFollower");
        }

        private void CreateVRCameraHierarchy()
        {
            vrCameraParent = GameObjectHelper.CreateChildGameObject("VRCameraParent", worldCamDefault.transform.root);
            vrCameraDolly = GameObjectHelper.CreateChildGameObject("VRCameraDolly", vrCameraParent.transform);
            vrCameraOffset = GameObjectHelper.CreateChildGameObject("VRCameraOffset", vrCameraDolly.transform);
        }

        private void ConfigureVRCamera()
        {
            VSVRMod.logger.LogInfo("Creating and configuring VR camera...");

            vrCamera = new GameObject("VRCamera");
            var vrCam = vrCamera.AddComponent<Camera>();
            vrCam.nearClipPlane = 0.01f;

            var tpd = vrCamera.AddComponent<TrackedPoseDriver>();
            tpd.UseRelativeTransform = true;

            float cameraScale = VRConfig.vrCameraScale.Value;
            vrCameraOffset.transform.localScale =
                new Vector3(cameraScale, cameraScale, cameraScale);

            vrCamera.transform.SetParent(vrCameraOffset.transform, false);

            VSVRMod.logger.LogInfo("VR camera configuration complete.");
        }

        private void SetupConstraints()
        {
            SetupPositionConstraint();
            SetupRotationConstraint();
        }

        private void SetupPositionConstraint()
        {
            VSVRMod.logger.LogInfo("Setting up position constraint...");
            PositionConstraint posConstraint = vrCameraParent.AddComponent<PositionConstraint>();
            ConstraintSource constraintSource = new ConstraintSource
            {
                sourceTransform = worldCamDefault.transform,
                weight = 1.0f
            };

            posConstraint.AddSource(constraintSource);
            posConstraint.translationAxis = VRConfig.fixCameraHeight.Value ? (Axis.X | Axis.Z) : (Axis.X | Axis.Y | Axis.Z);
            posConstraint.translationOffset = Vector3.zero;
            posConstraint.constraintActive = true;

            if (VRConfig.fixCameraHeight.Value)
            {
                vrCameraParent.transform.position = new Vector3(
                    vrCameraParent.transform.position.x,
                    0f,
                    vrCameraParent.transform.position.z
                );
            }
            VSVRMod.logger.LogInfo("Position constraint setup complete.");
        }

        private void SetupRotationConstraint()
        {
            VSVRMod.logger.LogInfo("Setting up rotation constraint...");
            RotationConstraint rotConstraint = vrCameraParent.AddComponent<RotationConstraint>();
            ConstraintSource constraintSource = new ConstraintSource
            {
                sourceTransform = worldCamDefault.transform,
                weight = 1.0f
            };

            rotConstraint.AddSource(constraintSource);
            rotConstraint.rotationAxis = VRConfig.fixCameraAngle.Value ? Axis.Y : (Axis.X | Axis.Y | Axis.Z);
            rotConstraint.rotationOffset = Vector3.zero;
            rotConstraint.constraintActive = true;

            VSVRMod.logger.LogInfo("Rotation constraint setup complete.");
        }

        public bool CenterCamera(bool fullReset)
        {
            if (vrCamera == null || VRConfig.fixCameraHeight.Value)
            {
                return false;
            }
            if (vrCamera.transform.localPosition.sqrMagnitude < 0.01)
            {
                return false;
            }
            VSVRMod.logger.LogInfo("Trying recenter lpos:" + vrCamera.transform.localPosition + " pos:" + vrCamera.transform.position);
            vrCameraOffset.transform.position = vrCamera.transform.position;
            vrCameraOffset.transform.localPosition = -vrCamera.transform.localPosition;
            if (fullReset)
            {
                vrCameraDolly.transform.localPosition = Vector3.zero;
            }
            VSVRMod.logger.LogInfo("Camera centered...");
            return true;
        }

        private bool didRecenter = false;

        public void CenterCameraIfFar()
        {
            if (didRecenter)
            {
                return;
            }
            Vector3 distanceVector = worldCamDefault.transform.position - vrCamera.transform.position;
            double distance = distanceVector.sqrMagnitude;
            //VSVRMod.logger.LogWarning(distance);
            if (distance > 0.1)
            {
                didRecenter = true;
                CenterCamera(false);
            }
        }

        private float HandNearFaceSuppression(Vector3 headPos)
        {
            if (leftHandBone == null) leftHandBone = GameObject.Find("J_Bip_L_Hand");
            if (rightHandBone == null) rightHandBone = GameObject.Find("J_Bip_R_Hand");

            float minDist = float.PositiveInfinity;
            if (leftHandBone != null)  minDist = Mathf.Min(minDist, Vector3.Distance(leftHandBone.transform.position,  headPos));
            if (rightHandBone != null) minDist = Mathf.Min(minDist, Vector3.Distance(rightHandBone.transform.position, headPos));
            if (float.IsPositiveInfinity(minDist)) return 0f;

            return 1f - Mathf.Clamp01((minDist - HAND_NEAR_FACE_FULL_M) / (HAND_NEAR_FACE_FADE_M - HAND_NEAR_FACE_FULL_M));
        }

        public void SetupHeadTargetFollower(bool revert)
        {
            if (headFollower == null)
            {
                headFollower = GameObjectHelper.GetGameObjectCheckFound("HeadTargetFollower");
                if (headFollower == null) return;
            }

            if (!revert && headLookController == null)
            {
                headLookController = UnityEngine.Object.FindObjectOfType<HeadLookController>();
            }
        }

        public void UpdateHeadTargetTracking()
        {
            if (headFollower == null || headLookController == null || vrCamera == null || !vrCamera.activeSelf) return;
            if (headBone == null)
            {
                headBone = GameObject.Find("J_Bip_C_Head");
                if (headBone == null) return;
            }

            Vector3 origin = headBone.transform.position;
            Vector3 fsmTarget = headFollower.transform.position;
            Vector3 player = vrCamera.transform.position;
            Vector3 cam = worldCamDefault.transform.position;

            float lookAwayAngle = Vector3.Angle(fsmTarget - origin, cam - origin);
            float cameraAlignment = 1f - Mathf.Clamp01(
                (lookAwayAngle - LOOK_AWAY_DEADZONE_DEG) / (LOOK_AWAY_FULL_DEG - LOOK_AWAY_DEADZONE_DEG));

            float playerAngle = Vector3.Angle(cam - origin, player - origin);
            float rampT = Mathf.Clamp01(
                (playerAngle - HEAD_TRACKING_RAMP_START_DEG) / (HEAD_TRACKING_RAMP_END_DEG - HEAD_TRACKING_RAMP_START_DEG));
            float playerOffset = Mathf.Lerp(HEAD_TRACKING_BASE, 1f, rampT);

            float handSuppression = HandNearFaceSuppression(origin);
            float target = playerOffset * cameraAlignment * (1f - handSuppression);
            headTrackingBlend = Mathf.Lerp(headTrackingBlend, target, Time.deltaTime * HEAD_TRACKING_BLEND_SMOOTHING);

            if (headTrackingBlend > 0.02f)
            {
                headLookController.target = Vector3.Lerp(fsmTarget, player, headTrackingBlend);
            }

            if (eyeLookAt != null && EyeTrackingPatches.Enabled)
            {
                eyeLookAt.Target = vrCamera.transform;
                eyeLookAt.enabled = true;
            }
        }

        public void SetupEyeTracking(bool revert)
        {
            if (eyeLookAt == null)
            {
                foreach (var lookAt in UnityEngine.Object.FindObjectsOfType<VRMLookAtHead>(true))
                {
                    bool hasApplyer = lookAt.GetComponent<VRMLookAtBoneApplyer>() != null;
                    if (hasApplyer && lookAt.enabled) { eyeLookAt = lookAt; break; }
                }
                if (eyeLookAt == null)
                {
                    VSVRMod.logger.LogWarning("SetupEyeTracking: no enabled VRMLookAtHead with a BoneApplyer found");
                    return;
                }
                eyeLookAtOriginalEnabled = eyeLookAt.enabled;
                originalEyeTarget = eyeLookAt.Target;
                foreach (var fsm in eyeLookAt.GetComponents<PlayMakerFSM>())
                {
                    if (fsm.FsmName == "LookAtManager") { lookAtManagerFsm = fsm; break; }
                }
            }

            if (revert)
            {
                EyeTrackingPatches.Enabled = false;
                if (lookAtManagerFsm != null) lookAtManagerFsm.enabled = true;
                eyeLookAt.Target = originalEyeTarget;
                eyeLookAt.enabled = eyeLookAtOriginalEnabled;
            }
            else
            {
                if (lookAtManagerFsm != null) lookAtManagerFsm.enabled = false;
                eyeLookAt.Target = vrCamera.transform;
                eyeLookAt.enabled = true;
                EyeTrackingPatches.Enabled = true;
            }
        }

        private bool shouldCenterCamera = true;
        private float timeCenterHeld = 0;

        public void CameraControls()
        {
            int gripCount = Controller.CountGripsPressed();
            if (gripCount == 2)
            {
                if (shouldCenterCamera)
                {
                    this.CenterCamera(false);
                }
                shouldCenterCamera = false;
                timeCenterHeld += Time.fixedDeltaTime;
                if (timeCenterHeld > 1 && timeCenterHeld < 2)
                {
                    this.CenterCamera(true);
                    timeCenterHeld += 99;
                }
            }
            else
            {
                shouldCenterCamera = true;
                timeCenterHeld = 0;
            }
            if (gripCount == 1)
            {
                float speed = Controller.GetMaximalJoystickValue().y;
                vrCameraDolly.transform.localPosition += Vector3.forward * speed * Time.fixedDeltaTime;
            }
            if (Controller.WasAGripClickedQuickly())
            {
                VRUI.ToggleGreenscreenUI();
            }
        }
    }
}
