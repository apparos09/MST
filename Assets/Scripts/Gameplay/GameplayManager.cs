using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using util;

namespace RM_MST
{
    // The gameplay manager.
    public class GameplayManager : MonoBehaviour
    {
        /*
         * Game Mode:
         *** focus: meteors only move when hit with a failed unit conversion.
         *** rush: meteors fall gradually towards the Earth as the stage goes on.
         */
        public enum gameMode { focus, rush };

        // The game UI.
        public GameplayUI gameUI;

        // The game speed.
        private float gameTimeScale = 1.0F;

        // The timer for the game.
        public float gameTime = 0;

        // The game score. This isn't saved since it gets calculated when needed.
        // public float gameScore = 0;

        // Pauses the timer if true.
        private bool gamePaused = false;

        // The number of frames the game waits when the app is unpaused.
        // This is done to ignore the delta time that has been built up from the app being paused.
        private int appUnpausedWaitFrames = 0;

        // The mouse touch object.
        public MouseTouchInput mouseTouch;

        // The tutorials object.
        public Tutorials tutorials;

        // The gameplay info object.
        public GameplayInfo gameInfo;

        // The units info.
        public UnitsInfo unitsInfo;

        // The gameplay mode for the game.
        public gameMode gameplayMode = gameMode.focus;

        // Set to 'true' when the late start function has been called.
        private bool calledLateStart = false;

        [Header("Scenes")]

        // The title scene.
        public string titleScene = "TitleScene";

        // The results scene.
        public string resultsScene = "ResultsScene";


        // NOTE: GameInfo and Tutorial aren't listed here because they're singletons.
        // Having them be in the scene from the start caused issues, so I'm not going to have them.

