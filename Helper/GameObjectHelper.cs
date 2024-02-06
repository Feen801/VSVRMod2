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
}

