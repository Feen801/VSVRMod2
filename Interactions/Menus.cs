using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VSVRMod2;
public class VSFindomButton
{
    static Color darkGrey = new Color(0.15f, 0.15f, 0.15f, 1);
    public string name;
    public Button button;
    public GameObject buttonObject;
    public Image highlight;

    public void Highlight(bool status)
    {
        highlight.color = status ? Color.white : darkGrey;
    }

    public void Populate(string name, string path)
    {
        this.name = name;
        this.buttonObject = GameObject.Find(path);
        if (this.buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button object");
        }
        this.button = buttonObject.GetComponent<Button>();
        if (this.button == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button");
        }
        highlight = this.buttonObject.GetComponent<Image>();
        if (this.highlight == null)
        {
            VSVRMod.logger.LogError(this.name + " had null image");
        }
    }

    public void Click() 
    {
        this.button.OnSubmit(null);
    }
}

class Menus
{
    private struct ChoiceMenu
    {
        public GameObject representative;
        public VSGenericButton left;
        public VSGenericButton right;
        public VSGenericButton favorite;
    }
    static ChoiceMenu choiceMenu;

    private struct StakesMenu
    {
        public GameObject representative;
        public VSGenericButton top;
        public VSGenericButton middle;
        public VSGenericButton bottom;
    }
    static StakesMenu stakesMenu = new StakesMenu();

    private struct SafewordMenu
    {
        public GameObject representative;
        public VSGenericButton goEasy;
        public VSGenericButton continueSession;
        public VSGenericButton endSession;
    }
    static SafewordMenu safewordMenu = new SafewordMenu();

    private struct IntInput
    {
        public GameObject representative;
        public GameObject inputField;
        public TextMeshProUGUI text;
    }
    static IntInput intInput = new IntInput();

    private struct FindomInput
    {
        public GameObject representative;
        public List<VSFindomButton> sendOptions;
        public VSFindomButton cancel;
        public GameObject sliderObject;
        public Slider slider;
    }
    static FindomInput findomInput = new FindomInput();

