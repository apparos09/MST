using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The audio for the title screen.
    public class TitleAudio : MST_GameAudio
    {
        // The singleton instance.
        private static TitleAudio instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        [Header("Title")]
        // Manager
        public TitleManager manager;

        // The title bgm.
        public AudioClip titleBgm;

        // Constructor
        private TitleAudio()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected virtual void Awake()
        {
            // If the instance hasn't been set, set it to this object.
            if (instance == null)
            {
                instance = this;
            }
            // If the instance isn't this, destroy the game object.
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Run code for initialization.
            if (!instanced)
            {
                instanced = true;
            }
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            if (manager == null)
                manager = TitleManager.Instance;

            // If the clip isn't set, and the title BGM has been set, play that music.
            if (bgmSource.clip == null && titleBgm != null)
            {
                PlayBackgroundMusic(titleBgm);
            }
        }

        // Gets the instance.
        public static TitleAudio Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<TitleAudio>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("TitleAudio (singleton)");
                        instance = go.AddComponent<TitleAudio>();
                    }

                }

                // Return the instance.
                return instance;
            }
        }

        // Returns 'true' if the object has been instanced.
        public static bool Instantiated
        {
            get
            {
                return instanced;
            }
        }

        // This function is called when the MonoBehaviour will be destroyed.
        private void OnDestroy()
        {
            // If the saved instance is being deleted, set 'instanced' to false.
            if (instance == this)
            {
                instanced = false;
            }
        }

    }
}