

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RM_MST
{
    // The game settings UI.
    public class GameSettingsUI : MonoBehaviour
    {
        // TODO: remove text components and have the text objects have scripts attached to them to auto translate...
        // Since that text doesn't change.

        // the game settings object.
        public GameSettings gameSettings;

        [Header("Volume")]

        // BGM
        // The bgm volume slider.
        public Slider bgmVolumeSlider;

        // SFX
        // The sound effect slider.
        public Slider sfxVolumeSlider;

        // TTS
        // The tts volume slider.
        public Slider ttsVolumeSlider;


        [Header("Misc")]
        // the toggle for the mute toggle.
        public Toggle muteToggle;

        // the toggle for the text-to-speech toggle.
        public Toggle textToSpeechToggle;

        // the tutorial toggle.
        public Toggle tutorialToggle;

        // the full-screen toggle
        public Toggle fullScreenToggle;

        // Start is called before the first frame update
        void Start()
        {
            // Save the instance.
            gameSettings = GameSettings.Instance;

            // Current bgm volume setting.
            bgmVolumeSlider.value = gameSettings.BgmVolume;

            // // Listener for the bgm slider.
            // bgmVolumeSlider.onValueChanged.AddListener(delegate
            // {
            //     OnBgmVolumeChange(bgmVolumeSlider);
            // });

            // Current sfx volume setting.
            sfxVolumeSlider.value = gameSettings.SfxVolume;

            // // Listener for the sfx slider.
            // sfxVolumeSlider.onValueChanged.AddListener(delegate
            // {
            //     OnSfxVolumeChange(sfxVolumeSlider);
            // });

            // Current tts volume setting.
            ttsVolumeSlider.value = gameSettings.TtsVolume;

            // Current muted setting.
            muteToggle.isOn = gameSettings.Mute;

            // Current text-to-speech tutorial.
            textToSpeechToggle.isOn = gameSettings.UseTextToSpeech;

            // // Listener for the text-to-speech.
            // textToSpeechToggle.onValueChanged.AddListener(delegate
            // {
            //     OnTextToSpeechChange(textToSpeechToggle);
            // });

            // Current tutorial toggle setting.
            tutorialToggle.isOn = gameSettings.UseTutorial;

            // // Listener for the tutorial toggle.
            // tutorialToggle.onValueChanged.AddListener(delegate
            // {
            //     OnTutorialChange(tutorialToggle);
            // });


            // If the SDK isn't initialized, some functions may be unavailable.
            // NOTE: the 'interactable' component of the tutorial toggle isn't changed because...
            // The toggle can only be interacted with on the title screen. It's non-interactable otherwise.


            // If the game is running in WebGL, disable the full-screen toggle, as this is handled by Itch.io.
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Disable the full-screen toggle.
                fullScreenToggle.interactable = false;
            }
            else
            {
                // Set the full screen toggle to be interactable.
                fullScreenToggle.interactable = true;
            }

            // Set the toggle based on if full-screen is being used or not.
            fullScreenToggle.isOn = gameSettings.UseFullScreen;


            // These functions are disabled here.
            if (SystemManager.IsLOLSDKInitialized())
            {
                // Hides the tutorial toggle since it shoudn't be usable.
                tutorialToggle.gameObject.SetActive(false);

                // Enable the TTS volume slider and TTS toggle.
                ttsVolumeSlider.interactable = true;
                textToSpeechToggle.interactable = true;
            }
            else
            {
                // Turn on the tutorial toggle since it should be accessible.
                tutorialToggle.gameObject.SetActive(true);

                // Disable the TTS volume slider and toggle.
                ttsVolumeSlider.interactable = false;
                textToSpeechToggle.interactable = false;
            }

        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            gameSettings = GameSettings.Instance;
        }

        // On the mute changes.
        public void OnMuteChange(Toggle toggle)
        {
            gameSettings.Mute = toggle.isOn;
        }

        // On the text-to-speech changes.
        public void OnTextToSpeechChange(Toggle toggle)
        {
            gameSettings.UseTextToSpeech = toggle.isOn;

            // Stops the text-to-speech if it was just turned off.
            if (SystemManager.IsLOLSDKInitialized() && !gameSettings.UseTextToSpeech)
                SystemManager.Instance.textToSpeech.StopSpeakText();
        }

        // On the tutorial changes.
        public void OnTutorialChange(Toggle toggle)
        {
            gameSettings.UseTutorial = toggle.isOn;
        }

        // On the bgm volume change.
        public void OnBgmVolumeChange(Slider slider)
        {
            gameSettings.BgmVolume = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
        }

        // On the sfx volume change.
        public void OnSfxVolumeChange(Slider slider)
        {
            gameSettings.SfxVolume = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
        }

        // On the tts volume change.
        public void OnTtsVolumeChange(Slider slider)
        {
            gameSettings.TtsVolume = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
        }

        // On the full-screen toggle.
        public void OnFullScreenChange(Toggle toggle)
        {
            gameSettings.UseFullScreen = toggle.isOn;
        }

    }
}