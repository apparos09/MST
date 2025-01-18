using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The world audio.
    public class WorldAudio : MST_GameAudio
    {
        // The singleton instance.
        private static WorldAudio instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        [Header("World")]
        // Manager
        public WorldManager manager;


        [Header("World/BGMs")]
        // The world background music.
        public AudioClip worldBgm;

        // The game results BGM.
        public AudioClip gameResultsBgm;

        [Header("World/JNGs")]

        // The game complete jingle.
        public AudioClip gameCompleteJng;

        // Constructor
        private WorldAudio()
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
                manager = WorldManager.Instance;

            // If nothing is set, and the world bgm is set, play it.
            if(bgmSource.clip == null && worldBgm != null)
            {
                PlayBackgroundMusic(worldBgm);
            }
        }

        // Gets the instance.
        public static WorldAudio Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<WorldAudio>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("WorldAudio (singleton)");
                        instance = go.AddComponent<WorldAudio>();
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

        // Plays the world background music.
        public void PlayWorldBgm()
        {
            PlayBackgroundMusic(worldBgm);
        }

        // Play the game complete music. It plays a jingle, then plays the results music.
        // The provided BGM delay is the gap between the jingle and the resuls music.
        public void PlayGameCompleteMusic(float bgmDelay)
        {
            float totalDelay = gameCompleteJng.length + bgmDelay;
            PlayBackgroundMusic(gameResultsBgm, totalDelay);
            PlayBackgroundMusicOneShot(gameCompleteJng, false);
        }

        // Plays the game complete music with a preset delay.
        public void PlayGameCompleteMusic()
        {
            // Starts the BGM 1 second after the jingle is done.
            PlayGameCompleteMusic(1.0F);
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