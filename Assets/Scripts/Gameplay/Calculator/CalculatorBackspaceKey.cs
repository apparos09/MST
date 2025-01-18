using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The backspace key for the calculator.
    public class CalculatorBackspaceKey : CalculatorKey
    {
        // Called when the key has been pressed.
        public override void OnKeyPressed()
        {
            calculator.TryClearError();
            calculator.RemoveLastCharacter();
        }
    }
}