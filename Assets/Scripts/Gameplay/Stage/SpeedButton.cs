using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The speed button.
    public class SpeedButton : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The button.
        public Button button;

        // The button audo.
        public ButtonAudio buttonAudio;

        // Symbols
        [Header("Images, Sprites")]

        // The button symbol's image.
        public Image speedSymbolImage;

        // The sprite for slow speed.
        public Sprite slowSpeedSprite;

        // The sprite for normal speed.
        public Sprite normalSpeedSprite;

        // The sprite for fast speed.
        public Sprite fastSpeedSprite;

        // Audio
        [Header("Audio")]

        // The slow down SFX.
        public AudioClip slowDownSfx;

        // The speed up SFX.
        public AudioClip speedUpSfx;

        // Awake is called when the script instance is being loaded.
        void Awake()
        {
            // Button not set, so get it.
            if (button == null)
                button = GetComponent<Button>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Gets the stage manager.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Gets the button audio.
            if (buttonAudio == null)
                buttonAudio = GetComponent<ButtonAudio>();

            // Add on click.
            AddOnClick();

            // Makes sure the symbol is set correctly.
            OnClick();
        }

        // Add OnClick Delegate
        private void AddOnClick()
        {
            // If the button has been set.
            if (button != null)
            {
                // Listener for the tutorial toggle.
                button.onClick.AddListener(delegate
                {
                    OnClick();
                });
            }
        }

        // Remove OnClick Delegate
        private void RemoveOnClick()
        {
            // Remove the listener for onClick if the button has been set.
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }

        // Called when the button is clicked.
        private void OnClick()
        {
            // Refreshes the stage manager.
            AutosetStageManager();

            // Checks the game speed to know what sprite to display.
            // The audio clip is also changed so that it goes along with...
            // What the next operation will be.
            // While there is a speed up SFX, there is no option to make the game go faster.
            if (stageManager.IsNormalSpeed()) // Normal
            {
                speedSymbolImage.sprite = normalSpeedSprite;
                buttonAudio.audioClip = slowDownSfx;
            }
            else if (stageManager.IsFastSpeed()) // Fast
            {
                speedSymbolImage.sprite = fastSpeedSprite;
                buttonAudio.audioClip = slowDownSfx;
            }
            else if (stageManager.IsSlowSpeed()) // Slow
            {
                speedSymbolImage.sprite = slowSpeedSprite;
                buttonAudio.audioClip = speedUpSfx;
            }
        }

        // Refreshes the stage manager to check that it's set.
        private void AutosetStageManager()
        {
            // Gets the instance if it's not already set.
            if (stageManager == null)
                stageManager = StageManager.Instance;
        }

        // Refreshes the button icon.
        public void RefreshButtonIcon()
        {
            // Makes sure the stage manager is set.
            AutosetStageManager();

            // Checks the game speed to know what icon to display.
            if (stageManager.IsNormalSpeed()) // Normal
            {
                speedSymbolImage.sprite = normalSpeedSprite;
            }
            else if (stageManager.IsFastSpeed()) // Fast
            {
                speedSymbolImage.sprite = fastSpeedSprite;
            }
            else if (stageManager.IsSlowSpeed()) // Slow
            {
                speedSymbolImage.sprite = slowSpeedSprite;

            }
        }
    }
}