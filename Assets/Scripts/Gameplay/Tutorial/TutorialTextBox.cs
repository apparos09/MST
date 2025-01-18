using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // A script for special functions for the tutorial text box.
    public class TutorialTextBox : TextBox
    {
        [Header("TutorialTextBox")]
        // The speaker image.
        public Image characterImage;

        // The border for the speaker image.
        public Image characterBorderImage;

        // SPEAKER //
        // Sets the speaker image.
        public void SetCharacterImage(Sprite charSprite)
        {
            SetCharacterImageAndBorderImage(charSprite, characterBorderImage.sprite);
        }

        // Clears the speaker image.
        public void ClearCharacterImage()
        {
            characterImage.sprite = null;
        }

        // Sets the speaker image.
        public void SetCharacterImageAndBorderImage(Sprite charSprite, Sprite borderSprite)
        {
            characterImage.sprite = charSprite;
            characterBorderImage.sprite = borderSprite;
        }

        // Clears the speaker image.
        public void ClearCharacterImageAndBorderImage()
        {
            characterImage.sprite = null;
            characterBorderImage.sprite = null;
        }

        // Gets the border color.
        public Color GetBorderColor()
        {
            return characterBorderImage.color;
        }

        // Sets the border oclor.
        public void SetBorderColor(Color color)
        {
            characterBorderImage.color = color;
        }

        // Sets the border color to white.
        public void ResetBorderColor()
        {
            characterBorderImage.color = Color.white;
        }


        // SHOW/HIDE
        // Shows the speaker image.
        public void ShowCharacterImage()
        {
            characterImage.gameObject.SetActive(true);
            characterBorderImage.gameObject.SetActive(true);
        }

        // Hides the speaker image.
        public void HideCharacterImage()
        {
            characterImage.gameObject.SetActive(false);
            characterBorderImage.gameObject.SetActive(false);
        }
    }
}