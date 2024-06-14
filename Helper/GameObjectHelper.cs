using System.Collections.Generic;
using UnityEngine;

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
}

