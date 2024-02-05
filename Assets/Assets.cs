using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VSVRMod2;
public class VSVRAssets
{
    static AssetBundle vsvrAssets;
    static Shader uiShader;
    static Shader textShader;

    public static void LoadAssets()
    {
        string assetPath = Path.Combine(Application.streamingAssetsPath, "vsvrassets");
        VSVRMod.logger.LogInfo("Loading VR assets at " + assetPath);
        vsvrAssets = AssetBundle.LoadFromFile(assetPath);
        if (vsvrAssets == null )
        {
            VSVRMod.logger.LogError("Failed to load VR assets at " + assetPath);
        }
        uiShader = vsvrAssets.LoadAsset<Shader>("UIIgnoreDepth");
        if (uiShader == null)
        {
            VSVRMod.logger.LogError("Failed to load VR UI Shader " + "UIIgnoreDepth");
        }

        textShader = vsvrAssets.LoadAsset<Shader>("TMP_SDF Overlay");
        if (textShader == null)
        {
            VSVRMod.logger.LogError("Failed to load VR text Shader " + "TMP_SDF Overlay");
        }
    }

    public static void ApplyUIShader()
    {
        GameObject eventManager = GameObjectHelper.GetGameObjectCheckFound("EventManager");
        if (eventManager == null)
        {
            VSVRMod.logger.LogError("Could not find eventManager for applying UI shader");
        }


        Transform background = eventManager.transform.Find("InstructionBorder/Background");
        if (background == null)
        {
            VSVRMod.logger.LogError("Could not find background for applying UI shader");
        }
        Image bgImage = background.gameObject.GetComponent<Image>();
        if (bgImage == null)
        {
            VSVRMod.logger.LogError("Could not find bgImage for applying UI shader");
        }
        bgImage.material.shader = uiShader;
        VSVRMod.logger.LogInfo("Applied new UI shader");

        //I hate this
        //But its the only way I know of to fix all of them easily because they are all instanced
        var textMeshProUGUIs = UnityEngine.Object.FindObjectsByType(typeof(TextMeshProUGUI), FindObjectsInactive.Include, FindObjectsSortMode.None); ;
        foreach (UnityEngine.Object tmpugui in  textMeshProUGUIs)
        {
            ((TextMeshProUGUI)tmpugui).fontMaterial.shader = textShader;
        }
        VSVRMod.logger.LogInfo("Applied new text shader");
    }
}
