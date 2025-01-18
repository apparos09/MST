using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The results data.
    public class ResultsData : MonoBehaviour
    {
        // The game time.
        public float gameTime = 0;

        // The game score.
        public float gameScore = 0;

        // The stage data.
        public StageData[] stageDatas = new StageData[WorldManager.STAGE_COUNT];

        // The game mode.
        public GameplayManager.gameMode gameMode = GameplayManager.gameMode.focus;
    }
}