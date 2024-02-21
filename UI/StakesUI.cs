using System;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI;

public class StakesUIManager : UIManager
{
    private struct StakesMenu
    {
        public GameObject representative;
        public VSGenericButton top;
        public VSGenericButton middle;
        public VSGenericButton bottom;
    }
    StakesMenu stakesMenu = new StakesMenu();

    public StakesUIManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform eventManager = GameObject.Find("GeneralCanvas/EventManager").transform;

            //StakesUI-------------------
            stakesMenu.representative = eventManager.Find("StakesUI").gameObject;
            if (stakesMenu.representative == null)
            {
                VSVRMod.logger.LogError("StakesUI had null representative");
            }
            stakesMenu.top = new(eventManager.Find("StakesUI"),
                "Stakes Top",
                "BG1",
                "/Collider",
                "/Borders/DarkBorder"
                );
            stakesMenu.middle = new(eventManager.Find("StakesUI"),
                "Stakes Middle",
                "BG2",
                "/Collider",
                "/Borders/DarkBorder"
                );
            stakesMenu.bottom = new(eventManager.Find("StakesUI"),
                "Stakes Bottom",
                "BG3",
                "/Collider",
                "/Borders/DarkBorder"
                );
            VSVRMod.logger.LogInfo("Setup StakesUI");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        if (stakesMenu.representative.activeSelf)
        {
            double y = Controller.GetMaximalJoystickValue().y;
            stakesMenu.top.Highlight(false);
            stakesMenu.middle.Highlight(false);
            stakesMenu.bottom.Highlight(false);
            VSGenericButton theButton = null;
            if (y > 0.1 && y < 0.4)
            {
                theButton = stakesMenu.bottom;
            }
            if (y > 0.4 && y < 0.7)
            {
                theButton = stakesMenu.middle;
            }
            if (y > 0.7)
            {
                theButton = stakesMenu.top;
            }
            if (theButton != null)
            {
                theButton.Highlight(true);
                if (Controller.WasATriggerClicked(105))
                {
                    theButton.Click();
                }
                return true;
            }
        }
        return false;
    }
}
