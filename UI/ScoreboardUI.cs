using System;
using UnityEngine.SceneManagement;
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

    public ScoreboardUIManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform scoreCanvas = GameObject.Find("ScoreCanvas").transform;
            scoreboard.representative = scoreCanvas.Find("Scoreboard").gameObject;
            scoreboard.mainMenu = new(scoreCanvas, "Main Menu", "Scoreboard/Finish/Button");
            VSVRMod.logger.LogInfo("Setup Scoreboard");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        if (!scoreboard.representative.activeSelf)
        {
            return false;
        }
        if (Controller.WasAFaceButtonClicked(200) || Controller.WasAStickClicked(200) || Controller.WasATriggerClicked(200))
        {
            scoreboard.mainMenu.Click();
        }
        return true;
    }
}
