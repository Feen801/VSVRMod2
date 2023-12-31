﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Tilemaps.Tilemap;

namespace VSVRMod2;

public struct VSButtonComponents
{
    public GameObject collider;
    public PlayMakerFSM buttonFsm;
    public GameObject buttonObject;
    public GameObject highlight;
}

public class VSGenericButton
{
    public string name;
    public VSButtonComponents components = new VSButtonComponents();
}

public class VSChoiceButton : VSGenericButton
{
    public static string colliderPath = "DoneBG/DoneText/Collider";
    public static string highlightPath = "DoneBG/DoneText/Collider/ButtonPressReact1";

    public enum ButtonType
    {
        Positive,
        Negative,
    }

    public ButtonType type;
}

public class VSRadialButton : VSGenericButton
{
    public double minDegrees;
    public double maxDegrees;
    public double maxMagnitude = 1;

    public enum RadialLevel
    {
        None = 0x00,
        Both = 0xFF,
        Level1 = 0x0F,
        Level2 = 0xF0
    }

    public RadialLevel radialLevel;
}

public class Buttons
{
    public static List<VSChoiceButton> vsChoiceButtons = new List<VSChoiceButton>();

    public static List<VSRadialButton> vsRadialButtons = new List<VSRadialButton>();
    public static List<VSRadialButton> vsStakesButtons = new List<VSRadialButton>();
    public static List<VSRadialButton> vsClothesButtons = new List<VSRadialButton>();

    public static void SetupChoiceButtons()
    {
        GameObject positiveButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Positives ------------");
        GameObject negativeButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Negatives ------------");

        foreach (Transform positiveButton in positiveButtonParent.transform)
        {
            VSChoiceButton positiveChoiceButton = new VSChoiceButton();
            positiveChoiceButton.type = VSChoiceButton.ButtonType.Positive;
            positiveChoiceButton.name = positiveButton.name;
            VSVRMod.logger.LogInfo("Found pos choice button: " + positiveChoiceButton.name);
            if (Equals(positiveChoiceButton.name, "PoTMercy"))
            {
                //?????? what is this button succudev?
                continue;
            }
            positiveChoiceButton.components.buttonFsm = positiveButton.Find(VSChoiceButton.colliderPath).GetComponent<PlayMakerFSM>();
            positiveChoiceButton.components.buttonObject = positiveButton.gameObject;
            positiveChoiceButton.components.highlight = positiveButton.Find(VSChoiceButton.highlightPath).gameObject;
            vsChoiceButtons.Add(positiveChoiceButton);
        }

        foreach (Transform negativeButton in negativeButtonParent.transform)
        { 
            VSChoiceButton negativeChoiceButton = new VSChoiceButton();
            negativeChoiceButton.type = VSChoiceButton.ButtonType.Negative;
            negativeChoiceButton.name = negativeButton.name;
            VSVRMod.logger.LogInfo("Found neg choice button: " + negativeChoiceButton.name);
            negativeChoiceButton.components.buttonFsm = negativeButton.Find(VSChoiceButton.colliderPath).GetComponent<PlayMakerFSM>();
            negativeChoiceButton.components.buttonObject = negativeButton.gameObject;
            negativeChoiceButton.components.highlight = negativeButton.Find(VSChoiceButton.highlightPath).gameObject;
            vsChoiceButtons.Add(negativeChoiceButton);
        }
    }

    private static GameObject level1;
    private static GameObject level2;

    private static VSGenericButton circle;
    private static VSGenericButton level2Arrow;
    private static GameObject exitButtonRadial;

