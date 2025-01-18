using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace RM_MST
{
    // The system manager (renamed from the LOL class manager).
    public class SystemManager : MonoBehaviour
    {
        // the instance of the LOL manager.
        private static SystemManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;
        
        // The save system for the game.
        public SaveSystem saveSystem;
        
        // The text-to-speech object.
        public TextToSpeech textToSpeech;

        // The maximum progress points for the game.
        const int MAX_PROGRESS = WorldManager.STAGE_COUNT; // Same as the stage count.

        // private constructor so that only one accessibility object exists.
        private SystemManager()
        {
            // ...
        }

        // Awake is called when the script instance is being loaded
        private void Awake()
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

                // This object should not be destroyed.
                DontDestroyOnLoad(this);

                // The LOLSDK version is the one you use.
                // It is automatically being used already, but I wanted to make a note of this...
                // Since you didn't realize you had to do it this way at the time.
                // LOLSDK.DontDestroyOnLoad(this);

                // Gets the instance if it's not set.
                if (saveSystem == null)
                    saveSystem = SaveSystem.Instance;

                // Gets the instance if it's not set.
                if (textToSpeech == null)
                    textToSpeech = TextToSpeech.Instance;
            }
       
        }

        // // Start is called before the first frame update
        // void Start()
        // {
        //     // ...
        // }

        // Returns the instance of the accessibility.
        public static SystemManager Instance
        {
            get
            {
                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    // Makes a new settings object.
                    GameObject go = new GameObject("LOL Manager");

                    // Adds the instance component to the new object.
                    instance = go.AddComponent<SystemManager>();
                }

                // returns the instance.
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

        // Returns 'true' if the LOLSDK is initialized.
        public static bool IsLOLSDKInitialized()
        {
            // return SystemManager.IsLOLSDKInitialized();
            return false;
        }

        // Checks if the LOL manager is instantiated, and if the LOL SDK is initialized.
        public static bool IsInstantiatedAndIsLOLSDKInitialized()
        {
            return Instantiated && IsLOLSDKInitialized();
        }

        // Gets the text from the language file.
        public string GetLanguageText(string key)
        {
            // The game no longer has translation, so this function returns an empty string.
            // // Gets the language definitions.
            // if(defs == null)
            //     defs = SharedState.LanguageDefs;
            // 
            // // Returns the text.
            // if (defs != null)
            //     return defs[key];
            // else
            //     return "";

            return "";
        }

        // Speaks the text.
        public void SpeakText(string key)
        {
            textToSpeech.SpeakText(key);
        }   
        

        // Submits progress for the game.
        // The value overrides the last progress value submitted, and must not go over the max.
        // NOTE: the value will be REPLACED, not added to.
        public void SubmitProgress(int score, int currentProgress)
        {
            //// SDK not initialized.
            //if(!SystemManager.IsLOLSDKInitialized())
            //{
            //    Debug.LogWarning("The SDK is not initialized. No data was submitted.");
            //    return;
            //}

            //// Clamps the current progress.
            //currentProgress = Mathf.Clamp(currentProgress, 0, MAX_PROGRESS);

            //// Submit the progress.
            //LOLSDK.Instance.SubmitProgress(score, currentProgress, MAX_PROGRESS);

            // NOTE: since the LOL content is gone, this function now does nothing.
        }

        // Submits progress to show that the game is complete.
        public void SubmitProgressComplete(int score)
        {
            // Submits the final score.
           SubmitProgress(score, MAX_PROGRESS);
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

