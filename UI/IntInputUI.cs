using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSVRMod2.UI;

public class IntInputUIMManager : UIManager
{
    private struct IntInput
    {
        public GameObject representative;
        public TMP_InputField text;
    }
    IntInput intInput = new IntInput();

    public IntInputUIMManager(Scene scene)
    {
        if (scene.isLoaded && Equals(scene.name, Constants.sessionScene))
        {
            Transform eventManager = GameObject.Find("GeneralCanvas/EventManager").transform;

            //Count Input-------------------
            intInput.representative = eventManager.Find("IntInputField").gameObject;
            if (intInput.representative == null)
            {
                VSVRMod.logger.LogError("Int input had null representative");
            }

            intInput.text = intInput.representative.GetComponent<TMP_InputField>();
            if (intInput.text == null)
            {
                VSVRMod.logger.LogError("Int input text was null");
            }
            VSVRMod.logger.LogInfo("Setup Count Input");
        }
        else
        {
            throw new ArgumentException("Session scene is incorrect or not yet loaded");
        }
    }

    private float intInputInteractionNext = 0;
    private float intInputInteractionAccel = 0.4f;
    public bool Interact()
    {
        if (intInput.representative.activeSelf)
        {
            Vector2 vector2 = Controller.GetMaximalJoystickValue();
            double x = vector2.x;
            double y = vector2.y;
            double magnitude = Controller.GetMaximalJoystickMagnitude();
            if (y > -0.1 && magnitude > 0.05)
            {
                if (intInputInteractionNext < Time.time)
                {
                    string current = intInput.text.text;
                    int.TryParse(current, out int currentInt);
                    currentInt += (int)Math.Round(x * 4);
                    intInputInteractionNext = Time.time + intInputInteractionAccel;
                    intInputInteractionAccel = Math.Clamp(intInputInteractionAccel - 0.04f, 0.04f, float.PositiveInfinity);
                    intInput.text.text = currentInt.ToString();
                }
                return true;
            }
            else
            {
                intInputInteractionAccel = 0.4f;
            }
        }
        return false;
    }
}
