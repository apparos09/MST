
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using util;

// the settings for the game, which is created in the init scene.
// the init isn't visited again, so this object will not be recreated.
namespace RM_MST
{
    public class GameSettings : MonoBehaviour
    {
        // the instance of the game settings.
        private static GameSettings instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // If this is the lol build, set it to 'true'. If it's the promo build, set this to false.
        public const bool IS_LOL_BUILD = true;

        [Header("Settings")]

        // TODO: enable by default for submission build.
        // Use the text-to-speech options.
        private bool useTTS = true;

        // TODO: enable by default for submission build.
        // Use the tutorial for the game.
        // This is only relevant when starting up the game scene.
        private bool useTutorial = false; // Off by default to help with game scene testing.

        // The global volume, which is used for muting/unmuting the entire game.
        // [HideInInspector] // Not needed since this is a private variable.
        private float globalVolume = 1.0F;

        // Audio Tags
        // The tag for BGM objects.
        public const string BGM_TAG = "BGM";

        // The volume for the background music.
        private float bgmVolume = 0.25F;

        // The tag for the SFX objects.
        public const string SFX_TAG = "SFX";

        // The volume for the sound effects.
        private float sfxVolume = 0.40F;

        // The tag for the voice objects.
        public const string VCE_TAG = "VCE";

        // The volume for the voices.
        private float vceVolume = 1.0F;

        // The audio for the TTS.
        public const string TTS_TAG = "TTS";

        // The volume for the TTS.
        private float ttsVolume = 1.0F;

        // The gameplay mode. This is used to set the game mode at the start of the game.
        [HideInInspector]
        public GameplayManager.gameMode gameMode = GameplayManager.gameMode.focus;

        // If 'true', the player can select the mode. If false, the game is forced into focus mode.
        [HideInInspector]        
        public bool allowPlayerSelectMode = true;

        // the constructor.
        private GameSettings()
        {
        }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Checks for the instance.
            if (instance == null) // Set to nothing.
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

                // This object should not be destroyed.
                DontDestroyOnLoad(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // ...
        }

        // Returns the instance of the game settings.
        public static GameSettings Instance
        {
            get
            {
                // Checks to see if the instance exists. If it doesn't, generate an object.
                if (instance == null)
                {
                    instance = FindObjectOfType<GameSettings>(true);

                    // Generate new instance if an existing instance was not found.
                    if (instance == null)
                    {
                        // makes a new settings object.
                        GameObject go = new GameObject("(Singleton) Settings");

                        // adds the instance component to the new object.
                        instance = go.AddComponent<GameSettings>();
                    }

                }

                // returns the instance.
                return instance;
            }
        }