    public static void SetupRadialButtons()
    {
        exitButtonRadial = GameObject.Find("GeneralCanvas/EventManager/NewButtons/Center/Level1/Exit/Collider");

        level1 = GameObject.Find("NewButtons/Center/Level1");
        level2 = GameObject.Find("NewButtons/Center/Level2");

        //so much fun doing all these
        circle = new VSGenericButton();
        circle.name = "Radial Circle";
        PrepareButtonComponents(circle, "NewButtons/Center/Circle");
        CheckButtonComponents(circle);

        level2Arrow = new VSGenericButton();
        level2Arrow.name = "Level 2 Arrow";
        PrepareButtonComponents(level2Arrow, "NewButtons/Center/Level1/OtherButtons/Grow");
        CheckButtonComponents(level2Arrow);

        VSRadialButton tribute = new VSRadialButton();
        tribute.name = "Tribute";
        tribute.radialLevel = VSRadialButton.RadialLevel.Both;
        tribute.maxMagnitude = 1;
        tribute.minDegrees = 60;
        tribute.maxDegrees = 120;
        PrepareButtonComponents(tribute, "NewButtons/Center/Level1/OtherButtons/TributeBG");

        VSRadialButton mercy = new VSRadialButton();
        mercy.name = "Mercy";
        mercy.radialLevel = VSRadialButton.RadialLevel.Level1;
        mercy.maxMagnitude = 1;
        mercy.minDegrees = 120;
        mercy.maxDegrees = 180;
        PrepareButtonComponents(mercy, "NewButtons/Center/Level1/OtherButtons/MercyBG");

        VSRadialButton edge = new VSRadialButton();
        edge.name = "Edge";
        edge.radialLevel = VSRadialButton.RadialLevel.Level1;
        edge.maxMagnitude = 0.9;
        edge.minDegrees = 60;
        edge.maxDegrees = 120;
        PrepareButtonComponents(edge, "NewButtons/Center/Level1/OtherButtons/EdgeBG");

        VSRadialButton good = new VSRadialButton();
        good.name = "Good";
        good.radialLevel = VSRadialButton.RadialLevel.Level1;
        good.maxMagnitude = 0.9;
        good.minDegrees = 60;
        good.maxDegrees = 120;
        PrepareButtonComponents(good, "NewButtons/Center/Level1/OtherButtons/KeepGoingBG");

        VSRadialButton taunt = new VSRadialButton();
        taunt.name = "Taunt";
        taunt.radialLevel = VSRadialButton.RadialLevel.Level1;
        taunt.maxMagnitude = 1;
        taunt.minDegrees = 0;
        taunt.maxDegrees = 60;
        PrepareButtonComponents(taunt, "NewButtons/Center/Level1/OtherButtons/TauntBG");

        VSRadialButton hideui = new VSRadialButton();
        hideui.name = "Hide UI";
        hideui.radialLevel = VSRadialButton.RadialLevel.Level2;
        hideui.maxMagnitude = 1;
        hideui.minDegrees = 135;
        hideui.maxDegrees = 180;
        PrepareButtonComponents(hideui, "NewButtons/Center/Level2/GameObject/Hide UI");

        VSRadialButton timeout = new VSRadialButton();
        timeout.name = "Timeout";
        timeout.radialLevel = VSRadialButton.RadialLevel.Level2;
        timeout.maxMagnitude = 0.9;
        timeout.minDegrees = 90;
        timeout.maxDegrees = 135;
        //super extra special button has collider named "Collider (1)" instead of just "Collider"
        timeout.components.buttonObject = GameObject.Find("NewButtons/Center/Level2/GameObject/Time Out");
        timeout.components.collider = GameObject.Find("NewButtons/Center/Level2/GameObject/Time Out/Collider (1)");
        if (timeout.components.collider == null)
        {
            VSVRMod.logger.LogError(timeout.name + " had null collider");
        }
        Assert.IsNotNull(timeout.components.collider);
        timeout.components.buttonFsm = timeout.components.collider.GetComponent<PlayMakerFSM>();
        timeout.components.highlight = GameObject.Find("NewButtons/Center/Level2/GameObject/Time Out/Collider (1)/ButtonReact");


        VSRadialButton safeword = new VSRadialButton();
        safeword.name = "Safeword";
        safeword.radialLevel = VSRadialButton.RadialLevel.Level2;
        safeword.maxMagnitude = 0.9;
        safeword.minDegrees = 45;
        safeword.maxDegrees = 90;
        PrepareButtonComponents(safeword, "NewButtons/Center/Level2/GameObject (1)/Safe Word");

        VSRadialButton oops = new VSRadialButton();
        oops.name = "Oops";
        oops.radialLevel = VSRadialButton.RadialLevel.Level2;
        oops.maxMagnitude = 1;
        oops.minDegrees = 0;
        oops.maxDegrees = 45;
        PrepareButtonComponents(oops, "NewButtons/Center/Level2/GameObject (1)/Oops");

        VSRadialButton plus = new VSRadialButton();
        plus.name = "Plus";
        plus.radialLevel = VSRadialButton.RadialLevel.Both;
        plus.maxMagnitude = 1;
        plus.minDegrees = 270;
        plus.maxDegrees = 360;
        PrepareButtonComponents(plus, "NewButtons/Center/Level1/ArousalMeter/Overlays/Plus");

        VSRadialButton minus = new VSRadialButton();
        minus.name = "Minus";
        minus.radialLevel = VSRadialButton.RadialLevel.Both;
        minus.maxMagnitude = 1;
        minus.minDegrees = 180;
        minus.maxDegrees = 270;
        PrepareButtonComponents(minus, "NewButtons/Center/Level1/ArousalMeter/Overlays/Minus");

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

        foreach (var button in vsRadialButtons)
        {
            CheckButtonComponents(button);
        }
    }

