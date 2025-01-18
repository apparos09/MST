
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

namespace RM_MST
{
    // A page for the MST namepsace.
    public class MST_Page : Page
    {
        // The language key for the page.
        public string languageKey = string.Empty;

        // The speak key for the page.
        public string speakKey = string.Empty;

        // Adds a page.
        public MST_Page() : base()
        {
            // Adds the speak text callback.
            AddSpeakTextCallback();
        }

        // Adds a page with text.
        public MST_Page(string text) : base(text)
        {
            // Adds the speak text callback.
            AddSpeakTextCallback();
        }

        // Adds a page with text and a language/speak key.
        public MST_Page(string text, string languageKey) : base(text)
        {
            // Sets the language key and translates the text.
            SetLanguageText(languageKey, true);

            // Adds the speak text callback.
            AddSpeakTextCallback();
        }

        // Adds a page with text, a langauge key, and a speak key.
        public MST_Page(string text, string languageKey, string speakKey) : base(text)
        {
            // Sets the language key and translates the text.
            // Also sets the speak key, which may be different than the language key.
            SetLanguageTextAndSpeakKey(languageKey, speakKey);

            // Adds the speak text callback.
            AddSpeakTextCallback();
        }

        // Translates the text using the language key.
        public void SetLanguageText()
        {
            // If the LOL Manager is instantiated...
            if(SystemManager.IsInstantiatedAndIsLOLSDKInitialized())
            {
                string newText = SystemManager.Instance.GetLanguageText(languageKey);
                text = newText;
            }
        }

        // Sets the language key and translates it.
        public void SetLanguageText(string newLangKey)
        {
            languageKey = newLangKey;
            SetLanguageText();
        }

        // Sets the language key and translates it.
        // If setSpeakKey is true, the speak key is also set.
        public void SetLanguageText(string newLangKey, bool setSpeakKey)
        {
            // Sets the language key.
            SetLanguageText(newLangKey);

            // If this is also the speak key, set it as the speak key.
            if(setSpeakKey)
            {
                SetSpeakKey(newLangKey);
            }
        }

        // Sets the new speak key.
        public void SetSpeakKey(string newSpeakKey)
        {
            speakKey = newSpeakKey;
        }

        public void SetLanguageTextAndSpeakKey(string newLangKey, string newSpeakKey)
        {
            SetLanguageText(newLangKey);
            SetSpeakKey(newSpeakKey);
        }

        // Speaks the text for the tutorial page.
        public void SpeakText()
        {
            // If there is no speak key, do nothing.
            // if(speakKey == string.Empty)
            // {
            //     return;
            // }

            // If the LOL SDK is initialized, and TTS is on.
            if (SystemManager.IsInstantiatedAndIsLOLSDKInitialized() && GameSettings.Instance.UseTextToSpeech)
            {
                SystemManager.Instance.SpeakText(speakKey);
            }
        }

        // Adds the speak text callback.
        public void AddSpeakTextCallback()
        {
            OnPageOpenedAddCallback(SpeakText);
        }

        // Removes the speak text callback.
        public void RemoveSpeakTextCallback()
        {
            OnPageOpenedRemoveCallback(SpeakText);
        }

    }
}