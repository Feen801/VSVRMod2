using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace VSVRMod2;

public struct VSButtonComponents
{
    public GameObject collider;
    public PlayMakerFSM buttonFsm;
    public GameObject buttonObject;
    public GameObject highlight;
    public GameObject triggerIcon;
}

public interface VSButton
{
    public void Click();
    public void Highlight(bool status);
}

public class VSGenericButton : VSButton
{
    public string name;
    public VSButtonComponents components = new VSButtonComponents();

    public static string colliderPath = "/DoneBG/DoneText/Collider";
    public static string highlightPath = "/DoneBG/DoneText/Collider/ButtonPressReact1";

    public VSGenericButton(Transform knownParent, string name, string path) : this(knownParent, name, path, colliderPath, highlightPath)
    {

    }

    public VSGenericButton(Transform knownParent, string name, string path, string colliderPath, string highlightPath)
    {
        VSVRMod.logger.LogInfo("Populating button: " + name);
        if (knownParent == null)
        {
            VSVRMod.logger.LogError(this.name + " had null PARENT?");
        }
        this.name = name;
        Transform buttonObject = knownParent.Find(path);
        if (buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null buttonObject at " + path);
        }
        Transform collider = knownParent.Find(path + colliderPath);
        Transform highlight = knownParent.Find(path + highlightPath);
        if (collider == null)
        {
            VSVRMod.logger.LogError(this.name + " had null collider at " + path + colliderPath);
        }
        this.components.collider = collider.gameObject;
        this.components.buttonFsm = this.components.collider.GetComponent<PlayMakerFSM>();
        if (this.components.buttonFsm == null)
        {
            VSVRMod.logger.LogError(this.name + " had null buttonFsm");
        }
        if (highlight == null)
        {
            VSVRMod.logger.LogError(this.name + " had null highlight at " + path + highlightPath);
        }
        this.components.buttonObject = buttonObject.gameObject;
        this.components.highlight = highlight.gameObject;
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            this.components.triggerIcon = VSVRAssets.InstantiatePromptIcon("Trigger");
            GameObjectHelper.SetParentAndMaintainScaleForUI(this.components.triggerIcon.transform, this.components.highlight.transform);
            this.components.triggerIcon.transform.SetAsLastSibling();
        }
        VSVRMod.logger.LogInfo("Verified button: " + this.name);
    }

    public void RemoveTriggerIcon()
    {
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            this.components.triggerIcon.SetActive(false);
        }
    }

    public void SetTriggerIconLocation(float x, float y)
    {
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            this.components.triggerIcon.transform.localPosition = new Vector3(x, y, 0);
        }
    }

    public void Click()
    {
        this.components.buttonFsm.SendEvent("Click");
    }

    public void Highlight(bool status)
    {
        this.components.highlight.SetActive(status);
    }
}

public class VSChoiceButton : VSGenericButton
{
    public enum ButtonType
    {
        Positive,
        Negative,
    }

    public ButtonType type;

    public string[] speechText;

    public VSChoiceButton(Transform knownParent, string name, string path, string colliderPath, string highlightPath, ButtonType buttonType) : base(knownParent, name, path, colliderPath, highlightPath)
    {
        this.type = buttonType;

        Transform doneTextTransform = knownParent.Find(path + "/DoneBG/DoneText");
        if (doneTextTransform == null)
        {
            VSVRMod.logger.LogError($"Could not find '{path}/DoneBG//DoneText' under {knownParent.name}");
            return;
        }

        var tmp = doneTextTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
        {
            VSVRMod.logger.LogError($"No TextMeshProUGUI found under '{path}/DoneBG/DoneText'");
            return;
        }

        this.speechText = StringHelper.GetWords(tmp.text);
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject joystick = null;
            if (this.type == ButtonType.Positive)
            {
                joystick = VSVRAssets.InstantiatePromptIcon("Left");
            }
            else if (this.type == ButtonType.Negative)
            {
                joystick = VSVRAssets.InstantiatePromptIcon("Right");
            }
            if (joystick != null)
            {
                GameObjectHelper.SetParentAndMaintainScaleForUI(joystick.transform, this.components.buttonObject.transform);
                joystick.transform.SetAsLastSibling();
                joystick.transform.localPosition = new Vector3(0, -55, 0);
            }
        }
    }

    public VSChoiceButton(Transform knownParent, string name, string path, ButtonType buttonType) : this(knownParent, name, path, colliderPath, highlightPath, buttonType)
    {

    }
}

