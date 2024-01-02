using System;
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

    public static void SetupRadialButtons()
    {
        level1 = GameObject.Find("NewButtons/Center/Level1");
        level2 = GameObject.Find("NewButtons/Center/Level2");

        //so much fun doing all these
        circle = new VSGenericButton();
        circle.name = "Radial Circle";
        circle.components.buttonObject = GameObject.Find("NewButtons/Center/Circle");
        circle.components.buttonFsm = GameObject.Find("NewButtons/Center/Circle/Collider").GetComponent<PlayMakerFSM>();
        circle.components.highlight = GameObject.Find("NewButtons/Center/Circle/Collider/ButtonReact");
        CheckButtonComponents(circle);

        level2Arrow = new VSGenericButton();
        level2Arrow.name = "Level 2 Arrow";
        level2Arrow.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/Grow");
        level2Arrow.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/Grow/Collider").GetComponent<PlayMakerFSM>();
        level2Arrow.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/Grow/Collider/ButtonReact");
        CheckButtonComponents(level2Arrow);

        VSRadialButton tribute = new VSRadialButton();
        tribute.name = "Tribute";
        tribute.radialLevel = VSRadialButton.RadialLevel.Both;
        tribute.maxMagnitude = 1;
        tribute.minDegrees = 60;
        tribute.maxDegrees = 120;
        tribute.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TributeBG");
        tribute.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TributeBG/Collider").GetComponent<PlayMakerFSM>();
        tribute.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TributeBG/Collider/ButtonReact");

        VSRadialButton mercy = new VSRadialButton();
        mercy.name = "Mercy";
        mercy.radialLevel = VSRadialButton.RadialLevel.Level1;
        mercy.maxMagnitude = 1;
        mercy.minDegrees = 120;
        mercy.maxDegrees = 180;
        mercy.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/MercyBG");
        mercy.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/MercyBG/Collider").GetComponent<PlayMakerFSM>();
        mercy.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/MercyBG/Collider/ButtonReact");

        VSRadialButton edge = new VSRadialButton();
        edge.name = "Edge";
        edge.radialLevel = VSRadialButton.RadialLevel.Level1;
        edge.maxMagnitude = 0.9;
        edge.minDegrees = 60;
        edge.maxDegrees = 120;
        edge.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/EdgeBG");
        edge.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/EdgeBG/Collider").GetComponent<PlayMakerFSM>();
        edge.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/EdgeBG/Collider/ButtonReact");

        VSRadialButton good = new VSRadialButton();
        good.name = "Good";
        good.radialLevel = VSRadialButton.RadialLevel.Level1;
        good.maxMagnitude = 0.9;
        good.minDegrees = 60;
        good.maxDegrees = 120;
        good.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/KeepGoingBG");
        good.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/KeepGoingBG/Collider").GetComponent<PlayMakerFSM>();
        good.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/KeepGoingBG/Collider/ButtonReact");

        VSRadialButton taunt = new VSRadialButton();
        taunt.name = "Taunt";
        taunt.radialLevel = VSRadialButton.RadialLevel.Level1;
        taunt.maxMagnitude = 1;
        taunt.minDegrees = 120;
        taunt.maxDegrees = 180;
        taunt.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TauntBG");
        taunt.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TauntBG/Collider").GetComponent<PlayMakerFSM>();
        taunt.components.highlight = GameObject.Find("NewButtons/Center/Level1/OtherButtons/TauntBG/Collider/ButtonReact");

        VSRadialButton hideui = new VSRadialButton();
        hideui.name = "Hide UI";
        hideui.radialLevel = VSRadialButton.RadialLevel.Level2;
        hideui.maxMagnitude = 1;
        hideui.minDegrees = 135;
        hideui.maxDegrees = 180;
        hideui.components.buttonObject = GameObject.Find("NewButtons/Center/Level2/GameObject/Hide UI");
        hideui.components.buttonFsm = GameObject.Find("NewButtons/Center/Level2/GameObject/Hide UI/Collider").GetComponent<PlayMakerFSM>();
        hideui.components.highlight = GameObject.Find("NewButtons/Center/Level2/GameObject/Hide UI/Collider/ButtonReact");

        VSRadialButton timeout = new VSRadialButton();
        timeout.name = "Timeout";
        timeout.radialLevel = VSRadialButton.RadialLevel.Level2;
        timeout.maxMagnitude = 0.9;
        timeout.minDegrees = 90;
        timeout.maxDegrees = 135;
        timeout.components.buttonObject = GameObject.Find("NewButtons/Center/Level2/GameObject/Timeout");
        timeout.components.buttonFsm = GameObject.Find("NewButtons/Center/Level2/GameObject/Timeout/Collider (1)").GetComponent<PlayMakerFSM>();
        timeout.components.highlight = GameObject.Find("NewButtons/Center/Level2/GameObject/Timeout/Collider (1)/ButtonReact");

        VSRadialButton safeword = new VSRadialButton();
        safeword.name = "Safeword";
        safeword.radialLevel = VSRadialButton.RadialLevel.Level2;
        safeword.maxMagnitude = 0.9;
        safeword.minDegrees = 45;
        safeword.maxDegrees = 90;
        safeword.components.buttonObject = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Safeword");
        safeword.components.buttonFsm = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Safeword/Collider").GetComponent<PlayMakerFSM>();
        safeword.components.highlight = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Safeword/Collider/ButtonReact");

        VSRadialButton oops = new VSRadialButton();
        oops.name = "Oops";
        oops.radialLevel = VSRadialButton.RadialLevel.Level2;
        oops.maxMagnitude = 1;
        oops.minDegrees = 0;
        oops.maxDegrees = 45;
        oops.components.buttonObject = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Oops");
        oops.components.buttonFsm = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Oops/Collider").GetComponent<PlayMakerFSM>();
        oops.components.highlight = GameObject.Find("NewButtons/Center/Level2/GameObject (1)/Oops/Collider/ButtonReact");

        VSRadialButton plus = new VSRadialButton();
        plus.name = "Plus";
        plus.radialLevel = VSRadialButton.RadialLevel.Both;
        plus.maxMagnitude = 1;
        plus.minDegrees = 180;
        plus.maxDegrees = 270;
        plus.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Plus");
        plus.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Plus/Collider").GetComponent<PlayMakerFSM>();
        plus.components.highlight = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Plus/Collider/ButtonReact");

        VSRadialButton minus = new VSRadialButton();
        minus.name = "Minus";
        minus.radialLevel = VSRadialButton.RadialLevel.Both;
        minus.maxMagnitude = 1;
        minus.minDegrees = 270;
        minus.maxDegrees = 360;
        minus.components.buttonObject = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Minus");
        minus.components.buttonFsm = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Minus/Collider").GetComponent<PlayMakerFSM>();
        minus.components.highlight = GameObject.Find("NewButtons/Center/Level1/ArousalMeter/Overlays/Minus/Collider/ButtonReact");

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

    private static void CheckButtonComponents(VSGenericButton button)
    {
        VSVRMod.logger.LogInfo("Verifying button: " + button.name);
        Assert.IsNotNull(button.components.buttonObject);
        Assert.IsNotNull(button.components.buttonFsm);
        Assert.IsNotNull(button.components.highlight);
    }

    private static void ClickButton(VSGenericButton button)
    {
        button.components.buttonFsm.SendEvent("Click");
    }

    private VSRadialButton.RadialLevel currentRadialLevel = VSRadialButton.RadialLevel.None;

    public void RadialMenuInteract()
    {
        bool stickClick = Controller.WasAStickClicked(777);
        bool triggerClick = Controller.WasATriggerClicked(777);
        double stickMagnitude = Controller.GetMaximalJoystickMagnitude();
        double stickDirection = Controller.GetMaximalJoystickAngle();

        if (!level1.activeSelf)
        {
            if (stickClick) ClickButton(circle);
            currentRadialLevel = VSRadialButton.RadialLevel.Level1;
        }
        else if (!level2.activeSelf) 
        {
            currentRadialLevel = VSRadialButton.RadialLevel.Level2;
            if (stickClick) ClickButton(level2Arrow);
        }
        else
        {
            currentRadialLevel = VSRadialButton.RadialLevel.None;
            if (stickClick) ClickButton(circle);
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
            }

            VSRadialButton trueButton = null;

            foreach (VSRadialButton button in candidateButtons)
            {
                if (trueButton == null || (button.maxMagnitude < trueButton.maxMagnitude && button.maxMagnitude > stickMagnitude))
                {
                    trueButton.components.highlight.SetActive(false);
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
