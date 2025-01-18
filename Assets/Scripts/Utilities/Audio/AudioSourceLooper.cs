using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resources:
// * https://docs.unity3d.com/ScriptReference/AudioSource-time.html

namespace util
{
    // Loops a section of the audio.
    // NOTE: if the audio is compressed or changed in some similar way this script may not work properly.
    public class AudioSourceLooper : MonoBehaviour
    {
        // audio source
        public AudioSource audioSource = null;

        // Start and end of the audio clip being played. This is in seconds.
        // NOTE: be aware that the time of the audio may not be accurate if the audio is compressed.
        // As such, it may be best not to use this.

        // The start of the clip. If this value is negative, the clip continues like normal.
        // If clip start is less than 0, then it doesn't function.
        [Tooltip("The start of the clip in seconds.")]
        public float clipStart = 0.0F;

        // The end of the clip
        // TODO: find out what happen if time is set greater than the length of an audio file.
        /// <summary>
        /// * I don't know what happens if the time is set beyond the clip length but I assume it just errors out.
        /// * on another note that if clipEnd is set to the end of the clip, the song will loop back to the start...
        /// * instead of looping back to the pre-defined clip start. 
        /// * so it's best to either have some silence, or continue the file a little longer so that the loop...
        /// * has time to work properly.
        /// </summary>
        [Tooltip("The end of the clip in seconds.")]
        public float clipEnd = 0.0F;

        // If 'true', a song will be limited to the clip range.
        // If 'false', the start of the song will play normally,...
        // But once within the clip range it will stay within the clip.
        [Tooltip("If true, the audio starts at clipStart instead of at the start of the audio clip itself when PlayAudio() is called.")]
        public bool playAtClipStart = false;


        // Adjusts the loop point dynamically based on where the audio is.
        // e.g., if the audio is 1 second past the endpoint, it loops back to 1 second past the clip start point.
        [Tooltip("If true, clipStart is offset by where the audio is in reference to clipEnd when a loop is being performed.")]
        public bool loopRelative = true;

        // If 'true', the clip loop is applied. If false, this script does not apply the clip loop.
        [Tooltip("Enables the clip loop if true. It's recommended that this is set to false if the audio clip file loops perfectly.")]
        public bool clipLoopEnabled = true;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // If the audio source is not set, try to grab it from the game object.
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            // gets the start and end of the clip if not set.
            if (audioSource != null)
            {
                // If the clip start and end are both set to zero (i.e. they weren't set)
                // Clip Start - avoid negative value.
                if (clipStart < 0.0F)
                    clipStart = 0.0F;

                // Clip End - avoid negative value.
                if (clipEnd < 0.0F)
                    clipEnd = 0.0F;

                // If the audio source is set.
                if(audioSource.clip != null)
                {
                    // If the clip end is less than 0, equal to 0, or is greater than the clip length...
                    // Set it to the clip length.
                    if (clipEnd <= 0.0F || clipEnd > audioSource.clip.length)
                        clipEnd = audioSource.clip.length;

                    // NOTE: this throws an error if clipStart is greater than the length of the audio.
                    // Starts at clipStart instead of at the start of the audio itself.
                    if (playAtClipStart)
                    {
                        // If the clipStart is less than the length of the clip, play at clipStart.
                        if (clipStart < audioSource.clip.length)
                            audioSource.time = clipStart;
                    }
                }                
                    
            }
        }

        // Plays the audio - if limited to the clip start, it starts from the loop point.
        public void PlayAudio(bool resetAudio, float delay)
        {
            // Audio source or audio clip doesn't exist.
            if (audioSource == null || audioSource.clip == null)
                return;

            // Stops audio if it's currently playing.
            audioSource.Stop();

            // If the audio should be reset.
            if (resetAudio)
            {
                // If the audio should start at the clip start when first played.
                if (playAtClipStart && clipStart >= 0.0F && clipStart < audioSource.clip.length)
                {
                    audioSource.time = clipStart;
                }
                else // Start source at the start of the audio.
                {
                    audioSource.time = 0.0F;
                }
            }

            // Plays the audio. If there is a delay, use play delayed.
            if(delay > 0)
            {
                audioSource.PlayDelayed(delay);
            }
            else
            {
                audioSource.Play();
            }
            
        }

        // Plays the audio with no delay.
        public void PlayAudio(bool resetAudio)
        {
            PlayAudio(resetAudio, 0);
        }

        // Stops the audio
        // If 'resetAudio' is true, the audio is set back to its start.
        public void StopAudio(bool resetAudio)
        {
            // Audio source or audio clip doesn't exist.
            if (audioSource == null || audioSource.clip == null)
                return;

            // Stops the audio source.
            audioSource.Stop();

            // If the audio should be reset.
            if(resetAudio)
            {
                // Bring audio to clip start.
                if (playAtClipStart && clipStart >= 0.0F && clipStart < audioSource.clip.length)
                {
                    audioSource.time = clipStart;
                }
                else // Bring audio to start of the song.
                {
                    audioSource.time = 0.0F;
                }
            }
            
        }

        // If the audio is set to loop
        public bool GetLooping()
        {
            if (audioSource != null)
                return audioSource.loop;
            else
                return false;
        }

        // Sets the audio to loop
        public void SetLooping(bool looping)
        {
            if (audioSource != null)
                audioSource.loop = looping;
        }

        // Returns the length of the audio clip
        public float GetClipLength()
        {
            return clipEnd - clipStart;
        }

        // Returns the value of clip start
        public float GetClipStart()
        {
            return clipStart;
        }

