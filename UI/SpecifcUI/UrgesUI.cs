using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI.SpecifcUI;
internal class UrgesUIManager : UIManager
{
    private VSGenericButton giveInButton = null;
    private VSGenericButton resistButton = null;
    private GameObject actionText = null;

    private TextMeshProUGUI actionTextActualText = null;

    public UrgesUIManager(Scene scene) : base(scene)
    {
        Transform parent = GameObject.Find("GeneralCanvas/EventManager").transform;
        giveInButton = new VSGenericButton(parent, "Give In", "Urges/ActionTextContainer/GiveIn/GiveInButton");
        resistButton = new VSGenericButton(parent, "Resist", "Urges/ActionTextContainer/IgnoreButton");

        if(VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject bottomButton = VSVRAssets.InstantiatePromptIcon("BottomPress");
            GameObjectHelper.SetParentAndMaintainScaleForUI(bottomButton.transform, giveInButton.components.buttonObject.transform);
            bottomButton.transform.localPosition = new Vector3(-160, 0);

            GameObject topButton = VSVRAssets.InstantiatePromptIcon("TopPress");
            GameObjectHelper.SetParentAndMaintainScaleForUI(topButton.transform, resistButton.components.buttonObject.transform);
            topButton.transform.localPosition = new Vector3(160, 0);
        }

        actionText = parent.Find("Urges/ActionTextContainer").gameObject;
        actionTextActualText = actionText.transform.Find("Text2").GetComponent<TextMeshProUGUI>();

        VSVRMod.logger.LogInfo("Finished setting up urge buttons");

        SetUrgeTimer();
    }

    public void SetUrgeTimer()
    {
        if (VRConfig.urgeSeconds.Value <= 0.0)
        {
            return;
        }
        FsmState[] states = GameObject.Find("Root/GeneralCanvas/EventManager/Urges").GetComponent<PlayMakerFSM>().FsmStates;
        foreach (FsmState state in states)
        {
            foreach (FsmStateAction action in state.Actions)
            {
                if (action is SetFloatValue && ((SetFloatValue)action).floatVariable.Name.Equals("Timer"))
                {
                    ((SetFloatValue)action).floatValue.Value += VRConfig.urgeSeconds.Value;
                }
            }
        }
        VSVRMod.logger.LogInfo("Increased urge timers");
    }

    public override bool Interact()
    {
        if (actionText.activeSelf)
        {
            bool faceButtonClicked = Controller.WasALowerFaceButtonClicked();
            if (giveInButton.components.buttonObject.activeSelf && faceButtonClicked)
            {
                giveInButton.Click();
                return true;
            }
            faceButtonClicked = Controller.WasAUpperFaceButtonClicked();
            if (resistButton.components.buttonObject.activeSelf && faceButtonClicked)
            {
                resistButton.Click();
                return true;
            }
        }
        return false;
    }

    public bool VoiceInteract(string[] words)
    {
        if (!actionText.activeSelf)
        {
            return false;
        }
        if (actionTextActualText == null)
        {
            VSVRMod.logger.LogError("Bad actual text for urges!");
        }
        if (VRConfig.urgeSpeech.Value) {
            string currentSayRequest = StringHelper.ExtractQuoted(actionTextActualText.text);
            if (currentSayRequest != null)
            {
                string[] currentSayRequestWords = StringHelper.GetWords(currentSayRequest);
                if (StringHelper.MatchPercent(words, currentSayRequestWords) >= 0.4)
                {
                    giveInButton.Click();
                    return true;
                }
            }
        }

        if (VRConfig.urgeAnswer.Value)
        {
            if (StringHelper.MatchPercent(words, new string[] { "give", "in" }) >= 1.0)
            {
                giveInButton.Click();
                return true;
            }
            if (StringHelper.MatchPercent(words, new string[] { "resist" }) >= 1.0)
            {
                resistButton.Click();
                return true;
            }
        }

        return false;
    }
}

