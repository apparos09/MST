
using RM_MST;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using util;

namespace RM_MST
{
    // The save system for the game.
    [System.Serializable]
    public class MST_GameData
    {
        // Shows if the game data is valid.
        public bool valid = false;

        // Gets set to 'true' if the game was completed.
        public bool complete = false;

        // To avoid problems, the tutorial parameter cannot be changed for a saved game.
        public bool useTutorial = true;

        // The game time
        public float gameTime = 0;

        // The player's overall score.
        public float gameScore = 0;

        // The stage data.
        public StageData[] stageDatas = new StageData[WorldManager.STAGE_COUNT];

        // Tutorial Clears
        public Tutorials.TutorialsData tutorialData;

        // Game Mode
        public GameplayManager.gameMode gameMode;
    }

    // Used to save the game.
    public class SaveSystem : MonoBehaviour
    {
        // The instance of Save System
        private static SaveSystem instance;

        // Becomes 'true' when the save system is initialized.
        private bool initialized = false;

        // If set to 'true', the game allows the player to save.
        public bool allowSaveLoad = true; // False by default.

        // The game data.
        // The last game save. This is only for testing purposes.
        public MST_GameData lastSave;

        // The data that was loaded.
        public MST_GameData loadedData;

        // The file reader.
        public FileReaderBytes fileReader = null;

        // The world manager for the game, which has the save information.
        public WorldManager worldManager;

        // LOL - AutoSave //
        // Added from the ExampleCookingGame. Used for feedback from autosaves.
        WaitForSeconds feedbackTimer = new WaitForSeconds(2);
        Coroutine feedbackMethod;
        public TMP_Text feedbackText;

        // The default saving data.
        private string FEEDBACK_STRING_DEFAULT = "Saving Data";

        // The string shown when having feedback.
        private string feedbackString = "Saving Data";

        // The string key for the feedback.
        private const string FEEDBACK_STRING_KEY = "sve_msg_savingGame";

        // Becomes 'true' when a save is in progress.
        private bool saveInProgress = false;

        // Private constructor so that only one save system object exists.
        private SaveSystem()
        {
            // ...
        }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // This is the instance.
            if (instance == null)
            {
                instance = this;

                // // Don't destroy the language manager on load.
                // DontDestroyOnLoad(gameObject);
            }

            // Initializes the save system.
            if (!initialized)
                Initialize();
        }

        // Start is called before the first frame update
        void Start()
        {
            //// Sets the save result to the instance.
            //LOLSDK.Instance.SaveResultReceived += OnSaveResult;

            //// Gets the language definition.
            //JSONNode defs = SharedState.LanguageDefs;

            //// Sets the save complete text.
            //if (defs != null)
            //    feedbackString = defs[FEEDBACK_STRING_KEY];
        }

