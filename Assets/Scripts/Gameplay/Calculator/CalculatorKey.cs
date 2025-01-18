using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RM_MST
{
    // A key for the calculator.
    public class CalculatorKey : MonoBehaviour
    {
        // The calculator this key is attached to.
        public Calculator calculator;

        // The button for this calculator key.
        public Button button;

        // The key text.
        public TMP_Text keyText;

        // The key character. This is used to determine what key is provided.
        public char keyChar;

        // Start is called before the first frame update
        void Start()
        {
            // Tries to get the calculator in the parent component if it hasn't been set.
            if(calculator == null)
                calculator = GetComponentInParent<Calculator>();

            // Tries to get the calculator button.
            if (button == null)
                button = GetComponent<Button>();
        }

        // Called when the key has been pressed.
        public virtual void OnKeyPressed()
        {
            calculator.AddCharacterToEquation(keyChar);
        }

        // Adds the character to the equation.
        public void AddCharacterToEquation()
        {
            calculator.AddCharacterToEquation(keyChar);
        }
    }
}