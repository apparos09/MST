using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RM_MST
{
    // The puzzle conversion display.
    public class PuzzleConversionDisplay : MonoBehaviour
    {
        // The symbol image.
        public Image symbolImage;

        // The symbol background image.
        public Image symbolBgImage;

        // The conversion text.
        public TMP_Text measurementValueText;

        // The multiplier text.
        public TMP_Text conversionMultipleText;

        // The units button this conversion display corresponds to.
        public UnitsButton unitsButton;

        // Checks if the units button info should be gotten on start.
        public bool getUnitsButtonInfoOnStart = true;

        // Start is called before the first frame update
        void Start()
        {
            // If the info should be set by the units button on start...
            // And there is a units button, set it.
            if(getUnitsButtonInfoOnStart && unitsButton != null)
            {
                SetInfoFromUnitsButton();
            }
        }

        // Sets the symbol sprite.
        public void SetSymbolSprite(Sprite symbolSprite)
        {
            symbolImage.sprite = symbolSprite;
        }

        // Sets the infro from the saved units button.
        public void SetInfoFromUnitsButton()
        {
            // Text
            measurementValueText.text = unitsButton.measurementValueText.text;
            conversionMultipleText.text = unitsButton.conversionMultipleText.text;

            // Colour
            symbolBgImage.color = unitsButton.button.image.color;

            // Sets the conversion text alpha from the units button.
            SetConversionTextAlphaFromUnitsButton();
        }

        // Sets the text using a units button.
        public void SetInfoFromUnitsButton(UnitsButton unitsButton)
        {
            this.unitsButton = unitsButton;

            SetInfoFromUnitsButton();
        }

        // Sets the conversion text colour from the units button.
        public void SetConversionTextAlphaFromUnitsButton()
        {
            // Copy the alpha from the unit button's multipler text.
            // If there is no units button, it resets to 1.0.
            float multAlpha = (unitsButton != null) ? unitsButton.conversionMultipleText.color.a : 1.0F;
            Color altColor = conversionMultipleText.color;

            // Apply the alpha from the units button's multipler text.
            altColor.a = multAlpha;
            conversionMultipleText.color = altColor;
        }

        // Resets the text alpha.
        public void ResetConversionMultipleTextAlpha()
        {
            // Reset the colour for the multiplier text.
            Color altColor = conversionMultipleText.color;
            altColor.a = 1.0F;
            conversionMultipleText.color = altColor;
        }

        // Clears all elements.
        public void Clear()
        {
            measurementValueText.text = "-";
            conversionMultipleText.text = "-";

            // Reset the colour.
            ResetConversionMultipleTextAlpha();
        }

        // Update is called once per frame
        void Update()
        {
            // Gets set to 'true' if the alpha of the text should be checked.
            bool checkTextAlpha;

            // Units button is set.
            if(unitsButton != null)
            {
                // If a multiplier reveal is playing, set the alpha based on it.
                if(unitsButton.IsMultipleRevealPlaying())
                {
                    SetConversionTextAlphaFromUnitsButton();
                    checkTextAlpha = false;
                }
                else // If there is no reveal, reset the alpha if it is 0.
                {
                    checkTextAlpha = true;
                }
            }
            else
            {
                checkTextAlpha = true;
            }

            // If the text alpha should be checked.
            if(checkTextAlpha)
            {
                // Alpha is wrong, so reset it.
                if (conversionMultipleText.color.a < 1.0F)
                {
                    ResetConversionMultipleTextAlpha();
                }
            }
        }

    }
}
