using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
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
        Level1 = 1,
        Level2 = 2
    }

    public RadialLevel radialLevel;
}

public class Buttons
{
    public static List<VSChoiceButton> vsChoiceButtons = new List<VSChoiceButton>();
    public static List<VSRadialButton> vsRadialButtons = new List<VSRadialButton>();

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

    public static void SetupRadialButtons()
    {
    }

    public static void PostiveAction() 
    {
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.type == VSChoiceButton.ButtonType.Positive && button.components.buttonObject.activeSelf)
            {
                VSVRMod.logger.LogInfo("Trying to click: " + button.name);
                button.components.buttonFsm.SendEvent("Click");
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
                button.components.buttonFsm.SendEvent("Click");
            }
        }
    }

    public class HeadMovementTracker
    {
        static int nods;
        static int headshakes;

        public static void Nod()
        {
            headshakes = 0;
            nods++;
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
            headshakes++;
            VSVRMod.logger.LogInfo("Partial Headshake");
            if (headshakes >= 2)
            {
                VSVRMod.logger.LogInfo("Full Headshake");
                NegativeAction();
            }
        }
    }

    
}
