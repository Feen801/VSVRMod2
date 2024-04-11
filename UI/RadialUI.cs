using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI;

public class RadialUIManager : UIManager
{
    private List<VSRadialButton> vsRadialButtons = [];

    private GameObject level1;
    private GameObject level2;

    private VSGenericButton circle;
    private VSGenericButton level2Arrow;
    private GameObject exitButtonRadial;

    private GameObject popupArousalMeter;
    private VSRadialButton plusPopup;
    private VSRadialButton minusPopup;

    public RadialUIManager(Scene scene) : base(scene)
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

        GameObject popupArousal = GameObjectHelper.GetGameObjectCheckFound("PopupArousal");
        Transform popupArousalMeterTransform = popupArousal.transform.Find("PopupArousalMeter");
        popupArousalMeter = popupArousalMeterTransform.gameObject;

        plusPopup = new(popupArousalMeterTransform, "PlusPopup", "Overlays/Plus", 1, 270, 360, VSRadialButton.RadialLevel.Both);
        minusPopup = new(popupArousalMeterTransform, "MinusPopup", "Overlays/Minus", 1, 180, 270, VSRadialButton.RadialLevel.Both);

        VSVRMod.logger.LogInfo("Finished setting up PopupArousal buttons");
    }

    private VSRadialButton.RadialLevel currentRadialLevel = VSRadialButton.RadialLevel.None;

    public override bool Interact()
    {
        bool stickClick = Controller.WasAStickClicked();
        bool triggerClick = Controller.WasATriggerClicked();
        double stickMagnitude = Controller.GetMaximalJoystickMagnitude();
        double stickDirection = Controller.GetMaximalJoystickAngle();
        double stickValueX = Controller.GetMaximalJoystickValue().x;

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

        if (currentRadialLevel != VSRadialButton.RadialLevel.None)
        {
            if(stickMagnitude > 0.3)
            {
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
                    if (trueButton == null || button.maxMagnitude < trueButton.maxMagnitude && button.maxMagnitude > stickMagnitude)
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
            else
            {
                foreach (VSRadialButton button in vsRadialButtons)
                {
                    button.components.highlight.SetActive(false);
                }
            }
        }
        else if(popupArousalMeter.activeSelf)
        {
            if (stickMagnitude > 0.3)
            {
                if(stickValueX > 0)
                {
                    plusPopup.Highlight(true);
                    minusPopup.Highlight(false);
                    if(triggerClick)
                    {
                        plusPopup.Click();
                    }
                }
                else
                {
                    plusPopup.Highlight(false);
                    minusPopup.Highlight(true);
                    if(triggerClick)
                    {
                        minusPopup.Click();
                    }
                }
            }
            else
            {
                plusPopup.Highlight(false);
                minusPopup.Highlight(false);
            }
        }
        return false;
    }
}