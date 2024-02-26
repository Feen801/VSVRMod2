using System;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;

public abstract class UIManager
{
    public UIManager(Scene scene)
    {
        if (!scene.isLoaded || !Equals(scene.name, Constants.sessionScene))
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public bool Interact()
    {
        return false;
    }
}