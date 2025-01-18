using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

namespace RM_MST
{
    // Game audio for the MST project.
    public class MST_GameAudio : GameAudio
    {
        // If 'true', the audio sources for the UI are automatically set.
        private bool autoSetUIAudio = true;

        // Used to trigger 'late start'.
        private bool calledLateStart = false;


        [Header("MST")]

        // The button (UI) SFX.
        public AudioClip buttonUISfx;

        // The slider (UI) SFX.
        public AudioClip sliderUISfx;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // Makes sure all button, slider, and toggles are set.
            // Only replaces the audio if the values are null.
            // Take out this autoset if you're going to manually set things.
            if(autoSetUIAudio)
            {
                // Finds all the ui element audio objects.
                UIElementAudio[] uIElementAudios = FindObjectsOfType<UIElementAudio>(true);

                // Goes through the list,and sets the audio source.
                foreach (UIElementAudio uiElementAudio in uIElementAudios)
                {
                    // If the audio source isn't set, set it.
                    if (uiElementAudio.audioSource == null)
                        uiElementAudio.audioSource = sfxUISource;
                }
            }

            // This is now done in late start to make sure everything has been set properly...
            // Before this is called.
            // Makes sure the audio is adjusted to the current settings.
            // GameSettings.Instance.AdjustAllAudioLevels();
        }

        // Called on the first update frame, after the Start function.
        protected virtual void LateStart()
        {
            calledLateStart = true;

            // Adjusts the audio levels once again to make sure everything is set properly.
            // Some scenes were not having their audio set properly when testing in the LOL harness.
            GameSettings.Instance.AdjustAllAudioLevels();
        }

        // Plays the button menu SFX.
        public void PlayButtonUISfx()
        {
            PlaySoundEffectUI(buttonUISfx);
        }

        // Plays the slider menu SFX.
        public void PlaySliderUISfx()
        {
            PlaySoundEffectUI(sliderUISfx);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Calls late start.
            if(!calledLateStart)
            {
                LateStart();
            }
        }
    }
}