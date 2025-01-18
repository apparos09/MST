using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RM_MST
{
    // The units table entry.
    public class UnitsTableEntry : MonoBehaviour
    {
        // The units table.
        public UnitsTable unitsTable;

        // The units info object.
        public UnitsInfo unitsInfo;

        // The conversion.
        public UnitsInfo.UnitsConversion conversion;

        // The input text and output text.
        public TMP_Text inputText;
        public TMP_Text outputText;

        // Start is called before the first frame update
        void Start()
        {
            // Sets the instance.
            if (unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // Apply the conversion.
            LoadConversion();
        }

        // Sets the conversion.
        public void SetConversion(UnitsInfo.UnitsConversion newConversion)
        {
            conversion = newConversion;
            LoadConversion();
        }

        // Loads the conversion information.
        public void LoadConversion()
        {
            // If the instance isn't set, set it.
            if (unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // If no conversion is set, clear the text. 
            if (conversion == null)
            {
                inputText.text = "";
                outputText.text = "";
                return;
            }

            // The conversion always has an input value of 1.
            conversion.inputValue = 1.0F;

            // The input and output units symbols.
            string inputSymbol = "";
            string outputSymbol = "";

            // Checks the conversion to get the input and output units.
            if(conversion is UnitsInfo.LengthConversion) // Length
            {
                UnitsInfo.LengthConversion lengthConvert = (UnitsInfo.LengthConversion)conversion;
                inputSymbol = unitsInfo.GetLengthUnitSymbol(lengthConvert.inputUnits);
                outputSymbol = unitsInfo.GetLengthUnitSymbol(lengthConvert.outputUnits);

            }
            else if(conversion is UnitsInfo.WeightConversion) // Weight
            {
                UnitsInfo.WeightConversion weightConvert = (UnitsInfo.WeightConversion)conversion;
                inputSymbol = unitsInfo.GetWeightUnitSymbol(weightConvert.inputUnits);
                outputSymbol = unitsInfo.GetWeightUnitSymbol(weightConvert.outputUnits);

            }
            else if(conversion is UnitsInfo.TimeConversion) // Time
            {
                UnitsInfo.TimeConversion timeConvert = (UnitsInfo.TimeConversion)conversion;
                inputSymbol = unitsInfo.GetTimeUnitSymbol(timeConvert.inputUnits);
                outputSymbol = unitsInfo.GetTimeUnitSymbol(timeConvert.outputUnits);
            }
            else if (conversion is UnitsInfo.CapacityConversion) // Capacity
            {
                UnitsInfo.CapacityConversion capacityConvert = (UnitsInfo.CapacityConversion)conversion;
                inputSymbol = unitsInfo.GetCapacityUnitSymbol(capacityConvert.inputUnits);
                outputSymbol = unitsInfo.GetCapacityUnitSymbol(capacityConvert.outputUnits);
            }

            // Sets the input text.
            inputText.text = conversion.inputValue.ToString() +  " " + inputSymbol;
            outputText.text = conversion.GetConvertedValue().ToString() + " " + outputSymbol;
        }

        // Clears the text.
        public void ClearText()
        {
            inputText.text = "";
            outputText.text = "";
        }
    }
}