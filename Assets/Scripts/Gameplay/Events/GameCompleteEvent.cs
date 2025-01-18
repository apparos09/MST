using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RM_MST
{
    // The event for when the game is completed.
    public class GameCompleteEvent : GameEvent
    {
        // Manager.
        public WorldManager manager;

        // If 'true', the game complete event is checked on update.
        public bool checkOnUpdate = true;

        // Start is called before the first frame update
        protected override void Start()
        {
            // Not set.
            if(manager == null)
                manager = WorldManager.Instance;

            // Base Start
            base.Start();
        }

        // Checks if the game is complete.
        public bool CheckGameComplete()
        {
            // Checks if the event is finished.
            bool finished = true;

            // Checks the stage list to see if there are stages in it.
            if (manager.stages.Count > 0)
            {
                // Goes through all the stages.
                foreach (StageWorld stageWorld in manager.stages)
                {
                    // If the stage hasn't been cleared,
                    if (!stageWorld.IsStageCleared())
                    {
                        finished = false;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No stages found in manager. Game is not complete by default .");
                finished = false;
            }

            // Sets finished and returns it.
            cleared = finished;
            return finished;
        }

        // Update Event
        public override void UpdateEvent()
        {
            // If the game complete should be checked every update.
            if(checkOnUpdate)
            {
                CheckGameComplete();
            }
        }

        // Event complete.
        public override void OnEventComplete()
        {
            // Calls the base functon.
            base.OnEventComplete();

            // Called to complete the game.
            manager.OnGameComplete();
        }

    }
}