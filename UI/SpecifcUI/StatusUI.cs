using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

namespace VSVRMod2.UI.SpecifcUI;

public class StatusUIManager : UIManager
{
    private List<VSStatusCancelButton> vsStatusButtons = [];
    List<VSStatusCancelButton> activeButtons = [];

    private GameObject parentToButtons;
    private GameObject level1;

    bool inFocus;

    public StatusUIManager(Scene scene) : base(scene)
    {
        inFocus = false;
        parentToButtons = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas/YourStatusMenuManager/Statuses2/ModifiersTextRepository");

        foreach (Transform button in parentToButtons.transform)
        {
            vsStatusButtons.Add(new VSStatusCancelButton(parentToButtons.transform, button.name, button.name));
        }

        GameObject centerGameObject = GameObject.Find("NewButtons/Center");
        if (centerGameObject == null)
        {
            VSVRMod.logger.LogError("centerGameObject not found.");
        }
        Transform center = centerGameObject.transform;

        Transform level1Transform = center.Find("Level1");
        if (level1Transform == null)
        {
            VSVRMod.logger.LogError("level1Transform not found.");
        }
        level1 = center.Find("Level1").gameObject;
    }

    public override bool Interact()
    {
        if (level1.activeSelf)
        {
            if (Controller.WasAFaceButtonClicked())
            {
                inFocus = !inFocus;
                CheckActiveButtons();
                foreach (VSStatusCancelButton button in activeButtons)
                {
                    button.Highlight(false);
                }
            }
            if (inFocus && activeButtons.Count > 0)
            {
                Vector2 vector2 = Controller.GetMaximalJoystickValue();
                double y = vector2.y;
                double sectionSize = 2.0 / activeButtons.Count;
                foreach (VSStatusCancelButton button in activeButtons)
                {
                    button.Highlight(false);
                }
                VSStatusCancelButton theButton = activeButtons[activeButtons.Count - 1 - (int)Math.Floor((y + 1.0) / sectionSize)];
                if (theButton != null)
                {
                    theButton.Highlight(true);
                    if (Controller.WasATriggerClicked())
                    {
                        VSVRMod.logger.LogError(theButton.name + "clicked somehow?");
                        theButton.Click();
                        inFocus = false;
                    }
                    return true;
                }
            }
            return false;
        }
        else
        {
            inFocus = false;
            return false;
        }
    }

    private void CheckActiveButtons()
    {
        activeButtons.Clear();
        foreach (VSStatusCancelButton button in vsStatusButtons)
        {
            if (button.components.buttonObject.activeSelf)
            {
                activeButtons.Add(button);
            }
        }
    }
}