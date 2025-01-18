using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The data for a stage.
    [System.Serializable]
    public class StageData
    {
        // The stage name.
        public string stageName;

        // The stage time.
        public float stageTime;

        // The stage score.
        public float stageScore;

        // The highest combo.
        public int highestCombo;

        // The number of losses.
        public int losses = 0;

        // Checks if the stage has been cleared.
        public bool cleared = false;
    }

    // The stage.
    public class Stage : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The minimum of the game area.
        public GameObject gameAreaMin;
        
        // The maximum of the game area.
        public GameObject gameAreaMax;

        // The meteor spawn point minimum.
        public GameObject meteorSpawnPointMin;

        // The meteor spawn point maximum.
        public GameObject meteorSpawnPointMax;

        // The parent object for a meteor.
        public GameObject meteorParent;

        // The laser shot spawn point.
        [Tooltip("The laser shot spawn point. This only concerns the y-position.")]
        public GameObject laserShotSpawnPoint;

        // Laser wave spawn point.
        [Tooltip("The laser wave spawn point.")]
        public GameObject laserWaveSpawnPoint;

        [Header("Surface")]

        // The stage surface.
        public StageSurface surface;

        [Header("Backgrounds")]

        // The background renderer.
        public SpriteRenderer backgroundRenderer;

        // Background 1
        public Sprite background01;

        // Background 2
        public Sprite background02;

        // Background 3
        public Sprite background03;

        // Start is called before the first frame update
        void Start()
        {
            // If the manager is not set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Sets the stage.
            if(stageManager.stage == null)
                stageManager.stage = this;
        }

        // Sets the background by the provided number.
        public void SetBackground(int bgdNumber)
        {
            // The new sprite background.
            Sprite newBgd = null;

            // Checks the background number.
            switch(bgdNumber)
            {
                default:
                case 1: // BGD 1
                    newBgd = background01;
                    break;

                case 2: // BGD 2
                    newBgd = background02;
                    break;

                case 3: // BGD 3
                    newBgd = background03;
                    break;
            }

            // Sets the background.
            if(newBgd != null)
                backgroundRenderer.sprite = newBgd;
        }

        // Returns 'true' if the game area is defined.
        public bool IsGameAreaDefined()
        {
            return gameAreaMin != null && gameAreaMax != null;
        }

        // Checks if the provided position is in the game area.
        public bool InGameArea(Vector3 position)
        {
            // If the game area is not defined.
            if(!IsGameAreaDefined())
            {
                Debug.LogWarning("Game area is undefined. Returning true by default.");
                return true;
            }

            // The min bounds and max bounds.
            Vector3 minBounds = gameAreaMin.transform.position;
            Vector3 maxBounds = gameAreaMax.transform.position;

            // Checks each axis.
            bool validX = Mathf.Clamp(position.x, minBounds.x, maxBounds.x) == position.x;
            bool validY = Mathf.Clamp(position.y, minBounds.y, maxBounds.y) == position.y;
            bool validZ = Mathf.Clamp(position.z, minBounds.z, maxBounds.z) == position.z;

            // If all three are valid, it's in the game area.
            return validX && validY && validZ;
        }

        // Checks if this game object is in the game area.
        public bool InGameArea(GameObject go)
        {
            return InGameArea(go.transform.position);
        }

        // Generrates a spawn point for a meteor.
        public Vector3 GenerateMeteorSpawnPoint()
        {
            // If either spawn point is empty, return a zero vector.
            if (meteorSpawnPointMin == null || meteorSpawnPointMax == null)
            {
                return Vector3.zero;
            }
            // If the max spawn point is missing, return the minimum.
            else if (meteorSpawnPointMin != null && meteorSpawnPointMax == null)
            {
                return meteorSpawnPointMin.transform.position;
            }
            // If the min spawn point is missing, return the maximum.
            else if(meteorSpawnPointMin == null && meteorSpawnPointMax != null)
            {
                return meteorSpawnPointMax.transform.position;
            }
            else // Randomize the position between the min and max.
            {
                // Gets the min and max.
                Vector3 min = meteorSpawnPointMin.transform.position;
                Vector3 max = meteorSpawnPointMax.transform.position;

                // If the values are the same, return the value.
                if (min == max)
                    return min;

                // Generates the resulting position.
                Vector3 result = new Vector3();
                result.x = Random.Range(min.x, max.x);
                result.y = Random.Range(min.y, max.y);
                result.z = Random.Range(min.z, max.z);

                // Returns the result.
                return result;
            }
        }

        // Sets the laser shot the (y) spawn position.
        public void SetLaserShotToSpawnPositionY(LaserShot laserShot)
        {
            Vector3 newPos = laserShot.transform.position;
            newPos.y = laserShotSpawnPoint.transform.position.y;
            laserShot.transform.position = newPos;
        }

        // Sets the laser wave to its spawn position.
        public void SetLaserWaveToSpawnPosition(LaserWave laserWave)
        {
            laserWave.transform.position = laserWaveSpawnPoint.transform.position;
        }

        // // Update is called once per frame
        // void Update()
        // {
        // 
        // }
    }
}