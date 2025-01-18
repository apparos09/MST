using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The barrier for the stage.
    public class Barrier : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The barrier's sprite.
        public SpriteRenderer spriteRenderer;

        // The collider for the meteor.
        public new Collider2D collider;

        // The health of the surface.
        // TOOD: make private when not testing.
        public float health = 1.0F;

        // The maximum health of the surface.
        public float maxHealth = 1.0F;

        // If true, the health is set to max on start.
        public bool setToMaxHealthOnStart = true;

        [Header("Colors")]

        // The color at maximum health.
        public Color maxHealthColor = Color.green;

        // The color at half health.
        public Color halfHealthColor = Color.yellow;

        // The color at no health.
        public Color noHealthColor = Color.red;

        [Header("Animation")]

        // The animator for the barrier.
        public Animator animator;

        // The barrier's revive animation.
        public string barrierReviveAnim = "Barrier - Revive Animation";

        // The barrier's death animation.
        public string barrierDeathAnim = "Barrier - Death Animation";

        // Uses the animations if set to 'true'.
        private bool useAnimations = true;

        [Header("Audio")]

        // The barrier revival SFX.
        public AudioClip reviveSfx;

        // The barrier death SFX.
        public AudioClip deathSfx;

        // Start is called before the first frame update
        void Start()
        {
            // Set the instance.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // If the collider is not set, try to set it.
            if (collider == null)
                collider = GetComponent<Collider2D>();


            // Animator setting.
            animator.enabled = useAnimations;

            // Set the health to max.
            if(setToMaxHealthOnStart)
                SetHealthToMax();
        }

        // Gets the health.
        public float GetHealth()
        {
            return health;
        }

        // Sets the health.
        public virtual void SetHealth(float newHealth)
        {
            health = Mathf.Clamp(newHealth, 0, maxHealth);
            UpdateBarrierColor();
        }

        // Checks if a barrier is dead.
        public bool IsDead()
        {
            return health <= 0;
        }

        // Returns 'true' if health is at max.
        public bool IsHealthAtMax()
        {
            return health >= maxHealth;
        }    

        // Set the health to the max.
        public void SetHealthToMax()
        {
            SetHealth(maxHealth);
        }

        // Applies damage to the surface.
        public void ApplyDamage(float damage)
        {
            health -= damage;

            // Updates the barrier color.
            UpdateBarrierColor();

            // Called when the barrier has been damaged.
            stageManager.OnBarrierDamaged();

            // If the surface health is 0 or less, kill it.
            if(health <= 0)
            {
                health = 0;
                KillBarrier();
            }
        }

        // Kills the barrier.
        public void KillBarrier()
        {
            health = 0;

            // If animations should be used.
            if (useAnimations)
            {
                PlayBarrierDeathAnimation();
            }
            else
            {
                OnBarrierKilled();
            }
        }

        // On the barrier being killed.
        protected void OnBarrierKilled()
        {
            gameObject.SetActive(false);
        }

        // Restores the barrier.
        public void RestoreBarrier()
        {
            // Checks if the barrier was dead.
            bool wasDead = IsDead();

            // Make the game object active.
            gameObject.SetActive(true);

            // Sets the health to max.
            SetHealthToMax();

            // If animations should be used.
            if (useAnimations)
            {
                // If the barrier was dead, play the revive animation.
                if(wasDead)
                {
                    PlayBarrierReviveAnimation();
                }
            }
        }

        // Updates the barrier's color.
        public void UpdateBarrierColor()
        {
            // The two colours, and the t-value between them.
            Color color1, color2;
            float colorT = 0.0F;

            // The health percentage.
            float healthPercent = Mathf.Clamp01(health / maxHealth);

            // Checks the health.
            if (healthPercent >= 0.5F) // Green to Yellow
            {
                color1 = maxHealthColor;
                color2 = halfHealthColor;
                colorT = Mathf.InverseLerp(1.0F, 0.5F, healthPercent);
            }
            else // Yellow to Red
            {
                color1 = halfHealthColor;
                color2 = noHealthColor;
                colorT = Mathf.InverseLerp(0.5F, 0.0F, healthPercent);
            }

            // Calculates the final color.
            Color finalColor = new Color
                (
                Mathf.Lerp(color1.r, color2.r, colorT),
                Mathf.Lerp(color1.g, color2.g, colorT),
                Mathf.Lerp(color1.b, color2.b, colorT)
                );

            // Sets the sprite renderer to the final colour.
            spriteRenderer.color = finalColor;
        }

        // ANIMATION
        // Revive animation.
        public void PlayBarrierReviveAnimation()
        {
            animator.Play(barrierReviveAnim);
        }

        // Revive start
        public void OnBarrierReviveAnimationStart()
        {
            // ...
        }

        // Revive end
        public void OnBarrierReviveAnimationEnd()
        {
            SetHealthToMax();
        }

        // Death Animation.
        public void PlayBarrierDeathAnimation()
        {
            animator.Play(barrierDeathAnim);
        }

        // Death Start
        public void OnBarrierDeathAnimationStart()
        {
            // ...
        }

        // Death End
        public void OnBarrierDeathAnimationEnd()
        {
            OnBarrierKilled();
        }


        // AUDIO
        // Plays the revive SFX.
        public void PlayReviveSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(reviveSfx);
        }

        // Plays the death SFX.
        public void PlayDeathSfx()
        {
            stageManager.stageAudio.PlaySoundEffectWorld(deathSfx);
        }
    }
}