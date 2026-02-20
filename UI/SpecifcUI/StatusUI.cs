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
    private GameObject statuses2;

    private GameObject leftArrow;
    private GameObject rightArrow;

    bool inFocus;

    public StatusUIManager(Scene scene) : base(scene)
    {
        inFocus = false;
        parentToButtons = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas/YourStatusMenuManager/Statuses2/ModifiersTextRepository");
        statuses2 = GameObjectHelper.GetGameObjectCheckFound("OverlayCanvas/YourStatusMenuManager");

        foreach (Transform button in parentToButtons.transform)
        {
            VSStatusCancelButton statusCancelButton = new(parentToButtons.transform, button.name, button.name);
            statusCancelButton.SetTriggerIconLocation(-50, 0);
            if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
            {
                statusCancelButton.components.triggerIcon.SetActive(false);
            }
            vsStatusButtons.Add(statusCancelButton);
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

        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject faceButton = VSVRAssets.InstantiatePromptIcon("BottomPress");
            GameObjectHelper.SetParentAndMaintainScaleForUI(faceButton.transform, statuses2.transform);
            faceButton.transform.localPosition = new Vector3(300, -200);
            faceButton.transform.localScale = Vector3.one;

            leftArrow = VSVRAssets.InstantiatePromptIcon("ArrowLeft");
            GameObjectHelper.SetParentAndMaintainScaleForUI(leftArrow.transform, faceButton.transform);
            leftArrow.transform.localPosition = new Vector3(-50, 0);
            leftArrow.transform.localScale = Vector3.one;

            rightArrow = VSVRAssets.InstantiatePromptIcon("ArrowRight");
            GameObjectHelper.SetParentAndMaintainScaleForUI(rightArrow.transform, faceButton.transform);
            rightArrow.transform.localPosition = new Vector3(50, 0);
            rightArrow.transform.localScale = Vector3.one;

            leftArrow.SetActive(true);
            rightArrow.SetActive(false);

            GameObject verticalJoystick = VSVRAssets.InstantiatePromptIcon("Vertical");
            GameObjectHelper.SetParentAndMaintainScaleForUI(verticalJoystick.transform, statuses2.transform);
            verticalJoystick.transform.localScale = Vector3.one;
            verticalJoystick.transform.localPosition = new Vector3(-475, -250);
        }
    }

    public override bool Interact()
    {
        if (level1.activeSelf)
        {
            if (Controller.WasAFaceButtonClicked())
            {
                inFocus = !inFocus;
                if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
                {
                    leftArrow.SetActive(!inFocus);
                    rightArrow.SetActive(inFocus);
                }
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
                        theButton.Click();
                        inFocus = false;
                    }
                    return true;
                }
            }
            else if (activeButtons.Count <= 0)
            {
                inFocus = false;
                if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
                {
                    leftArrow.SetActive(!inFocus);
                    rightArrow.SetActive(inFocus);
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