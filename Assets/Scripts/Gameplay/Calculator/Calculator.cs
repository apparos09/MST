using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RM_MST
{
    // An in-game calculator.
    public class Calculator : MonoBehaviour
    {
        // The text for the calculator display.
        public TMP_Text displayText;

        // The maximum number of characters for the calculator display.
        public const int DISPLAY_CHAR_MAX = 35;

        // If 'true', the character limit is used.
        private bool useCharLimit = true;

        // The keys for the calculator.
        public List<CalculatorKey> keys = new List<CalculatorKey>();

        // Used to see if the calculator is interactable.
        private bool interactable = true;

        // The error string.
        private string errorStr = "Error";

        // The error string key.
        public const string ERROR_STR_KEY = "kwd_error";

        // Start is called before the first frame update
        void Start()
        {
            // If there are no saved keys, auto-set.
            if(keys.Count == 0)
            {
                keys = new List<CalculatorKey>(GetComponentsInChildren<CalculatorKey>());
            }

            // If the LOL SDK and the LOL Manager is initialized.
            if(SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            {
                // Translates the error string.
                errorStr = SystemManager.Instance.GetLanguageText(ERROR_STR_KEY);
            }


            // Clear the calculator to start.
            Clear();
        }

        // Adds the key's character.
        public void AddCharacterToEquation(char keyChar)
        {
            // Tries to clear the error.
            TryClearError();

            // Checks what kind of character has been sent.
            switch(keyChar)
            {
                case 'B':
                case 'b': // "B" means Backspace
                    RemoveLastCharacter();
                    break;

                case 'C': // "C" means Clear
                case 'c':
                    Clear();
                    break;

                case '=': // "=" means Solve
                    TrySolve();
                    break;

                default:
                    // If the display text plus 1 is below the max char count...
                    // Add the character. This only happens if the character limit is enabled.
                    // If the character limit is disabled, just add characters no matter what.
                    if((useCharLimit && displayText.text.Length + 1 <= DISPLAY_CHAR_MAX) || !useCharLimit)
                    {
                        displayText.text += keyChar;
                    }
                    
                    break;
            }
        }

        // Tries to solve the equation put into the calculator.
        // The result is saved in the display text.
        public bool TrySolve()
        {
            // Sets the equation.
            string equation = displayText.text;

            // Replaces all "X" and "x" with "*".
            equation = equation.Replace("X", "*");
            equation = equation.Replace("x", "*");

            // Calculates the result.
            string result = util.CustomMath.CalculateMathString(equation);

            // Checks if the result is valid, and returns the result.
            bool valid = result != "";

            // Sets the display text's result.
            // A blank string is returned when a calculation fails.

            // If the string is valid, set it as the display string.
            if(valid)
            {
                displayText.text = result;
            }
            else // Not valid, so set the display text as an error.
            {
                displayText.text = errorStr;

                // This gets overwritten by other TTS calls, so it was removed.
                // // If the game settings and the LOL Manager is usable.
                // if(LOLManager.IsInstantiatedAndIsLOLSDKInitialized() && GameSettings.Instantiated)
                // {
                //     // If TTS is enabled.
                //     if(GameSettings.Instance.UseTextToSpeech)
                //     {
                //         // Read out 'error'.
                //         LOLManager.Instance.SpeakText(ERROR_STR_KEY);
                //     }
                // }
            }

            // Returns valid.
            return valid;
        }

        // Removes the last character in the string.
        public void RemoveLastCharacter()
        {
            // There are characters to remove.
            if(displayText.text.Length > 0)
            {
                // Gets the string, removes the last character, and sets it.
                string str = displayText.text;
                str = str.Remove(str.Length - 1);
                displayText.text = str;
            }
        }

        // Clears the equation.
        public void Clear()
        {
            displayText.text = "";
        }

        // Tries to clear the error from the calculator.
        public bool TryClearError()
        {
            // If the display text is the error string, clear it.
            if (displayText.text == errorStr)
            {
                Clear();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Sets if the calculator is interactable or not.
        public void SetCalculatorInteractable(bool interactable)
        {
            // Save the value.
            this.interactable = interactable;

            // TODO: change the display text? Maybe change the text colour, or leave it as is?

            // Goes through all the keys.
            foreach(CalculatorKey key in keys)
            {
                // Change interactable of all buttons.
                key.button.interactable = interactable;
            }
        }
    }
}