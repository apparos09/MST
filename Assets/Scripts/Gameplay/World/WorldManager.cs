using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RM_MST
{
    // The world manager.
    public class WorldManager : GameplayManager
    {
        // The stage count.
        public const int STAGE_COUNT = 9;

        // the instance of the class.
        private static WorldManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        [Header("WorldManager")]
        // The world user interface.
        public WorldUI worldUI;

        // The world audio for the game.
        public WorldAudio worldAudio;

        // The player in the world.
        public PlayerWorld playerWorld;

        // The world area.
        public WorldArea worldArea;

        // The game complete event.
        public GameCompleteEvent gameCompleteEvent;

        // If 'true', auto saving is enabled.
        private bool autoSave = true;

        [Header("Area")]

        // The stage list.
        public List<StageWorld> stages = new List<StageWorld>();

        // The stage scene.
        public string stageScene = "StageScene";

        // Constructor
        private WorldManager()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected override void Awake()
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
            }
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // TODO: check for save data first.

            // Sets the world UI.
            if (worldUI == null)
                worldUI = WorldUI.Instance;

            // Sets the world audio.
            if(worldAudio == null)
                worldAudio = WorldAudio.Instance;

            // If there are no stages in the stage list.
            if(stages.Count == 0)
            {
                // Finds all the stages in the world and puts them in the list.
                stages = new List<StageWorld>(FindObjectsOfType<StageWorld>(true));
            }

            // Checks that the stage count matches the expected count.
            if(stages.Count != STAGE_COUNT)
            {
                Debug.LogWarning("The stage list length does not match the expected stage count.");
            }

            // If the game settings exist.
            if(GameSettings.Instantiated)
            {
                // Gets the game mode from the game settings. This will be overwritten by gameplay info if it exists.
                gameplayMode = GameSettings.Instance.gameMode;
            }
        }

        // The function called after the start function.
        protected override void LateStart()
        {
            base.LateStart();

            // Tries to load the game if this is set to 'true'.
            bool tryLoadGame = false;

            // If the gameplay info has been instantiated.
            if (GameplayInfo.Instantiated)
            {
                // Load info into the world if it exists.
                if (gameInfo.hasWorldInfo)
                {
                    gameInfo.LoadWorldInfo(this);

                    // Checks if the most recently used stage was cleared.
                    if(gameInfo.recentStageCleared)
                    {
                        // If the game should auto save, the game can save...
                        // And the most recent stage was cleared.
                        if (autoSave && IsSavingLoadingEnabled())
                        {
                            SaveGame();
                        }

                        // Submits progress
                        SubmitProgress();
                    }

                }
                else
                {
                    tryLoadGame = true;
                }

            }
            else
            {
                tryLoadGame = true;
            }

            // If saving/loading is enabled, and the game should try to load data.
            if(IsSavingLoadingEnabled() && tryLoadGame)
            {
                // If the save system has data to load.
                if(SystemManager.Instance.saveSystem.HasLoadedData())
                {
                    LoadGame();
                }
            }

            // Update the game mode display.
            worldUI.gameModeText.text = GetGameplayModeAsString();

            // The game complete event is set.
            if (gameCompleteEvent != null)
            {
                // Checks game complete to see if the game is finished.
                if(!gameCompleteEvent.cleared)
                    gameCompleteEvent.CheckGameComplete();
            }

            // If the tutorial is being used.
            if(IsUsingTutorial())
            {
                // If the intro hasn't been cleared, play it.
                if(!tutorials.clearedIntroTutorial)
                {
                    tutorials.LoadIntroTutorial();
                }
                else // Intro cleared.
                {
                    // The first win hasn't been cleared.
                    if(!tutorials.clearedFirstWinTutorial)
                    {
                        // Only use this tutorial if a stage has been cleared.
                        if(GetStagesClearedCount() > 0)
                        {
                            // Load the first win tutorial.
                            tutorials.LoadFirstWinTutorial();
                        }
                    }
                }
            }

            // Loads the test tutorial.
            // tutorials.LoadTutorialTest();
        }

        // Gets the instance.
        public static WorldManager Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<WorldManager>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("WorldManager (singleton)");
                        instance = go.AddComponent<WorldManager>();
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

        // Checks if the stage world index is valid.
        public bool IsStageWorldIndexValid(int index)
        {
            // Returns true if the index is valid.
            if(index >= 0 && index < stages.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Gets the stage index.
        public int GetStageWorldIndex(StageWorld stage)
        {
            // If the stage is in the list, returns the index.
            if(stages.Contains(stage))
            {
                return stages.IndexOf(stage);
            }
            else
            {
                return -1;
            }
        }

        // Gets the number of stages that have been cleared.
        public int GetStagesClearedCount()
        {
            int result = 0;

            // Goes through all stages.
            for(int i = 0; i < stages.Count; i++)
            {
                // If the stage is cleared, add to the result.
                if (stages[i].IsStageCleared())
                {
                    result++;
                }
            }

            // Return the result.
            return result;
        }

        // SAVING/LOADING
        // Generates the save data or the game.
        public MST_GameData GenerateSaveData()
        {
            // The data.
            MST_GameData data = new MST_GameData();

            // Sets if the tutorial is being used.
            data.useTutorial = GameSettings.Instance.UseTutorial;

            // Sets the game time.
            data.gameTime = gameTime;

            // Sets the game score.
            data.gameScore = CalculateGameScore();

            // Adds in the stage data.
            for(int i = 0; i < data.stageDatas.Length && i < gameInfo.worldStages.Length; i++)
            {
                data.stageDatas[i] = gameInfo.worldStages[i];
            }

            // Sets the tutorials data.
            data.tutorialData = Tutorials.Instance.GenerateTutorialsData();

            // Saves the gameplay mode.
            data.gameMode = gameplayMode;

            // Saves if the game is completed.
            data.complete = IsGameComplete();

            // The data is valid.
            data.valid = true;

            // Return the data.
            return data;
        }

        // Sets if the game should be auto saving.
        public bool AutoSave
        {
            get 
            { 
                return autoSave;
            }
            
            set 
            {
                autoSave = value;
            }
        }

        // Saves the data for the game.
        public bool SaveGame()
        {
            // The LOL system is not checked for anymore since the game no longer uses it.

            // // If the LOL Manager does not exist, or the LOL SDK has not been initialized, return false.
            // if (!SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            // {
            //     Debug.LogError("The LOL Manager does not exist.");
            //     return false;
            // }

            // Gets the save system.
            SaveSystem saveSys = SystemManager.Instance.saveSystem;

            // Checks if the save system exists.
            if (saveSys == null)
            {
                Debug.LogError("The save system could not be found.");
                return false;
            }

            // Set the world manager.
            if (saveSys.worldManager == null)
                saveSys.worldManager = this;

            // Saves the game (asynchrnously)
            bool result = saveSys.SaveGame(true);

            // Return result.
            return result;
        }

        // Loads data, and return a 'bool' to show it was successful.
        public bool LoadGame()
        {
            // If the LOL Manager does not exist, or the LOL SDK has not been initialized, return false.
            if (!SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            {
                Debug.LogError("The LOL Manager does not exist or the LOL SDK has not been initialized.");
                return false;
            }

            // Gets the save system.
            SaveSystem saveSys = SystemManager.Instance.saveSystem;

            // Checks if the save system exists.
            if (saveSys == null)
            {
                Debug.LogError("The save system could not be found.");
                return false;
            }


            // Gets the loaded data.
            MST_GameData loadedData = saveSys.loadedData;

            // No data to load.
            if (loadedData == null)
            {
                Debug.LogError("The save data does not exist.");
                return false;
            }

            // Data invalid.
            if (loadedData.valid == false)
            {
                Debug.LogError("The save data is invalid.");
                return false;
            }

            // Game complete
            if (loadedData.complete)
            {
                // Changed from assertion to normal log.
                // Debug.LogAssertion("The game was completed, so the data hasn't been loaded.");
                Debug.Log("The game was completed, so the data hasn't been loaded.");
                return false;
            }

            // Tutorial setting.
            GameSettings.Instance.UseTutorial = loadedData.useTutorial;

            // Sets the game time.
            gameTime = loadedData.gameTime;

            // Sets the game score.
            // gameScore = loadedData.gameScore; // Not needed.

            // Loads the stage data.
            for (int i = 0; i < loadedData.stageDatas.Length && i < stages.Count; i++)
            {
                // The data exists.
                if(loadedData.stageDatas[i] != null)
                {
                    gameInfo.worldStages[i] = loadedData.stageDatas[i]; // Game Info
                    stages[i].LoadStageDataFromSavedGame(loadedData.stageDatas[i]); // Stages
                }
            }

            // Sets the tutorials data.
            Tutorials.Instance.LoadTutorialsData(loadedData.tutorialData);

            // Loads the gameplay mode.
            gameplayMode = loadedData.gameMode;

            // Save the world info.
            gameInfo.SaveWorldInfo(this);

            // The data has been loaded successfully.
            return true;
        }

        // GAME PROGRESS

        // Gets the game progress.
        public int GetGameProgress()
        {
            // Progress
            int progress = 0;

            // Increases progress for every defeated challenger.
            for (int i = 0; i < stages.Count; i++)
            {
                // If the stage is cleared, mark it as complete.
                if (stages[i].IsStageCleared())
                    progress++;
            }

            // Returns the progress.
            return progress;
        }

        // Gets the game progress as a percentage.
        // If the percentage can't be calculated, -1 is returned.
        public float GetGameProgressAsPercentage()
        {
            // Gets the number of cleared stages.
            int clearedCount = GetGameProgress();
            float progress = 0;

            // Calculates the amount of progress that has been made.
            if(stages.Count > 0)
            {
                progress = (float)clearedCount / stages.Count;
            }
            else // Count is unknown, so just set it to -1.
            {
                progress = -1;
            }

            // Returns the progress.
            return progress;
        }

        // Submits the current game progress.
        public void SubmitProgress()
        {
            // If the LOLManager and the SDK have both been initialized, submit the score and game progress.
            if (SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
                SystemManager.Instance.SubmitProgress(CalculateGameScoreAsInt(), GetGameProgress());
        }

        // Submits the game progress complete.
        public void SubmitProgressComplete()
        {
            // Submits the game score and progress complete.
            if (SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
                SystemManager.Instance.SubmitProgressComplete(CalculateGameScoreAsInt());
        }


        // Returns cleared on game complete event.
        public bool IsGameComplete()
        {
            return gameCompleteEvent.cleared;
        }

        // Overrides on game complete.
        public override void OnGameComplete()
        {
            // Opens the UI.
            worldUI.OnGameComplete();

            // Play the game complete music.
            worldAudio.PlayGameCompleteMusic();

            // Called when going to the results scene.
            // Submit progress complete.
            // SubmitProgressComplete();
        }


        // SCENES/GAME END //

        // Goes to the stage.
        public void ToStage(StageWorld stageWorld)
        {
            // Saves the world info and goes into the stage.
            // CalculateAndSetGameScore(); // Not needed.
            gameInfo.SaveWorldInfo(this);
            LoadStageScene();
        }

        // Loads the stage scene.
        public void LoadStageScene()
        {
            UnpauseGame();

            LoadScene(stageScene);
        }


        // When going to the results scene, create the results data.
        public override void ToResults()
        {
            // Loads the world information and saves the game.
            gameInfo.SaveWorldInfo(this);

            // The results data and object.
            GameObject resultsObject = new GameObject("Results Data");
            ResultsData resultsData = resultsObject.AddComponent<ResultsData>();
            DontDestroyOnLoad(resultsObject);

            // Caluclates and sets the game score.
            // CalculateAndSetGameScore(); - Not Needed

            // Sets the time and score.
            resultsData.gameTime = gameTime;
            // resultsData.gameScore = gameScore; // Old
            resultsData.gameScore = CalculateGameScore(); // New

            // Saves the stage data.
            for (int i = 0; i < resultsData.stageDatas.Length && i < gameInfo.worldStages.Length; i++)
            {
                resultsData.stageDatas[i] = gameInfo.worldStages[i];
            }

            // Sets the gameplay mode for the results data.
            resultsData.gameMode = gameplayMode;

            // Saves the game.
            SaveGame();

            // Submit progress complete.
            SubmitProgressComplete();

            // Go to the results scene.
            base.ToResults();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // ...
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