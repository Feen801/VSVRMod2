﻿using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI;
public class ScoreboardUIManager : UIManager
{
    private struct Scoreboard
    {
        public GameObject representative;
        public VSFindomButton mainMenu;
    }
    Scoreboard scoreboard = new Scoreboard();

    public ScoreboardUIManager(Scene scene) : base(scene)
    {
        Transform scoreCanvas = GameObject.Find("ScoreCanvas").transform;
        scoreboard.representative = scoreCanvas.Find("Scoreboard").gameObject;
        scoreboard.mainMenu = new(scoreCanvas, "Main Menu", "Scoreboard/Finish/Button");
        VSVRMod.logger.LogInfo("Setup Scoreboard");
    }

    public override bool Interact()
    {
        if (!scoreboard.representative.activeSelf)
        {
            return false;
        }
        if (Controller.WasAFaceButtonClicked() || Controller.WasAStickClicked() || Controller.WasATriggerClicked())
        {
            scoreboard.mainMenu.Click();
        }
        return true;
    }
}
