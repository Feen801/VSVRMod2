using System.Collections.Generic;
using UnityEngine;

namespace VSVRMod2;
public class BasicUI
{
    public static List<VSChoiceButton> vsChoiceButtons = [];

    public static List<VSRadialButton> vsRadialButtons = [];
    public static List<VSRadialButton> vsStakesButtons = [];
    public static List<VSRadialButton> vsClothesButtons = [];

    public static List<VSGenericButton> vsOpportunityButtons = [];
    public static VSGenericButton giveInButton = null;

    public static void SetupChoiceButtons()
    {
        //Reference each postive (left) and negative (right) button by looping through the childern of each parent
        GameObject positiveButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Positives ------------");
        GameObject negativeButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Negatives ------------");

        foreach (Transform positiveButton in positiveButtonParent.transform)
        {
            if (Equals(positiveButton.name, "PoTMercy"))
            {
                //?????? what is this button succudev?
                continue;
            }
            VSChoiceButton positiveChoiceButton = new(positiveButtonParent.transform, positiveButton.name, positiveButton.name, VSChoiceButton.ButtonType.Positive);
            VSVRMod.logger.LogInfo("Found pos choice button: " + positiveChoiceButton.name);
            vsChoiceButtons.Add(positiveChoiceButton);
        }

        foreach (Transform negativeButton in negativeButtonParent.transform)
        { 
            VSChoiceButton negativeChoiceButton = new(negativeButtonParent.transform, negativeButton.name, negativeButton.name, VSChoiceButton.ButtonType.Negative);
            VSVRMod.logger.LogInfo("Found neg choice button: " + negativeChoiceButton.name);
            vsChoiceButtons.Add(negativeChoiceButton);
        }
    }

