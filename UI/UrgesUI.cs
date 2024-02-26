using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;
internal class UrgesUIManager : UIManager
{
    private VSGenericButton giveInButton = null;
    private VSGenericButton resistButton = null;
    public UrgesUIManager(Scene scene) : base(scene)
    {
        Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;
        giveInButton = new VSGenericButton(parent, "Give In", "Urges/ActionTextContainer/GiveIn/GiveInButton");
        resistButton = new VSGenericButton(parent, "Resist", "Urges/ActionTextContainer/Resist/ResistnButton");

        VSVRMod.logger.LogInfo("Finished setting up urge  buttons");
    }

    public new bool Interact()
    {
        bool faceButtonClicked = Controller.WasAFaceButtonClicked();
        if (giveInButton.components.buttonObject.activeSelf && faceButtonClicked)
        {
            giveInButton.Click();
            return true;
        }
        return false;
    }
}

