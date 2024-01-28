using UnityEngine;
using UnityEngine.UI;

namespace VSVRMod2;

public struct VSButtonComponents
{
    public GameObject collider;
    public PlayMakerFSM buttonFsm;
    public GameObject buttonObject;
    public GameObject highlight;
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
        Transform collider = knownParent.Find(path + colliderPath);
        Transform highlight = knownParent.Find(path + highlightPath);
        if (collider == null)
        {
            VSVRMod.logger.LogError(this.name + " had null collider");
        }
        this.components.collider = collider.gameObject;
        this.components.buttonFsm = this.components.collider.GetComponent<PlayMakerFSM>();
        if (buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null buttonObject");
        }
        if (this.components.buttonFsm == null)
        {
            VSVRMod.logger.LogError(this.name + " had null buttonFsm");
        }
        if (highlight == null)
        {
            VSVRMod.logger.LogError(this.name + " had null highlight");
        }
        this.components.buttonObject = buttonObject.gameObject;
        this.components.highlight = highlight.gameObject;
        VSVRMod.logger.LogInfo("Verified button: " + this.name);
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

    public VSChoiceButton(Transform knownParent, string name, string path, string colliderPath, string highlightPath, ButtonType buttonType) : base(knownParent, name, path, colliderPath, highlightPath)
    {
        this.type = buttonType;
    }

    public VSChoiceButton(Transform knownParent, string name, string path, ButtonType buttonType) : this(knownParent, name, path, colliderPath, highlightPath, buttonType)
    {

    }
}

public class VSRadialButton : VSGenericButton
{
    public static new string colliderPath = "/Collider";
    public static new string highlightPath = "/Collider/ButtonReact";

    public VSRadialButton(Transform knownParent, string name, string path, string colliderPath, string highlightPath, double maxMagnitude, double minDegrees, double maxDegrees, RadialLevel radialLevel) : base(knownParent, name, path, colliderPath, highlightPath)
    {
        this.minDegrees = minDegrees;
        this.maxDegrees = maxDegrees;
        this.maxMagnitude = maxMagnitude;
        this.radialLevel = radialLevel;
    }

    public VSRadialButton(Transform knownParent, string name, string path, double maxMagnitude, double minDegrees, double maxDegrees, RadialLevel radialLevel) : this(knownParent, name, path, colliderPath, highlightPath, maxMagnitude, minDegrees, minDegrees, radialLevel)
    {

    }

    public double minDegrees;
    public double maxDegrees;
    public double maxMagnitude = 1;

    public enum RadialLevel
    {
        None = 0x00,
        Both = 0xFF,
        Level1 = 0x0F,
        Level2 = 0xF0
    }

    public RadialLevel radialLevel;
}

public class VSFindomButton : VSButton
{
    static Color darkGrey = new Color(0.15f, 0.15f, 0.15f, 1);
    public string name;
    public Button button;
    public GameObject buttonObject;
    public Image highlight;

    public void Highlight(bool status)
    {
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
        VSVRMod.logger.LogInfo("Verified findom button: " + this.name);
    }

    public void Click()
    {
        this.button.OnSubmit(null);
    }
}