    public static bool ChoiceButtonInteract()
    {
        bool triggerClick = Controller.WasATriggerClicked(776);
        double x = Controller.GetMaximalJoystickValue().x;
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.components.buttonObject.activeSelf)
            {
                button.Highlight(false);
                if (x < -0.3 && button.type == VSChoiceButton.ButtonType.Positive)
                {
                    button.Highlight(true);
                    if(triggerClick)
                    {
                        button.Click();
                        return true;
                    }
                }
                if (x > 0.3 && button.type == VSChoiceButton.ButtonType.Negative)
                {
                    button.Highlight(true);
                    if (triggerClick)
                    {
                        button.Click();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static void SetupOtherButtons()
    {
        Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;
        giveInButton= new VSGenericButton(parent, "Give In", "Urges/ActionTextContainer/GiveIn/GiveInButton");

        VSGenericButton opportunityProvoke = new(parent, "Provoke", "Buttons/OpportunityProvoke");
        VSGenericButton opportunityTaunt = new(parent, "Taunt", "Buttons/OpportunityTaunt");
        VSGenericButton opportunityEntice = new(parent, "Entice", "Buttons/OpportunityEntice");
        VSGenericButton opportunityPraise = new(parent, "Praise", "Buttons/OpportunityPraise");

        vsOpportunityButtons.Add(opportunityProvoke);
        vsOpportunityButtons.Add(opportunityTaunt);
        vsOpportunityButtons.Add(opportunityEntice);
        vsOpportunityButtons.Add(opportunityPraise);

        VSVRMod.logger.LogInfo("Finished setting up other buttons");
    }

    public static bool TemporaryButtonInteract()
    {
        bool faceButtonClicked = Controller.WasAFaceButtonClicked(775);
        if (giveInButton.components.buttonObject.activeSelf && faceButtonClicked)
        {
            giveInButton.Click();
            return true;
        }
        foreach (VSGenericButton button in vsOpportunityButtons)
        {
            if (button.components.buttonObject.activeSelf && faceButtonClicked)
            {
                button.Click();
                return true;
            }
        }
        return false;
    }

    private static GameObject level1;
    private static GameObject level2;

    private static VSGenericButton circle;
    private static VSGenericButton level2Arrow;
    private static GameObject exitButtonRadial;

    public static void SetupRadialButtons()
    {
        GameObject centerGameObject = GameObject.Find("NewButtons/Center");
        if (centerGameObject == null)
        {
            VSVRMod.logger.LogError("centerGameObject not found.");
        }
        Transform center = centerGameObject.transform;

        Transform exitButtonRadialTransform = center.Find("Level1/Exit");
        if (exitButtonRadialTransform == null)
        {
            VSVRMod.logger.LogError("Radial exit not found.");
        }
        exitButtonRadial = exitButtonRadialTransform.gameObject;

        Transform level1Transform = center.Find("Level1");
        if (level1Transform == null)
        {
            VSVRMod.logger.LogError("level1Transform not found.");
        }
        level1 = center.Find("Level1").gameObject;

        Transform level2Transform = center.Find("Level2");
        if (level2Transform == null)
        {
            VSVRMod.logger.LogError("level2Transform not found.");
        }
        level2 = level2Transform.gameObject;

        //so much fun doing all these
        circle = new VSGenericButton(center, "Radial Circle", "Circle", "/Collider", "/Collider/ButtonReact");

        level2Arrow = new VSGenericButton(center, "Level 2 Arrow", "Level1/OtherButtons/Grow", "/Collider", "/Collider/ButtonReact");

        VSRadialButton tribute = new(center, "Tribute", "Level1/OtherButtons/TributeBG", 1, 60, 120, VSRadialButton.RadialLevel.Both);

        VSRadialButton mercy = new(center, "Mercy", "Level1/OtherButtons/MercyBG", 1, 120, 180, VSRadialButton.RadialLevel.Level1);

        //Good and Edge both appear at the same location, but not at the same time
        VSRadialButton edge = new(center, "Edge", "Level1/OtherButtons/EdgeBG", 0.9, 60, 120, VSRadialButton.RadialLevel.Level1);
        VSRadialButton good = new(center, "Good", "Level1/OtherButtons/KeepGoingBG", 0.9, 60, 120, VSRadialButton.RadialLevel.Level1);

        VSRadialButton taunt = new(center, "Taunt", "Level1/OtherButtons/TauntBG", 1, 0, 60, VSRadialButton.RadialLevel.Level1);

        VSRadialButton hideui = new(center, "Hide UI", "Level2/GameObject/Hide UI", 1, 135, 180, VSRadialButton.RadialLevel.Level2);

        //Super extra special button has collider named "Collider (1)" instead of just "Collider"
        VSRadialButton timeout = new(center, "Timeout", "Level2/GameObject/Time Out", "/Collider (1)", "/Collider (1)/ButtonReact", 0.9, 90, 135, VSRadialButton.RadialLevel.Level2);

        VSRadialButton safeword = new(center, "Safeword", "Level2/GameObject (1)/Safe Word", 0.9, 45, 90, VSRadialButton.RadialLevel.Level2);

        VSRadialButton oops = new(center, "Oops", "Level2/GameObject (1)/Oops", 1, 0, 45, VSRadialButton.RadialLevel.Level2);

        VSRadialButton plus = new(center, "Plus", "Level1/ArousalMeter/Overlays/Plus", 1, 270, 360, VSRadialButton.RadialLevel.Both);

        VSRadialButton minus = new(center, "Minus", "Level1/ArousalMeter/Overlays/Minus", 1, 180, 270, VSRadialButton.RadialLevel.Both);

        vsRadialButtons.Add(tribute);
        vsRadialButtons.Add(mercy);
        vsRadialButtons.Add(good);
        vsRadialButtons.Add(edge);
        vsRadialButtons.Add(taunt);
        vsRadialButtons.Add(hideui);
        vsRadialButtons.Add(timeout);
        vsRadialButtons.Add(safeword);
        vsRadialButtons.Add(oops);
        vsRadialButtons.Add(plus);
        vsRadialButtons.Add(minus);

        VSVRMod.logger.LogInfo("Finished setting up radial buttons");
    }

    private static VSRadialButton.RadialLevel currentRadialLevel = VSRadialButton.RadialLevel.None;

    public static bool RadialMenuInteract()
    {
        bool stickClick = Controller.WasAStickClicked(777);
        bool triggerClick = Controller.WasATriggerClicked(777);
        double stickMagnitude = Controller.GetMaximalJoystickMagnitude();
        double stickDirection = Controller.GetMaximalJoystickAngle();

        if (!level1.activeSelf)
        {
            currentRadialLevel = VSRadialButton.RadialLevel.None;
        }
        else if (!level2.activeSelf)
        {
            currentRadialLevel = VSRadialButton.RadialLevel.Level1;
        }
        else
        {
            currentRadialLevel = VSRadialButton.RadialLevel.Level2;
        }

        if (stickClick)
        {
            switch (currentRadialLevel)
            {
                case VSRadialButton.RadialLevel.None:
                    circle.Click();
                    break;
                case VSRadialButton.RadialLevel.Level1:
                    level2Arrow.Click(); 
                    break;
                case VSRadialButton.RadialLevel.Level2:
                    exitButtonRadial.GetComponent<PlayMakerFSM>().SendEvent("Click");
                    break;
                default:
                    VSVRMod.logger.LogError("Unexpected Radial State");
                    break;
            }
        }

        if (stickMagnitude > 0.3 && currentRadialLevel != VSRadialButton.RadialLevel.None) {
            List<VSRadialButton> candidateButtons = [];
            foreach (VSRadialButton button in vsRadialButtons)
            {
                if (button.minDegrees < stickDirection && button.maxDegrees > stickDirection && button.components.buttonObject.activeSelf)
                {
                    if (button.IsOnRadialLevel(currentRadialLevel))
                    {
                        candidateButtons.Add(button);
                    }
                    else
                    {
                        button.components.highlight.SetActive(false);
                    }
                }
                else
                {
                    button.components.highlight.SetActive(false);
                }
            }

            VSRadialButton trueButton = null;

            foreach (VSRadialButton button in candidateButtons)
            {
                if (trueButton == null || (button.maxMagnitude < trueButton.maxMagnitude && button.maxMagnitude > stickMagnitude))
                {
                    if (trueButton != null)
                    {
                        trueButton.components.highlight.SetActive(false);
                    }
                    trueButton = button;
                }
                else
                {
                    button.components.highlight.SetActive(false);
                }
            }

            if (trueButton != null)
            {
                trueButton.components.highlight.SetActive(true);
                if (triggerClick)
                {
                    trueButton.Click();
                }
                return true;
            }
        }
        return false;
    }

    public static void PostiveAction() 
    {
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.type == VSChoiceButton.ButtonType.Positive && button.components.buttonObject.activeSelf)
            {
                VSVRMod.logger.LogInfo("Trying to click: " + button.name);
                button.Click();
            }
        }
    }

    public static void NegativeAction()
    {
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.type == VSChoiceButton.ButtonType.Negative && button.components.buttonObject.activeSelf)
            {
                VSVRMod.logger.LogInfo("Trying to click: " + button.name);
                button.Click();
            }
        }
    }

    public class HeadMovementTracker
    {
        static int nods;
        static int headshakes;

        static long lastNodTime;
        static long lastHeadshakeTime;

        public static void Nod()
        {
            headshakes = 0;
            if (MathHelper.CurrentTimeMillis() - lastNodTime > 1000)
            {
                nods = 0;
            }
            nods++;
            lastNodTime = MathHelper.CurrentTimeMillis();
            VSVRMod.logger.LogInfo("Partial Nod");
            if (nods >= 2) 
            {
                VSVRMod.logger.LogInfo("Full Nod");
                PostiveAction();
            }
        }
        public static void Headshake()
        {
            nods = 0;
            if(MathHelper.CurrentTimeMillis() - lastHeadshakeTime > 1000)
            {
                headshakes = 0;
            }
            headshakes++;
            lastHeadshakeTime = MathHelper.CurrentTimeMillis();
            VSVRMod.logger.LogInfo("Partial Headshake");
            if (headshakes >= 2)
            {
                VSVRMod.logger.LogInfo("Full Headshake");
                NegativeAction();
            }
        }
    }
}
