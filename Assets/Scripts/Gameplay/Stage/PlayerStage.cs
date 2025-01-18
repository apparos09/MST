using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The player for the stage.
    public class PlayerStage : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The number of points.
        private float points = 0;

        // If 'true', the player can be stunned.
        public bool stunPlayer = true;

        // The player stun timer.
        private float playerStunTimer = 0.0F;

        // Player stun timer max.
        public const float PLAYER_STUN_TIMER_MAX = 1.30F; // Originally 0.8F

        [Header("Laser Shot, Wave")]
        // The laser prefab.
        public LaserShot laserShotPrefab;

        // The player's active laser shot.
        public LaserShot laserShotActive;

        // The laser shot object pool.
        private List<LaserShot> laserShotPool = new List<LaserShot>();

        // If the laser shot pool should be used.
        private bool useLaserShotPool = true;

        // If 'true', the player can shoot multiple laser shots.
        private bool multipleLaserShots = false;

        // The laser wave prefab.
        public LaserWave laserWavePrefab;

        // If 'true', the laser wave is used.
        private bool useLaserWave = true;

        // The laser wave object pool.
        private List<LaserWave> laserWavePool = new List<LaserWave>();

        // If the laser wave pool should be used.
        private bool useLaserWavePool = true;

        // Start is called before the first frame update
        void Start()
        {
            // If the stage manger isn't set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Set the player.
            if(stageManager.player == null)
                stageManager.player = this;
        }

        // LASER SHOT
        // Shoots the laser shot with the default colour.
        public LaserShot ShootLaserShot(float outputValue)
        {
            return ShootLaserShot(outputValue, Color.white);
        }

        // Shoots the laser shot and gives it the provided color.
        public LaserShot ShootLaserShot(float outputValue, Color color)
        {
            // If there are not multiple laser shots.
            if (!multipleLaserShots)
            {
                // The player already has an active laser shot.
                if (laserShotActive != null)
                {
                    // If the laser shot is active, return that shot.
                    if (laserShotActive.isActiveAndEnabled)
                    {
                        return laserShotActive;
                    }
                }
            }

            
            // // If the game is slowed down, return to normal speed.
            // if (stageManager.IsSlowSpeed())
            //     stageManager.SetToNormalSpeed();

            // The laser shot to be set.
            LaserShot newShot = null;

            // Checks if the laser shot pool should be used.
            if(useLaserShotPool)
            {
                // There's a laser shot in the pool.
                if(laserShotPool.Count > 0)
                {
                    // Get the shot at the front of the list, and remove it.
                    newShot = laserShotPool[0];
                    laserShotPool.RemoveAt(0);

                    // Turn the shot on.
                    newShot.gameObject.SetActive(true);

                }
                else // No laser shot in the pool.
                {
                    newShot = Instantiate(laserShotPrefab);
                }
            }
            else // Don't use the pool, so make a new object.
            {
                newShot = Instantiate(laserShotPrefab);
            }

            // Set the player for the laser shot.
            newShot.player = this;

            // Set the spawn point.
            stageManager.stage.SetLaserShotToSpawnPositionY(newShot);

            // Gets the meteor.
            Meteor meteor = stageManager.meteorTarget.GetMeteor();

            // If the meteor is not null, target it.
            if (meteor != null)
            {
                newShot.Shoot(meteor.gameObject);
            }
            else // No meteor, so no target.
            {
                newShot.Shoot(null);
            }


            // Sets the laser shot's output value, color, and sets it as the active laser shot.
            newShot.outputValue = outputValue;
            newShot.spriteRenderer.color = color;
            laserShotActive = newShot;

            // Returns the new shot.
            return newShot;
        }

        // Returns 'true' if the laser shot pool should be used.
        public bool IsUsingLaserShotPool()
        {
            return useLaserShotPool;
        }

        // Returns the laser shot to the object pool.
        public void ReturnLaserShotToPool(LaserShot laserShot)
        {
            // If this is the active laser shot, remove it.
            if (laserShotActive == laserShot)
                laserShotActive = null;

            // Resets the laser shot.
            laserShot.ResetLaserShot();

            // Turn off the laser shot and return it to the pool.
            laserShot.gameObject.SetActive(false);
            laserShotPool.Add(laserShot);
        }

        // LASER WAVE
        // Returns 'true' if the laser wave is used.
        public bool IsUsingLaserWave()
        {
            // The result to be returned.
            bool result;

            // If the stage manager is set.
            if(stageManager != null)
            {
                // Checks the gameplay mode.
                switch(stageManager.gameplayMode)
                {
                    // If in focus mode, the laser wave will not be used no matter what.
                    default:
                    case GameplayManager.gameMode.focus:
                        result = false;
                        break;

                    case GameplayManager.gameMode.rush:
                        result = useLaserWave;
                        break;
                }
            }
            else
            {
                result = useLaserWave;
            }

            return result;
        }

        // Shoots a laser wave.
        public LaserWave ShootLaserWave()
        {
            // Generate the wave and set its colour.
            LaserWave newWave = null;

            // Checks if the laser wave pool should be used.
            if (useLaserWavePool)
            {
                // There's a laser wave in the pool.
                if (laserWavePool.Count > 0)
                {
                    // Get the wave at the front of the list, and remove it.
                    newWave = laserWavePool[0];
                    laserWavePool.RemoveAt(0);

                    // Turn the wave on.
                    newWave.gameObject.SetActive(true);

                }
                else // No laser wave in the pool, so instantiate one.
                {
                    newWave = Instantiate(laserWavePrefab);
                }
            }
            else // Don't use the pool, so make a new object.
            {
                newWave = Instantiate(laserWavePrefab);
            }

            // Sets the new wave's player.
            newWave.player = this;

            // Set the wave to the spawn point and launch it.
            stageManager.stage.SetLaserWaveToSpawnPosition(newWave);
            newWave.Launch();
            
            return newWave;
        }

        // Shoots a laser wave and sets its colour.
        public LaserWave ShootLaserWave(Color waveColor)
        {
            LaserWave newWave = ShootLaserWave();
            newWave.spriteRenderer.color = waveColor;

            return newWave;
        }

        // Returns 'true' if the laser wave pool should be used.
        public bool IsUsingLaserWavePool()
        {
            return useLaserWavePool;
        }

        // Returns the laser wave to the object pool.
        public void ReturnLaserWaveToPool(LaserWave laserWave)
        {
            // Returns the laser wave to its spawn position.
            stageManager.stage.SetLaserWaveToSpawnPosition(laserWave);

            // Turn off the laser wave and puts it back in the pool.
            laserWave.gameObject.SetActive(false);
            laserWavePool.Add(laserWave);
        }


        // OTHER 
        // Gets the points.
        public float GetPoints()
        {
            return points;
        }
        
        // Set the player's points.
        public void SetPoints(float newPoints)
        {
            // Set value.
            points = newPoints;

            // The points can't be negative.
            if(points < 0)
                points = 0;

            // On points changed.
            OnPointsChanged();
        }

        // Gives points to the player.
        public void GivePoints(float pointsAdd)
        {
            SetPoints(points + pointsAdd);
        }

        // Calculates and gives the player points.
        public void CalculateAndGivePoints(Meteor meteor)
        {
            GivePoints(stageManager.CalculatePoints(meteor));
        }

        // Removes points from the player.
        public void RemovePoints(float pointsMinus)
        {
            SetPoints(points - pointsMinus);
        }

        // Called when the points have changed.
        public void OnPointsChanged()
        {
            // The player's points have changed.
            stageManager.OnPlayerPointsChanged();
        }

        // STUN
        // Is the player stunned?
        public bool IsPlayerStunned()
        {
            return playerStunTimer > 0.0F;
        }

        // Stuns the player.
        public virtual void StunPlayer()
        { 
            OnPlayerStunStarted();
        }

        // Unstuns the player.
        public virtual void UnstunPlayer()
        {
            OnPlayerStunEnded();
        }

        // The player has gotten stunned.
        protected virtual void OnPlayerStunStarted()
        {
            playerStunTimer = PLAYER_STUN_TIMER_MAX;
            stageManager.stageUI.MakeUnitButtonsUninteractable();
        }

        // The player is no longer stunned.
        protected virtual void OnPlayerStunEnded()
        {
            playerStunTimer = 0.0F;

            // The meteor's target is being targeted exactly, make the buttons interactable again.
            if(stageManager.meteorTarget.IsMeteorTargetedExactly())
            {
                stageManager.stageUI.MakeUnitButtonsInteractable();
            }            
        }

        // Update is called once per frame
        void Update()
        {
            // If the player stun timer is running, and the game is playing.
            if(playerStunTimer > 0.0F && stageManager.IsGamePlaying())
            {
                // Reduce by unscaled delta time.
                playerStunTimer -= Time.unscaledDeltaTime;

                // No longer stunned.
                if(playerStunTimer <= 0)
                {
                    playerStunTimer = 0;
                    OnPlayerStunEnded();
                }
            }
        }
    }
}