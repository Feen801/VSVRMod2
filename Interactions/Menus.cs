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

    public void Populate(Transform knownParent, string name, string path)
    {
        this.name = name;
        Transform buttonObject = knownParent.Find(path);
        if (buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button object");
        }
        this.buttonObject = buttonObject.gameObject;
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
        VSVRMod.logger.LogInfo("Verified findom button: " + this.name);
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
        public TMP_InputField text;
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

    private struct Scoreboard
    {
        public GameObject representative;
        public VSFindomButton mainMenu;
    }
    static Scoreboard scoreboard = new Scoreboard();

    public static void SetupMenus()
    {
        Transform eventManager = GameObject.Find("GeneralCanvas/EventManager").transform;
        Transform overlayCanvas = GameObject.Find("Root/OverlayCanvas").transform;

        //ChoiceUI-------------------
        choiceMenu.representative = eventManager.Find("ChoiceUI").gameObject;
        if (choiceMenu.representative == null)
        {
            VSVRMod.logger.LogError("ChoiceUI had null representative");
        }
        choiceMenu.favorite = new();
        choiceMenu.favorite.Populate(
            eventManager.Find("Buttons"),
            "Favorite Heart",
            "FavoriteHeart",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        choiceMenu.left = new();
        choiceMenu.left.Populate(
            eventManager.Find("ChoiceUI"),
            "Choice Left",
            "Choice1",
            "/Collider",
            "/Image (1)/Borders/DarkBorder"
            );
        choiceMenu.right = new();
        choiceMenu.right.Populate(
            eventManager.Find("ChoiceUI"),
            "Choice Right",
            "Choice2",
            "/Collider",
            "/Image (1)/Borders/DarkBorder"
            );
        VSVRMod.logger.LogInfo("Setup ChoiceUI");

        //StakesUI-------------------
        stakesMenu.representative = eventManager.Find("StakesUI").gameObject;
        if (stakesMenu.representative == null)
        {
            VSVRMod.logger.LogError("StakesUI had null representative");
        }
        stakesMenu.top = new();
        stakesMenu.top.Populate(
            eventManager.Find("StakesUI"),
            "Stakes Top",
            "BG1",
            "/Collider",
            "/Borders/DarkBorder"
            );
        stakesMenu.middle = new();
        stakesMenu.middle.Populate(
            eventManager.Find("StakesUI"),
            "Stakes Middle",
            "BG2",
            "/Collider",
            "/Borders/DarkBorder"
            );
        stakesMenu.bottom = new();
        stakesMenu.bottom.Populate(
            eventManager.Find("StakesUI"),
            "Stakes Bottom",
            "BG3",
            "/Collider",
            "/Borders/DarkBorder"
            );
        VSVRMod.logger.LogInfo("Setup StakesUI");

        //Safeword-------------------
        safewordMenu.representative = eventManager.Find("Buttons/EndSession").gameObject;
        if (safewordMenu.representative == null)
        {
            VSVRMod.logger.LogError("Safeword had null representative");
        }
        safewordMenu.goEasy = new();
        safewordMenu.goEasy.Populate(
            eventManager.Find("Buttons"),
            "Safeword Go Easy",
            "GoEasy",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        safewordMenu.continueSession = new();
        safewordMenu.continueSession.Populate(
            eventManager.Find("Buttons"),
            "Safeword Continue",
            "ContinueSession",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        safewordMenu.endSession = new();
        safewordMenu.endSession.Populate(
            eventManager.Find("Buttons"),
            "Safeword End Session",
            "EndSession",
            "/DoneBG/DoneText/Collider",
            "/DoneBG/DoneText/Collider/ButtonPressReact1"
            );
        VSVRMod.logger.LogInfo("Setup Safeword");

        //Count Input-------------------
        intInput.representative = eventManager.Find("IntInputField").gameObject;
        if (intInput.representative == null)
        {
            VSVRMod.logger.LogError("Int input had null representative");
        }

        intInput.text = intInput.representative.GetComponent<TMP_InputField>();
        if (intInput.text == null)
        {
            VSVRMod.logger.LogError("Int input text was null");
        }
        VSVRMod.logger.LogInfo("Setup Count Input");

        //Tribute Menu
        findomInput.representative = overlayCanvas.Find("TributeMenu").gameObject;
        if (findomInput.representative == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null representative");
        }

        findomInput.sendOptions = new List<VSFindomButton>();
        findomInput.cancel = new VSFindomButton();
        findomInput.cancel.Populate(overlayCanvas, "Tribute Cancel", "TributeMenu/Cancel");
        findomInput.sliderObject = overlayCanvas.Find("TributeMenu/Slider - Standard (Value)").gameObject;
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

        tributeButton.Populate(overlayCanvas, "Tribute Button", "TributeMenu/Options/Group/Text (TMP) (7)/Button");
        bribeButton.Populate(overlayCanvas, "Bribe Button", "TributeMenu/Options/Group/Text (TMP) (6)/Button");
        placateButton.Populate(overlayCanvas, "Placate Button", "TributeMenu/Options/Group/Text (TMP) (8)/Button");
        comfortButton.Populate(overlayCanvas, "Comfort Button", "TributeMenu/Options/Group/Text (TMP) (9)/Button");

        //From bottom to top
        findomInput.sendOptions.Add(comfortButton);
        findomInput.sendOptions.Add(placateButton);
        findomInput.sendOptions.Add(bribeButton);
        findomInput.sendOptions.Add(tributeButton);

        VSVRMod.logger.LogInfo("Setup Tribute Menu");

        Transform scoreCanvas = GameObject.Find("ScoreCanvas").transform;
        scoreboard.representative = scoreCanvas.Find("Scoreboard").gameObject;
        scoreboard.mainMenu = new VSFindomButton();
        scoreboard.mainMenu.Populate(scoreCanvas, "Main Menu", "Scoreboard/Finish/Button");
        VSVRMod.logger.LogInfo("Setup Scoreboard");
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
                if(Controller.WasATriggerClicked(105))
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
                if (Controller.WasATriggerClicked(105))
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
                if (Controller.WasATriggerClicked(106))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }

    private static float intInputInteractionNext = 0;
    private static float intInputInteractionAccel = 0.4f;
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
                VSVRMod.logger.LogWarning("CJ ");
                if (intInputInteractionNext < Time.time)
                {
                    string current = intInput.text.text;
                    VSVRMod.logger.LogWarning("C " + current);
                    int.TryParse(current, out int currentInt);
                    currentInt += (int)Math.Round(x * 4);
                    VSVRMod.logger.LogWarning("CI " + currentInt);
                    intInputInteractionNext = Time.time + intInputInteractionAccel;
                    intInputInteractionAccel = Math.Clamp(intInputInteractionAccel - 0.04f, 0.04f, float.PositiveInfinity);
                    intInput.text.text = currentInt.ToString();
                }
                return true;
            }
            else
            {
                intInputInteractionAccel = 0.4f;
            }
        }
        return false;
    }

    private static float findomInputInteractionNext = 0;
    private static float findomInputInteractionAccel = 0.4f;
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
            if (Controller.WasAStickClicked(106))
            {
                findomInputInteractState = !findomInputInteractState;
                return true;
            }
            if (findomInputInteractState)
            {
                if(magnitude > 0.05)
                {
                    if (findomInputInteractionNext < Time.time)
                    {
                        int currentInt = (int)findomInput.slider.value;
                        currentInt += (int)Math.Round(x * 4);
                        findomInputInteractionNext = Time.time + findomInputInteractionAccel;
                        findomInputInteractionAccel = Math.Clamp(findomInputInteractionAccel - 0.04f, 0.04f, float.PositiveInfinity);
                        currentInt = Math.Clamp(currentInt, (int)findomInput.slider.minValue, (int)findomInput.slider.maxValue);
                        findomInput.slider.value = currentInt;
                    }
                }
                else
                {
                    findomInputInteractionAccel = 0.4f;
                }
                return true;
            }
            else
            {
                findomInput.cancel.Highlight(false);
                List<VSFindomButton> activeFindomButtons = new List<VSFindomButton>();
                foreach (VSFindomButton button in findomInput.sendOptions)
                {
                    if (button.buttonObject.transform.parent.gameObject.activeSelf)
                    {
                        activeFindomButtons.Add(button);
                        button.Highlight(false);
                    }
                }
                double sectionSize = 2.0 / (activeFindomButtons.Count + 1);
                VSFindomButton theButton = findomInput.cancel;
                if(y > -1.0 + sectionSize)
                {
                    theButton = activeFindomButtons[(int)Math.Floor((y + 1.0) / sectionSize) - 1];
                }
                if (theButton != null)
                {
                    theButton.Highlight(true);
                    if (Controller.WasATriggerClicked(777))
                    {
                        theButton.Click();
                    }
                    return true;
                }
            }
        }
        return false;
    }
    
    public static bool ScoreboardInteract()
    {
        if(!scoreboard.representative.activeSelf)
        {
            return false;
        }
        if(Controller.WasAFaceButtonClicked(200) || Controller.WasAStickClicked(200) || Controller.WasATriggerClicked(200))
        {
            scoreboard.mainMenu.Click();
        }
        return true;
    }
}