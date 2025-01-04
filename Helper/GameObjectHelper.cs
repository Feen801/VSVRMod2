using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2;
public class GameObjectHelper
{
    public static GameObject GetGameObjectCheckFound(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            VSVRMod.logger.LogError(path + " gameobject not found");
        }
        return go;
    }

    public static void AddChecked(List<GameObject> list, string path, GameObject parent)
    {
        Transform bg = parent.transform.Find(path);
        if (bg != null)
        {
            list.Add(bg.gameObject);
        }
        else
        {
            VSVRMod.logger.LogError(path + " not found for disabling gradients");
        }
    }
    public static GameObject CreateChildGameObject(string name, Transform parent)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        return child;
    }

    public static GameObject FindDisabledRootObjectInScene(string sceneName, string objectName)
    {
        // Get the scene by name
        Scene scene = SceneManager.GetSceneByName(sceneName);

        // Check if the scene is loaded
        if (!scene.isLoaded)
        {
            Debug.LogError($"Scene '{sceneName}' is not loaded.");
            return null;
        }

        // Iterate through all root GameObjects in the scene
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (rootObject.name == objectName)
            {
                return rootObject; // Found the matching GameObject
            }
        }

        // Return null if not found
        return null;
    }
}

