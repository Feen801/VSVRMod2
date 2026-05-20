using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI.SpecifcUI;
public class ScoreboardUIManager : UIManager
{
    private const float READY_ALPHA_THRESHOLD = 0.95f;

    private static float EffectiveCanvasGroupAlpha(Transform start)
    {
        float alpha = 1f;
        for (Transform t = start; t != null; t = t.parent)
        {
            var cg = t.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                alpha *= cg.alpha;
                if (cg.ignoreParentGroups) break;
            }
        }
        return alpha;
    }

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

        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject trigger = VSVRAssets.InstantiatePromptIcon("Trigger");
            GameObjectHelper.SetParentAndMaintainScaleForUI(trigger.transform, scoreboard.mainMenu.button.transform);
            trigger.transform.localPosition = new Vector3(0, -100);
        }
    }

    public override bool Interact()
    {
        if (!scoreboard.representative.activeSelf)
        {
            return false;
        }
        var btn = scoreboard.mainMenu.button;
        if (btn == null || !btn.gameObject.activeInHierarchy || !btn.IsInteractable())
        {
            return true;
        }
        if (EffectiveCanvasGroupAlpha(btn.transform) < READY_ALPHA_THRESHOLD)
        {
            return true;
        }
        if (Controller.WasAFaceButtonClicked() || Controller.WasAStickClicked() || Controller.WasATriggerClicked())
        {
            GameObject finalScreen = GameObject.Instantiate(VSVRAssets.finalScreen);
            finalScreen.transform.SetParent(scoreboard.representative.transform, false);
            scoreboard.mainMenu.Click();
        }
        return true;
    }
}
