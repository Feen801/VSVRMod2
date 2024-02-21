using System;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI;
internal class UrgesUIManager : UIManager
{
    private VSGenericButton giveInButton = null;
    private VSGenericButton resistButton = null;
    public UrgesUIManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;
            giveInButton = new VSGenericButton(parent, "Give In", "Urges/ActionTextContainer/GiveIn/GiveInButton");
            resistButton = new VSGenericButton(parent, "Resist", "Urges/ActionTextContainer/Resist/ResistnButton");

            VSVRMod.logger.LogInfo("Finished setting up urge  buttons");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        bool faceButtonClicked = Controller.WasAFaceButtonClicked(775);
        if (giveInButton.components.buttonObject.activeSelf && faceButtonClicked)
        {
            giveInButton.Click();
            return true;
        }
        return false;
    }
}

