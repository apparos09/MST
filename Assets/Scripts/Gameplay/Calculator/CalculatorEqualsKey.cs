using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The equals key.
    public class CalculatorEqualsKey : CalculatorKey
    {
        // Called when the key has been pressed.
        public override void OnKeyPressed()
        {
            calculator.TryClearError();
            calculator.TrySolve();
        }
    }
}