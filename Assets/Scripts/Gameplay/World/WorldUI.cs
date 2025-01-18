using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RM_MST
{
    // The world UI.
    public class WorldUI : GameplayUI
    {
        // The singleton instance.
        private static WorldUI instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        [Header("WorldUI")]
        // The world manager.
        public WorldManager worldManager;

        // The saving text.
        public TMP_Text saveText;

        [Header("WorldUI/Game Mode")]

        // The game mode display.
        public GameObject gameModeDisplay;

        // The game mode text.
        public TMP_Text gameModeText;

        [Header("WorldUI/Prompts")]
        // The stage world UI.
        public StageWorldUI stageWorldUI;

        // The game complete UI.
        public GameObject gameCompleteUI;

        [Header("WorldUI/Options Windows")]

        // The units info button.
        public Button unitsInfoButton;

        // The units info window.
        public UnitsInfoMenu unitsInfoMenu;

        // The save button.
        public Button saveButton;

        // Constructor
        private WorldUI()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected override void Awake()
        {
            base.Awake();

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

            // Sets the world manager.
            if (worldManager == null)
                worldManager = WorldManager.Instance;

            // Sets the tutorial isntance.
            if (tutorialUI == null)
                tutorialUI = TutorialUI.Instance;

            // A variable that checks if saving is possible.
            bool savingPossible;

            // If the save system has been instantiated...
            if (SaveSystem.Instantiated())
            {
                // Gets the save system instance.
                SaveSystem saveSystem = SaveSystem.Instance;

                // Set the save text.
                saveSystem.feedbackText = saveText;
                saveText.text = string.Empty;

                // Checks if saving is possible.
                savingPossible = saveSystem.allowSaveLoad;
            }
            else // Save system doesn't exist.
            {
                // Saving not possible.
                savingPossible = false;
            }

            // If saving is possible, check if the LOL SDK is initialized.
            // If it isn't, then saving isn't possible no matter what, since it's done through the LOL SDK.
            if(savingPossible)
            {
                // If the LOL manager is not instantiated, or if the LOL SDK is not initialized.
                // Since saving is done through the LOL SDK, if that isn't initialized, saving isn't possible.
                if(!SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
                {
                    savingPossible = false;
                }
            }

            // If saving is possible, then keep the save button interactable.
            // If saving is not possible, then disable the save button.
            SetSaveButtonInteractable(savingPossible);

            // Not needed anymore.
            // Open the units info window. This is done so that the units info menu gets initialized.
            // OpenWindow(unitsInfoMenu.gameObject);

            // Updates the game mode text by default (this will likely get overwritten when data is loaded in).
            gameModeText.text = worldManager.GetGameplayModeAsString();

            // Hide the display if it won't be used.
            // If the player is playing in the LOL build, there is only one mode.
            if (GameSettings.Instance.allowPlayerSelectMode)
            {
                gameModeDisplay.gameObject.SetActive(true);
            }
            else
            {
                gameModeDisplay.gameObject.SetActive(false);
            }
        }

        // The late start function.
        protected override void LateStart()
        {
            base.LateStart();

            // Closes all the windows.
            CloseAllWindows();

            // Hide the stage world UI.
            HideStageWorldUI(true);
        }

        // Gets the instance.
        public static WorldUI Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<WorldUI>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("World UI (singleton)");
                        instance = go.AddComponent<WorldUI>();
                    }

                }

                // Return the instance.
                return instance;
            }
        }

        // Returns 'true' if the object has been instanced.
        public static bool Instantiated
        {
            get
            {
                return instanced;
            }
        }

        // WINDOWS //
        // Checks if a window is open.
        public override bool IsWindowOpen()
        {
            // Gets the windows.
            List<GameObject> windows = new List<GameObject>
            {
                gameSettingsUI.gameObject,
                unitsInfoMenu.gameObject,
                stageWorldUI.gameObject,
                gameCompleteUI.gameObject,
            };

            // Only checks the settings window here.
            bool open = false;

            // Goes through all windows.
            foreach(GameObject window in windows)
            {
                // Window is active.
                if(window.activeSelf)
                {
                    open = true;
                    break;
                }
            }

            // Result.
            return open;
        }

        // WINDOWS
        // Closes all the windows.
        public override void CloseAllWindows()
        {
            // Close windows.
            unitsInfoMenu.gameObject.SetActive(false);
            stageWorldUI.gameObject.SetActive(false);
            gameCompleteUI.SetActive(false);

            base.CloseAllWindows();
        }

        // SAVE BUTTON
        // Checks if the save button is interactable.
        public bool IsSaveButtonnteractable()
        {
            return saveButton.interactable;
        }

        // Sets the interactable for the save button.
        public void SetSaveButtonInteractable(bool interactable)
        {
            saveButton.interactable = interactable;
        }

        // Make the save button interactable.
        public void MakeSaveButtonInteractable()
        {
            SetSaveButtonInteractable(true);
        }

        // Make the save button non-interactable
        public void MakeSaveButtonUninteractable()
        {
            SetSaveButtonInteractable(false);
        }

        // UNITS BUTTON
        // Returns the units info button interactable.
        public bool IsUnitsInfoButtonInteractable()
        {
            return unitsInfoButton.interactable;
        }

        // Sets the interactable for the units info button.
        public void SetUnitsInfoButtonInteractable(bool interactable)
        {
            unitsInfoButton.interactable = interactable;
        }

        // Make the units info button interactable.
        public void MakeUnitsInfoButtonInteractable()
        {
            SetUnitsInfoButtonInteractable(true);
        }

        // Make the units info button non-interactable
        public void MakeUnitsInfoButtonUninteractable()
        {
            SetUnitsInfoButtonInteractable(false);
        }

        // Toggles the interactable on the units info button.
        public void ToggleUnitsInfoButtonInteractable()
        {
            SetUnitsInfoButtonInteractable(!IsUnitsInfoButtonInteractable());
        }

        // STAGE UI
        // Returns 'true' if the stage world UI is active.
        public bool IsStageWorldUIActive()
        {
            return stageWorldUI.isActiveAndEnabled;
        }

        // Show the stage world UI.
        public void ShowStageWorldUI(StageWorld stageWorld, int index)
        {
            // Set the stage world.
            stageWorldUI.SetStageWorld(stageWorld, index);
            // stageWorldUI.gameObject.SetActive(true);

            // Don't use the options panel, use the stage UI's own panel.
            OpenWindow(stageWorldUI.gameObject, false);

            // TODO: start the tutorial where applicable
            // if(GameSettings.Instance.UseTutorial)
            // {
            // 
            // }
        }

        // Hide the stage world UI.
        public void HideStageWorldUI(bool clearUI)
        {
            // If the UI should be cleared.
            if(clearUI)
            {
                stageWorldUI.ClearStageWorld();
            }

            // Hids the stage world UI.
            // stageWorldUI.gameObject.SetActive(false);
            CloseAllWindows();
        }

        // Starts the stage in the world UI.
        public void StartStage(StageWorld stage)
        {
            worldManager.ToStage(stage);
        }

        // Rejects the stage in the world UI.
        public void RejectStage()
        {
            HideStageWorldUI(true);
        }
        
        // SAVE //

        // Saves the game.
        public void SaveGame()
        {
            worldManager.SaveGame();
        }

        // SCENES //
        // Opens the game complete window.
        public void OnGameComplete()
        {
            OpenWindow(gameCompleteUI, false);
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
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