using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The results UI.
    public class ResultsUI : MonoBehaviour
    {
        // The results manager.
        public ResultsManager resultsManager;

        [Header("Text")]

        // The title text.
        public TMP_Text titleText;

        // The game time text.
        public TMP_Text gameTimeText;

        // The score text.
        public TMP_Text gameScoreText;

        [Header("Results Entries")]

        // The results entries.
        public List<ResultsEntry> resultsEntries;


        [Header("Buttons")]

        // Button for going to the title screen.
        public Button titleButton;

        // Button for completing the game.
        public Button finishButton;

        // Start is called before the first frame update
        void Start()
        {
            // Manager
            if (resultsManager == null)
                resultsManager = ResultsManager.Instance;


            // Results entries.
            if(resultsEntries == null)
                resultsEntries = new List<ResultsEntry>(FindObjectsOfType<ResultsEntry>());

            // If the LOLSDK has been initialized.
            if (GameSettings.InitializedLOLSDK)
            {
                // Turn off the title button, and turn on the finish button.
                titleButton.gameObject.SetActive(false);
                finishButton.gameObject.SetActive(true);
            }
            else
            {
                // Turn on title button, and turn off finish button.
                titleButton.gameObject.SetActive(true);
                finishButton.gameObject.SetActive(false);
            }

        }

        // Applies the results data.
        public void ApplyResultsData(ResultsData data)
        {
            // Time
            gameTimeText.text = StringFormatter.FormatTime(data.gameTime, false, true, false);

            // Score
            gameScoreText.text = data.gameScore.ToString();

            // Applies the entries from the stage data.
            for(int i = 0; i < resultsEntries.Count && i < data.stageDatas.Length; i++)
            {
                // Apply the data.
                resultsEntries[i].ApplyStageData(data.stageDatas[i]);
            }

            // If the game settings exist.
            if(GameSettings.Instantiated)
            {
                // If the player can select the mode, show what mode they selected.
                if (GameSettings.Instance.allowPlayerSelectMode)
                {
                    // Looks at the game mode and checks what to display.
                    switch (data.gameMode)
                    {
                        case GameplayManager.gameMode.focus:
                            titleText.text = "Results - Focus Mode";
                            break;

                        case GameplayManager.gameMode.rush:
                            titleText.text = "Results - Rush Mode";
                            break;
                    }
                }
            }

        }

        // Goes to the title scene.
        public void ToTitleScene()
        {
            resultsManager.ToTitleScene();
        }
        
        // Complete the game.
        public void CompleteGame()
        {
            resultsManager.CompleteGame();
        }
    }
}