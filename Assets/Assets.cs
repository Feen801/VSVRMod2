using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VSVRMod2;
public class VSVRAssets
{
    static AssetBundle vsvrAssets;
    static Shader uiShader;
    static Shader textShader;
    public static GameObject leftHandFlame;
    public static GameObject rightHandFlame;
    public static GameObject finalScreen;
    public static Dictionary<string, GameObject> promptIcons = new();
    static string[] hypnoObjectNames = ["Hypno", "Hypno/Image", "Hypno/GameObject (1)/Hypno (1)", "Hypno/GameObject (1)/Hypno (1)/Hypno (3)", "Hypno/GameObject (1)/Hypno (1)/Hypno (4)", "Hypno/GameObject/Hypno (2)/Hypno (4)", "Hypno/GameObject/Hypno (2)/Hypno (5)"];
    static string[] iconNames = ["BottomPress", "Click", "Grip", "Horizontal", "Left", "Right", "TopPress", "Trigger", "Up", "UpLeft", "UpRight", "UpUpLeft", "UpUpRight", "Vertical"];

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

        textShader = vsvrAssets.LoadAsset<Shader>("TMP_SDF");
        if (textShader == null)
        {
            VSVRMod.logger.LogError("Failed to load VR text Shader " + "TMP_SDF");
        }

        leftHandFlame = vsvrAssets.LoadAsset<GameObject>("LeftHand");
        if (leftHandFlame == null)
        {
            VSVRMod.logger.LogError("Failed to load hand flame " + "LeftHand");
        }

        rightHandFlame = vsvrAssets.LoadAsset<GameObject>("RightHand");
        if (rightHandFlame == null)
        {
            VSVRMod.logger.LogError("Failed to load hand flame " + "RightHand");
        }

        finalScreen = vsvrAssets.LoadAsset<GameObject>("FinalScreen");
        if (finalScreen == null)
        {
            VSVRMod.logger.LogError("Failed to load final screen");
        }

        LoadPromptIcons();
    }

    private static void LoadPromptIcons()
    {
        foreach (string iconName in iconNames)
        {
            LoadPromptIcon(iconName);
        }
    }

    private static void LoadPromptIcon(string name) {
        GameObject go = vsvrAssets.LoadAsset<GameObject>(name);
        promptIcons.Add(name, go);
    }

    public static void ApplyUIShader()
    {
        //Prevent tattoos from rendering under the floor
        GameObject tattoos = GameObjectHelper.GetGameObjectCheckFound("TattooContainer");
        GameObject sceneContainer = GameObjectHelper.GetGameObjectCheckFound("Root/SceneContainer");
        if (tattoos == null)
        {
            VSVRMod.logger.LogError("Could not find TattooContainer for NOT applying UI shader");
        }
        if (sceneContainer == null)
        {
            VSVRMod.logger.LogError("Could not find SceneContainer for NOT applying UI shader");
        }
        Transform basicTattoos = tattoos.transform.Find("SkinRendering/Basic");
        Image imagex = basicTattoos.GetComponent<Image>();
        imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        foreach (Transform t in basicTattoos)
        {
            Image image = t.GetComponent<Image>();
            if (image != null)
            {
                image.material = UnityEngine.Object.Instantiate(image.material);
            }
        }
        Transform thirstyTattoos = tattoos.transform.Find("GlowTattooRendering/Thirsty");
        imagex = thirstyTattoos.GetComponent<Image>();
        imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        foreach (Transform t in thirstyTattoos)
        {
            Image image = t.GetComponent<Image>();
            if (image != null)
            {
                image.material = UnityEngine.Object.Instantiate(image.material);
            }
        }
        Transform mischievousTattoos = tattoos.transform.Find("GlowTattooRendering/Mischievous");
        imagex = mischievousTattoos.GetComponent<Image>();
        imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        foreach (Transform t in mischievousTattoos)
        {
            Image image = t.GetComponent<Image>();
            if (image != null)
            {
                image.material = UnityEngine.Object.Instantiate(image.material);
            }
        }
        Transform furiousTattoos = tattoos.transform.Find("GlowTattooRendering/Furious");
        imagex = furiousTattoos.GetComponent<Image>();
        imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        foreach (Transform t in furiousTattoos)
        {
            Image image = t.GetComponent<Image>();
            if (image != null)
            {
                image.material = UnityEngine.Object.Instantiate(image.material);
            }
        }
        //Prevent hypno eye renderer from rendering under the floor
        GameObject hypnoTexRendererGO = GameObjectHelper.FindDisabledRootObjectInScene(Constants.SessionScene, "EyeTexRendering");
        if (hypnoTexRendererGO == null)
        {
            VSVRMod.logger.LogError("Failed to find EyeTexRendering");
        }
        foreach (string hypnoObjectName in hypnoObjectNames) {
            Transform hypnoTexRenderer = hypnoTexRendererGO.transform.Find(hypnoObjectName);
            if (hypnoTexRenderer == null)
            {
                VSVRMod.logger.LogError("Failed to find " + hypnoObjectName);
            }
            imagex = hypnoTexRenderer.GetComponent<Image>();
            imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        }

        //Exclude spinning wheel from UI depth fix
        string[] wheelParts = { "Image (3)", "Reward", "Pleasure", "Punishment", "PleasurePunishment", "RewardPunishment"};
        foreach (string wheelPart in wheelParts)
        {
            Transform wheel = sceneContainer.transform.Find("SpecialAnimations/Animate_SpinWheel/Objects/Wheel of Fortune/Wheel/SpinWheelWheel/Canvas/WheelCanvas/" + wheelPart);
            if (wheel == null)
            {
                VSVRMod.logger.LogError("Could not find spin wheel for NOT applying UI shader");
            }
            imagex = wheel.GetComponent<Image>();
            imagex.material = UnityEngine.Object.Instantiate(imagex.material);
        }

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
