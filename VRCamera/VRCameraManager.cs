using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.UIElements.Collections;
using HutongGames.PlayMaker.Actions;
using VSVRMod2.VRCamera;

namespace VSVRMod2;
public class VRCameraManager
{
    MoveableVRCamera vrcamera;
    public VRCameraManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.SessionScene))
        {
            vrcamera = new MoveableVRCamera();
            VRUI.Start(vrcamera);
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