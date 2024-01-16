using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VSVRMod2;

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
        public TMP_Text text;
    }
    static IntInput intInput = new IntInput();

    private struct FindomInput
    {
        public GameObject representative;
        public List<VSGenericButton> sendOptions;
        public VSGenericButton cancel;
        public Slider slider;
    }
    static FindomInput findomInput = new FindomInput();

    public static void SetupMenus()
    {
        //ChoiceUI-------------------
        choiceMenu.representative = GameObject.Find("ChoiceUI");

        PrepareUnusualButtonComponents(
            choiceMenu.favorite, 
            "Favorite Heart",
            "FavoriteHeart",
            "FavoriteHeart/DoneBG/DoneText/Collider",
            "FavoriteHeart/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        PrepareUnusualButtonComponents(
            choiceMenu.left,
            "Choice Left",
            "ChoiceUI/Choice1",
            "ChoiceUI/Choice1/Collider",
            "ChoiceUI/Choice1/Image (1)/Borders/LightBorder"
            );
        PrepareUnusualButtonComponents(
            choiceMenu.right,
            "Choice Right",
            "ChoiceUI/Choice2",
            "ChoiceUI/Choice2/Collider",
            "ChoiceUI/Choice2/Image (1)/Borders/LightBorder"
            );

        //StakesUI-------------------
        stakesMenu.representative = GameObject.Find("StakesUI");

        PrepareUnusualButtonComponents(
            stakesMenu.top,
            "Stakes Top",
            "StakesUI/BG1",
            "StakesUI/BG1/Collider",
            "StakesUI/BG1/Borders/LightBorder"
            );
        PrepareUnusualButtonComponents(
            stakesMenu.middle,
            "Stakes Middle",
            "StakesUI/BG2",
            "StakesUI/BG2/Collider",
            "StakesUI/BG2/Borders/LightBorder"
            );
        PrepareUnusualButtonComponents(
            stakesMenu.bottom,
            "Stakes Bottom",
            "StakesUI/BG3",
            "StakesUI/BG3/Collider",
            "StakesUI/BG3/Borders/LightBorder"
            );

        //Safeword-------------------
        stakesMenu.representative = GameObject.Find("Buttons/EndSession");

        PrepareUnusualButtonComponents(
            safewordMenu.goEasy,
            "Safeword Go Easy",
            "Buttons/GoEasy",
            "Buttons/GoEasy/DoneBG/DoneText/Collider",
            "Buttons/GoEasy/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        PrepareUnusualButtonComponents(
            safewordMenu.continueSession,
            "Safeword Continue",
            "Buttons/ContinueSession",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider",
            "Buttons/ContinueSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );
        PrepareUnusualButtonComponents(
            safewordMenu.endSession,
            "Safeword End Session",
            "Buttons/EndSession",
            "Buttons/EndSession/DoneBG/DoneText/Collider",
            "Buttons/EndSession/DoneBG/DoneText/Collider/ButtonPressReact"
            );

        //text.SetText("100");
        //Slider.maxValue;
        //Slider.minValue;
        //Slider.value;
    }

    private static void PrepareUnusualButtonComponents(VSGenericButton button, string name, string buttonObjectPath, string colliderPath, string highlightPath)
    {
        button = new VSChoiceButton();
        button.name = name;
        button.components.buttonObject = GameObject.Find(buttonObjectPath);
        button.components.collider = GameObject.Find(colliderPath);
        Buttons.CheckButtonComponentsCollider(choiceMenu.favorite);
        button.components.buttonFsm = choiceMenu.favorite.components.collider.GetComponent<PlayMakerFSM>();
        button.components.highlight = GameObject.Find(highlightPath);
        Buttons.CheckButtonComponents(choiceMenu.favorite);
    }
}