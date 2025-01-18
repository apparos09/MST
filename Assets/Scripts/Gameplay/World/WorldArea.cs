using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // An area in the world.
    public class WorldArea : MonoBehaviour
    {
        // The world manager.
        public WorldManager manager;

        // The number of the area.
        public int areaNumber = 0;

        // The list of stages.
        public List<StageWorld> stages = new List<StageWorld>();


        // Since there's one area, there are no area switch events needed.

        // Start is called before the first frame update
        void Start()
        {
            if (manager == null)
                manager = WorldManager.Instance;

            // If there are no stages added, add them automatically.
            if(stages.Count == 0)
            {
                GetComponentsInChildren(stages);
            }
        }

        // // Update is called once per frame
        // void Update()
        // {
        // 
        // }
    }
}