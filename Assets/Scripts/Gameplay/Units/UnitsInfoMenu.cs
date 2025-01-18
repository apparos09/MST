using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RM_MST
{
    // The information window for units.
    public class UnitsInfoMenu : MonoBehaviour
    {
        // THe Units Info Entry
        public struct UnitsInfoEntry
        {
            // The units type.
            public UnitsInfo.unitGroups group;

            // The group the units belong to, and its key.
            public string groupName;
            public string groupNameKey;

            // TODO: need description

            // Description and Speak Key
            public string groupDesc;
            public string groupDescKey;

            // Meaning and Speak Key (reuses tutorial text).
            public string groupMeaning;
            public string groupMeaningKey;
        }

        // The units info.
        public UnitsInfo unitsInfo;
        
        // The group name.
        public TMP_Text groupName;

        // The group description.
        public TMP_Text groupDesc;

        // The group/units meaning.
        public TMP_Text groupMeaning;

        // The list of entries.
        public List<UnitsInfoEntry> entries = new List<UnitsInfoEntry>();

        // The entry index.
        public int entryIndex = 0;

        // Loads the entries on enable if true.
        // This is false by default so that it's called from Start() first.
        private bool loadEntriesOnEnable = false;

        // Gets set to 'true' during LateStart to show that TTS is allowed.
        private bool allowTTS = false;

        // Set to true when late start is called.
        private bool calledLateStart = false;

        [Header("Buttons")]

        // Previous Page Button
        public Button prevButton;

        // Next Page Button
        public Button nextButton;

        [Header("Table View")]

        // The units table view.
        public GameObject tableView;

        // The units table.
        public UnitsTable unitsTable;

        [Header("Comparison View")]

        // The comparison view.
        public GameObject comparsionView;

        // The comparison bars for the comparsion view.
        public List<UnitsComparisonBar> comparsionBars = new List<UnitsComparisonBar>();

        [Header("Groups")]
        // Measurement groups
        public bool clearedWeightImperial;
        public bool clearedLengthImperial;
        public bool clearedTime;
        
        public bool clearedLengthMetric;
        public bool clearedWeightMetric;
        public bool clearedCapcity;

        // Start is called before the first frame update
        void Start()
        {
            // Gets the units info instance.
            if (unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // Entries are blank by default, and the entries are loaded in LateStart() first.
            // Loads the entries in Start. This is done to make sure the default text isn't shown.
            // Originally this caused issues due to translated text overriding it, but that has been addressed.
            LoadEntries();

            // Allow TTS to be used.
            allowTTS = true;

            // Load entries on enable.
            loadEntriesOnEnable = true;

            // Shows the table view on start.
            ShowTableView();
        }

        // Called on the first update frame.
        void LateStart()
        {
            calledLateStart = true;

            // Originally entries were loaded here, but that caused the placeholder text to be visible for a frame.
            // So the function calls have been moved.

            // Loads the entries, and sets it to do this on enable.
            // OnEnable is triggered before start.
            // This probably isn't needed anymore, but this is still here just to be sure it loaded properly.
            LoadEntries();
            loadEntriesOnEnable = true;
        }

        // This function is called when the object becomes enabled and active.
        private void OnEnable()
        {
            // Saves the old index and old count.
            int oldIndex = entryIndex;
            int oldCount = entries.Count;

            // Loads entries on enable.
            if(loadEntriesOnEnable)
            {
                LoadEntries();
            }

            // If the count hasn't changed, set the entry to the old index.
            if (oldCount == entries.Count)
            {
                SetEntry(oldIndex);
            }
        }

        // This function is called when the behaviour becomes disabled or inactive
        private void OnDisable()
        {
            // Clears all the entries.
            foreach(UnitsTableEntry entry in unitsTable.entries)
            {
                entry.ClearText();
            }
        }

        // Generates the units info entry.
        public UnitsInfoEntry GenerateUnitsInfoEntry(UnitsInfo.unitGroups group)
        {
            // A new entry.
            UnitsInfoEntry newEntry = new UnitsInfoEntry();

            // Set the group.
            newEntry.group = group;

            // Name
            newEntry.groupName = unitsInfo.GetUnitsGroupName(group);
            newEntry.groupNameKey = UnitsInfo.GetUnitsGroupNameKey(group);

            // Description
            newEntry.groupDesc = unitsInfo.GetUnitsGroupDescription(group);
            newEntry.groupDescKey = UnitsInfo.GetUnitsGroupDescriptionKey(group);

            // Meaning
            newEntry.groupMeaning = unitsInfo.GetUnitsGroupMeaning(group);
            newEntry.groupMeaningKey = UnitsInfo.GetUnitsGroupMeaningKey(group);

            return newEntry;
        }

        // Enables all entry objects in the units table.
        private void EnableAllEntryObjects()
        {
            // Enables all entries.
            foreach(UnitsTableEntry tableEntry in unitsTable.entries)
            {
                if(tableEntry != null)
                    tableEntry.gameObject.SetActive(true);
            }
        }

        // Loads the entries.
        public void LoadEntries()
        {
            // Checks if the tutorial is being used.
            bool usingTutorial = false;

            // Checks if the tutorial is instantiated.
            if (Tutorials.Instantiated)
            {
                // If the game settings exist, reference it to see if the tutorial is active.
                // If it doesn't exist, just set it to false.
                usingTutorial = GameSettings.Instantiated ? GameSettings.Instance.UseTutorial : false;
            }

            // // For testing purposes only.
            // usingTutorial = false;

            // Checks if the tutorial is being used.
            if (usingTutorial) // Only enable encountered rules.
            {
                // Gets the tutorial.
                Tutorials tutorials = Tutorials.Instance;

                clearedLengthImperial = tutorials.clearedLengthImperialTutorial;
                clearedWeightImperial = tutorials.clearedWeightImperialTutorial;
                clearedTime = tutorials.clearedTimeTutorial;
                clearedLengthMetric = tutorials.clearedLengthMetricTutorial;
                clearedWeightMetric = tutorials.clearedWeightMetricTutorial;
                clearedCapcity = tutorials.clearedCapacityTutorial;
            }
            else // Not being used, so enable all.
            {
                clearedLengthImperial = true;
                clearedWeightImperial = true;
                clearedTime = true;
                clearedLengthMetric = true;
                clearedWeightMetric = true;
                clearedCapcity = true;
            }


            // Clears the list to make sure no old entries are left.
            entries.Clear();

            // Creating Entries
            // While the weight (imperial) stage is set to be the first one the player does...
            // It is not the first unit group by number.

            // Length (Imperial)
            if(clearedLengthImperial)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.lengthImperial));
            }

            // Weight (Imperial)
            if (clearedWeightImperial)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.weightImperial));
            }

            // Time
            if (clearedTime)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.time));
            }

            // Length (Metric)
            if(clearedLengthMetric)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.lengthMetric));

            }

            // Weight (Metric)
            if(clearedWeightMetric)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.weightMetric));
            }

            // Capacity
            if(clearedCapcity)
            {
                entries.Add(GenerateUnitsInfoEntry(UnitsInfo.unitGroups.capacity));
            }

            // Refreshes the info menu buttons.
            RefreshButtons();

            // Loads an entry.
            SetEntry(0);
        }

        // Refreshes the buttons for the info menu.
        public void RefreshButtons()
        {
            // Enabling/disabling the previous and next buttons.
            if (entries.Count > 1)
            {
                prevButton.interactable = true;
                nextButton.interactable = true;
            }
            else
            {
                prevButton.interactable = false;
                nextButton.interactable = false;
            }
        }

        // Goes to the previous entry.
        public void PreviousEntry()
        {
            // Index
            int index = entryIndex - 1;

            // Boudns check.
            if (index < 0)
                index = entries.Count - 1;

            // Load entry.
            SetEntry(index);
        }

        // Goe to the next entry.
        public void NextEntry()
        {
            // Index
            int index = entryIndex + 1;

            // Boudns check.
            if (index >= entries.Count)
                index = 0;

            // Load entry.
            SetEntry(index);
        }


        // Sets the entry.
        public void SetEntry(int index)
        {
            // No entry to set if the index is invalid.
            if (index < 0 || index >= entries.Count)
                return;

            // Sets the index.
            entryIndex = index;

            // Gets the entry.
            UnitsInfoEntry entry = entries[index];

            // Set the text.
            groupName.text = entry.groupName;
            groupDesc.text = entry.groupDesc;
            groupMeaning.text = entry.groupMeaning;

            // Updates the table.
            unitsTable.SetGroup(entry.group);

            // Updates the comparison bars.
            UpdateComparsionBars();

            // If the LOL Manager has been instantiated.
            if(GameSettings.Instance.UseTextToSpeech && SystemManager.IsInstantiatedAndIsLOLSDKInitialized() && allowTTS)
            {
                // The LOL Manager
                SystemManager lolManager = SystemManager.Instance;

                // The speak key to be used.
                string speakKey = "";

                // If the table view is active, read the description.
                // If the comparison view is active, read the group.
                if(IsTableViewActive())
                {
                    // Use the group key.
                    speakKey = entry.groupDescKey;
                }
                else if(IsComparisonViewActive())
                {
                    // Use the group name key.
                    speakKey = entry.groupNameKey;
                }
                else
                {
                    // Just use the description key.
                    speakKey = entry.groupDescKey;
                }

                // If there's a speak key, use it.
                if (speakKey != "")
                    lolManager.SpeakText(speakKey);

            }
        }

        // COMPARISON BARS
        // Updates the comparison bars.
        public void UpdateComparsionBars()
        {
            // Goes through each comparsion bar.
            for(int i = 0; i < comparsionBars.Count; i++)
            {
                // Activate the bar.
                comparsionBars[i].gameObject.SetActive(true);

                // Get set to 'true' if the bar is updated.
                bool updated;

                // There is an entry in the units table.
                if(i < unitsTable.entries.Count)
                {
                    // If the entry is active, and the conversion exists.
                    if (unitsTable.entries[i].gameObject.activeSelf && unitsTable.entries[i].conversion != null)
                    {
                        // Loads the comparison info.
                        comparsionBars[i].LoadConversionInfo(unitsTable.entries[i].conversion);
                        updated = true;
                    }
                    else // Entry is off, so clear the bar.
                    {
                        updated = false;
                    }
                }
                else // No entry, so clear the bar.
                {
                    updated = false;
                }

                // If the bar wasn't updated, clear it.
                if(!updated)
                {
                    // Clear the info.
                    comparsionBars[i].ClearConversionInfo();

                    // Disable the bar.
                    comparsionBars[i].gameObject.SetActive(false);
                }
            }
        }

        // VIEWS
        // Checks if the table view is visible.
        public bool IsTableViewActive()
        {
            return tableView.activeSelf;
        }

        // Checks if the comparison view is active.
        public bool IsComparisonViewActive()
        {
            return comparsionView.activeSelf;
        }

        // Shows the provided view.
        public void ShowView(GameObject view)
        {
            // Turns off both views.
            tableView.SetActive(false);
            comparsionView.SetActive(false);

            // Turns on the provided view.
            view.SetActive(true);
        }

        // Shows the table view.
        public void ShowTableView()
        {
            ShowView(tableView);
        }

        // Shows the comparison view.
        public void ShowComparsionView()
        {
            ShowView(comparsionView);
        }

        // Swaps the table view and comparison view.
        public void SwapViews()
        {
            // If the table view is active, activate the comparison view.
            if(tableView.activeSelf)
            {
                ShowComparsionView();
            }
            // If the comparsion view is active, activate the table view.
            else if(comparsionView.activeSelf)
            {
                ShowTableView();
            }
            else // Show table view by default.
            {
                ShowTableView();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Calls late start.
            if (!calledLateStart)
                LateStart();
        }
    }
}