        // Sets the value of clip start in seconds
        // This only works if the audioClip has been set.
        public void SetClipStartInSeconds(float seconds)
        {
            // If the audio source is null.
            if (audioSource == null)
                return;

            // If the audio clip is null.
            if (audioSource.clip == null)
                return;


            // Setting value to clip start.
            clipStart = (seconds >= 0.0F && seconds <= audioSource.clip.length) ?
                seconds : clipStart;
        }

        // Sets the clip start time as a percentage, with 0 being 0% and 1 being 100%.
        public void SetClipStartAsPercentage(float t)
        {
            // If the audio source is null.
            if (audioSource == null)
                return;

            // If the audio clip is null.
            if (audioSource.clip == null)
                return;

            t = Mathf.Clamp(t, 0.0F, 1.0F);
            clipStart = Mathf.Lerp(0.0F, audioSource.clip.length, t);
        }

        // Returns the value of clip end
        public float GetClipEnd()
        {
            return clipEnd;
        }

        // Sets the value of clip end in seconds
        // This only works if the audioClip has been set.
        public void SetClipEndInSeconds(float seconds)
        {
            // If the audio source is null.
            if (audioSource == null)
                return;

            // If the audio clip is null.
            if (audioSource.clip == null)
                return;


            // Setting value to clip start.
            clipEnd = (seconds >= 0.0F && seconds <= audioSource.clip.length) ?
                seconds : clipEnd;
        }

        // Sets the clip end time as a percentage, with 0 being 0% and 1 being 100%.
        public void SetClipEndAsPercentage(float t)
        {
            // If the audio source is null.
            if (audioSource == null)
                return;

            // If the audio clip is null.
            if (audioSource.clip == null)
                return;

            t = Mathf.Clamp(t, 0.0F, 1.0F);
            clipEnd = Mathf.Lerp(0.0F, audioSource.clip.length, t);
        }

        // Gets the variable that says whether or not to start the song at the start of the clip.
        public bool GetPlayAtClipStart()
        {
            return playAtClipStart;
        }

        // Sets the play at clip start
        public void SetPlayAtClipStart(bool pacs)
        {
            playAtClipStart = pacs;

            // If the audio source is null.
            if (audioSource == null)
                return;

            // If the audio clip is null.
            if (audioSource.clip == null)
                return;

            // If the audio should play at the start of the clip.
            if (playAtClipStart)
            {
                // If the start of the clip is greater than the current time of the clip...
                // The audioSouce is set to the start of the clip.
                if (clipStart > audioSource.time)
                    audioSource.time = clipStart;
            }
        }

        // TODO: check how well this works with compressed audio.
        // Sets the current time in clip as a percentage of the whole c
        public void SetClipTime(float t)
        {
            // Sets the clip time
            audioSource.time = Mathf.Clamp(t, 0, audioSource.clip.length);
        }

        // Sets the clip time as a percentage. Argument 'percent' ranges from 0 to 1.
        public void SetClipTimeAsPercentage(float percent)
        {
            // Sets the clip time
            audioSource.time = audioSource.clip.length * Mathf.Clamp01(percent);
        }

        // Called to loop the clip back to its start.
        protected virtual void OnLoopClip()
        {
            // Checks to see if the audio is looping
            switch (audioSource.loop)
            {
                case true: // Audio is looping
                    // Checks if clipStart should be offset relative to how far past clipEnd the audio currently is.
                    if(loopRelative) // Offset clip start.
                    {
                        // Calculates how much clipStart should be offset by.
                        float offsetStart = audioSource.time - clipEnd;

                        // Set current clip start as clipStart adjusted by the offset amount.
                        float currClipStart = clipStart + offsetStart;


                        // If the current clip start is negative (i.e., it's before the start of the audio itself)...
                        // Then use normal clipStart.
                        if(currClipStart >= 0)
                            audioSource.time = currClipStart;
                        else
                            audioSource.time = clipStart;
                    }
                    else // Set back to clipStart.
                    {
                        audioSource.time = clipStart;
                    }
                    break;

                case false: // Audio is not looping
                            
                    // Audio is stopped, and returns to clip start.
                    audioSource.Stop();
                    audioSource.time = clipStart;
                    break;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Audio source, or the clip are not set.
            if (audioSource == null)
                return;
            if (audioSource.clip == null)
                return;

            // Clamp clipStart and clipEnd
            // TODO: see if using if statements is less computationally expensive (2 conditional statements per clamp)
            clipStart = Mathf.Clamp(clipStart, 0.0F, audioSource.clip.length);
            clipEnd = Mathf.Clamp(clipEnd, 0.0F, audioSource.clip.length);

            // if the clips are the same, no audio can play.
            if (clipStart == clipEnd)
            {
                return;
            }
            // If the clip end is greater than the clip start, then the values are swapped.
            else if (clipStart > clipEnd)
            {
                float temp = clipStart;
                clipStart = clipEnd;
                clipEnd = temp;
            }

            // If the clip loop is enabled, and the audio source is playing.
            if (clipLoopEnabled && audioSource.isPlaying)
            {
                // This isn't needed since using the Play() function in this class handles this.
                // Puts the audio source at the clip start.
                // if (audioSource.time < clipStart && !playAtClipStart)
                //     audioSource.time = clipStart;

                // The audioSource has reached the end of the clip.
                if (audioSource.time >= clipEnd)
                {
                    // Call to loop the clip.
                    OnLoopClip();
                }
            }
        }
    }
}