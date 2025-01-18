using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The meteor being spawned.
    public class Meteor : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The collider for the meteor.
        public new Collider2D collider;

        // The rigidbody for the meteor.
        public new Rigidbody2D rigidbody;

        // The rigidbody's base gravity scale.
        private float baseGravityScale = 1;

        // The meteor's spawn point.
        public Vector3 spawnPoint = new Vector3();

        // TODO: generate random values to display as options.
        // The conversion for the meteor.
        public UnitsInfo.UnitsConversion conversion;

        // The possible outputs count.
        public const int POSSIBLE_OUTPUTS_COUNT = 7;

        // TODO: make the possible outputs private?
        // A list of possible outputs for the meteor.
        // One of these will be correct.
        [HideInInspector()]
        public float[] possibleOutputs = new float[POSSIBLE_OUTPUTS_COUNT];

        // The possible output multipliers. These are set based on the unit group.
        [HideInInspector()]
        public float[] possibleOutputMults = new float[POSSIBLE_OUTPUTS_COUNT];

        // The max health of the meteor.
        public float maxHealth = 1.0F;

        // This value gets set to 0 when the meteor dies. This is just used to check if it's dead.
        private float health = 1.0F;

        // Gets set to 'true' when the meteor is suffering from knockback (rush mode).
        private bool inKnockback = false;

        // If 'true', the target on the meteor is released upon knockback wearing off (rush mode).
        private bool untargetOnKnockbackEnd = true;

        // The meteor's position when it was last hit.
        private Vector3 lastHitPos;

        // The amount of time it took for the player to provide an answer for this meteor's conversion last time.
        private float recentAnswerLength = 0.0F;

        // Called the late start.
        private bool calledLateStart = false;

        // The list of meteors active.
        private static List<Meteor> meteorsActive = new List<Meteor>();

        [Header("Sprites")]

        // The sprite renderer.
        public SpriteRenderer meteorSpriteRenderer;

        // The list of meteor sprites.
        public List<Sprite> meteorSprites;

        // If 'true', the sprite is randomized upon spawning.
        public bool randomSpriteOnSpawn = true;

        [Header("Animation")]

        // The animator.
        public Animator animator;

        // The death animation for the meteor.
        public string deathAnim = "Meteor - Death Animation";

        // Sets if animations are being used.
        private bool useAnimations = true;

        [Header("Audio")]

        // The successful destruction SFX.
        public AudioClip meteorDestroySuccessSfx;

        // The failed destruction sfx.
        public AudioClip meteorDestroyFailSfx;

        // A sound that plays when the player can't select the meteor.
        public AudioClip meteorSelectFailSfx;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Adds the meteor to the active list.
            AddMeteorToMeteorsActiveList(this);

            // Tries to get the rigidbody to try and set the meteor's base gravity scale.
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            // Saves the rigidbody's gravity scale.
            if (rigidbody != null)
            {
                // If the gravity scale isn't 0, save what it's set to.
                if(rigidbody.gravityScale != 0)
                    baseGravityScale = rigidbody.gravityScale;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // If the stage manager is not set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // If the collider is not set, try to set it.
            if (collider == null)
                collider = GetComponent<Collider2D>();

            // Already checked in awake, but this check is kept in just to be safe.
            // If the rigidbody is not set, try to set it.
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            // Sets if animations are being used.
            animator.enabled = useAnimations;
        }

        // Late start function.
        void LateStart()
        {
            calledLateStart = true;

            // Regenerates alternate outputs to fix a bug.
            GenerateAlternateOutputs();
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            AddMeteorToMeteorsActiveList(this);
        }

        // This function is called when the object becomes disabled and inactive.
        private void OnDisable()
        {
            RemoveMeteorFromMeteorsActiveList(this);
        }

        // OnCollisionEnter2D is called when this collider2D/rigidbody2D has begun touching another collider2D/rigidbody2D.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEvent(collision.gameObject);
        }

        // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D only)
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollisionEvent(collision.gameObject);
        }

        // Called with mouse down.
        private void OnMouseDown()
        {
            // Checks if the player can manually target meteors.
            // This function checks if the function is enabled.
            if(stageManager.IsManualMeteorTargetingEnabled())
            {
                // If the game is in focus mode.
                if(stageManager.UsingFocusMode())
                {
                    // If the meteor is effected by gravity, it shouldn't be manually selectable...
                    if(IsGravityEnabled()) // Can't be manually selected.
                    {
                        // Play the selection failure sound if this isn't the meteor that's being targeted.
                        if(!stageManager.meteorTarget.IsMeteorTargeted(this))
                        {
                            PlaySelectFailureSfx();
                        }
                    }
                    else // Can be manaully selected.
                    {
                        // If the player isn't stunned, and this meteor isn't being targeted, target this meteor.
                        if (!stageManager.player.IsPlayerStunned() && !stageManager.meteorTarget.IsMeteorTargeted(this))
                        {
                            stageManager.meteorTarget.RemoveTarget();
                            stageManager.meteorTarget.SetTarget(this);
                        }
                    }
                }
            }
        }

        // Called on a collision event.
        private void OnCollisionEvent(GameObject other)
        {
            // Possible collision objects.
            LaserShot laserShot;
            Barrier barrier;
            StageSurface surface;

            // Tries to grab relevant components.
            if (other.TryGetComponent(out laserShot)) // Laser
            {
                TryGivePoints(laserShot);
            }
            else if (other.TryGetComponent(out barrier)) // Barrier
            {
                ApplyDamageToBarrier(barrier);
            }
            else if (other.transform.TryGetComponent(out surface)) // Surface
            {
                ApplyDamageToStageSurface(surface);
            }
        }



        // METEOR LIST
        // Adds the meteor to the meteors instantiated list.
        private void AddMeteorToMeteorsActiveList(Meteor meteor)
        {
            // If the meteor is not in the list, put it in the list.
            if (!meteorsActive.Contains(meteor))
                meteorsActive.Add(meteor);
        }

        // Remove the meteor to the meteors instantiated list.
        private void RemoveMeteorFromMeteorsActiveList(Meteor meteor)
        {
            // If the meteor is in the list, remove it.
            if (meteorsActive.Contains(meteor))
                meteorsActive.Remove(meteor);
        }

        // Gets the meteors instantiated count.
        public static int GetMeteorsActiveCount()
        {
            return meteorsActive.Count;
        }

        // Gets a copy of the meteors instantiated list.
        public static List<Meteor> GetMeteorsActiveListCopy()
        {
            return new List<Meteor>(meteorsActive);
        }

        // Refreshes the meteors active list to remove null values.
        // This shouldn't be needed, but it's been kept here.
        public static void RefreshMeteorsActiveList()
        {
            // The meteors active.
            for (int i = meteorsActive.Count - 1; i >= 0; i--)
            {
                // If the index is null, remove it.
                if (meteorsActive[i] == null)
                {
                    meteorsActive.RemoveAt(i);
                }
            }
        }

        // Destroys all the meteors in the active list.
        public static void KillAllMeteorsInActiveList()
        {
            // Goes through all meteors.
            for (int i = 0; i < meteorsActive.Count; i++)
            {
                // If the meteor exists, kill it.
                if (meteorsActive[i] != null)
                {
                    meteorsActive[i].Kill();
                }
            }

            // Clear out the list.
            meteorsActive.Clear();
        }

        // SPAWN
        // Called when the meteor has been spawned.
        public void OnSpawn()
        {
            // If the sprite should be randomized.
            if (randomSpriteOnSpawn)
            {
                RandomizeSprite();
            }

            // Other
            SetHealthToMax();
            ResetVelocity();
            SetMeteorToSpawnPoint();
            SetRandomizeAngularVelocity();

            // The recent answer length is set to 0.
            recentAnswerLength = 0;
        }

        // Randomizes the meteor's sprite.
        public void RandomizeSprite()
        {
            // There are sprites.
            if (meteorSprites.Count > 0)
            {
                // Gets a random index and a sprite.
                int randIndex = Random.Range(0, meteorSprites.Count);
                Sprite sprite = meteorSprites[randIndex];

                // The sprite is not null, so use it.
                if (sprite != null)
                {
                    meteorSpriteRenderer.sprite = sprite;
                }
            }
        }


        // Sets the spawn point.
        public void SetSpawnPoint(Vector3 newSpawnPoint, bool resetToSpawn)
        {
            // Change spawn point.
            spawnPoint = newSpawnPoint;
            
            // Sets the meteor to the spawn point.
            if(resetToSpawn)
            {
                SetMeteorToSpawnPoint();
            }
        }

        // Sets the meteor to its spawn point.
        public void SetMeteorToSpawnPoint()
        {
            transform.position = spawnPoint;
            lastHitPos = transform.position; // Save last hit position as spawn position.
        }

        // Resets the meteor's velocity.
        public void ResetVelocity()
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0;
        }

        // Randomzies the angular velocity.
        public void SetRandomizeAngularVelocity()
        {
            // The min and max for the rotation.
            float rotMin = 30;
            float rotMax = 310;

            // Randomizes the velocity and the rotation direction.
            rigidbody.angularVelocity = Random.Range(rotMin, rotMax);
            rigidbody.angularVelocity *= (Random.Range(0, 2) == 0) ? 1 : -1;
        }

        // If the meteor is effected by gravity, it returns true.
        public bool IsGravityEnabled()
        {
            return rigidbody.gravityScale != 0.0F;
        }

        // CONVERSIONS

        // Gets the converted value for the meteor.
        public float GetConvertedValue()
        {
            // Checks if conversion is set to get the value.
            if (conversion != null)
            {
                return conversion.GetConvertedValue();
            }
            else
            {
                return -1;
            }
        }

        // Gets the alternate value multipliers.
        public static float[] GenerateAlternateValueMultipliers(UnitsInfo.unitGroups group)
        {
            // The output multiples.
            float[] outputMults = new float[POSSIBLE_OUTPUTS_COUNT];

            // Checks the group to know what set to use.
            switch (group)
            {
                // Set 1 (specific multiples, mainly of 3)
                case UnitsInfo.unitGroups.lengthImperial:
                case UnitsInfo.unitGroups.weightImperial:
                case UnitsInfo.unitGroups.time:

                    // Factors
                    // 3, 6, 12, 16, 24, 30, 60
                    outputMults[0] = 3.0F;
                    outputMults[1] = 6.0F;
                    outputMults[2] = 12.0F;
                    outputMults[3] = 16.0F;
                    outputMults[4] = 24.0F;
                    outputMults[5] = 30.0F;
                    outputMults[6] = 60.0F;

                    break;

                // Set 2 (multiples of 10)
                default:
                case UnitsInfo.unitGroups.lengthMetric:
                case UnitsInfo.unitGroups.weightMetric:
                case UnitsInfo.unitGroups.capacity:
                    // Factors (multiples of 10)
                    // 0.1, 1, 10, 100, 1000, 10,000, 100,000

                    outputMults[0] = 0.1F;
                    outputMults[1] = 1.0F;
                    outputMults[2] = 10.0F;
                    outputMults[3] = 100.0F;
                    outputMults[4] = 1000.0F;
                    outputMults[5] = 10000.0F;
                    outputMults[6] = 100000.0F;

                    break;
            }

            // Returns the output multipliers.
            return outputMults;
        }

        // Generates possible conversion outputs.
        public void GenerateAlternateOutputs()
        {
            // No conversion set, so just fill 0 for everything.
            if (conversion == null)
            {
                // Fill all spots with 0.
                for (int i = 0; i < possibleOutputs.Length; i++)
                {
                    possibleOutputs[i] = 0;
                }

                return;
            }

            // The true output of the operation.
            float inputValue = conversion.inputValue;
            float trueOutputValue = conversion.GetConvertedValue();

            // Generates the alternate value multipliers.
            float[] outputMults = GenerateAlternateValueMultipliers(conversion.group);

            // Goes through all indexes to generate the alternate results.
            for (int i = 0; i < possibleOutputs.Length; i++)
            {
                // The multiplication factor.
                float mult = outputMults[i];

                // Generates the result and rounds it.
                float result = inputValue * mult;
                result = util.CustomMath.Round(result, StageManager.UNITS_DECIMAL_PLACES);

                // Save the result and the output mult.                
                possibleOutputs[i] = result;
                possibleOutputMults[i] = mult;
            }

            // TODO: replace the value that's the closest to the correct output value, or do an approx equals check instead.

            // The input for the correct output.
            int trueOutputIndex = -1;

            // Checks if the possible output is set.
            // It's done this way instead of using System.Array.IndexOf()...
            // Because of floating point impreicision
            for (int i = 0; i < possibleOutputs.Length; i++)
            {
                // If these values are approximately the same...
                // Then the correct value has been set.
                if (Mathf.Approximately(possibleOutputs[i], trueOutputValue))
                {
                    // Sets the possible outputs and possible mult.
                    possibleOutputs[i] = trueOutputValue;
                    possibleOutputMults[i] = conversion.GetsConverisonMultiplier();

                    // Saves the index and breaks the loop.
                    trueOutputIndex = i;
                    break;
                }
            }


            // If the output value is not in the list, put it in a random location.
            if (trueOutputIndex < 0)
            {
                // Generate a random index.
                int randIndex = Random.Range(0, possibleOutputs.Length);

                // Save the values.
                possibleOutputs[randIndex] = trueOutputValue;
                possibleOutputMults[randIndex] = conversion.GetsConverisonMultiplier();
                trueOutputIndex = randIndex;
            }
        }

        // HEALTH/DAMAGE

        // Returns 'true 'if the meteor is alive.
        public bool IsAlive()
        {
            return health > 0.0F;
        }

        // Returns 'true' if the meteor is dead.
        public bool IsDead()
        {
            return health <= 0.0F;
        }

        // Returns the health.
        public float GetHealth()
        {
            return health;
        }

        // Sets the health.
        public void SetHealth(float newHealth)
        {
            health = Mathf.Clamp(newHealth, 0, maxHealth);
        }

        // Sets the health to the max.
        private void SetHealthToMax()
        {
            SetHealth(maxHealth);
        }

        // Checks if this is the closest meteor to the surface.
        // If no closest meteor is found, this returns false.
        public bool IsClosestMeteor()
        {
            // Gets the closest meteor.
            Meteor m1 = stageManager.GetClosestMeteor();

            // If a meteor was found.
            if (m1 != null)
            {
                // Returns 'true' if this the closest meteor is this meteor.
                return m1 == this;
            }
            else
            {
                // It's not known which is the cloest meteor.
                return false;
            }
        }


        // OTHER
        // Give points to the player.
        public bool TryGivePoints(LaserShot laserShot)
        {
            // Gets set based on if the laser shot's output value is correct.
            bool success;

            // The conversion output value.
            float outputValue = conversion.GetConvertedValue();

            // Save this as the last hit position.
            lastHitPos = transform.position;

            // If the values match, the laser shot was a success.
            // Now uses an approximate check in case the vales are slightly off.
            if (Mathf.Approximately(laserShot.outputValue, outputValue))
            {
                success = true;
            }
            else // If the values don't match, the laser shot was a failure.
            {
                success = false;
            }

            // If the shot was not a success, check the shot's units button.
            if(!success && laserShot.unitsButton != null)
            {
                // If the laser shot's units button is supposed to be the correct value...
                // Override 'success' to register as true.
                if (laserShot.unitsButton.correctValue)
                    success = true;
            }

            // Give the player points.

            // Add force for knockback.
            if(UsingKnockbackForce())
            {
                // Knock back the meteor and kill the laser.
                Vector3 forceDirec = laserShot.moveDirec;

                // If the force direction is 0, set it to the forward of the laser shot.
                if (forceDirec == Vector3.zero)
                    forceDirec = laserShot.transform.forward;

                // Apply the knockback force.
                ApplyKnockbackForce(forceDirec, laserShot.meteorHitForce, true);
            }
            else
            {
                // Don't move the meteor.
                rigidbody.velocity = Vector2.zero;
            }

            // Calculates the time it took to give the most recent answer.
            recentAnswerLength = stageManager.stageTime - stageManager.meteorTarget.GetStageTimeOfTargeting();

            // Bounds check for the answer length.
            if(recentAnswerLength <= 0.0F)
                recentAnswerLength = 0.0F;

            // If the laser shot was a success, kill the meteor.
            if (success)
            {
                stageManager.IncreaseCombo(); // Increase the combo.
                stageManager.player.CalculateAndGivePoints(this); // Give the player points.                                                                  // Kill the laser shot.
                laserShot.Kill(success); // Kill the laser.

                // Add to the consecutive success counter.
                stageManager.IncreaseConsecutiveSuccessesCount();

                // Play the combo animation if the player has more than one in the combo count.
                if (stageManager.combo > 1)
                {
                    // Play the animation at the meteor's position.
                    stageManager.comboDisplay.PlayComboAnimationAtPosition(transform.position);
                }

                // Plays the green screen effect.
                if (stageManager.stageUI.UseScreenEffects)
                    stageManager.stageUI.screenEffects.PlayEdgeGlowGreenAnimation();

                // Plays the event text animation (correct).
                stageManager.stageUI.eventTextDisplay.PlayDisplayCorrectAnimation();

                // Kill the meteor.
                Kill();
            }
            else
            {
                // The meteor has survived.

                // Reset the success count, unit button text reveals, and the combo.
                stageManager.ResetConsecutiveSuccessesCount();
                stageManager.stageUI.EndUnitButtonMultipleReveals();
                stageManager.ResetCombo(false); // Reset the combo.

                // Kill the laser.
                laserShot.Kill(success);

                // If the game is in focus mode, turn on the gravity for the meteor.
                if(stageManager.UsingFocusMode())
                {
                    // Trigger the metoer falling.
                    rigidbody.gravityScale = baseGravityScale;

                    // Remove the meteor from the targeting system.
                    stageManager.meteorTarget.RemoveTarget();
                }

                // Call related function.
                stageManager.OnMeteorSurivived(this);

                // Plays the sound effect.
                PlayDestroyFailureSfx();

                // Plays the blue screen effect.
                if (stageManager.stageUI.UseScreenEffects)
                    stageManager.stageUI.screenEffects.PlayEdgeGlowBlueAnimation();

                // Plays the event text animation (incorrect).
                stageManager.stageUI.eventTextDisplay.PlayDisplayIncorrectAnimation();
            }


            // Returns the success value.
            return success;
        }

        // Checks if the meteor is using the knockback mechanic.
        public bool UsingKnockbackForce()
        {
            bool result;

            // Checks the gameplay mode.
            switch(stageManager.gameplayMode)
            {
                default:
                case GameplayManager.gameMode.focus:
                    result = false;
                    break;

                case GameplayManager.gameMode.rush:
                    result = true; 
                    break;
            }

            // Returns the result.
            return result;
        }

        // Applies knockback force to the meteor.
        // If 'untargetOnEnd' is true, the meteor is untargeted once the knockback wears off.
        public void ApplyKnockbackForce(Vector2 forceDirec, float knockbackForce, bool untargetOnEnd)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(forceDirec.normalized * knockbackForce, ForceMode2D.Impulse);
            inKnockback = true;

            // Sets if the meteor should be untargeted upon the knockback wearing off.
            untargetOnKnockbackEnd = untargetOnEnd;
        }

        // Returns 'true' if the meteor is in a knockback state.
        public bool IsInKnockback()
        {
            return inKnockback;
        }

        // Damage the barrier.
        public void ApplyDamageToBarrier(Barrier barrier)
        {
            // Applies damage.
            barrier.ApplyDamage(1.0F);

            // If screen effects are enabled, play an orange flash if a barrier is damaged.
            if (stageManager.stageUI.UseScreenEffects)
            {
                stageManager.stageUI.screenEffects.PlayEdgeGlowOrangeAnimation();
            }

            // Kills the meteor.
            Kill();
        }

        // Damage the surface.
        public void ApplyDamageToStageSurface(StageSurface surface)
        {
            // TODO: implement.
            surface.ReduceHealth(1.0F);
            Kill();
        }

        // Kills the meteor.
        public void Kill()
        {
            // Meteor killed, so set health to 0, stop moving and call on killed function.
            SetHealth(0);
            ResetVelocity();
            stageManager.OnMeteorKilled(this);

            // Checks if animations should be used.
            if (useAnimations)
            {
                PlayDeathAnimation();
            }
            else
            {
                OnDeath();
            }
        }

        // Called when the meteor has been destroyed.
        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        // Animation
        // Death
        protected void PlayDeathAnimation()
        {
            animator.Play(deathAnim);
        }

        // Death Start
        public void OnDeathAnimationStart()
        {
            // Reset the meteor's velocity again to make sure the explosion moves from the meteor as little as possible.
            ResetVelocity();
        }

        // Death End
        public void OnDeathAnimationEnd()
        {
            OnDeath();
        }

        // Audio
        // Plays the destruction success SFX.
        public void PlayDestroySuccessSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(meteorDestroySuccessSfx);
        }

        // Plays the destruction fail SFX.
        public void PlayDestroyFailureSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(meteorDestroyFailSfx);
        }

        // Plays the select fail SFX.
        public void PlaySelectFailureSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(meteorSelectFailSfx);
        }



        // Update is called once per frame
        void Update()
        {
            // Call the late start function.
            if (!calledLateStart)
                LateStart();

            // If the meteor is moving downwards, cap the velocity.
            if (rigidbody.velocity.y < 0)
            {
                // The meteor was experiencing knockback.
                if (inKnockback)
                {
                    // Look for a target again to see if another meteor has gotten closer.
                    // Only do this if this was the meteor being targeted...
                    // And this meteor should be untargeted when the knockback ends.
                    // Checks first if this is the meteor being targeted.
                    if (stageManager.meteorTarget.IsMeteorTargeted(this))
                    {
                        // Remove the meteor from the target if it should be untargeted.
                        if (untargetOnKnockbackEnd)
                        {
                            stageManager.meteorTarget.RemoveTarget();
                        }
                        else // If this meteor should still be targeted...
                        {
                            // Check to make sure that it's still the closest meteor.
                            // If it isn't the closest meteor, remove it from the target.
                            if (!IsClosestMeteor())
                            {
                                stageManager.meteorTarget.RemoveTarget();
                            }
                        }
                    }


                    // Set knockback to false.
                    inKnockback = false;
                }

                // Gets the velocity and clamps it.
                Vector2 velocity = rigidbody.velocity;
                velocity = Vector2.ClampMagnitude(velocity, stageManager.GetModifiedMeteorSpeedMax());

                // Set the velocity.
                rigidbody.velocity = velocity;

                // If using focus mode.
                if(stageManager.UsingFocusMode())
                {
                    // The position distance and the movement distance.
                    float posDist = Vector3.Distance(lastHitPos, transform.position);
                    float moveDist = stageManager.GetModifiedMeteorMoveDistance(recentAnswerLength);

                    // If the meteor has moved far enough from where it was hit...
                    // Stop moving. This happens when the postDist is over the moveDist.
                    if (posDist >= moveDist)
                    {
                        // Saves the angular velocity.
                        float angVelo = rigidbody.angularVelocity;

                        // Stops the meteor from falling.
                        rigidbody.gravityScale = 0;
                        rigidbody.velocity = Vector2.zero;

                        // Restore the angular velocity.
                        rigidbody.angularVelocity = angVelo;
                    }
                }
            }

            // Not in game area, so kill it.
            if (!stageManager.stage.InGameArea(gameObject))
            {
                Kill();
            }
        }

        // This function is called when the MonoBehaviour will be destroyed
        private void OnDestroy()
        {
            RemoveMeteorFromMeteorsActiveList(this);
        }

    }
}