public class VSRadialButton : VSGenericButton
{
    public static new string colliderPath = "/Collider";
    public static new string highlightPath = "/Collider/ButtonReact";

    public enum RadialLevel
    {
        None = 0x00,
        Both = 0xFF,
        Level1 = 0x0F,
        Level2 = 0xF0
    }

    public double minDegrees;
    public double maxDegrees;
    public double maxMagnitude = 1;
    public RadialLevel radialLevel;

    public VSRadialButton(Transform knownParent, string name, string path, string colliderPath, string highlightPath, double maxMagnitude, double minDegrees, double maxDegrees, RadialLevel radialLevel) : base(knownParent, name, path, colliderPath, highlightPath)
    {
        this.minDegrees = minDegrees;
        this.maxDegrees = maxDegrees;
        this.maxMagnitude = maxMagnitude;
        this.radialLevel = radialLevel;
    }

    public VSRadialButton(Transform knownParent, string name, string path, double maxMagnitude, double minDegrees, double maxDegrees, RadialLevel radialLevel) : this(knownParent, name, path, colliderPath, highlightPath, maxMagnitude, minDegrees, maxDegrees, radialLevel)
    {

    }

    public bool IsOnRadialLevel(RadialLevel radialLevel)
    {
        return (this.radialLevel & radialLevel) != 0x00;
    }

    public void SetIcon(string icon)
    {
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            GameObject joystick = null;
            try
            {
                joystick = GameObject.Instantiate(VSVRAssets.promptIcons[icon]);
            }
            finally
            {
                if (joystick != null)
                {
                    GameObjectHelper.SetParentAndMaintainScaleForUI(joystick.transform, this.components.buttonObject.transform);
                    joystick.transform.SetAsLastSibling();
                    joystick.transform.localPosition = new Vector3(0, -180, 0);
                    this.SetTriggerIconLocation(0, 225);
                }
            }
        }
    }
}

public class VSFindomButton : VSButton
{
    static Color darkGrey = new Color(0.15f, 0.15f, 0.15f, 1);
    public string name;
    public Button button;
    public GameObject buttonObject;
    public Image highlight;
    private GameObject triggerIcon;

    public void Highlight(bool status)
    {
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            triggerIcon.SetActive(status);
        }
        highlight.color = status ? Color.white : darkGrey;
    }

    public VSFindomButton(Transform knownParent, string name, string path)
    {
        this.name = name;
        Transform buttonObject = knownParent.Find(path);
        if (buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button object");
        }
        this.buttonObject = buttonObject.gameObject;
        this.button = buttonObject.GetComponent<Button>();
        if (this.button == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button");
        }
        highlight = this.buttonObject.GetComponent<Image>();
        if (this.highlight == null)
        {
            VSVRMod.logger.LogError(this.name + " had null image");
        }

        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            this.triggerIcon = VSVRAssets.InstantiatePromptIcon("Trigger");
            GameObjectHelper.SetParentAndMaintainScaleForUI(this.triggerIcon.transform, this.highlight.transform);
            this.triggerIcon.transform.SetAsLastSibling();
            this.triggerIcon.transform.localScale = new Vector3(1.2f, 1, 1);
            this.triggerIcon.transform.localPosition = new Vector3(-90, 0);
            triggerIcon.SetActive(false);
        }
        VSVRMod.logger.LogInfo("Verified findom button: " + this.name);
    }

    public void Click()
    {
        this.button.OnSubmit(null);
    }
}

public class VSStatusCancelButton : VSGenericButton
{
    static Color defaultColor = new Color(0.7843f, 0.3059f, 0.3059f, 1);
    public Image highlight;

    new public void Highlight(bool status)
    {
        if (VRConfig.showButtonPrompts.Value && !VSVRMod.noVR)
        {
            this.components.triggerIcon.SetActive(status);
        }
        highlight.color = status ? Color.white : defaultColor;
    }

    new public void Click()
    {
        components.buttonFsm.SendEvent("EVENT_TRIGGER");
    }

    public static new string colliderPath = "/Button";
    public static new string highlightPath = "/Button/Icon";

    public VSStatusCancelButton(Transform knownParent, string name, string path) : base(knownParent, name, path, colliderPath, highlightPath) 
    {
        Transform tf = knownParent.Find(name + highlightPath);
        if(tf == null)
        {
            VSVRMod.logger.LogError(this.name + " had null image transform at " + highlightPath);
        }
        highlight = tf.gameObject.GetComponent<Image>();
    }
}