    private static void PrepareButtonComponents(VSGenericButton button, string basePath)
    {
        button.components.buttonObject = GameObject.Find(basePath);
        button.components.collider = GameObject.Find(basePath + "/Collider");
        button.components.highlight = GameObject.Find(basePath + "/Collider/ButtonReact");
        if (button.components.collider == null)
        {
            VSVRMod.logger.LogError(button.name + " had null collider");
        }
        Assert.IsNotNull(button.components.collider);
        button.components.buttonFsm = button.components.collider.GetComponent<PlayMakerFSM>();
    }

    private static void CheckButtonComponents(VSGenericButton button)
    {
        VSVRMod.logger.LogInfo("Verifying button: " + button.name);
        if (button.components.buttonObject == null )
        {
            VSVRMod.logger.LogError(button.name + " had null buttonObject");
        }
        if (button.components.collider == null)
        {
            VSVRMod.logger.LogError(button.name + " had null collider");
        }
        if (button.components.buttonFsm == null)
        {
            VSVRMod.logger.LogError(button.name + " had null buttonFsm");
        }
        if (button.components.highlight == null)
        {
            VSVRMod.logger.LogError(button.name + " had null highlight");
        }
        Assert.IsNotNull(button.components.buttonObject);
        Assert.IsNotNull(button.components.collider);
        Assert.IsNotNull(button.components.buttonFsm);
        Assert.IsNotNull(button.components.highlight);
    }

    private static void ClickButton(VSGenericButton button)
    {
        button.components.buttonFsm.SendEvent("Click");
    }

    private static VSRadialButton.RadialLevel currentRadialLevel = VSRadialButton.RadialLevel.None;

    public static void RadialMenuInteract()
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
                    ClickButton(circle);
                    break;
                case VSRadialButton.RadialLevel.Level1:
                    ClickButton(level2Arrow); 
                    break;
                case VSRadialButton.RadialLevel.Level2:
                    exitButtonRadial.GetComponent<PlayMakerFSM>().SendEvent("Click");
                    break;
                default:
                    VSVRMod.logger.LogError("Unexpected Radial State");
                    break;
            }
        }

        if (stickMagnitude > 0.3) { 
            List<VSRadialButton> candidateButtons = new List<VSRadialButton>();

            foreach (VSRadialButton button in vsRadialButtons)
            {
                if (button.minDegrees < stickDirection && button.maxDegrees > stickDirection && button.components.buttonObject.activeSelf) {
                    if ((button.radialLevel & currentRadialLevel) != 0x00)
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
                    ClickButton(trueButton);
                }
            }
        }
    }

    public static void PostiveAction() 
    {
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.type == VSChoiceButton.ButtonType.Positive && button.components.buttonObject.activeSelf)
            {
                VSVRMod.logger.LogInfo("Trying to click: " + button.name);
                ClickButton(button);
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
                ClickButton(button);
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
