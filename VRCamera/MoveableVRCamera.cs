using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SpatialTracking;

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
        PlayMakerFSM headResetter;

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
            vrCamera.AddComponent<Camera>().nearClipPlane = 0.01f;
            vrCamera.AddComponent<TrackedPoseDriver>().UseRelativeTransform = true;

            float cameraScale = VRConfig.vrCameraScale.Value;
            vrCameraOffset.transform.localScale = new Vector3(cameraScale, cameraScale, cameraScale);
            vrCamera.transform.SetParent(vrCameraOffset.transform);

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

        public void CenterCamera(bool fullReset)
        {
            if (vrCamera == null || VRConfig.fixCameraHeight.Value)
            {
                return;
            }
            vrCameraOffset.transform.position = vrCamera.transform.position;
            vrCameraOffset.transform.localPosition = -vrCamera.transform.localPosition;
            if (fullReset)
            {
                vrCameraDolly.transform.localPosition = Vector3.zero;
            }
            VSVRMod.logger.LogInfo("Camera centered...");
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

        public void SetupHeadTargetFollower(bool revert)
        {
            if (headFollower == null)
            {
                VSVRMod.logger.LogInfo("what the hell");
                VSVRMod.logger.LogInfo("how did this become null");
                VSVRMod.logger.LogInfo("why does this only happen when starting a second one");
                headFollower = GameObjectHelper.GetGameObjectCheckFound("HeadTargetFollower");
            }
            if (revert)
            {
                headFollower.transform.SetParent(worldCamDefault.transform);
            }
            else
            {
                headFollower.transform.SetParent(vrCamera.transform);
            }
            headResetter = headFollower.GetComponent<PlayMakerFSM>();
            if (headResetter != null)
            {
                headResetter.enabled = revert;
            }
            headFollower.transform.localPosition = new Vector3(0, 0, 0);
            headFollower.transform.localRotation = new Quaternion(0, 0, 0, 0);
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
