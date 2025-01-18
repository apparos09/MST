
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RM_MST
{
    // The UI for the a stage to be selected from the world.
    public class StageWorldUI : MonoBehaviour
    {
        // The world manager.
        public WorldManager worldManager;

        // The world UI.
        public WorldUI worldUI;

        // The world stage being displayed.
        public StageWorld stageWorld;

        // The index of the challenger.
        public int stageWorldIndex = -1;

        // If 'true', group tutorials are enabled.
        // Since they are no longer necessary, this variable stops them from showing.
        private bool useUnitGroupTutorials = false;

        // The number of frames that the game waits before refreshing the units table.
        // This is to fix an issue where the units table isn't refreshed properly.
        private int unitsTableRefreshFrames = 0;

        // Gets set to 'true' when late start has been called.
        protected bool calledLateStart = false;

        // Gets set to 'true' when the late on opened function for the stage UI is called.
        protected bool calledLateOnOpened = false;

        [Header("Units Table")]
        // The units table that's used to show the converison information.
        public UnitsTable unitsTable;

        // The name text for the units table's group name.
        public TMP_Text unitsTableGroupNameText;

        [Header("Images")]

        // The renderer of the stage art.
        public Image stageRenderer;

        // The stage backgrounds that are used for the stage art.
        public Sprite stageThumbnail01;
        public Sprite stageThumbnail02;
        public Sprite stageThumbnail03;

        [Header("Text")]

        // The stage name text.
        public TMP_Text stageNameText;

        // The stage description text.
        public TMP_Text stageDescText;

        // TODO: add stage difficulty text?

        [Header("Buttons")]

        // The button for going to the previous group.
        public Button previousGroupButton;

        // The button for going to the next group.
        public Button nextGroupButton;

        // The stage accept button.
        public Button startButton;

        // The stage reject button.
        public Button rejectButton;


        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Clear the entries to start.
            unitsTable.ClearEntries();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Sets the world manager.
            if (worldManager == null)
                worldManager = WorldManager.Instance;

            // Sets the world UI.
            if (worldUI == null)
                worldUI = WorldUI.Instance;
        }

        // Late Start is called on the first frame update.
        protected void LateStart()
        {
            calledLateStart = true;

            // Nothing needed here since other functions handle it.
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            // May not be needed.
            // EnableButtons();

            // Grabs the instance.
            if(worldManager == null)
                worldManager = WorldManager.Instance;

            // Sets the world UI.
            if (worldUI == null)
                worldUI = WorldUI.Instance;

            // The stage world UI has been opened.
            OnStageWorldUIOpened();

            // TODO: play the button SFX for the stage UI?
            // // Plays the button SFX upon being activated.s
            // if (manager.worldAudio != null)
            //     manager.worldAudio.PlayButtonSfx();

        }

        // Not needed since this is called by another function.
        // // This function is called when the behaviour becomes disabled or inactive.
        // private void OnDisable()
        // {
        //     // This technically gets called twice due to another function.
        //     OnStageWorldUIClosed();
        // }

        // TODO: add clear challenger option?
        // Sets the challenger.
        public void SetStageWorld(StageWorld newStageWorld, int index)
        {
            // Sets the stage and the index.
            stageWorld = newStageWorld;
            stageWorldIndex = index;

            // Updates the UI.
            UpdateUI();
        }

        // Clears the stage world information.
        public void ClearStageWorld()
        {
            stageWorld = null;
            stageWorldIndex = -1;

            // Updates the UI.
            UpdateUI();
        }

        // UI
        // Updates the UI.
        public void UpdateUI()
        {
            UpdateStageSprite();
            UpdateStageNameText();
            UpdateStageDescriptionText();

            // Clears the units table.
            ClearUnitsTable();
        }

        // Updates the Challenger Sprite
        public void UpdateStageSprite()
        {
            if(stageWorld != null) // Set sprite.
            {
                // The new sprite to be set.
                Sprite newSprite = null;
            
                // Checks the BGM number.
                switch(stageWorld.bgdNumber)
                {
                    default:
                    case 1:
                        newSprite = stageThumbnail01;
                        break;

                    case 2:
                        newSprite = stageThumbnail02;
                        break;

                    case 3:
                        newSprite = stageThumbnail03;
                        break;
                }

                // Sets the background for the sprite renderer.
                stageRenderer.sprite = newSprite;
            }
            else // Clear sprite.
            {
                stageRenderer.sprite = null;
            }
            
        }

        // Updates the name text.
        public void UpdateStageNameText()
        {
            // Checks if the stage name is set.
            if (stageWorld != null)
                stageNameText.text = stageWorld.stageName;
            else
                stageNameText.text = "-";
        }

        // Updates the stage description text.
        public void UpdateStageDescriptionText()
        {
            // Checks if the stage description is set.
            if (stageWorld != null)
                stageDescText.text = stageWorld.stageDesc;
            else
                stageDescText.text = "-";
        }

        // Called when the stage world UI has been opened.
        public void OnStageWorldUIOpened()
        {
            // If the stage world is not equal to none.
            if (stageWorld != null)
            {
                // If the units info table's units info is not set, set it.
                if (unitsTable.unitsInfo == null)
                    unitsTable.unitsInfo = UnitsInfo.Instance;

                // Clear the units table to start off.
                ClearUnitsTable();

                // Load the unit group conversion information.
                SetUnitsTableGroupByIndex(0);

                // Tutorial running and reading the UI is now done in a late opened update.

                // Call the late opened function.
                calledLateOnOpened = false;
            }
        }

        // Called on the first frame after the stage world UI has been opened.
        public void LateOnStageWorldUIOpened()
        {
            // The late opened has been called.
            calledLateOnOpened = true;

            // Load the unit group conversion information.
            // This is done again to make sure that it properly updated.
            SetUnitsTableGroupByIndex(0);

            // Wait 2 update loops to refresh the units table.
            // Since this is checked after late start, it will be decreased by 1 by default.
            // As it turns out, this fix isn't needed since the problem was being caused by another script.
            // It's being kept in, but it's not needed anymore.
            unitsTableRefreshFrames = 1;

            // Tries to run the tutorial.
            RunTutorial();

            // If TTS is enabled, and the LOL SDK is active.
            if (GameSettings.Instance.UseTextToSpeech && SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            {
                // If the tutorial is not running, and TTS is set, read the description.
                if (!worldManager.IsTutorialRunning() && SystemManager.Instance.textToSpeech != null)
                {
                    // Speak the text.
                    SystemManager.Instance.textToSpeech.SpeakText(stageWorld.stageDescKey);
                }
            }
        }

        // Taken out because it is no longer needed, and because it could cause issues.
        // // Called when the stage world UI has been closed.
        // public void OnStageWorldUIClosed()
        // {
        //     ClearStageWorld();
        // }

        // CONVERSIONS
        // Gets the stage world unit groups count.
        public int GetStageWorldUnitGroupsCount()
        {
            // If the stage world is set, use it. If there is no stage world, return -1;
            if(stageWorld != null)
            {
                return stageWorld.unitGroups.Count;
            }
            else
            {
                return -1;
            }
        }

        // Checks if the stage world contains the provided group.
        public bool StageWorldContainsGroup(UnitsInfo.unitGroups group)
        {
            // If there is no group, return false by default.
            if(stageWorld != null)
            {
                return stageWorld.unitGroups.Contains(group);
            }
            else
            {
                return false;
            }
        }

        // Generates the stage world unit groups list.
        public List<UnitsInfo.unitGroups> GenerateStageWorldUnitGroupsList()
        {
            // The group list.
            List<UnitsInfo.unitGroups> groupsList;
            
            // The stage world is not equal to null.
            if(stageWorld != null)
            {
                groupsList = new List<UnitsInfo.unitGroups>(stageWorld.unitGroups);
            }
            else // Equal to null, so make empty list.
            {
                groupsList = new List<UnitsInfo.unitGroups>();
            }
            
            return groupsList;
        }

        // Sets the units table group.
        public void SetUnitsTableGroup(UnitsInfo.unitGroups group)
        {
            // Set the group.
            unitsTable.SetGroup(group);

            // Gets the group name for the units table.
            unitsTableGroupNameText.text = unitsTable.unitsInfo.GetUnitsGroupName(group);

            // Refresh the buttons.
            RefreshUnitsTableButtons();
        }

        // Gets the current index of the units table.
        public int GetCurrentUnitsTableGroupIndex()
        {
            // The group list and the index.
            List<UnitsInfo.unitGroups> groupList = GenerateStageWorldUnitGroupsList();
            int index;

            // There are entries.
            if(groupList.Count > 0)
            {
                // Gets the index.
                index = groupList.IndexOf(unitsTable.GetGroup());
            }
            else // No entries.
            {
                index = -1;
            }

            return index;
        }

        // Sets the untis table's group by an index value of the stage world's unit groups.
        public void SetUnitsTableGroupByIndex(int index)
        {
            // If the stage world does not exist, clear the entries.
            if(stageWorld == null)
            {
                unitsTable.ClearEntries();
                return;
            }

            // There are no unit groups, so do nothing.
            if(stageWorld.unitGroups.Count <= 0)
            {
                return;
            }

            // Gets the group count.
            int groupCount = GetStageWorldUnitGroupsCount();

            // Clamp the index.
            index = Mathf.Clamp(index, 0, groupCount);

            // Gets the units group.
            UnitsInfo.unitGroups newGroup = stageWorld.unitGroups[index];
            
            // Sets the units group.
            SetUnitsTableGroup(newGroup);
        }

        // Goes to the previous stage world units group.
        public void PreviousUnitsTableGroup()
        {
            // Get the index and subtrat 1.
            int index = GetCurrentUnitsTableGroupIndex() - 1;

            // Loop to the end if out of bounds.
            if (index < 0)
                index = GetStageWorldUnitGroupsCount() - 1;

            // Set the index.
            SetUnitsTableGroupByIndex(index);
        }

        // Goes to the next stage world units group.
        public void NextUnitsTableGroup()
        {
            // Get the index and add 1.
            int index = GetCurrentUnitsTableGroupIndex() + 1;

            // Loop to the start if out of bounds.
            if (index >= GetStageWorldUnitGroupsCount())
                index = 0;

            // Set the index.
            SetUnitsTableGroupByIndex(index);
        }

        // Refreshes the units table buttons.
        public void RefreshUnitsTableButtons()
        {
            // If there are multiple groups, enable the buttons.
            // If there's one group or less, disable the buttons.
            if(stageWorld != null)
            {
                // Only turn them on if there's more than one group.
                if (stageWorld.unitGroups.Count > 1)
                {
                    previousGroupButton.interactable = true;
                    nextGroupButton.interactable = true;
                }
                else
                {
                    previousGroupButton.interactable = false;
                    nextGroupButton.interactable = false;
                }
            }
            else // No stage world, so turn them both off.
            {
                previousGroupButton.interactable = false;
                nextGroupButton.interactable = false;
            }
            
        }

        // Clears the units table and refreshes the buttons.
        public void ClearUnitsTable()
        {
            unitsTable.ClearGroupAndEntries();
            RefreshUnitsTableButtons();
        }

        // Start/Reject
        // Accepts the challenge.
        public void StartStage()
        {
            // If the world is set.
            if(stageWorld != null)
                worldUI.StartStage(stageWorld);
        }

        // Declines the challenge.
        public void RejectStage()
        {
            worldUI.RejectStage();
        }

        // Enables the buttons for the challenger UI.
        public void EnableButtons()
        {
            previousGroupButton.interactable = true;
            nextGroupButton.interactable = true;

            startButton.interactable = true;
            rejectButton.interactable = true;
        }

        // Disables the buttons for the challenger UI.
        public void DisableButtons()
        {
            previousGroupButton.interactable = false;
            nextGroupButton.interactable = false;

            startButton.interactable = false;
            rejectButton.interactable = false;
        }

        // Tutorials
        // Tries to run a tutorial for the stage world.
        public void RunTutorial()
        {
            // If the tutorial is being used.
            if (worldManager.IsUsingTutorial() && worldManager.tutorials != null)
            {
                // Gets the tutorials object.
                Tutorials tutorials = worldManager.tutorials;

                // If there are multiple unit groups, try loading the mix stage tutorial.
                if (stageWorld.unitGroups.Count > 1)
                {
                    // Loads the mix stage tutorial if it hasn't been used.
                    if (!tutorials.clearedMixStageTutorial)
                    {
                        tutorials.LoadMixStageTutorial();
                    }
                }
                else if (stageWorld.unitGroups.Count == 1)
                {
                    // Gets set to 'true' if a tutorial was skipped.
                    bool tutorialSkipped = false;

                    // Gets the group.
                    UnitsInfo.unitGroups group = stageWorld.unitGroups[0];

                    // Checks the group to see what intro should be loaded.
                    // If group unit tutorials are not being used, they are automatically cleared.
                    switch (group)
                    {
                        case UnitsInfo.unitGroups.lengthImperial:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedLengthImperialTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadLengthImperialTutorial();
                            }
                            else
                            {
                                tutorials.clearedLengthImperialTutorial = true;
                                tutorialSkipped = true;
                            }

                            break;

                        case UnitsInfo.unitGroups.weightImperial:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedWeightImperialTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadWeightImperialTutorial();
                            }
                            else
                            {
                                tutorials.clearedWeightImperialTutorial = true;
                                tutorialSkipped = true;
                            }

                            break;

                        case UnitsInfo.unitGroups.time:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedTimeTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadTimeTutorial();
                            }
                            else
                            {
                                tutorials.clearedTimeTutorial = true;
                                tutorialSkipped = true;
                            }

                            break;

                        case UnitsInfo.unitGroups.lengthMetric:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedLengthMetricTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadLengthMetricTutorial();
                            }
                            else
                            {
                                tutorials.clearedLengthMetricTutorial = true;
                                tutorialSkipped = true;
                            }

                            break;

                        case UnitsInfo.unitGroups.weightMetric:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedWeightMetricTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadWeightMetricTutorial();
                            }
                            else
                            {
                                tutorials.clearedWeightMetricTutorial = true;
                                tutorialSkipped = true;
                            }
                                
                            break;

                        case UnitsInfo.unitGroups.capacity:

                            // Tutorial not cleared, load it.
                            if (!tutorials.clearedCapacityTutorial && useUnitGroupTutorials)
                            {
                                tutorials.LoadCapacityTutorial();
                            }
                            else
                            {
                                tutorials.clearedCapacityTutorial = true;
                                tutorialSkipped = true;
                            }

                            break;
                    }

                    // If a tutorial was skipped.
                    if(tutorialSkipped)
                    {
                        // If the world UI's unit info button is disabled, make it interactable.
                        if(!worldManager.worldUI.unitsInfoButton.interactable)
                        {
                            worldManager.worldUI.unitsInfoButton.interactable = true;
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Calls LateStart if it hasn't been called yet.
            if(!calledLateStart)
            {
                LateStart();
            }

            // If the late function has not been called yet, call it.
            if(!calledLateOnOpened)
            {
                LateOnStageWorldUIOpened();
            }
        }

        private void LateUpdate()
        {
            // If the game is waiting for a set amount of frames to update the units table.
            if (unitsTableRefreshFrames > 0)
            {
                // Reduce the frame counter.
                unitsTableRefreshFrames--;

                // Bounds check.
                if (unitsTableRefreshFrames < 0)
                    unitsTableRefreshFrames = 0;

                // Refresh the units table.
                if (unitsTableRefreshFrames <= 0)
                {
                    SetUnitsTableGroupByIndex(0);
                }
            }
        }
    }
}