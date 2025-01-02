using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI.SpecifcUI;
internal class UrgesUIManager : UIManager
{
    private VSGenericButton giveInButton = null;
    private VSGenericButton resistButton = null;
    private GameObject actionText = null;
    public UrgesUIManager(Scene scene) : base(scene)
    {
        Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;
        giveInButton = new VSGenericButton(parent, "Give In", "Urges/ActionTextContainer/GiveIn/GiveInButton");
        resistButton = new VSGenericButton(parent, "Resist", "Urges/ActionTextContainer/IgnoreButton");
        actionText = parent.Find("Urges/ActionTextContainer").gameObject;

        VSVRMod.logger.LogInfo("Finished setting up urge buttons");
    }

    public override bool Interact()
    {
        if (actionText.activeSelf)
        {
            bool faceButtonClicked = Controller.WasALowerFaceButtonClicked();
            if (giveInButton.components.buttonObject.activeSelf && faceButtonClicked)
            {
                giveInButton.Click();
                return true;
            }
            faceButtonClicked = Controller.WasAUpperFaceButtonClicked();
            if (resistButton.components.buttonObject.activeSelf && faceButtonClicked)
            {
                resistButton.Click();
                return true;
            }
        }
        return false;
    }
}

