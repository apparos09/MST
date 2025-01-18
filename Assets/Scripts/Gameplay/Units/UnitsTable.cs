using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The units table.
    public class UnitsTable : MonoBehaviour
    {
        // The units info object.
        public UnitsInfo unitsInfo;

        // The unit group.
        // TODO: this should probably be private.
        public UnitsInfo.unitGroups group = UnitsInfo.unitGroups.none;

        // The entries for the units table.
        public List<UnitsTableEntry> entries = new List<UnitsTableEntry>();

        // Gets set to 'true' when the groups have been initialized.
        private bool groupsInits = false;

        // Conversion groups.
        private List<UnitsInfo.UnitsConversion> lengthImperialConversions = new List<UnitsInfo.UnitsConversion>();
        private List<UnitsInfo.UnitsConversion> weightImperialConversions = new List<UnitsInfo.UnitsConversion>();
        private List<UnitsInfo.UnitsConversion> timeConversions = new List<UnitsInfo.UnitsConversion>();
        private List<UnitsInfo.UnitsConversion> lengthMetricConversions = new List<UnitsInfo.UnitsConversion>();
        private List<UnitsInfo.UnitsConversion> weightMetricConversions = new List<UnitsInfo.UnitsConversion>();
        private List<UnitsInfo.UnitsConversion> capcityConversions = new List<UnitsInfo.UnitsConversion>();

        // Start is called before the first frame update
        void Start()
        {
            // Sets the instance.
            if (unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // Gets the components in the children.
            if (entries.Count == 0)
                entries = new List<UnitsTableEntry>(GetComponentsInChildren<UnitsTableEntry>());

            // Initialize the lists.
            InitializeGroupLists();
        }

        // Initializes the group lists.
        private void InitializeGroupLists()
        {
            // Clears the lists.
            lengthImperialConversions.Clear();
            weightImperialConversions.Clear();
            timeConversions.Clear();
            lengthMetricConversions.Clear();
            weightMetricConversions.Clear();
            capcityConversions.Clear();

            // Not initialized.
            groupsInits = false;

            // If units group info is initialized, get the copies.
            if(UnitsInfo.Instantiated)
            {
                // Adds in the values.
                lengthImperialConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.lengthImperial);
                weightImperialConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.weightImperial);
                timeConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.time);
                lengthMetricConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.lengthMetric);
                weightMetricConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.weightMetric);
                capcityConversions = UnitsInfo.Instance.GetGroupConversionListCopy(UnitsInfo.unitGroups.capacity);

                // Groups initialized.
                groupsInits = true;
            }
            
        }

        // Gets the group.
        public UnitsInfo.unitGroups GetGroup()
        {
            return group;
        }

        // Sets the group
        public void SetGroup(UnitsInfo.unitGroups newGroup)
        {
            group = newGroup;
            LoadConversions();
        }

        // Gets the converison list by group.
        private List<UnitsInfo.UnitsConversion> GetConversionListByGroup()
        {
            return GetConversionListByGroup(group);
        }

        // Gets the conversion list by the set group.
        private List<UnitsInfo.UnitsConversion> GetConversionListByGroup(UnitsInfo.unitGroups convertGroup)
        {
            // The conversions list.
            List<UnitsInfo.UnitsConversion> conversions;

            // Checks the group to know which one to get.
            switch (convertGroup)
            {
                default:
                case UnitsInfo.unitGroups.none: // Empty list.
                    conversions = new List<UnitsInfo.UnitsConversion>();
                    break;

                case UnitsInfo.unitGroups.lengthImperial:
                    conversions = lengthImperialConversions;
                    break;

                case UnitsInfo.unitGroups.weightImperial:
                    conversions = weightImperialConversions;
                    break;

                case UnitsInfo.unitGroups.time:
                    conversions = timeConversions;
                    break;

                case UnitsInfo.unitGroups.lengthMetric:
                    conversions = lengthMetricConversions;
                    break;

                case UnitsInfo.unitGroups.weightMetric:
                    conversions = weightMetricConversions;
                    break;

                case UnitsInfo.unitGroups.capacity:
                    conversions = capcityConversions;
                    break;
            }

            // Returns the conversions.
            return conversions;
        }

        // Loads entries from the provided group.
        public void LoadConversions()
        {
            // If the instance isn't set, set it.
            if (unitsInfo == null)
                unitsInfo = UnitsInfo.Instance;

            // If the group is none, or if the units info is unavailable, clear the entries.
            if (group == UnitsInfo.unitGroups.none)
            {
                // Set the units info if it's not set already.
                if (unitsInfo == null)
                    unitsInfo = UnitsInfo.Instance;

                // Clears the entries.
                ClearEntries();
            }
            else // Load the entries.
            {
                // Gets the conversion list for the provided group.
                // TODO: it's inefficient to get these as copies, but this is to prevent the lists from being edited.
                List<UnitsInfo.UnitsConversion> conversions;

                // if the groups have been initialized, grap one of the exisitng groups.
                // If they haven't been initialized, generate a group copy.
                if (groupsInits)
                {
                    // Gets by group.
                    conversions = GetConversionListByGroup();

                    // Empty group, so get a group copy from UnitsInfo.
                    if(conversions.Count == 0)
                        conversions = unitsInfo.GetGroupConversionListCopy(group);
                }
                else
                {
                    // Gets a units info copy.
                    conversions = unitsInfo.GetGroupConversionListCopy(group);
                }
                    

                // The entry index.
                int entryIndex = 0;

                // Enables all the entries by default to make sure text is properly updated.
                foreach(UnitsTableEntry unitsTableEntry in entries)
                {
                    unitsTableEntry.gameObject.SetActive(true);
                }

                // Goes through all the conversions and loads them in.
                for(int i = 0; i < conversions.Count && i < entries.Count; i++)
                {
                    // entries[i].gameObject.SetActive(true); // No longer needed.
                    entries[i].SetConversion(conversions[i]);
                    entryIndex++; // Add to the index.
                }

                // While there are remaining entries, clear them out.
                while(entryIndex < entries.Count)
                {
                    // entries[entryIndex].gameObject.SetActive(true); // No longer needed.
                    entries[entryIndex].ClearText();
                    entries[entryIndex].gameObject.SetActive(false);

                    entryIndex++;
                }
            }
        }

        // Clears all the entries.
        public void ClearEntries()
        {
            // Clear out all the entries.
            foreach (UnitsTableEntry entry in entries)
            {
                entry.gameObject.SetActive(true);
                entry.ClearText();
                entry.gameObject.SetActive(false);
            }
        }

        // Clears the group and the entries.
        public void ClearGroupAndEntries()
        {
            group = UnitsInfo.unitGroups.none;
            ClearEntries();
        }
    }
}