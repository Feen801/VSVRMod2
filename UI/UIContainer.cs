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
            priorityUIList.Add(0, new ScoreboardUIManager(scene));
            priorityUIList.Add(5, new SafewordUIManager(scene));
            priorityUIList.Add(10, new OpportunityUIManager(scene));
            priorityUIList.Add(15, new UrgesUIManager(scene));
            priorityUIList.Add(20, new FindomUIManager(scene));
            priorityUIList.Add(25, new RadialUIManager(scene));
            priorityUIList.Add(30, new StakesUIManager(scene));
            priorityUIList.Add(35, new ChoiceUIManager(scene));
            priorityUIList.Add(40, new IntInputUIMManager(scene));
            priorityUIList.Add(45, basicUIManager = new BasicUIManager(scene));
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