using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RM_MST
{
    // The event text display.
    public class EventTextDisplay : MonoBehaviour
    {
        // The stage UI.
        public StageUI stageUI;

        // The display text.
        public TMP_Text displayText;

        [Header("Strings")]
        // The correct string.
        private string correctStr = "Correct!";

        // The translation key for a correct response.
        public const string CORRECT_KEY = "kwd_correct";

        // The incorrect string.
        private string incorrectStr = "Incorrect!";

        // The translation key for a incorrect response.
        public const string INCORRECT_KEY = "kwd_incorrect";

        [Header("Animation")]
        // The animator for the event text.
        public Animator animator;

        // The empty animation.
        public string emptyAnim;

        // The hide animation.
        public string hideAnim;

        // The display animation.
        public string displayAnim;

        // Start is called before the first frame update
        void Start()
        {
            // Sets the instance for the stage UI if it isn't set.
            if (stageUI == null)
                stageUI = StageUI.Instance;

            // If the display text isn't set.
            if(displayText == null)
                displayText = GetComponentInChildren<TMP_Text>();

            // If animator not set, set it.
            if(animator == null)
                animator = GetComponent<Animator>();


            // Checks if the LOL manager is instnatiated and initialized.
            if(SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            {
                SystemManager lolManager = SystemManager.Instance;

                correctStr = lolManager.GetLanguageText(CORRECT_KEY);
                incorrectStr = lolManager.GetLanguageText(INCORRECT_KEY);
            }

            // Plays the hide animation.
            PlayHideAnimation();
        }

        // Gets the correct string.
        public string GetCorrectString()
        {
            return correctStr;
        }

        // Gets the incorrect string.
        public string GetIncorrectString()
        {
            return incorrectStr;
        }

        // ANIMATION
        // Plays the empty animation.
        public void PlayEmptyAnimation()
        {
            animator.Play(emptyAnim);
        }

        // Plays the hide animation.
        public void PlayHideAnimation()
        {
            animator.Play(hideAnim);
        }


        // Plays the display animation.
        public void PlayDisplayAnimation(string text)
        {
            displayText.text = text;
            animator.Play(displayAnim);
        }

        // Plays the display animation with the correct string.
        public void PlayDisplayCorrectAnimation()
        {
            PlayDisplayAnimation(correctStr);

            // Checks if text-to-speech is being used.
            if(GameSettings.Instance.UseTextToSpeech)
            {
                // If text-to-speech is available.
                if (SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
                {
                    SystemManager.Instance.SpeakText(CORRECT_KEY);
                }
            }
            
        }

        // Plays the display animation with the incorrect string.
        public void PlayDisplayIncorrectAnimation()
        {
            PlayDisplayAnimation(incorrectStr);

            // Checks if text-to-speech is being used.
            if (GameSettings.Instance.UseTextToSpeech)
            {
                // If text-to-speech is available.
                if (SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
                {
                    SystemManager.Instance.SpeakText(INCORRECT_KEY);
                }
            }
        }

        // Display animation start.
        public void OnDisplayAnimationStart()
        {
            // ...
        }

        // Display animation end.
        public void OnDisplayAnimationEnd()
        {
            displayText.text = "";
            PlayHideAnimation();
        }
    }
}