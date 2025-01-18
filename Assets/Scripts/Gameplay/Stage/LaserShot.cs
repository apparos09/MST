using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The laser shot by the player.
    public class LaserShot : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The player that this laser shot belongs to. 
        public PlayerStage player;

        // The collider for the meteor.
        public new Collider2D collider;

        // The rigidbody for the meteor.
        public new Rigidbody2D rigidbody;

        // The sprite renderer.
        public SpriteRenderer spriteRenderer;

        // The speed of the laser shot.
        public float maxSpeed = 70.0F;

        // The movement direction.
        public Vector3 moveDirec = Vector3.up;

        // The force for the laser shot when it hits the meteor. 
        public float meteorHitForce = 7.5F;

        // Gets set to 'true' when force should be applied.
        public bool applyForce = true;

        // TODO: account for measurement type?
        // The output value for this laser shot.
        [Tooltip("The output value attached to this laser shot. If it matches that of the meteor's, it is correct.")]
        public float outputValue = 0;

        // The units button this laser shot is attached to. If this units button is marked as correct, it will trigger a...
        // Successful hit regardless of the value comparison.
        [Tooltip("The units button that was used to fire this laser shot. If null, then no button is attached to this shot.")]
        public UnitsButton unitsButton = null;

        // Gets set to 'true' when start has been called.
        private bool startCalled = false;

        [Header("Animation")]

        // The animator.
        public Animator animator;

        // The launch animation.
        public string launchAnim = "Laser Shot - Launch Animation";

        // The idle animation.
        public string idleAnim = "Laser Shot - Idle Animation";

        // The death animation.
        public string deathAnim = "Laser Shot - Death Animation";

        // Sets if animations are being used.
        private bool useAnimations = true;

        [Header("Audio")]

        // The launch sound effect.
        public AudioClip launchSfx;

        // The death sound effect.
        public AudioClip deathSfx;

        // Start is called before the first frame update
        void Start()
        {
            // Start has been called.
            startCalled = true;

            // If the stage manager is not set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // If the collider is not set, try to set it.
            if (collider == null)
                collider = GetComponent<Collider2D>();

            // If the rigidbody is not set, try to set it.
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            // Apply the ignores for the physics bodies.
            ApplyPhysicsBodyIgnores();

            // Sets if animations are being used.
            animator.enabled = useAnimations;  
           
        }

        // Shoot
        // Shoots the shot at the provided target.
        public void Shoot(Vector3 target)
        {
            // Set the starting position.
            Vector3 startPos = transform.position;
            startPos.x = target.x;
            transform.position = startPos;

            // Sets the moveDirec of the shot based on the target.
            if(target != transform.position)
            {
                // Gets the target in a 2D space.
                Vector3 target2D = target;
                target2D.z = transform.position.z;

                // Calculates the move direction.
                moveDirec = (target2D - gameObject.transform.position).normalized;
            }
            else // No target, so just go up.
            {
                moveDirec = Vector3.up;
            }

            // TODO: rotate in direction of movement. This may not be necessary, but you may want to do this.

            // Apply force to the object.
            applyForce = true;

            // Re-applies the physics body ignores since the laser shot has been activated.
            // For some reason, turning off the laser shot resets all these physics ignores...
            // So they need to be set again.
            // Start called is checked to know that everything that's needed has been set properly.
            if(startCalled)
            {
                ApplyPhysicsBodyIgnores();
            }

            // Plays the launch animation.
            if(useAnimations)
            {
                PlayLaunchAnimation();
            }
        }

        // Shoots the shot at the provided game object.
        public void Shoot(GameObject target)
        {
            // If the target is valid, shoot at it.
            // If there is no target, just shoot up.
            if(target != null)
            {
                Shoot(target.transform.position);
            }
            else
            {
                // No target, so send the laser shot's position.
                Shoot(transform.position);
            }
        }

        // Applies the ignore settings for the physics bodies.
        public void ApplyPhysicsBodyIgnores()
        {
            // Layer-based ignores are handled by the stage manager.

            // If the stage manager has not been set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Does manual ignores just to be sure. These aren't really necessary, but they're here regardless.
            // Ignore collision with the stage surface.
            Physics2D.IgnoreCollision(collider, stageManager.stageSurface.collider, true);

            // Goes through all the barriers and ignores collision with them.
            foreach (Barrier barrier in stageManager.stageBarriers)
            {
                Physics2D.IgnoreCollision(collider, barrier.collider, true);
            }
        }

        // Clamps velocity using the provided max velocity.
        public void ClampVelocity(float maxVelocity)
        {
            // Clamp the velocity at the max speed.
            Vector2 velocity = rigidbody.velocity;
            velocity = Vector2.ClampMagnitude(velocity, maxVelocity);
            rigidbody.velocity = velocity;
        }

        // Resets the laser shot's velocity.
        public void ResetVelocity()
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0;
        }

        // Kills the laser shot. If 'true', then the laser shot's hit was a success.
        public virtual void Kill(bool success)
        {
            // Gets the player.
            PlayerStage player = stageManager.player;

            // Checks if the shot was a success.
            if(success)
            {
                // Play a happy animation.
                // If the points goal has been reached, don't play the happy animation because it would...
                // Override a different animation.
                if(!stageManager.IsPointsGoalReached(player))
                    stageManager.stageUI.PlayPartnersAnimation(CharacterIcon.charIconAnim.happy);

                // Release a laser wave.
                if(player.IsUsingLaserWave())
                {
                    // Shoot the laser wave.
                    player.ShootLaserWave(spriteRenderer.color);
                }
                
            }
            else // Not a success.
            {
                // Play a sad aniamtion.
                stageManager.stageUI.PlayPartnersAnimation(CharacterIcon.charIconAnim.sad);

                // Stuns the player if they can be stunned.
                if (player.stunPlayer)
                    player.StunPlayer();
            }

            // If this is the player's active shot, remove it.
            if (player.laserShotActive == this)
                player.laserShotActive = null;

            // Kill the rigidbody velocity.
            ResetVelocity();
            
            // Checks if animations should be used.
            if(useAnimations)
            {
                PlayDeathAnimation();
            }
            else
            {
                OnDeath();
            }
        }

        // Called when the laser shot has died.
        protected virtual void OnDeath()
        {
            // Clear out the units button since the button is dead.
            unitsButton = null;

            // If the player has been set.
            if(player != null)
            {
                // Checks if the laser shot pool is being used.
                // If so, return the shot to the pool. If not, destroy the object.
                if(player.IsUsingLaserShotPool())
                {
                    player.ReturnLaserShotToPool(this);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else // Destroy the object.
            {
                Destroy(gameObject);
            }
        }

        // Resets the laser shot.
        public void ResetLaserShot()
        {
            // Resets the transform and force.
            transform.forward = Vector3.forward;
            moveDirec = Vector3.up;
            applyForce = true;

            // Resets the velocity.
            ResetVelocity();

            // Clears the units button.
            unitsButton = null;
        }

        // ANIMATION

        // Launch Animation
        protected void PlayLaunchAnimation()
        {
            animator.Play(launchAnim);
        }

        // Launch animation start.
        protected void OnLaunchAnimationStart()
        {
            // ...
        }


        // Launch animation end.
        protected void OnLaunchAnimationEnd()
        {
            PlayIdleAnimation();
        }

        // Idle animation.
        protected void PlayIdleAnimation()
        {
            animator.Play(idleAnim);
        }

        // Death
        protected void PlayDeathAnimation()
        {
            animator.Play(deathAnim);
        }

        // Death Start
        public void OnDeathAnimationStart()
        {
            applyForce = false;
            rigidbody.velocity = Vector2.zero;
        }

        // Death End
        public void OnDeathAnimationEnd()
        {
            applyForce = true;
            OnDeath();
        }

        // AUDIO
        // Plays the launch sound effect.
        public void PlayLaunchSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(launchSfx);
        }

        // Plays the death sound effect.
        public void PlayDeathSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(deathSfx);
        }


        // Update is called once per frame
        void Update()
        {
            // If force should be applied.
            if(applyForce)
            {
                // TODO: maybe don't include delta time?
                // Calculate the force.
                Vector2 force = new Vector2();
                force.x = moveDirec.normalized.x * maxSpeed * Time.deltaTime;
                force.y = moveDirec.normalized.y * maxSpeed * Time.deltaTime;

                // Adds the amount of force.
                rigidbody.AddForce(force, ForceMode2D.Impulse);

                // Clamp the velocity at the max speed.
                ClampVelocity(maxSpeed);
            }

            // If the laser isn't in the game area, destroy it.
            if (!stageManager.stage.InGameArea(gameObject))
            {
                Kill(false);
            }

        }
    }
}