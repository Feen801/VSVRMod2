using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;
public class StartUIManager
{
    public StartUIManager()
    {

    }

    public bool Interact()
    {
        if (Controller.WasAFaceButtonClicked())
        {
            GameObject preparationMenu = GameObject.Find("Pre-Game/MainMenu/Camera Canvas/MenuManager/PreparationMenu");
            if (preparationMenu == null || !preparationMenu.activeSelf) {
                VSVRMod.logger.LogWarning("Not in preparation menu, this may be expected.");
            }
            GameObject beginButton = null;
            foreach (Transform possible in preparationMenu.transform)
            {
                if (possible.name.Contains("Begin"))
                {
                    beginButton = possible.Find("Collider").gameObject;
                    break;
                }
            }
            if (beginButton != null)
            {
                //press it!
                PlayMakerFSM button = beginButton.GetComponent<PlayMakerFSM>();
                button.SendEvent("Click");
                return true;
            }
            else
            {
                VSVRMod.logger.LogWarning("Tried but failed to find begin button, this may be expected.");
            }
        }
        return false;
    } 
}
