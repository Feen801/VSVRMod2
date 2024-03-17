using System;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;

public abstract class UIManager
{
    public UIManager(Scene scene)
    {
        if (!scene.isLoaded || !Equals(scene.name, Constants.SessionScene))
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    public virtual bool Interact()
    {
        VSVRMod.logger.LogWarning("This UIManager Interact should not be called!");
        return false;
    }
}