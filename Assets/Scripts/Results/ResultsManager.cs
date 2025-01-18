
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RM_MST
{
    // The results manager.
    public class ResultsManager : MonoBehaviour
    {
        // The singleton instance.
        private static ResultsManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The results UI.
        public ResultsUI resultsUI;

        // The results audio.
        public ResultsAudio resultsAudio;

        // The title scene.
        public string titleScene = "TitleScene";

        // Constructor
        private ResultsManager()
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
            }

            // Run code for initialization.
            if (!instanced)
            {
                instanced = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Looks for the result data.
            ResultsData data = FindObjectOfType<ResultsData>();

            // Applies the results data.
            if(data != null)
            {
                // Applies the data.
                ApplyResultsData(data);

                // Destroys the object.
                Destroy(data.gameObject);
            }
                
        }

        // Gets the instance.
        public static ResultsManager Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<ResultsManager>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Results Manager (singleton)");
                        instance = go.AddComponent<ResultsManager>();
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

        // Applies the results data.
        public void ApplyResultsData(ResultsData data)
        {
            resultsUI.ApplyResultsData(data);
        }

        // Goes to the title scene.
        public void ToTitleScene()
        {
            // If the loading screen is being used.
            if (LoadingScreenCanvas.Instance.IsUsingLoadingScreen())
            {
                LoadingScreenCanvas.Instance.LoadScene(titleScene);
            }
            else
            {
                SceneManager.LoadScene(titleScene);
            }
        }

        // Call this function to complete the game. This is called by the "finish" button.
        public void CompleteGame()
        {
            // The SDK has been initialized.
            if (SystemManager.IsLOLSDKInitialized())
            {
                // Complete the game (removed).
                // LOLSDK.Instance.CompleteGame();
            }
            else
            {
                // Logs the error (no longer needed since the LOL content is gone).
                // Debug.LogError("SDK NOT INITIALIZED. RETURNING TO THE TITLE SCREEN.");

                // Return to the main menu scene.
                ToTitleScene();
            }

            // Do not return to the title scene if running through the LOL platform.
            // This is because you can't have the game get repeated in the same session.
            // ToTitleScene();
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