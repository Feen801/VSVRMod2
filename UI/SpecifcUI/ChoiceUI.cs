using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI.SpecifcUI;
public class ChoiceUIManager : UIManager
{
    private struct ChoiceMenu
    {
        public GameObject representative;
        public VSGenericButton left;
        public VSGenericButton right;
        public VSGenericButton favorite;
    }
    ChoiceMenu choiceMenu;

    public ChoiceUIManager(Scene scene) : base(scene)
    {
        Transform eventManager = GameObject.Find("GeneralCanvas/EventManager").transform;

        //ChoiceUI-------------------
        choiceMenu.representative = eventManager.Find("ChoiceUI").gameObject;
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject joystick = GameObject.Instantiate(VSVRAssets.promptIcons["Horizontal Angled"]);
            GameObjectHelper.SetParentAndMaintainScaleForUI(joystick.transform, choiceMenu.representative.transform);
            joystick.transform.localPosition = new Vector3(0, 0);
        }
        if (choiceMenu.representative == null)
        {
            VSVRMod.logger.LogError("ChoiceUI had null representative");
        }
        choiceMenu.favorite = new(eventManager.Find("Buttons"),
            "Favorite Heart",
            "FavoriteHeart",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        choiceMenu.favorite.SetTriggerIconLocation(0, -100);
        choiceMenu.left = new(eventManager.Find("ChoiceUI"),
            "Choice Left",
            "Choice1",
            "/Collider",
            "/Image (1)/Borders/DarkBorder"
            );
        choiceMenu.left.SetTriggerIconLocation(0, 335);
        choiceMenu.right = new(eventManager.Find("ChoiceUI"),
            "Choice Right",
            "Choice2",
            "/Collider",
            "/Image (1)/Borders/DarkBorder"
            );
        choiceMenu.right.SetTriggerIconLocation(0, 335);
        VSVRMod.logger.LogInfo("Setup ChoiceUI");
    }

    public override bool Interact()
    {
        if (choiceMenu.representative.activeSelf)
        {
            double angle = Controller.GetMaximalJoystickAngle();
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            choiceMenu.left.Highlight(false);
            choiceMenu.favorite.Highlight(false);
            choiceMenu.right.Highlight(false);
            VSGenericButton theButton = null;
            if (magnitude > 0.3)
            {
                if (angle > 0 && angle < 60)
                {
                    theButton = choiceMenu.right;
                }
                else if (angle > 60 && angle < 120)
                {
                    if (choiceMenu.favorite.components.buttonObject.activeSelf)
                    {
                        theButton = choiceMenu.favorite;
                    }
                }
                else if (angle > 120 && angle < 180)
                {
                    theButton = choiceMenu.left;
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

