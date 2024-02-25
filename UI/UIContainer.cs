using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;
class UIContainer
{
    SortedList<int, UIManager> priorityUIList = [];

    public BasicUIManager basicUIManager = null;

    public UIContainer(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            int i = 0;
            priorityUIList.Add(i++, new ScoreboardUIManager(scene));
            priorityUIList.Add(i++, new SafewordUIManager(scene));
            priorityUIList.Add(i++, new OpportunityUIManager(scene));
            priorityUIList.Add(i++, new UrgesUIManager(scene));
            priorityUIList.Add(i++, new FindomUIManager(scene));
            priorityUIList.Add(i++, new RadialUIManager(scene));
            priorityUIList.Add(i++, new StakesUIManager(scene));
            priorityUIList.Add(i++, new ChoiceUIManager(scene));
            priorityUIList.Add(i++, new IntInputUIMManager(scene));
            priorityUIList.Add(i++, basicUIManager = new BasicUIManager(scene));
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public void Interact()
    {
        foreach (UIManager uiManager in priorityUIList.Values)
        {
            bool foundInteractableUI = uiManager.Interact();
            if (foundInteractableUI) return;
        }
    }
}