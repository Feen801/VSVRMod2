using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace VSVRMod2.UI;

public class FindomUIManager : UIManager
{
    private struct FindomInput
    {
        public GameObject representative;
        public List<VSFindomButton> sendOptions;
        public VSFindomButton cancel;
        public GameObject sliderObject;
        public Slider slider;
    }
    FindomInput findomInput = new FindomInput();

    public FindomUIManager(Scene scene) : base(scene)
    {
        Transform overlayCanvas = GameObject.Find("Root/OverlayCanvas").transform;

        //Tribute Menu
        findomInput.representative = overlayCanvas.Find("TributeMenu").gameObject;
        if (findomInput.representative == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null representative");
        }

        findomInput.sendOptions = new List<VSFindomButton>();
        findomInput.cancel = new(overlayCanvas, "Tribute Cancel", "TributeMenu/Cancel");
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
        VSFindomButton tributeButton = new(overlayCanvas, "Tribute Button", "TributeMenu/Options/Group/Text (TMP) (7)/Button");
        VSFindomButton bribeButton = new(overlayCanvas, "Bribe Button", "TributeMenu/Options/Group/Text (TMP) (6)/Button");
        VSFindomButton placateButton = new(overlayCanvas, "Placate Button", "TributeMenu/Options/Group/Text (TMP) (8)/Button");
        VSFindomButton comfortButton = new(overlayCanvas, "Comfort Button", "TributeMenu/Options/Group/Text (TMP) (9)/Button");

        //From bottom to top
        findomInput.sendOptions.Add(comfortButton);
        findomInput.sendOptions.Add(placateButton);
        findomInput.sendOptions.Add(bribeButton);
        findomInput.sendOptions.Add(tributeButton);

        //weird thingy dunno why its even there? not related
        VSVRMod.logger.LogInfo("Hiding weird thing in Tribute Menu");
        Transform annoyingThing = overlayCanvas.transform.Find("TributeMenu/Slider - Standard (Value)/Text (TMP) (7)/Image");
        annoyingThing.gameObject.SetActive(false);

        VSVRMod.logger.LogInfo("Setup Tribute Menu");
    }

    private float findomInputInteractionNext = 0;
    private float findomInputInteractionAccel = 0.4f;
    private float findomInputInteractionAccelScale = 1;
    //false = controlling buttons, true = controlling slider
    private bool findomInputInteractState = false;
    public override bool Interact()
    {
        if (findomInput.representative.activeSelf)
        {
            Vector2 vector2 = Controller.GetMaximalJoystickValue();
            double x = vector2.x;
            double y = vector2.y;
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            if (Controller.WasAStickClicked())
            {
                findomInputInteractState = !findomInputInteractState;
                return true;
            }
            if (findomInputInteractState)
            {
                if (magnitude > 0.05)
                {
                    if (findomInputInteractionNext < Time.time)
                    {
                        int currentInt = (int)findomInput.slider.value;
                        currentInt += (int)Math.Round(Math.Sign(x) * findomInputInteractionAccelScale);
                        findomInputInteractionNext = Time.time + findomInputInteractionAccel;
                        findomInputInteractionAccel = Math.Clamp(findomInputInteractionAccel - (0.1f * (float)magnitude), 0.04f, float.PositiveInfinity);
                        if (findomInputInteractionAccel <= 0.05f)
                        {
                            findomInputInteractionAccelScale *= 1 + (0.1f*(float)magnitude);
                        }
                        currentInt = Math.Clamp(currentInt, (int)findomInput.slider.minValue, (int)findomInput.slider.maxValue);
                        findomInput.slider.value = currentInt;
                    }
                }
                else
                {
                    findomInputInteractionAccel = 0.4f;
                    findomInputInteractionAccelScale = 1;
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
                if (y > -1.0 + sectionSize)
                {
                    theButton = activeFindomButtons[(int)Math.Floor((y + 1.0) / sectionSize) - 1];
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
        }
        return false;
    }
}
