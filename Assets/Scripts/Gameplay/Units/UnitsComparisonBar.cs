using System.Collections;
using System.Collections.Generic;
using TMPro;
using util;
using UnityEngine;

namespace RM_MST
{
    // The units comparison bar.
    public class UnitsComparisonBar : MonoBehaviour
    {
        // The progress bar for the units comparison.
        public ProgressBar progressBar;

        // If 'true', the bar has a transition for showing the comparison.
        [Tooltip("If 'true', changes to the progress bar was gradual rather than instant.")]
        public bool barTransition = false;

        // The comparison for this units bar.
        public UnitsInfo.UnitsConversion conversion;

        // The fraction text.
        public TMP_Text fractionText;

        // Units conversion text.
        public TMP_Text conversionText;

        // If 'true', the conversion info is loaded on start.
        public bool loadConversionInfoOnStart = true;

        // Start is called before the first frame update
        void Start()
        {
            // If the conversion should be loaded on start, and there is a conversion to load.
            if(loadConversionInfoOnStart && conversion != null)
            {
                LoadConversionInfo();
            }
        }

        // Loads the entry information.
        public void LoadConversionInfo()
        {
            // Checks if there's a conversion set. If not, return null.
            if(conversion == null)
            {
                Debug.LogError("There is no conversion to pull from. Update failed.");
                return;
            }

            // Calculates the percentage.
            float percent = CalculatePercentage(true);

            // Sets the progress bar amount.
            progressBar.bar.interactable = true;
            progressBar.SetValueAsPercentage(percent, barTransition);

            // Creates the fraction and converison strings.
            // Fraction String
            // This now includes the input unit symbol.
            string fractionStr = 
                conversion.inputValue.ToString() + "/" + 
                conversion.GetConvertedValue().ToString() + " " +
                conversion.GetInputSymbol();

            // Conversion String
            string conversionStr = conversion.ToString();

            // Sets the text.
            fractionText.text = fractionStr;
            conversionText.text = conversionStr;
        }

        // Sets and loads the conversion info.
        public void LoadConversionInfo(UnitsInfo.UnitsConversion newConversion)
        {
            conversion = newConversion;
            LoadConversionInfo();

        }

        public void ClearConversionInfo()
        {
            // Conversion
            conversion = null;

            // Bars
            progressBar.bar.interactable = true;
            progressBar.SetValueAsPercentage(0.0F, barTransition);
            progressBar.bar.interactable = false;

            // Text
            fractionText.text = "-";
            conversionText.text = "-";
        }

        // Calculates the percentage comparison.
        // The percentage is measured as input/output.
        public float CalculatePercentage(bool applyClamp01)
        {
            // If there is no conversion set, return 0.
            if(conversion == null)
            {
                Debug.LogWarning("There is no conversion set, returning 0");
                return 0.0F;
            }

            // Gets the input and output values.
            float inputValue = conversion.inputValue;
            float outputValue = conversion.GetConvertedValue();

            // The percentage amount.
            float percent;

            // If the output is 0, set the percent as 0.
            if (outputValue == 0.0f)
            {
                percent = 0.0F;
            }
            else // Equal
            {
                percent = inputValue / outputValue;
            }

            // If the percentage should be clamped.
            if (applyClamp01)
            {
                percent = Mathf.Clamp01(percent);
            }

            // Returns the percent.
            return percent;
        }
    }
}