        // OnEnable
        private void OnEnable()
        {
            // This is called if the object is enabled when the program starts running.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // OnDisable
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        // Returns 'true' if the object has been initialized.
        public static bool Instantiated
        {
            get
            {
                return instanced;
            }
        }


        // the LOL SDK has been initialized.
        public static bool InitializedLOLSDK
        {
            get
            {
                return SystemManager.IsLOLSDKInitialized();
            }
        }

        // Is text-to-speech being used?
        public bool UseTextToSpeech
        {
            get
            {
                return useTTS;
            }

            set
            {
                useTTS = value;
            }
        }

        // If the tutorial should be used.
        public bool UseTutorial
        {
            get { return useTutorial; }

            set
            {
                useTutorial = value;
            }
        }

        // Is the audio muted?
        public bool Mute
        {
            get
            {
                // Checks if the audio is muted.
                // return AudioListener.pause; // Old

                // New
                // Checks if the global volume is less than or equal to 0.
                // If it is, then the game is muted.
                if (AudioListener.volume <= 0.0F)
                {
                    return true;
                }
                else
                {
                    // Updates the global volume to make sure it matches the set volume.
                    globalVolume = AudioListener.volume;
                    return false;
                }
            }

            set
            {
                // NOTE: using AudioListener.pause still stores the audio play calls.
                // When the AudioListener is unpaused, those stored sound effects end up playing.
                // As such, global volume is used instead of AudioListener.pause.

                // Mutes/unmutes all audio.
                // Old: uses AudioListener.pause.
                // AudioListener.pause = value;

                // New: uses AudioListener.volume.
                if (value) // Mute the game.
                {
                    // Gets the AudioListener's volume, and sets it to 0.
                    globalVolume = AudioListener.volume;
                    AudioListener.volume = 0.0F;
                }
                else // Unmute the game.
                {
                    // If the global volume is 0, set it to 1.
                    if (globalVolume <= 0.0F)
                        globalVolume = 1.0F;

                    // Sets the audio listener's volume to the global volume.
                    AudioListener.volume = globalVolume;
                }
            }
        }

        // Setting the background volume.
        public float BgmVolume
        {
            get
            {
                return bgmVolume;
            }

            set
            {
                // Adjusts all audio levels (value is clamped in function).
                // AdjustAllAudioLevels(value, sfxVolume, ttsVolume);

                // Adjusts the BGM volume and adjusts all the audio objects with the BGM tag.
                bgmVolume = Mathf.Clamp01(value);
                AdjustBgmAudioLevels();
            }
        }

        // Setting the sound effect volume.
        public float SfxVolume
        {
            get
            {
                return sfxVolume;
            }

            set
            {
                // Adjusts all audio levels (value is clamped in function).
                // AdjustAllAudioLevels(bgmVolume, value, ttsVolume);

                // Adjusts the SFX volume and adjusts all the audio objects with the SFX tag.
                sfxVolume = Mathf.Clamp01(value);
                AdjustSfxAudioLevels();
            }
        }

        // Setting the text-to-speech volume.
        public float TtsVolume
        {
            get
            {
                return ttsVolume;
            }

            set
            {
                // Adjusts all audio levels 
                // AdjustAllAudioLevels(bgmVolume, sfxVolume, value);

                // Adjusts the TTS volume and adjusts all the audio objects with the TTS tag.
                ttsVolume = Mathf.Clamp01(value);
                AdjustTtsAudioLevels();
            }
        }

        // adjusts the audio source that's supplied through this function.
        // for this to work, it needs to have a usable tag and set source audio object.
        public void AdjustAudio(AudioSourceControl audio)
        {
            // checks which tag to use.
            if (audio.CompareTag(BGM_TAG)) // BGM
            {
                audio.audioSource.volume = audio.MaxVolume * bgmVolume;
            }
            else if (audio.CompareTag(SFX_TAG)) // SFX
            {
                audio.audioSource.volume = audio.MaxVolume * sfxVolume;
            }
            else if (audio.CompareTag(VCE_TAG)) // VCE
            {
                audio.audioSource.volume = audio.MaxVolume * vceVolume;
            }
            else if (audio.CompareTag(TTS_TAG)) // TTS (Text-To-Speech);
            {
                audio.audioSource.volume = audio.MaxVolume * ttsVolume;
            }
            else // no recognizable tag.
            {
                Debug.LogAssertion("No recognizable audio tag has been set, so the audio can't be adjusted.");
            }
        }


        // applies the audio levels by using the saved audio settings.
        public void AdjustAllAudioLevels()
        {
            AdjustAllAudioLevels(bgmVolume, sfxVolume, ttsVolume);
        }

        // Adjusts all the audio levels.
        // TODO: create a function for only adjusting one of the audio parameters, instead of all of them at once.
        public void AdjustAllAudioLevels(float newBgmVolume, float newSfxVolume, float newTtsVolume)
        {
            // Finds all the audio source controls.
            AudioSourceControl[] audios = FindObjectsOfType<AudioSourceControl>();

            // Saves the bgm, sfx, and tts volume objects.
            bgmVolume = Mathf.Clamp01(newBgmVolume);
            sfxVolume = Mathf.Clamp01(newSfxVolume);
            ttsVolume = Mathf.Clamp01(newTtsVolume);

            // Goes through each source.
            foreach (AudioSourceControl audio in audios)
            {
                // Adjusts the audio.
                AdjustAudio(audio);
            }

            // TODO: what was the point of this?
            // // changing the values.
            // bgmVolume = newBgmVolume;
            // sfxVolume = newSfxVolume;
            // ttsVolume = newTtsVolume;

        }

        // Adjust all audio levels with the provided tag.
        private void AdjustAllAudioLevelsWithTag(string audioTag)
        {
            // Finds objects with the right tag.
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(audioTag);

            // Goes through the tagged objects.
            foreach (GameObject tagged in taggedObjects)
            {
                // The audio control object.
                AudioSourceControl asc;

                // Tries to get the audio control object.
                if (tagged.TryGetComponent(out asc))
                {
                    // Adjusts the audio level.
                    AdjustAudio(asc);
                }
            }
        }

        // Adjust all BGM audio levels.
        public void AdjustBgmAudioLevels()
        {
            AdjustAllAudioLevelsWithTag(BGM_TAG);
        }

        // Adjusts all BGM audio levels with new volume level.
        public void AdjustBgmAudioLevels(float newVolume)
        {
            // Don't use the shorthand since it calls a function to adjust the audio levels anyway.
            bgmVolume = Mathf.Clamp01(newVolume);
            AdjustBgmAudioLevels();
        }

        // Adjust all SFX audio levels.
        public void AdjustSfxAudioLevels()
        {
            AdjustAllAudioLevelsWithTag(SFX_TAG);
        }

        // Adjusts all SFX audio levels with new volume level.
        public void AdjustSfxAudioLevels(float newVolume)
        {
            // Don't use the shorthand since it calls a function to adjust the audio levels anyway.
            sfxVolume = Mathf.Clamp01(newVolume);
            AdjustSfxAudioLevels();
        }

        // Adjust all TTS audio levels.
        public void AdjustTtsAudioLevels()
        {
            AdjustAllAudioLevelsWithTag(TTS_TAG);
        }

        // Adjusts all TTS audio levels with new volume level.
        public void AdjustTtsAudioLevels(float newVolume)
        {
            // Don't use the shorthand since it calls a function to adjust the audio levels anyway.
            ttsVolume = Mathf.Clamp01(newVolume);
            AdjustTtsAudioLevels();
        }

        // Called when the scene was loaded.
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Adjusts all the audio levels.
            AdjustAllAudioLevels();

            // Refreshes the game mute, since this caused problems before.
            Mute = Mute;
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