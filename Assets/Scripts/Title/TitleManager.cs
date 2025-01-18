

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using util;

namespace RM_MST
{
    // The title manager.
    public class TitleManager : MonoBehaviour
    {
        // The singleton instance.
        private static TitleManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The title screen UI.
        public TitleUI titleUI;

        // The title audio for the game.
        public TitleAudio titleAudio;

        // The scene loaded when start is selected.
        public string startScene = "WorldScene";

        // Gets set to 'true' when late start is called.
        private bool calledLateStart = false;

        // Constructor
        private TitleManager()
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

                // Checks if LOL SDK has been initialized.
                GameSettings settings = GameSettings.Instance;

                // Gets an instance of the LOL manager.
                SystemManager systemManager = SystemManager.Instance;

                // You can save and go back to the menu, so the continue button is usable under that circumstance.
                if (systemManager.saveSystem.HasLoadedData()) // Game has loaded data.
                {
                    // TODO: manage tutorial content.
                }
                else // No loaded data.
                {
                    // TODO: manage tutorial content.
                }

                // TODO: do you need this?
                // // Have the button be turned on no matter what for testing purposes.
                // titleUI.continueButton.interactable = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // ...
        }

        // Called on the first update frame.
        public void LateStart()
        {
            // Late start has been called.
            calledLateStart = true;

            // Makes sure the audio is adjusted to the current settings.
            // For some reason, this doesn't happen properly in the LOL harness when done in Start()...
            // By other scripts, so it's done here to make another correction.
            // Opening the settings window automatically adjusts the audio levels...
            // So doing this in the title script should be fine.
            GameSettings.Instance.AdjustAllAudioLevels();
        }

        // Gets the instance.
        public static TitleManager Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<TitleManager>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Title Manager (singleton)");
                        instance = go.AddComponent<TitleManager>();
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

        // Starts the game (general function for moving to the GameScene).
        public void StartGame()
        {
            // If the loading screen has been instnatiated.
            if (LoadingScreenCanvas.Instantiated)
            {
                // If the loading screen is being used, use it to load the scene.
                if(LoadingScreenCanvas.Instance.IsUsingLoadingScreen())
                {
                    LoadingScreenCanvas.Instance.LoadScene(startScene);
                }
                else // The loading screen is not being used, so load the scene without it.
                {
                    SceneManager.LoadScene(startScene);
                }
            }
            else
            {
                SceneManager.LoadScene(startScene);
            }
        }

        // Starts a new game.
        public void StartNewGame()
        {
            // Clear out the loaded data and last save if the LOLSDK has been initialized.
            SystemManager.Instance.saveSystem.ClearLoadedAndLastSaveData(true); // Deletes the file if it exists.

            // If this is the LOL version, set it to focus mode by default.
            if(SystemManager.IsInstantiatedAndIsLOLSDKInitialized() && GameSettings.Instantiated)
            {
                GameSettings.Instance.gameMode = GameplayManager.gameMode.focus;
            }

            // Start the game.
            StartGame();
        }

        // Continues a saved game.
        public void ContinueGame()
        {
            // New
            // NOTE: a callback is setup onclick to load the save data.
            // Since that might happen after this function is processed...
            // Loaded data does not need to be checked for at this stage.

            //// If the user's tutorial settings should be overwritten, do so.
            //if (overrideTutorial)
            //    GameSettings.Instance.UseTutorial = continueTutorial;

            // Starts the game.
            StartGame();
        }


        // Clears out the save.
        // TODO: This is only for testing, and the button for this should not be shown in the final game.
        public void ClearSave()
        {
            SystemManager.Instance.saveSystem.lastSave = null;
            SystemManager.Instance.saveSystem.loadedData = null;

            titleUI.continueButton.interactable = false;
        }

        // Quits the game (will not be used in LOL version).
        public void QuitGame()
        {
            Application.Quit();
        }

        // Update is called once per frame
        void Update()
        {
            // Call late start.
            if(!calledLateStart)
            {
                LateStart();
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