        // Gets the instance.
        public static SaveSystem Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<SaveSystem>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("SaveSystem (singleton)");
                        instance = go.AddComponent<SaveSystem>();
                    }

                }

                // Return the instance.
                return instance;
            }
        }

        // Returns 'true' if the save system is 
        public static bool Instantiated()
        {
            return instance != null;
        }

        // Checks if the save system has been initialized.
        public bool Initialized
        {
            get { return initialized; }
        }

        // Set save and load operations.
        public void Initialize()
        {
            // The result.
            bool result;

            // Checks if the file reader exists.
            if (fileReader == null)
            {
                // Tries to grab component.
                if (!TryGetComponent<FileReaderBytes>(out fileReader))
                {
                    // Add component.
                    fileReader = gameObject.AddComponent<FileReaderBytes>();
                }
            }

            fileReader.filePath = "Assets\\Resources\\Data\\";
            fileReader.fileName = "save.dat";

            // Checks if the file exists.
            result = fileReader.FileExists();

            // If the file exists, the save system checks if it's empty.
            if (result)
            {
                // If the file is empty, delete the file.
                bool empty = fileReader.IsFileEmpty();

                // If empty, delete the file.
                if (empty)
                    fileReader.DeleteFile();

            }

            // Save system has been initialized.
            initialized = true;
        }

        // Checks if the world manager has been set.
        private bool IsWorldManagerSet()
        {
            // Tries to set the world manager.
            if (worldManager == null)
                worldManager = WorldManager.Instance;

            // Game manager does not exist.
            if (worldManager == null)
            {
                Debug.LogWarning("The World Manager couldn't be found.");
                return false;
            }

            return true;
        }

        // Sets the last bit of saved data to the loaded data object.
        public void SetLastSaveAsLoadedData()
        {
            loadedData = lastSave;
        }

        // Clears out the last save and the loaded data object.
        public void ClearLoadedAndLastSaveData(bool deleteFile)
        {
            lastSave = null;
            loadedData = null;


            // If the file should be deleted.
            if (deleteFile)
            {
                // If the file exists, delete it.
                if (fileReader.FileExists())
                {
                    // Checks if a meta file exists so that that can be deleted too.
                    string meta = fileReader.GetFileWithPath() + ".meta";

                    // Delete the main file.
                    fileReader.DeleteFile();

                    // If the meta file exists, delete it.
                    if (File.Exists(meta))
                        File.Delete(meta);
                }
            }
        }

        // Converts an object to bytes (requires seralizable object) and returns it.
        static public byte[] SerializeObject(object data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, data); // Serialize the data for them emory stream.
            return ms.ToArray();
        }

        // Deserialize the provided object, converting it to an object and returning it.
        static public object DeserializeObject(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            ms.Write(data, 0, data.Length); // Write data.
            ms.Seek(0, 0); // Return to start of data.

            return bf.Deserialize(ms); // return content
        }

        // Checks if a save is in progress.
        public bool IsSaveInProgress()
        {
            return saveInProgress;
        }

        // Saves data.
        public bool SaveGame(bool async)
        {
            // The game manager does not exist if false.
            if (!IsWorldManagerSet())
            {
                Debug.LogWarning("The Game Manager couldn't be found.");
                return false;
            }

            // Determines if saving wa a success.
            bool success = false;

            // Generates the save data.
            MST_GameData savedData = worldManager.GenerateSaveData();

            // Stores the most recent save.
            lastSave = savedData;

            // OLD
            // // If the instance has been initialized.
            // if (LOLSDK.Instance.IsInitialized)
            // {
            //     // Makes sure that the feedback string is set.
            //     if (FEEDBACK_STRING_KEY != string.Empty)
            //     {
            //         // Gets the language definition.
            //         JSONNode defs = SharedState.LanguageDefs;
            // 
            //         // Sets the feedback string if it wasn't already set.
            //         if (feedbackString != defs[FEEDBACK_STRING_KEY])
            //             feedbackString = defs[FEEDBACK_STRING_KEY];
            //     }
            // 
            // 
            //     // Send the save state.
            //     LOLSDK.Instance.SaveState(savedData);
            //     success = true;
            // }
            // else // Not initialized.
            // {
            //     Debug.LogError("The SDK has not been initialized. Improper save made.");
            //     success = false;
            // }


            // NEW
            // If the instance has been initialized.
            if (SystemManager.Instantiated)
            {
                feedbackString = "Saving Data";
                success = true;
            }
            else // Not initialized.
            {
                Debug.LogError("Save failed.");
                success = false;
            }

            // Checks if save/load should be allowed.
            if (allowSaveLoad)
            {
                // Save to a file.
                if (async) // Asynchronous save.
                {
                    success = SaveToFileAsync(savedData);
                }
                else // Synchronous save.
                {
                    success = SaveToFile(savedData);
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        // Save the information to a file.
        private bool SaveToFile(MST_GameData data)
        {
            // Gets the file.
            string file = fileReader.GetFileWithPath();

            // Will generate the file if it doesn't exist.
            // // Checks that the file exists.
            // if (!fileReader.FileExists())
            //     return false;

            // Seralize the data.
            byte[] dataArr = SerializeObject(data);

            // Data did not serialize properly.
            if (dataArr.Length == 0)
                return false;

            // Save started.
            saveInProgress = true;

            // Write to the file.
            File.WriteAllBytes(file, dataArr);

            // Save finished.
            saveInProgress = false;

            // Data written successfully.
            return true;
        }

        // Saves the game asynchronously.
        public bool SaveToFileAsync(MST_GameData data)
        {
            // Checks if the feedback method exists.
            if (feedbackMethod == null)
            {
                feedbackMethod = StartCoroutine(SaveToFileAsyncCourtine(data));
                return true;
            }
            else
            {
                Debug.LogWarning("Save already in progress.");
                return false;
            }
        }

        // Refreshes the feedback string.
        public void RefreshFeedbackString()
        {
            //// The language manager.
            //LanguageManager lm = LanguageManager.Instance;

            //// If the language should be translated.
            //if (lm.TranslateAndLanguageSet())
            //{
            //    feedbackString = LanguageManager.Instance.GetLanguageText(FEEDBACK_STRING_KEY);
            //}
            //else
            //{
            //    feedbackString = "Saving Game...";
            //}

            feedbackString = FEEDBACK_STRING_DEFAULT;
        }

        // Refreshes the feedback text.
        public void RefreshFeedbackText()
        {
            // If the text exists.
            if (feedbackText != null)
            {
                // Checks if a save is in progress.
                if (saveInProgress)
                    feedbackText.text = feedbackString;
                else
                    feedbackText.text = string.Empty;
            }
        }

        // Save the information to a file asynchronously (cannot return anything).
        private IEnumerator SaveToFileAsyncCourtine(MST_GameData data)
        {
            // Save started.
            saveInProgress = true;

            // Show saving text.
            RefreshFeedbackText();

            // Gets the file.
            string file = fileReader.GetFileWithPath();

            // Seralize the data.
            byte[] dataArr = SerializeObject(data);

            // Yield return before file wrting begins.
            yield return null;

            // Show saving text in case scene has changed.
            RefreshFeedbackText();

            // Opens the file in the file stream.
            FileStream fs = File.OpenWrite(file);

            // NOTE: this is pretty scuffed, but because of the way it's set up I don't really have a better option.
            // File.WriteAsync would probably be better.

            // Ver. 1
            // // The number of bytes to write, and the offset.
            // int count = 32;
            // int offset = 0;

            // // While there's still bytes to write.
            // while(offset < dataArr.Length)
            // {
            //     // If the count exceeds the amount of remaining bytes, adjust it.
            //     if (offset + count > dataArr.Length)
            //         count = dataArr.Length - offset;
            // 
            //     fs.Write(dataArr, offset, count);
            // 
            //     // Increase the offset.
            //     offset += count;
            // 
            //     // Run other operations.
            //     // yield return null;
            // 
            //     // Pause the courtine for 2 seconds.
            //     yield return feedbackTimer;
            // }

            // Ver. 2 - write the data and suspend for the amount of time set to feedbackTimer.
            fs.Write(dataArr, 0, dataArr.Length);
            yield return feedbackTimer;

            // Show saving text in case scene has changed.
            RefreshFeedbackText();

            // Close the file stream.
            fs.Close();

            // Save finished.
            saveInProgress = false;

            // Hide feedback text now that the save is done.
            RefreshFeedbackText();

            // Save is complete, so set the method to null.
            if (feedbackMethod != null)
                feedbackMethod = null;
        }

        // Checks if the game has loaded data.
        public bool HasLoadedData()
        {
            // Used to see if the data is available.
            bool result;

            // Checks to see if the data exists.
            if (loadedData != null) // Exists.
            {
                // Checks to see if the data is valid.
                result = loadedData.valid;
            }
            else // No data.
            {
                // Not readable.
                result = false;
            }

            // Returns the result.
            return result;
        }

        // Removes the loaded data.
        public void ClearLoadedData()
        {
            loadedData = null;
        }

        // The gameplay manager now checks if there is loadedData. If so, then it will load in the data when the game starts.
        // Loads a saved game. This returns 'false' if there was no data.
        public bool LoadGame()
        {
            // Loading a save is not allowed.
            if (!allowSaveLoad)
                return false;

            // The result of loading the save data.
            bool success;

            // The file doesn't exist.
            if (!fileReader.FileExists())
            {
                return false;
            }

            // Loads the file.
            loadedData = LoadFromFile();

            // The data has been loaded successfully.
            success = loadedData != null;

            return success;
        }

        // Loads information from a file.
        private MST_GameData LoadFromFile()
        {
            // Gets the file.
            string file = fileReader.GetFileWithPath();

            // Checks that the file exists.
            if (!fileReader.FileExists())
                return null;

            // Read from the file.
            byte[] dataArr = File.ReadAllBytes(file);

            // Data did not serialize properly.
            if (dataArr.Length == 0)
                return null;

            // Deseralize the data.
            object data = DeserializeObject(dataArr);

            // Convert to loaded data.
            MST_GameData loadData = (MST_GameData)(data);

            // Return loaded data.
            return loadData;
        }


    }
}