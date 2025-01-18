using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The player in the world scene.
    public class PlayerWorld : MonoBehaviour
    {
        // The match manager.
        public WorldManager worldManager;

        // Start is called before the first frame update
        void Start()
        {
            // World manager is null.
            if (worldManager == null)
                worldManager = WorldManager.Instance;

            // If the player isn't set, set it.
            if(worldManager.playerWorld == null)
                worldManager.playerWorld = this;
        }
    }
}