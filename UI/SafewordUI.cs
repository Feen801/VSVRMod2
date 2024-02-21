using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;

public class SafewordUIManager : UIManager
{
    private struct SafewordMenu
    {
        public GameObject representative;
        public VSGenericButton goEasy;
        public VSGenericButton continueSession;
        public VSGenericButton endSession;
    }
    SafewordMenu safewordMenu = new SafewordMenu();

    public SafewordUIManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform eventManager = GameObject.Find("GeneralCanvas/EventManager").transform;

            //Safeword-------------------
            safewordMenu.representative = eventManager.Find("Buttons/EndSession").gameObject;
            if (safewordMenu.representative == null)
            {
                VSVRMod.logger.LogError("Safeword had null representative");
            }
            safewordMenu.goEasy = new(eventManager.Find("Buttons"),
                "Safeword Go Easy",
                "GoEasy",
                "/DoneBG/DoneText/Collider",
                "/DoneBG/DoneText/Collider/ButtonPressReact1"
                );
            safewordMenu.continueSession = new(eventManager.Find("Buttons"),
                "Safeword Continue",
                "ContinueSession",
                "/DoneBG/DoneText/Collider",
                "/DoneBG/DoneText/Collider/ButtonPressReact1"
                );
            safewordMenu.endSession = new(eventManager.Find("Buttons"),
                "Safeword End Session",
                "EndSession",
                "/DoneBG/DoneText/Collider",
                "/DoneBG/DoneText/Collider/ButtonPressReact1"
                );
            VSVRMod.logger.LogInfo("Setup Safeword");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        if (safewordMenu.representative.activeSelf)
        {
            double angle = Controller.GetMaximalJoystickAngle();
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            safewordMenu.goEasy.Highlight(false);
            safewordMenu.continueSession.Highlight(false);
            safewordMenu.endSession.Highlight(false);
            VSGenericButton theButton = null;
            if (magnitude > 0.3)
            {
                if (angle > 0 && angle < 60)
                {
                    theButton = safewordMenu.endSession;
                }
                else if (angle > 60 && angle < 120)
                {
                    theButton = safewordMenu.continueSession;
                }
                else if (angle > 120 && angle < 180)
                {
                    theButton = safewordMenu.goEasy;
                }
            }
            if (theButton != null)
            {
                theButton.Highlight(true);
                if (Controller.WasATriggerClicked(106))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }
}