using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI.SpecifcUI;

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

    public SafewordUIManager(Scene scene) : base(scene)
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
        safewordMenu.goEasy.SetTriggerIconLocation(0, 100);
        safewordMenu.continueSession = new(eventManager.Find("Buttons"),
            "Safeword Continue",
            "ContinueSession",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        safewordMenu.continueSession.SetTriggerIconLocation(0, 100);
        safewordMenu.endSession = new(eventManager.Find("Buttons"),
            "Safeword End Session",
            "EndSession",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        safewordMenu.endSession.SetTriggerIconLocation(0, 100);

        if (VRConfig.showButtonPrompts.Value)
        {
            GameObject joystick1 = GameObject.Instantiate(VSVRAssets.promptIcons["UpLeft"]);
            GameObjectHelper.SetParentAndMaintainScaleForUI(joystick1.transform, safewordMenu.goEasy.components.buttonObject.transform);
            joystick1.transform.localPosition = new Vector3(0, -55);

            GameObject joystick2 = GameObject.Instantiate(VSVRAssets.promptIcons["Up"]);
            GameObjectHelper.SetParentAndMaintainScaleForUI(joystick2.transform, safewordMenu.continueSession.components.buttonObject.transform);
            joystick2.transform.localPosition = new Vector3(0, -55);

            GameObject joystick3 = GameObject.Instantiate(VSVRAssets.promptIcons["UpRight"]);
            GameObjectHelper.SetParentAndMaintainScaleForUI(joystick3.transform, safewordMenu.endSession.components.buttonObject.transform);
            joystick3.transform.localPosition = new Vector3(0, -55);
        }

        VSVRMod.logger.LogInfo("Setup Safeword");
    }

    public override bool Interact()
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
                if (Controller.WasATriggerClicked())
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }
}