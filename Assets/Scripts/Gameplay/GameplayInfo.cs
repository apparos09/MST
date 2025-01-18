using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // This script is used for game info that's shared between all scenes.
    public class GameplayInfo : MonoBehaviour
    {
        // The information for starting a stage.
        public struct StageStartInfo
        {
            // Valid - see if the stage start info is valid.
            public bool valid;

            // Name
            public string name;

            // Unit Groups
            public List<UnitsInfo.unitGroups> stageUnitGroups;

            // Puzzle Type
            public PuzzleManager.puzzleType stagePuzzleType;

            // Background Number
            public int bgdNumber;

            // Music/BGM Number
            public int bgmNumber;

            // Difficulty
            public int difficulty;

            // Losses
            public int losses;

            // Index in the Stage List
            public int index;
        }

        // The singleton instance.
        private static GameplayInfo instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The game time.
        public float gameTime = 0.0F;

        // The game score - no longer needed.
        // public float gameScore = 0;

        // Game Mode
        public GameplayManager.gameMode gameMode;

        [Header("WorldInfo")]

        // If the class has world info.
        public bool hasWorldInfo = false;

        // The data for all the world stages.
        public StageData[] worldStages = new StageData[WorldManager.STAGE_COUNT];

        // The stage start information.
        public StageStartInfo stageStartInfo;

        [Header("StageInfo")]

        // If the class has stage info.
        public bool hasStageInfo = false;

        // Gets set to 'true' when the most recent stage was cleared.
        public bool recentStageCleared = false;
        
        // Constructor
        private GameplayInfo()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected virtual void Awake()
        {
            // If the instance hasn't been set, set it to this object.
            if (instance == null)
            {
                instance = this;
            }
            // If the instance isn't this, destroy the game object.
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Run code for initialization.
            if (!instanced)
            {
                instanced = true;

                // Don't destroy this game object on load.
                DontDestroyOnLoad(gameObject);

                // Blank data to start.
                stageStartInfo = new StageStartInfo();
                stageStartInfo.valid = false; // Don't read from this.
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            // ...
        }

        // Gets the instance.
        public static GameplayInfo Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<GameplayInfo>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Gameplay Info (singleton)");
                        instance = go.AddComponent<GameplayInfo>();
                    }

                }

                // Return the instance.
                return instance;
            }
        }

        // Returns 'true' if the object has been initialized.
        public static bool Instantiated
        {
            get
            {
                return instanced;
            }
        }

        // Saves info from the game manager.
        public void SaveGameplayInfo(GameplayManager gameManager)
        {
            gameTime = gameManager.gameTime;
            // gameScore = gameManager.gameScore;
            gameMode = gameManager.gameplayMode;
        }

        // Loads game info into the game manager.
        public void LoadGameplayInfo(GameplayManager gameManager)
        {
            gameManager.gameTime = gameTime;
            // gameManager.gameScore = gameScore;
            gameManager.gameplayMode = gameMode;
        }


        // Saves the world info from the world manager object.
        public void SaveWorldInfo(WorldManager worldManager)
        {
            SaveGameplayInfo(worldManager);

            // Tries to grab the next stage.
            StageWorld nextStage = worldManager.worldUI.stageWorldUI.stageWorld;

            // Next stage is set.
            if (nextStage != null)
            {
                // Generates the stage info and sets it.
                stageStartInfo = nextStage.GenerateStageInfo();
            }

            // Saves what stages have been cleared.
            for(int i = 0; i < worldStages.Length && i < worldManager.stages.Count; i++)
            {
                // If there are stages, get the clear value and the losses.
                if (worldStages[i] != null && worldManager.stages[i] != null)
                {
                    worldStages[i].losses = worldManager.stages[i].losses;
                    worldStages[i].cleared = worldManager.stages[i].IsStageCleared();
                }
                // There's no data in game info, but there is data in the world manager.
                else if (worldStages[i] == null && worldManager.stages[i] != null)
                {
                    // Overwrites the stage data.
                    worldStages[i] = worldManager.stages[i].GenerateStageData();
                }
                    
            }

            // There is world info.
            hasWorldInfo = true;
        }

        // Loads the world info into the world object.
        public void LoadWorldInfo(WorldManager worldManager)
        {
            LoadGameplayInfo(worldManager);

            // Sets what stages have been cleared.
            for (int i = 0; i < worldStages.Length && i < worldManager.stages.Count; i++)
            {
                // If there is a stage, set the losses and clear value.
                if (worldManager.stages[i] != null && worldStages[i] != null)
                {
                    worldManager.stages[i].losses = worldStages[i].losses;
                    worldManager.stages[i].SetStageCleared(worldStages[i].cleared);
                }
            }
        }

        // Saves the stage info from the stage manager object.
        public void SaveStageInfo(StageManager stageManager)
        {
            SaveGameplayInfo(stageManager);

            // If the stage index is valid, save the information.
            if(IsValidStageIndex(stageManager.stageIndex))
            {
                // Replace the stage data.
                worldStages[stageManager.stageIndex] = stageManager.GenerateStageData();
            }

            // Sets if the stage was cleared or not.
            recentStageCleared = stageManager.cleared;

            // There is stage info.
            hasStageInfo = true;
        }

        // Loads the stage info into the stage object.
        public void LoadStageInfo(StageManager stageManager)
        {
            LoadGameplayInfo(stageManager);

            // Apply the stage start info.
            stageManager.ApplyStageStartInfo(stageStartInfo);

            // The most recent stage has not been played yet, so don't mark it as cleared.
            recentStageCleared = false;
        }

        // Checks if the provided index is valid for the stage array.
        public bool IsValidStageIndex(int index)
        {
            // If the index is valid, return true. If invalid, return false.
            if(index >= 0 && index < worldStages.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // This function is called when the MonoBehaviour will be destroyed.
        private void OnDestroy()
        {
            // If the saved instance is being deleted, set 'instanced' to false.
            if (instance == this)
            {
                instanced = false;
            }
        }
    }
}