    public static void SetupMenus()
    {
        //ChoiceUI-------------------
        choiceMenu.representative = GameObject.Find("ChoiceUI");
        if (choiceMenu.representative == null)
        {
            VSVRMod.logger.LogError("ChoiceUI had null representative");
        }

        choiceMenu.favorite = PrepareUnusualButtonComponents(
            "Favorite Heart",
            "FavoriteHeart",
            "FavoriteHeart/DoneBG/DoneText/Collider",
            "FavoriteHeart/DoneBG/DoneText/Collider/ButtonPressReact"
            );

        choiceMenu.left = PrepareUnusualButtonComponents(
            "Choice Left",
            "ChoiceUI/Choice1",
            "ChoiceUI/Choice1/Collider",
            "ChoiceUI/Choice1/Image (1)/Borders/DarkBorder"
            );

        choiceMenu.right = PrepareUnusualButtonComponents(
            "Choice Right",
            "ChoiceUI/Choice2",
            "ChoiceUI/Choice2/Collider",
            "ChoiceUI/Choice2/Image (1)/Borders/DarkBorder"
            );

        //StakesUI-------------------
        stakesMenu.representative = GameObject.Find("StakesUI");
        if (stakesMenu.representative == null)
        {
            VSVRMod.logger.LogError("StakesUI had null representative");
        }

        stakesMenu.top = PrepareUnusualButtonComponents(
            "Stakes Top",
            "StakesUI/BG1",
            "StakesUI/BG1/Collider",
            "StakesUI/BG1/Borders/DarkBorder"
            );
        stakesMenu.middle = PrepareUnusualButtonComponents(
            "Stakes Middle",
            "StakesUI/BG2",
            "StakesUI/BG2/Collider",
            "StakesUI/BG2/Borders/DarkBorder"
            );
        stakesMenu.bottom = PrepareUnusualButtonComponents(
            "Stakes Bottom",
            "StakesUI/BG3",
            "StakesUI/BG3/Collider",
            "StakesUI/BG3/Borders/DarkBorder"
            );

        //Safeword-------------------
        safewordMenu.representative = GameObject.Find("Buttons/EndSession");
        if (safewordMenu.representative == null)
        {
            VSVRMod.logger.LogError("Safeword had null representative");
        }

        safewordMenu.goEasy = PrepareUnusualButtonComponents(
            "Safeword Go Easy",
            "Buttons/GoEasy",
            "Buttons/GoEasy/DoneBG/DoneText/Collider",
            "Buttons/GoEasy/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        safewordMenu.continueSession = PrepareUnusualButtonComponents(
            "Safeword Continue",
            "Buttons/ContinueSession",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        safewordMenu.endSession = PrepareUnusualButtonComponents(
            "Safeword End Session",
            "Buttons/EndSession",
            "Buttons/EndSession/DoneBG/DoneText/Collider",
            "Buttons/EndSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );

        //Count Input-------------------
        intInput.representative = GameObject.Find("EventManager/IntInputField");
        if (intInput.representative == null)
        {
            VSVRMod.logger.LogError("Int input had null representative");
        }

        intInput.inputField = GameObject.Find("EventManager/IntInputField/Text Area/IntInputFieldText");
        if (intInput.inputField == null)
        {
            VSVRMod.logger.LogError("Int input field was null");
        }
        intInput.text = intInput.inputField.GetComponent<TextMeshProUGUI>();
        if (intInput.text == null)
        {
            VSVRMod.logger.LogError("Int input text was null");
        }

        //Tribute Menu
        findomInput.representative = GameObject.Find("OverlayCanvas/TributeMenu");
        if (findomInput.representative == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null representative");
        }

        findomInput.sendOptions = new List<VSFindomButton>();
        findomInput.cancel = new VSFindomButton();
        findomInput.cancel.Populate("Tribute Cancel", "TributeMenu/Cancel");
        findomInput.sliderObject = GameObject.Find("OverlayCanvas/TributeMenu/Slider - Standard (Value)");
        if (findomInput.sliderObject == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null slider object");
        }
        findomInput.slider = findomInput.sliderObject.GetComponent<Slider>();
        if (findomInput.slider == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null slider");
        }
        VSFindomButton tributeButton = new VSFindomButton();
        VSFindomButton bribeButton = new VSFindomButton();
        VSFindomButton placateButton = new VSFindomButton();
        VSFindomButton comfortButton = new VSFindomButton();

        tributeButton.Populate("Tribute Button", "TributeMenu/Options/Group/Text (TMP) (7)");
        bribeButton.Populate("Bribe Button", "TributeMenu/Options/Group/Text (TMP) (6)");
        placateButton.Populate("Placate Button", "TributeMenu/Options/Group/Text (TMP) (8)");
        comfortButton.Populate("Comfort Button", "TributeMenu/Options/Group/Text (TMP) (9)");

        findomInput.sendOptions.Add(tributeButton);
        findomInput.sendOptions.Add(bribeButton);
        findomInput.sendOptions.Add(placateButton);
        findomInput.sendOptions.Add(comfortButton);
    }

    private static VSGenericButton PrepareUnusualButtonComponents(string name, string buttonObjectPath, string colliderPath, string highlightPath)
    {
        VSGenericButton button = new VSGenericButton();
        button.name = name;
        button.components.buttonObject = GameObject.Find(buttonObjectPath);
        button.components.collider = GameObject.Find(colliderPath);
        if(button.components.buttonObject == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button object");
        }
        if (button.components.collider == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button collider");
        }
        button.components.buttonFsm = choiceMenu.favorite.components.collider.GetComponent<PlayMakerFSM>();
        if (button.components.buttonFsm == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button buttonFsm");
        }
        button.components.highlight = GameObject.Find(highlightPath);
        if (button.components.highlight == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button highlight");
        }
        return button;
    }

    public static bool ChoiceMenuInteract()
    {
        if(choiceMenu.representative.activeSelf)
        {
            double angle = Controller.GetMaximalJoystickAngle();
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            choiceMenu.left.Highlight(false);
            choiceMenu.favorite.Highlight(false);
            choiceMenu.right.Highlight(false);
            VSGenericButton theButton = null;
            if (magnitude > 0.3) { 
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
                else if(angle > 120 && angle < 180)
                {
                    theButton = choiceMenu.left;
                }
            }
            if (theButton != null) {
                theButton.Highlight(true);
                if(Controller.WasATriggerClicked(101))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }

    public static bool StakesMenuInteract()
    {
        if (stakesMenu.representative.activeSelf)
        {
            double y = Controller.GetMaximalJoystickValue().y;
            stakesMenu.top.Highlight(false);
            stakesMenu.middle.Highlight(false);
            stakesMenu.bottom.Highlight(false);
            VSGenericButton theButton = null;
            if (y > 0.1 && y < 0.4)
            {
                theButton = stakesMenu.bottom;
            }
            if (y > 0.4 && y < 0.7)
            {
                theButton = stakesMenu.middle;
            }
            if (y > 0.7)
            {
                theButton = stakesMenu.top;
            }
            if (theButton != null)
            {
                theButton.Highlight(true);
                if (Controller.WasATriggerClicked(102))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }

    public static bool SafewordMenuInteract()
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
                if (Controller.WasATriggerClicked(103))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }

    private static float intInputInteractionNext = 0;
    public static bool IntInputInteract()
    {
        if (intInput.representative.activeSelf)
        {
            Vector2 vector2 = Controller.GetMaximalJoystickValue();
            double x = vector2.x;
            double y = vector2.y;
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            if (y > -0.5 && magnitude > 0.05)
            {
                if (intInputInteractionNext < Time.time)
                {
                    string current = intInput.text.text;
                    int currentInt = 0;
                    int.TryParse(current, out currentInt);
                    currentInt += (int)Math.Round(x) * 4;
                    intInputInteractionNext = Time.time + 0.4f;
                    intInput.text.SetText(currentInt.ToString());
                }
                return true;
            }
        }
        return false;
    }

    private static float findomInputInteractionNext = 0;
    //false = controlling buttons, true = controlling slider
    private static bool findomInputInteractState = false;
    public static bool FindomInputInteract()
    {
        if (findomInput.representative.activeSelf)
        {
            Vector2 vector2 = Controller.GetMaximalJoystickValue();
            double x = vector2.x;
            double y = vector2.y;
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            if (findomInputInteractState)
            {
                if(magnitude > 0.05)
                {
                    if (findomInputInteractionNext < Time.time)
                    {
                        int currentInt = (int)findomInput.slider.value;
                        currentInt += (int)Math.Round(x) * 4;
                        findomInputInteractionNext = Time.time + 0.4f;
                        currentInt = Math.Clamp(currentInt, (int)findomInput.slider.minValue, (int)findomInput.slider.maxValue);
                        findomInput.slider.value = currentInt;
                    }
                }
            }
            else
            {
                findomInput.cancel.Highlight(false);
                List<VSFindomButton> activeFindomButtons = new List<VSFindomButton>();
                foreach (VSFindomButton button in findomInput.sendOptions)
                {
                    if (button.buttonObject.activeSelf)
                    {
                        activeFindomButtons.Add(button);
                        button.Highlight(false);
                    }
                }
                double sectionSize = 2.0 / (activeFindomButtons.Count + 1);
                VSFindomButton theButton = findomInput.cancel;
                if(x > -1.0 + sectionSize)
                {
                    theButton = activeFindomButtons[(int)Math.Floor((x + 1.0) / sectionSize) - 1];
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
            if (Controller.WasAStickClicked(106))
            {
                findomInputInteractState = !findomInputInteractState;
            }
        }
        return false;
    }
}