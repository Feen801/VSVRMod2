using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VSVRMod2;
public class VSFindomButton
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

    public void Populate(string name, string path)
    {
        this.name = name;
        this.buttonObject = GameObject.Find(path);
        if (this.buttonObject == null)
        {
            VSVRMod.logger.LogError(this.name + " had null button object");
        }
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
    }

    public void Click() 
    {
        this.button.OnSubmit(null);
    }
}

class Menus
{
    private struct ChoiceMenu
    {
        public GameObject representative;
        public VSGenericButton left;
        public VSGenericButton right;
        public VSGenericButton favorite;
    }
    static ChoiceMenu choiceMenu;

    private struct StakesMenu
    {
        public GameObject representative;
        public VSGenericButton top;
        public VSGenericButton middle;
        public VSGenericButton bottom;
    }
    static StakesMenu stakesMenu = new StakesMenu();

    private struct SafewordMenu
    {
        public GameObject representative;
        public VSGenericButton goEasy;
        public VSGenericButton continueSession;
        public VSGenericButton endSession;
    }
    static SafewordMenu safewordMenu = new SafewordMenu();

    private struct IntInput
    {
        public GameObject representative;
        public GameObject inputField;
        public TextMeshProUGUI text;
    }
    static IntInput intInput = new IntInput();

    private struct FindomInput
    {
        public GameObject representative;
        public List<VSFindomButton> sendOptions;
        public VSFindomButton cancel;
        public GameObject sliderObject;
        public Slider slider;
    }
    static FindomInput findomInput = new FindomInput();

    public static void SetupMenus()
    {
        //ChoiceUI-------------------
        choiceMenu.representative = GameObject.Find("ChoiceUI");
        if (choiceMenu.representative == null)
        {
            VSVRMod.logger.LogError("ChoiceUI had null representative");
        }

        choiceMenu.favorite = PrepareUnusualButtonComponents(
            "Favorite Heart",
            "FavoriteHeart",
            "FavoriteHeart/DoneBG/DoneText/Collider",
            "FavoriteHeart/DoneBG/DoneText/Collider/ButtonPressReact"
            );

        choiceMenu.left = PrepareUnusualButtonComponents(
            "Choice Left",
            "ChoiceUI/Choice1",
            "ChoiceUI/Choice1/Collider",
            "ChoiceUI/Choice1/Image (1)/Borders/LightBorder"
            );

        choiceMenu.right = PrepareUnusualButtonComponents(
            "Choice Right",
            "ChoiceUI/Choice2",
            "ChoiceUI/Choice2/Collider",
            "ChoiceUI/Choice2/Image (1)/Borders/LightBorder"
            );

        //StakesUI-------------------
        stakesMenu.representative = GameObject.Find("StakesUI");
        if (stakesMenu.representative == null)
        {
            VSVRMod.logger.LogError("StakesUI had null representative");
        }

        stakesMenu.top = PrepareUnusualButtonComponents(
            "Stakes Top",
            "StakesUI/BG1",
            "StakesUI/BG1/Collider",
            "StakesUI/BG1/Borders/LightBorder"
            );
        stakesMenu.middle = PrepareUnusualButtonComponents(
            "Stakes Middle",
            "StakesUI/BG2",
            "StakesUI/BG2/Collider",
            "StakesUI/BG2/Borders/LightBorder"
            );
        stakesMenu.bottom = PrepareUnusualButtonComponents(
            "Stakes Bottom",
            "StakesUI/BG3",
            "StakesUI/BG3/Collider",
            "StakesUI/BG3/Borders/LightBorder"
            );

        //Safeword-------------------
        safewordMenu.representative = GameObject.Find("Buttons/EndSession");
        if (safewordMenu.representative == null)
        {
            VSVRMod.logger.LogError("Safeword had null representative");
        }

        safewordMenu.goEasy = PrepareUnusualButtonComponents(
            "Safeword Go Easy",
            "Buttons/GoEasy",
            "Buttons/GoEasy/DoneBG/DoneText/Collider",
            "Buttons/GoEasy/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        safewordMenu.continueSession = PrepareUnusualButtonComponents(
            "Safeword Continue",
            "Buttons/ContinueSession",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        safewordMenu.endSession = PrepareUnusualButtonComponents(
            "Safeword End Session",
            "Buttons/EndSession",
            "Buttons/EndSession/DoneBG/DoneText/Collider",
            "Buttons/EndSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );

        //Count Input-------------------
        intInput.representative = GameObject.Find("EventManager/IntInputField");
        if (intInput.representative == null)
        {
            VSVRMod.logger.LogError("Int input had null representative");
        }

        intInput.inputField = GameObject.Find("EventManager/IntInputField/Text Area/IntInputFieldText");
        if (intInput.inputField == null)
        {
            VSVRMod.logger.LogError("Int input field was null");
        }
        intInput.text = intInput.inputField.GetComponent<TextMeshProUGUI>();
        if (intInput.text == null)
        {
            VSVRMod.logger.LogError("Int input text was null");
        }

        //Tribute Menu
        findomInput.representative = GameObject.Find("OverlayCanvas/TributeMenu");
        if (findomInput.representative == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null representative");
        }

        findomInput.sendOptions = new List<VSFindomButton>();
        findomInput.cancel = new VSFindomButton();
        findomInput.cancel.Populate("Tribute Cancel", "TributeMenu/Cancel");
        findomInput.sliderObject = GameObject.Find("OverlayCanvas/TributeMenu/Slider - Standard (Value)");
        if (findomInput.sliderObject == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null slider object");
        }
        findomInput.slider = findomInput.sliderObject.GetComponent<Slider>();
        if (findomInput.slider == null)
        {
            VSVRMod.logger.LogError("Tribute Menu had null slider");
        }
        VSFindomButton tributeButton = new VSFindomButton();
        VSFindomButton bribeButton = new VSFindomButton();
        VSFindomButton placateButton = new VSFindomButton();
        VSFindomButton comfortButton = new VSFindomButton();

        tributeButton.Populate("Tribute Button", "TributeMenu/Options/Group/Text (TMP) (7)");
        bribeButton.Populate("Bribe Button", "TributeMenu/Options/Group/Text (TMP) (6)");
        placateButton.Populate("Placate Button", "TributeMenu/Options/Group/Text (TMP) (8)");
        comfortButton.Populate("Comfort Button", "TributeMenu/Options/Group/Text (TMP) (9)");

        findomInput.sendOptions.Add(tributeButton);
        findomInput.sendOptions.Add(bribeButton);
        findomInput.sendOptions.Add(placateButton);
        findomInput.sendOptions.Add(comfortButton);
    }

    private static VSGenericButton PrepareUnusualButtonComponents(string name, string buttonObjectPath, string colliderPath, string highlightPath)
    {
        VSGenericButton button = new VSGenericButton();
        button.name = name;
        button.components.buttonObject = GameObject.Find(buttonObjectPath);
        button.components.collider = GameObject.Find(colliderPath);
        if(button.components.buttonObject == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button object");
        }
        if (button.components.collider == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button collider");
        }
        button.components.buttonFsm = choiceMenu.favorite.components.collider.GetComponent<PlayMakerFSM>();
        if (button.components.buttonFsm == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button buttonFsm");
        }
        button.components.highlight = GameObject.Find(highlightPath);
        if (button.components.highlight == null)
        {
            VSVRMod.logger.LogError(button.name + " had null button highlight");
        }
        return button;
    }

    //How to do stuff for later
    //text.SetText("100");
    //Slider.maxValue;
    //Slider.minValue;
    //Slider.value;
}