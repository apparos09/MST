using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using util;

namespace RM_MST
{
    // The stage end UI.
    public class StageEndUI : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The time text.
        public TMP_Text timeText;

        // The score text.
        public TMP_Text scoreText;

        // Start is called before the first frame update
        void Start()
        {
            // Sets the stage manager instance.
            if(stageManager == null)
                stageManager = StageManager.Instance;
        }

        // This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            ClearText();
        }

        // Automatically sets the time and score.
        public void AutoSetTimeAndScore()
        {
            // Grabs the instance.
            if(stageManager == null)
                stageManager = StageManager.Instance;

            // Sets with the current time, and gets the current final score.
            SetTimeText(stageManager.stageTime);
            SetScoreText(stageManager.CalculateStageFinalScore());
        }

        // Sets the time text using the provided sseconds.
        public void SetTimeText(float seconds)
        {
            timeText.text = StringFormatter.FormatTime(seconds, false, true, false);
        }

        // Sets the score text.
        public void SetScoreText(float score)
        {
            scoreText.text = score.ToString();
        }

        // Clears the text.
        public void ClearText()
        {
            timeText.text = "-";
            scoreText.text = "-";
        }
    }
}