using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace VSVRMod2.UI.SpecifcUI;
class OpportunityUIManager : UIManager
{
    private List<VSGenericButton> vsOpportunityButtons = [];
    public OpportunityUIManager(Scene scene) : base(scene)
    {
        Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;

        VSGenericButton opportunityProvoke = new(parent, "Provoke", "Buttons/OpportunityProvoke");
        VSGenericButton opportunityTaunt = new(parent, "Taunt", "Buttons/OpportunityTaunt");
        VSGenericButton opportunityEntice = new(parent, "Entice", "Buttons/OpportunityEntice");
        VSGenericButton opportunityPraise = new(parent, "Praise", "Buttons/OpportunityPraise");

        opportunityProvoke.RemoveTriggerIcon();
        opportunityTaunt.RemoveTriggerIcon();
        opportunityEntice.RemoveTriggerIcon();
        opportunityPraise.RemoveTriggerIcon();

        vsOpportunityButtons.Add(opportunityProvoke);
        vsOpportunityButtons.Add(opportunityTaunt);
        vsOpportunityButtons.Add(opportunityEntice);
        vsOpportunityButtons.Add(opportunityPraise);

        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            foreach (VSGenericButton button in vsOpportunityButtons)
            {
                opportunityProvoke.RemoveTriggerIcon();
                GameObject buttonIcon = GameObject.Instantiate(VSVRAssets.promptIcons["BottomPress"]);
                GameObjectHelper.SetParentAndMaintainScaleForUI(buttonIcon.transform, button.components.buttonObject.transform);
                buttonIcon.transform.localPosition = new Vector3(200, 0);
            }
        }
        
        VSVRMod.logger.LogInfo("Finished setting up opportunity buttons");
    }

    public override bool Interact()
    {
        bool faceButtonClicked = Controller.WasAFaceButtonClicked();
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
