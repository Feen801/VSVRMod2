using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VSVRMod2.Interactions;

public struct VSButtonComponents
{
    public PlayMakerFSM buttonFsm;
    public GameObject buttonObject;
    public GameObject highlight;
}

public class VSGenericButton
{
    public string name;
    public VSButtonComponents components;
}

public class VSChoiceButton : VSGenericButton
{
    public enum VSButtonType
    {
        Postive,
        Negative,
    }
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
}

public class Buttons
{
    public static List<VSChoiceButton> vsChoiceButtons = new List<VSChoiceButton>();
    public static List<VSChoiceButton> vsRadialButtons = new List<VSChoiceButton>();
    public static void SetupChoiceButtons()
    {
        GameObject postiveButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Positives ------------");
        GameObject negativeButtonParent = GameObject.Find("GeneralCanvas/EventManager/Buttons/Negatives ------------");

        foreach (Transform postiveButton in postiveButtonParent.transform)
        {
        }

        foreach (Transform negativeButton in negativeButtonParent.transform)
        {
        }
    }

    public static void SetupRadialButtons()
    {
    }
}
