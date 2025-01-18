using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RM_MST
{
    // The loading screen.
    public class MST_LoadingScreen : MonoBehaviour
    {
        // The scene that is loaded when the loading screen starts.
        public string nextScene = "";

        // The loader for loading scenes asynchronously.
        public MST_AsyncSceneLoader asyncLoader;

        // Loads the next scene on Loading Screen - Opening End.
        [Tooltip("If true, the next scene is loaded at the end of the opening animation.")]
        public bool loadNextScene = true;

        // Loads the scene asynchronously if true.
        public bool loadSceneAsync = true;

        // CALLBACKS
        // A callback for the loading screen.
        public delegate void LoadingCallback();

        // Callback for the start of an animation.
        private LoadingCallback animStartCallback;

        // Callback for the end of an animation.
        private LoadingCallback animEndCallback;

        // A callback for the start of the loading screen opening.
        private LoadingCallback openingStartCallback;

        // A callback for the end of the loading screen opening.
        private LoadingCallback openingEndCallback;

        // A callback for the start of the loading screen closing.
        private LoadingCallback closingStartCallback;

        // A callback for the end of the loading screen closing.
        private LoadingCallback closingEndCallback;

        [Header("Animation")]
        // The animator for the loading screen.
        public Animator animator;

        // Gets set to 'true' when an animation is playing.
        private bool animPlaying = false;

        // Opening animation.
        public string openingAnim = "Loading Screen - Opening Animation";

        // Closing animation.
        public string closingAnim = "Loading Screen - Closing Animation";

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // If the animator is not set, set it.
            if (animator == null)
                animator = GetComponent<Animator>();

            // Gets the asynchronous loader.
            if(asyncLoader == null)
            {
                asyncLoader = GetComponent<MST_AsyncSceneLoader>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // ...
        }

        // Returns 'true' if the async loader is loading.
        public bool IsLoading
        {
            get
            {
                return asyncLoader.IsLoading;
            }
            
        }

        // CALLBACKS
        // Adds a callback for when an animation starts.
        public void OnAnimationStartAddCallback(LoadingCallback callback)
        {
            animStartCallback += callback;
        }

        // Removes a callback for when an animation starts.
        public void OnAnimationStartRemoveCallback(LoadingCallback callback)
        {
            animStartCallback -= callback;
        }

        // Adds a callback for when an animation ends.
        public void OnAnimationEndAddCallback(LoadingCallback callback)
        {
            animEndCallback += callback;
        }

        // Removes a callback for when an animation ends.
        public void OnAnimationEndRemoveCallback(LoadingCallback callback)
        {
            animEndCallback -= callback;
        }

        // Opening
        // Adds a callback for when the loading screen opening starts.
        public void OnLoadingScreenOpeningStartAddCallback(LoadingCallback callback)
        {
            openingStartCallback += callback;
        }

        // Removes a callback for when the loading screen opening starts.
        public void OnLoadingScreenOpeningStartRemoveCallback(LoadingCallback callback)
        {
            openingStartCallback -= callback;
        }

        // Adds a callback for when the loading screen opening ends.
        public void OnLoadingScreenOpeningEndAddCallback(LoadingCallback callback)
        {
            openingEndCallback += callback;
        }

        // Removes a callback for when the loading screen opening ends.
        public void OnLoadingScreenOpeningEndRemoveCallback(LoadingCallback callback)
        {
            openingEndCallback -= callback;
        }



        // Closing
        // Adds a callback for when the loading screen closing starts.
        public void OnLoadingScreenClosingStartAddCallback(LoadingCallback callback)
        {
            closingStartCallback += callback;
        }

        // Removes a callback for when the loading screen closing starts.
        public void OnLoadingScreenClosingStartRemoveCallback(LoadingCallback callback)
        {
            closingStartCallback -= callback;
        }

        // Adds a callback for when the loading screen closing ends.
        public void OnLoadingScreenClosingEndAddCallback(LoadingCallback callback)
        {
            closingEndCallback += callback;
        }

        // Removes a callback for when the loading screen closing ends.
        public void OnLoadingScreenClosingEndRemoveCallback(LoadingCallback callback)
        {
            closingEndCallback -= callback;
        }


        // ANIMATIONS 
        // Returns 'true' if the animation is playing.
        public bool IsAnimationPlaying()
        {
            return animPlaying;
        }

        // Called when an animation starts.
        protected virtual void OnAnimationStart()
        {
            animPlaying = true;

            // Trigger the callbacks.
            if (animStartCallback != null)
                animStartCallback();
        }

        // Called when an animation ends.
        protected virtual void OnAnimationEnd()
        {
            animPlaying = false;

            // Trigger the callbacks.
            if (animEndCallback != null)
                animEndCallback();
        }

        // Plays the loading screen opening animation.

        public void PlayLoadingScreenOpeningAnimation()
        {
            animator.Play(openingAnim);
        }

        // Loading Screen - Opening Start
        public void OnLoadingScreenOpeningStart()
        {
            OnAnimationStart();

            // Trigger the callbacks.
            if (openingStartCallback != null)
                openingStartCallback();
        }

        // Loading Screen - Opening End
        public void OnLoadingScreenOpeningEnd()
        {
            OnAnimationEnd();

            // Trigger the callbacks.
            if (openingEndCallback != null)
                openingEndCallback();

            // If the next scene should be loaded, load it.
            if (loadNextScene)
            {
                // The next scene is set.
                if(nextScene != string.Empty)
                {
                    // Checks if the scene should be loaded asynchronously or not.
                    if (loadSceneAsync) // Async
                    {
                        asyncLoader.LoadScene(nextScene);
                    }
                    else // Sync
                    {
                        SceneManager.LoadScene(nextScene);
                    }
                }
                
            }
        }

        // Plays the loading screen closing animation.

        public void PlayLoadingScreenClosingAnimation()
        {
            animator.Play(closingAnim);
        }

        // Loading Screen - Closing Start
        public void OnLoadingScreenClosingStart()
        {
            OnAnimationStart();

            // Trigger the callbacks.
            if (closingStartCallback != null)
                closingStartCallback();
        }

        // Loading Screen - Closing End
        public void OnLoadingScreenClosingEnd()
        {
            OnAnimationEnd();

            // Trigger the callbacks.
            if (closingEndCallback != null)
                closingEndCallback();
        }
    }
}