        // Awake is called when the script is being loaded
        protected virtual void Awake()
        {
            // ...
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // If the game UI isn't set, try to find it.
            if(gameUI == null)
                gameUI = FindObjectOfType<GameplayUI>();

            // For some reason, when coming back from the match scene this is listed as 'missing'.

            // Creates/gets the game info instance.
            if(gameInfo == null)
                gameInfo = GameplayInfo.Instance;

            // Creates or gets the units info.
            if(unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // // Creates/gets the tutorial instance if it will be used.
            // if(IsUsingTutorial())
            // {
            //     Tutorial tutorial = Tutorial.Instance;
            // }

            // Sets the tutorials object.
            if (tutorials == null)
                tutorials = Tutorials.Instance;


            // If the gameUI is set, check for the tutorial text box.
            if (gameUI != null)
            {
                // If the tutorial text box is set...
                if (gameUI.tutorialUI.textBox != null)
                {
                    // Adds the callbakcs from the tutorial text box.
                    // I don't think I need to remove them.
                    gameUI.AddTutorialTextBoxCallbacks(this);
                }
            }        
        }
        
        // LateStart is called on the first update frame of this object.
        protected virtual void LateStart()
        {
            calledLateStart = true;
        }

        // Sent to all game objects when the player pauses.
        private void OnApplicationPause(bool pause)
        {
            // If the app has been paused, set the number of wait frames.
            if (pause)
            {
                // In the event that some updates are still called when the app is unpaused...
                // The game waits 2 frames instead of 1.
                appUnpausedWaitFrames = 2;
            }
        }

        // Gameplay Mode
        // // Gets the gameplay mode.
        // public gameMode GetGameplayMode()
        // {
        //     return gameplayMode;
        // }

        // Gets the gameplay mode as a string.
        public static string GetGameplayModeAsString(GameplayManager.gameMode mode)
        {
            // The result to be returned.
            string result;

            switch(mode)
            {
                default:
                    result = "";
                    break;

                case gameMode.focus:
                    result = "Focus Mode";
                    break;

                case gameMode.rush:
                    result = "Rush Mode";
                    break;
            }

            // Returns the result.
            return result;
        }

        // Returns the gameplay mode as a string.
        public string GetGameplayModeAsString()
        {
            return GetGameplayModeAsString(gameplayMode);
        }

        // GAME SCORE //
        // Returns the game score.
        public float CalculateGameScore()
        {
            // The game score result.
            float result = 0;

            // Goes through all the stages.
            for (int i = 0; i < gameInfo.worldStages.Length; i++)
            {
                // Add to the score.
                if (gameInfo.worldStages[i] != null)
                    result += gameInfo.worldStages[i].stageScore;
            }

            // Returns the game score.
            return result;
        }

        // Calcualtes the game score as an int and returns it.
        public int CalculateGameScoreAsInt()
        {
            // Gets the result as an int. It rounds up to the nearest integer.
            int result = Mathf.CeilToInt(CalculateGameScore());

            // Bounds check.
            if (result < 0)
                result = 0;

            return result;
        }

        // // Updates the game score.
        // public void CalculateAndSetGameScore()
        // {
        //     gameScore = CalculateGameScore();
        // }


        // GAME TIME
        // Returns the provided time (in seconds), formatted.
        public static string GetTimeFormatted(float seconds, bool roundUp = true)
        {
            // Gets the time and rounds it up to the nearest whole number.
            float time = (roundUp) ? Mathf.Ceil(seconds) : seconds;

            // Formats the time.
            string formatted = StringFormatter.FormatTime(time, false, true, false);

            // Returns the formatted time.
            return formatted;
        }

        // Gets the game time scale.
        public float GetGameTimeScale()
        {
            return gameTimeScale;
        }

        // Sets the game time scale.
        public void SetGameTimeScale(float value)
        {
            gameTimeScale = value;
            Time.timeScale = gameTimeScale;
        }

        // Resets the time scale.
        public void ResetGameTimeScale()
        {
            gameTimeScale = 1.0F;
            Time.timeScale = gameTimeScale;
        }

        // Returns 'true' if the game time scale is 1.0.
        public bool IsGameTimeScaleNormal()
        {
            return gameTimeScale == 1.0F;
        }

        // Returns 'true' if the game is paused.
        public bool IsGamePaused()
        {
            return gamePaused;
        }

        // Returns 'true' if the game is paused or if the application is waiting.
        public bool IsGamePausedOrApplicationWaiting()
        {
            return IsGamePaused() || IsApplicationWaiting();
        }

        // Sets if the game should be paused.
        public virtual void SetGamePaused(bool paused)
        {
            gamePaused = paused;

            // If the game is paused.
            if(gamePaused)
            {
                // NOTE: this does not change the time scale.
                Time.timeScale = 0.0F;
            }
            else // If the game is not paused.
            {
                // If the tutorial is not running, set the time scale to 1.0F.
                if (!IsTutorialRunning())
                {
                    // Gets the game time scale.
                    float gts = GetGameTimeScale();

                    // If the time scale is 0, reset the time scale.
                    if(gts == 0)
                    {
                        ResetGameTimeScale();
                        gts = GetGameTimeScale();
                    }
                    
                    // Set the game time scale.
                    Time.timeScale = gts;


                }
            }
        }

        // Pauses the game.
        public virtual void PauseGame()
        {
            SetGamePaused(true);
        }

        // Unpauses the game.
        public virtual void UnpauseGame()
        {
            SetGamePaused(false);
        }

        // Toggles if the game is paused or not.
        public virtual void TogglePausedGame()
        {
            SetGamePaused(!gamePaused);
        }

        // Returns 'true' if the application is waiting (happens when the app is paused).
        public bool IsApplicationWaiting()
        {
            return appUnpausedWaitFrames > 0;
        }

        // TUTORIAL //

        // Checks if the game is using the tutorial.
        public bool IsUsingTutorial()
        {
            // The result.
            bool result;
            
            // If the game settings is instantiated.
            if(GameSettings.Instantiated)
            {
                result = GameSettings.Instance.UseTutorial;
            }
            else
            {
                // Not instantiated, so return false by default.
                result = false;
            }

            
            return result;
        }

        // Set if the tutorial will be used.
        public void SetUsingTutorial(bool value)
        {
            GameSettings.Instance.UseTutorial = value;
        }

        // Returns 'true' if the tutorial is available to be activated.
        public bool IsTutorialAvailable()
        {
            return gameUI.IsTutorialAvailable();
        }

        // Checks if the text box is open.
        public bool IsTutorialTextBoxOpen()
        {
            return gameUI.IsTutorialTextBoxOpen();
        }

        // Checks if the tutorial is running.
        public bool IsTutorialRunning()
        {
            // Check this function.
            return gameUI.IsTutorialRunning();
            
        }

        // Starts a tutorial using the provided pages.
        public virtual void StartTutorial(List<Page> pages)
        {
            gameUI.StartTutorial(pages);
        }

        // Called when a tutorial is started.
        public virtual void OnTutorialStart()
        {
            gameUI.OnTutorialStart();
        }

        // Called when a tutorial is ended.
        public virtual void OnTutorialEnd()
        {
            gameUI.OnTutorialEnd();
        }

        // SAVING
        // If saving and loading is enabled.
        public bool IsSavingLoadingEnabled()
        {
            // It's disabled if the save system is not instantiated.
            if (SaveSystem.Instantiated())
            {
                SaveSystem saveSystem = SaveSystem.Instance;
                return saveSystem.allowSaveLoad;
            }
            else
            {
                return false;
            }

        }

        // SCENES //
        // Called when the game is completed.
        public virtual void OnGameComplete()
        {
            ToResults();
        }

        // Called when leaving the scene.
        protected virtual void OnGameEnd()
        {
            // Destroys 'DontDestroyOnLoad' Objects
            // Game Info
            if (GameplayInfo.Instantiated)
            {
                Destroy(GameplayInfo.Instance.gameObject);
                gameInfo = null;
            }

            // Tutorial
            if (Tutorials.Instantiated)
                Destroy(Tutorials.Instance.gameObject);

                           

            // Makes sure the game is not paused, and that the game is at normal speed.
            ResetGameTimeScale();
            UnpauseGame();
        }

        // Loads the provided scene.
        protected void LoadScene(string sceneName)
        {
            // If the loading screen should be used.
            if (LoadingScreenCanvas.IsInstantiatedAndUsingLoadingScreen())
            {
                LoadingScreenCanvas.Instance.LoadScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        // Go to the title scene.
        public virtual void ToTitle()
        {
            // Called when the game is ending (to title or results).
            OnGameEnd();

            // Loads the title scene.
            LoadTitleScene();
        }

        // Load the title scene.
        public virtual void LoadTitleScene()
        {
            LoadScene(titleScene);
        }


        // Go to the results scene.
        public virtual void ToResults()
        {            
            // Called when the game is ending (to title or results).
            OnGameEnd();

            // TODO: add loading screen.
            LoadResultsScene();
        }

        // Loads the results scene.
        public virtual void LoadResultsScene()
        {
            LoadScene(resultsScene);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // If late start has not been called, call it.
            if (!calledLateStart)
            {
                LateStart();
            }

            // The game isn't paused.
            if (!IsGamePaused())
            {
                gameTime += Time.unscaledDeltaTime;
            }
        }

        // LateUpdate is called every frame if a Behaviour is enabled.
        protected virtual void LateUpdate()
        {
            // Now that this update has happened, reset the check for the app being made out of focus.
            if(appUnpausedWaitFrames > 0)
            {
                appUnpausedWaitFrames--;
            }
            // If the app has negative wait frames, cap it at 0.
            else if(appUnpausedWaitFrames < 0)
            {
                appUnpausedWaitFrames = 0;
            }
        }

        
        // This function is called when the MonoBehaviour will be destroyed.
        private void OnDestroy()
        {
            // Resets the game time scale.
            ResetGameTimeScale();

            // Return the time scale to normal.
            Time.timeScale = 1.0F;
        }
    }
}