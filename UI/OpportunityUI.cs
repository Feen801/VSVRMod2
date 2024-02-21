using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI;
class OpportunityUIManager : UIManager
{
    private List<VSGenericButton> vsOpportunityButtons = [];
    public OpportunityUIManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;

            VSGenericButton opportunityProvoke = new(parent, "Provoke", "Buttons/OpportunityProvoke");
            VSGenericButton opportunityTaunt = new(parent, "Taunt", "Buttons/OpportunityTaunt");
            VSGenericButton opportunityEntice = new(parent, "Entice", "Buttons/OpportunityEntice");
            VSGenericButton opportunityPraise = new(parent, "Praise", "Buttons/OpportunityPraise");

            vsOpportunityButtons.Add(opportunityProvoke);
            vsOpportunityButtons.Add(opportunityTaunt);
            vsOpportunityButtons.Add(opportunityEntice);
            vsOpportunityButtons.Add(opportunityPraise);

            VSVRMod.logger.LogInfo("Finished setting up opportunity buttons");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        bool faceButtonClicked = Controller.WasAFaceButtonClicked(1001);
        foreach (VSGenericButton button in vsOpportunityButtons)
        {
            if (button.components.buttonObject.activeSelf && faceButtonClicked)
            {
                button.Click();
                return true;
            }
        }
        return false;
    }
}
