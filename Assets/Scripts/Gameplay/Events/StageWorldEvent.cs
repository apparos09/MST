using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // An event to unlock a stage in the world.
    public class StageWorldEvent : GameEvent
    {
        [Header("StageWorld")]

        // The world stage to be unlocked.
        [Tooltip("The world stage this event is attached to.")]
        public StageWorld stageWorld;

        // The challengers that must be beaten for this challenger to be available.
        [Tooltip("The stages that must be cleared for this stage to be active.")]
        public List<StageWorld> reqStageWorlds;

        // If 'true', the unlock animation is played.
        public bool playUnlockAnim = true;

        // Start is called before the first frame update
        protected override void Start()
        {
            // Sets the event tag.
            if(eventTag == "")
                eventTag = "StageWorld";

            // Tries to get the challenger.
            if (stageWorld == null)
                stageWorld = GetComponent<StageWorld>();

            // Calls base.Start() at the end to make sure everything's set.
            base.Start();
        }

        // Initializes the event.
        public override void InitalizeEvent()
        {
            // Make the stage unavailable from the start.
            if(!CheckStageAvailable())
                stageWorld.SetStageAvailable(false);
        }

        // Checks if the stage is available.
        public bool CheckStageAvailable()
        {
            // Checks if all stages have been cleared.
            bool allCleared = true;

            // Checks if there are prior stages that need to be cleared.
            if (reqStageWorlds.Count > 0)
            {
                // Goes through the required stages.
                for (int i = 0; i < reqStageWorlds.Count; i++)
                {
                    // Not all stages have been cleared.
                    if (!reqStageWorlds[i].IsStageCleared())
                    {
                        allCleared = false;
                        break;
                    }
                }
            }

            // Sets the cleared parameter.
            cleared = allCleared;

            // Returns the result.
            return cleared;
        }

        // Updates the event.
        public override void UpdateEvent()
        {
            CheckStageAvailable();
        }

        // Completes the event.
        public override void OnEventComplete()
        {
            // Calls the event complete base function.
            base.OnEventComplete();

            // Make challenger available.
            stageWorld.SetStageAvailable(true);
        }

    }
}