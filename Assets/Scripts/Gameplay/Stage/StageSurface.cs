using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The surface of the stage.
    public class StageSurface : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The collider for the meteor.
        public new Collider2D collider;

        // TODO: make private when not testing.
        // The health of the surface.
        public float health = 1.0F;

        // The maximum health of the surface.
        public float maxHealth = 1.0F;

        [Header("Sprites")]

        // The sprite set for the surface.
        public int spriteSet = 0;

        // The number of surface sprite sets.
        public const int SURFACE_SPRITE_SET_COUNT = 3;

        [Header("Sprites/Top Layer")]
        // Top
        public SpriteRenderer topLayerRenderer;

        // Sprites
        public Sprite topLayerSprite01;
        public Sprite topLayerSprite02;
        public Sprite topLayerSprite03;

        [Header("Sprites/Middle Layer")]

        // Middle
        public SpriteRenderer middleLayerRenderer;

        // Sprites
        public Sprite middleLayerSprite01;
        public Sprite middleLayerSprite02;
        public Sprite middleLayerSprite03;

        [Header("Sprites/Bottom Layer")]

        // Bottom
        public SpriteRenderer bottomLayerRenderer;

        // Sprites
        public Sprite bottomLayerSprite01;
        public Sprite bottomLayerSprite02;
        public Sprite bottomLayerSprite03;

        [Header("Animation")]

        // The animator for the surface.
        public Animator animator;

        // The empty animation state.
        public string emptyAnim = "Empty State";

        // The damage animation.
        public string damageAnim = "Surface - Damage Animation";

        // Start is called before the first frame update
        void Start()
        {
            // Set the instance.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // If the collider is not set, try to set it.
            if (collider == null)
                collider = GetComponent<Collider2D>();

            // Sets the surface sprites.
            SetSurfaceSpriteSet(spriteSet);

            // Set health to max.
            SetHealthToMax();
        }

        // SPRITES
        // Sets the surface sprites.
        public void SetSurfaceSpriteSet(int newSpriteSet)
        {
            // Sets the sprite set.
            spriteSet = Mathf.Clamp(newSpriteSet, 1, SURFACE_SPRITE_SET_COUNT);

            // The top, middle, and bottom sprites.
            Sprite topSprite = null;
            Sprite middleSprite = null;
            Sprite bottomSprite = null;

            // Checks the sprite set number to know...
            // Which one to use.
            switch(spriteSet)
            {
                default:
                case 1:
                    topSprite = topLayerSprite01;
                    middleSprite = middleLayerSprite01;
                    bottomSprite = bottomLayerSprite01;
                    break;

                case 2:
                    topSprite = topLayerSprite02;
                    middleSprite = middleLayerSprite02;
                    bottomSprite = bottomLayerSprite02;
                    break;

                case 3:
                    topSprite = topLayerSprite03;
                    middleSprite = middleLayerSprite03;
                    bottomSprite = bottomLayerSprite03;
                    break;
            }

            // If all of the sprites are set, change the layers.
            if(topSprite != null && middleSprite != null && bottomSprite != null)
            {
                topLayerRenderer.sprite = topSprite;
                middleLayerRenderer.sprite = middleSprite;
                bottomLayerRenderer.sprite = bottomSprite;
            }
        }


        // MECHANICS
        // Returns 'true' if health is at max.
        public bool IsHealthAtMax()
        {
            return health >= maxHealth;
        }


        // Set the health to the max.
        public void SetHealthToMax()
        {
            health = maxHealth;
        }

        // Sets the health of the stage surface (0 - max health).
        public void SetHealth(float newHealth)
        {
            // Saves the old health.
            float oldHealth = health;

            // Sets the health, and calls the related functions.
            health = Mathf.Clamp(newHealth, 0, maxHealth);

            // Calls related functions.
            OnHealthChanged();

            // The surface has been damaged.
            if (health < oldHealth)
            {
                // Play the damage animation.
                PlayDamageAnimation();

                // The surface has been damaged.
                stageManager.OnSurfaceDamaged();
            }

            // Check for death.
            CheckDeath();
        }

        // Adds health to the barrier.
        public void AddHealth(float heal)
        {
            SetHealth(health + heal);
        }

        // Applies damage to the surface.
        public void ReduceHealth(float damage)
        {
            SetHealth(health - damage);
        }

        // Called when the surface's health has changed.
        public virtual void OnHealthChanged()
        {
            stageManager.stageUI.UpdateSurfaceHealthBar();
        }

        // Checks if the surface is dead.
        public bool CheckDeath()
        {
            // If the surface health is 0 or less, kill it.
            if (health <= 0)
            {
                health = 0;
                KillSurface();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Kills the surface.
        public void KillSurface()
        {
            health = 0;
            OnSurfaceKilled();
        }

        // Called when the surface has been killed.
        protected void OnSurfaceKilled()
        {
            // TOOD: add animation.

            // The game is over.
            stageManager.OnStageLost();
        }

        // Restores the stage surface.
        public void RestoreSurface()
        {
            gameObject.SetActive(true); // TOOD: replace with animation.
            SetHealthToMax();
            stageManager.stageUI.UpdateSurfaceHealthBar();
        }


        // ANIMATIONS
        // Plays the damage animation.
        public void PlayDamageAnimation()
        {
            animator.Play(damageAnim);
        }

        // On the start of the damage animation.
        public void OnDamageAnimationStart()
        {
            // ...
        }

        // On the end of the damage animation.
        public void OnDamageAnimationEnd()
        {
            // Plays the empty animation so that the damage animation can play again.
            animator.Play(emptyAnim);
        }
    }

}