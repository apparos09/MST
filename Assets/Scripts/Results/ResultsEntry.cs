using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using util;

namespace RM_MST
{
    // A results entry.
    public class ResultsEntry : MonoBehaviour
    {
        // The name text.
        public TMP_Text stageNameText;

        // The time text.
        public TMP_Text stageTimeText;

        // The score text.
        public TMP_Text stageScoreText;

        // The combo text.
        public TMP_Text stageHighestComboText;

        // Start is called before the first frame update
        void Start()
        {
            // ...
        }

        // Applies the stage data.
        public void ApplyStageData(StageData stageData)
        {
            // If the stage data is set, apply it.
            // If it's not set, clear the results entry.
            if (stageData != null)
                ApplyStageData(stageData.stageName, stageData.stageTime, stageData.stageScore, stageData.highestCombo);
            else
                ClearResultsEntry();
        }

        // Applies the stage data by individual values.
        public void ApplyStageData(string stageName, float stageTime, float stageScore, int stageHighestCombo)
        {
            stageNameText.text = stageName;
            stageTimeText.text = StringFormatter.FormatTime(stageTime, false, true, false);
            stageScoreText.text = stageScore.ToString();
            stageHighestComboText.text = stageHighestCombo.ToString();
        }

        // Clears the stage data.
        public void ClearResultsEntry()
        {
            ApplyStageData("-", 0, 0, 0);
        }
    }
}