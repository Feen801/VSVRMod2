using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;
public class BasicUIManager : UIManager
{
    private List<VSChoiceButton> vsChoiceButtons = [];

    public HeadMovementTracker headMovementTracker;

    VSRadialButton good;

    public BasicUIManager(Scene scene) : base(scene)
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

        VSVRMod.logger.LogInfo("Finished setting up basic buttons");

        headMovementTracker = new HeadMovementTracker(this);

        GameObject centerGameObject = GameObject.Find("NewButtons/Center");
        if (centerGameObject == null)
        {
            VSVRMod.logger.LogError("centerGameObject not found (basicUI).");
        }
        Transform center = centerGameObject.transform;
        good = new(center, "Good", "Level1/OtherButtons/KeepGoingBG", 1, 0, 360, VSRadialButton.RadialLevel.Both);
    }

    public override bool Interact()
    {
        bool triggerClick = Controller.WasATriggerClicked();
        double x = Controller.GetMaximalJoystickValue().x;
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.components.buttonObject.activeSelf)
            {
                button.Highlight(false);
                if (x < -0.3 && button.type == VSChoiceButton.ButtonType.Positive)
                {
                    button.Highlight(true);
                    if (triggerClick)
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

    private void PostiveAction()
    {
        foreach (VSChoiceButton button in vsChoiceButtons)
        {
            if (button.type == VSChoiceButton.ButtonType.Positive && button.components.buttonObject.activeSelf)
            {
                VSVRMod.logger.LogInfo("Trying to click: " + button.name);
                button.Click();
            }
        }
        if(good.components.buttonObject.activeSelf)
        {
            VSVRMod.logger.LogInfo("Trying to click: " + good.name);
            good.Click();
        }
    }

    private void NegativeAction()
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
        int nods;
        int headshakes;

        long lastNodTime;
        long lastHeadshakeTime;

        BasicUIManager basicUIManager;

        public HeadMovementTracker(BasicUIManager basicUIManager)
        {
            this.basicUIManager = basicUIManager;
        }

        public void Nod()
        {
            if (!Controller.IsHeadsetWorn() || VSVRMod.controllerHeadset.lastPutOn > Time.time - 5.0f)
            {
                return;
            }
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
                basicUIManager.PostiveAction();
            }
        }
        public void Headshake()
        {
            if (!Controller.IsHeadsetWorn() || VSVRMod.controllerHeadset.lastPutOn > Time.time - 5.0f)
            {
                return;
            }
            nods = 0;
            if (MathHelper.CurrentTimeMillis() - lastHeadshakeTime > 1000)
            {
                headshakes = 0;
            }
            headshakes++;
            lastHeadshakeTime = MathHelper.CurrentTimeMillis();
            VSVRMod.logger.LogInfo("Partial Headshake");
            if (headshakes >= 2)
            {
                VSVRMod.logger.LogInfo("Full Headshake");
                basicUIManager.NegativeAction();
            }
        }
    }
}
