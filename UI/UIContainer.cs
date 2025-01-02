using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using VSVRMod2.UI.SpecifcUI;

namespace VSVRMod2.UI;
class UIContainer
{
    List<UIManager> priorityUIList = [];

    public BasicUIManager basicUIManager = null;

    public UIContainer(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.SessionScene))
        {
            priorityUIList.Add(new ScoreboardUIManager(scene));
            priorityUIList.Add(new SafewordUIManager(scene));
            priorityUIList.Add(new OpportunityUIManager(scene));
            priorityUIList.Add(new UrgesUIManager(scene));
            priorityUIList.Add(new FindomUIManager(scene));
            priorityUIList.Add(new StatusUIManager(scene));
            priorityUIList.Add(new RadialUIManager(scene));
            priorityUIList.Add(new StakesUIManager(scene));
            priorityUIList.Add(new ChoiceUIManager(scene));
            priorityUIList.Add(new IntInputUIMManager(scene));
            priorityUIList.Add(basicUIManager = new BasicUIManager(scene));
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public void Interact()
    {
        foreach (UIManager uiManager in priorityUIList)
        {
            bool foundInteractableUI = uiManager.Interact();
            if (foundInteractableUI)
            {
                return;
            }
        }
    }
}