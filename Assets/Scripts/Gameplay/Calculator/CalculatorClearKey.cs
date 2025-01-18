using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The calculator clear key.
    public class CalculatorClearKey : CalculatorKey
    {
        // Called when the key has been pressed.
        public override void OnKeyPressed()
        {
            calculator.TryClearError();
            calculator.Clear();
        }
    }
}