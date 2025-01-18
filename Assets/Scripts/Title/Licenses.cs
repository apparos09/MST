using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The licenses for the game.
    public class Licenses : MonoBehaviour
    {
        // The credits interface.
        public AudioCreditsInterface creditsInterface;

        // The BGM audio credits for the game.
        public AudioCredits bgmCredits;

        // The BGM credits page.
        public int bgmCreditsIndex = 0;

        // The SFX audio credits for the game.
        public AudioCredits sfxCredits;

        // The sound effects credit page.
        public int sfxCreditsIndex = 0;

        // The font credits for the game.
        public AudioCredits fontsCredits;

        // The fonts credit page.
        public int fontsCreditsIndex = 0;

        [Header("UI")]

        // BGM Button
        public Button bgmButton;

        // SFX Button
        public Button sfxButton;

        // Fonts Button
        public Button fontsButton;

        // Start is called before the first frame update
        void Start()
        {
            // Loads all the credits.
            LoadBackgroundMusicCredits();
            LoadSoundEffectCredits();
            LoadFontCredits();

            // Sets this if it has not been set already.
            if (creditsInterface == null)
                creditsInterface = GetComponent<AudioCreditsInterface>();

            // Show the background credits.
            ShowBackgroundMusicCredits();
        }

        // Loads the BGM credits.
        private void LoadBackgroundMusicCredits()
        {
            AudioCredits.AudioCredit credit;

            // Title
            credit = new AudioCredits.AudioCredit();
            credit.title = "Space Dominator Squadron";
            credit.artist = "Antti Luode";
            credit.collection = "GameSounds.xyz/Anttis instrumentals/Songs";
            credit.source = "GameSounds.xyz";
            credit.link1 = "https://gamesounds.xyz/?dir=Anttis%20instrumentals/Songs";
            credit.link2 = "";
            credit.copyright = 
                "\"Space Dominator Squadron\"" +
                "\nAntti Luode" +
                "\nLicensed under Creative Commons: By Attribution 3.0" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            bgmCredits.audioCredits.Add(credit);


            // World / Stage Select
            credit = new AudioCredits.AudioCredit();
            credit.title = "Galactic Rap";
            credit.artist = "Kevin MacLeod";
            credit.collection = "Incompetech.com";
            credit.source = "Incompetech.com";
            credit.link1 = "https://incompetech.com/music/royalty-free/music.html";
            credit.link2 = "";
            credit.copyright = 
                "\"Galactic Rap\" Kevin MacLeod (incompetech.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);


            // Stage - Theme 1
            credit = new AudioCredits.AudioCredit();
            credit.title = "Vanishing Horizon";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Dance";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Dance";
            credit.link2 = "https://audionautix.com/";
            credit.copyright = 
                "\"Vanishing Horizon\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);


            // Stage - Theme 2
            credit = new AudioCredits.AudioCredit();
            credit.title = "Thump";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Dance";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Dance";
            credit.link2 = "https://audionautix.com/";
            credit.copyright =
                "\"Thump\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);

            // Stage - Theme 3/Final Stage
            credit = new AudioCredits.AudioCredit();
            credit.title = "Sports Action";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Soundtrack";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Soundtrack";
            credit.link2 = "https://audionautix.com/";
            credit.copyright =
                "\"Sports Action\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);

            // Stage - Results
            credit = new AudioCredits.AudioCredit();
            credit.title = "Transcend";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Soundtrack";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Soundtrack";
            credit.link2 = "https://audionautix.com/";
            credit.copyright =
                "\"Transcend\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);

            // Game - Results
            credit = new AudioCredits.AudioCredit();
            credit.title = "Tired";
            credit.artist = "Antti Luode";
            credit.collection = "GameSounds.xyz/Anttis instrumentals/Songs";
            credit.source = "GameSounds.xyz";
            credit.link1 = "https://gamesounds.xyz/?dir=Anttis%20instrumentals/Songs";
            credit.link2 = "";
            credit.copyright =
                "\"Tired\"" +
                "\nAntti Luode" +
                "\nLicensed under Creative Commons: By Attribution 3.0" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            bgmCredits.audioCredits.Add(credit);


            // Jingles
            // Stage Cleared / Game Complete
            credit = new AudioCredits.AudioCredit();
            credit.title = "Big Intro";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Soundtrack";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Soundtrack";
            credit.link2 = "https://audionautix.com/";
            credit.copyright =
                "\"Big Intro\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);

            // Stage Failed
            credit = new AudioCredits.AudioCredit();
            credit.title = "Suspense Action";
            credit.artist = "Jason Shaw";
            credit.collection = "GameSounds.xyz/Audionautix/Soundtrack";
            credit.source = "GameSounds.xyz, Audionautix.com";
            credit.link1 = "https://gamesounds.xyz/?dir=Audionautix/Soundtrack";
            credit.link2 = "https://audionautix.com/";
            credit.copyright =
                "\"Suspense Action\"" +
                "\nMusic by Jason Shaw on Audionautix.com (https://audionautix.com)" +
                "\nLicensed under Creative Commons: By Attribution 4.0 License" +
                "\nhttp://creativecommons.org/licenses/by/4.0/";

            bgmCredits.audioCredits.Add(credit);
        }

        // Loads the SFX and JNG credits.
        private void LoadSoundEffectCredits()
        {
            AudioCredits.AudioCredit credit;

            // Loads sound effects and jingles.

            // Sound Effects
            // World - Stage Select
            credit = new AudioCredits.AudioCredit();
            credit.title = "Text Message Alert 5";
            credit.artist = "Daniel Simon";
            credit.collection = "SoundBible.com";
            credit.source = "SoundBible.com";
            credit.link1 = "https://soundbible.com/2158-Text-Message-Alert-5.html";
            credit.link2 = "";
            credit.copyright = 
                "\"Text Message Alert 5\"" +
                "\nDaniel Simon" +
                "\nLicensed under Creative Commons: By Attribution 3.0 License" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            sfxCredits.audioCredits.Add(credit);

            // World - Stage Select - Confirm
            credit = new AudioCredits.AudioCredit();
            credit.title = "Text Message Alert 2";
            credit.artist = "Daniel Simon";
            credit.collection = "SoundBible.com";
            credit.source = "SoundBible.com";
            credit.link1 = "https://soundbible.com/2155-Text-Message-Alert-2.html";
            credit.link2 = "";
            credit.copyright =
                "\"Text Message Alert 2\"" +
                "\nDaniel Simon" +
                "\nLicensed under Creative Commons: By Attribution 3.0 License" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            sfxCredits.audioCredits.Add(credit);

            // World - Stage Select - Decline
            credit = new AudioCredits.AudioCredit();
            credit.title = "Text Message Alert 1";
            credit.artist = "Daniel Simon";
            credit.collection = "SoundBible.com";
            credit.source = "SoundBible.com";
            credit.link1 = "https://soundbible.com/2154-Text-Message-Alert-1.html";
            credit.link2 = "";
            credit.copyright =
                "\"Text Message Alert 1\"" +
                "\nDaniel Simon" +
                "\nLicensed under Creative Commons: By Attribution 3.0 License" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            sfxCredits.audioCredits.Add(credit);

            // Stage - Units Button
            credit = new AudioCredits.AudioCredit();
            credit.title = "Button";
            credit.artist = "Mike Koenig";
            credit.collection = "SoundBible.com";
            credit.source = "SoundBible.com";
            credit.link1 = "https://soundbible.com/772-Button.html";
            credit.link2 = "";
            credit.copyright =
                "\"Button\"" +
                "\nMike Koenig" +
                "\nLicensed under Creative Commons: By Attribution 3.0 License" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            sfxCredits.audioCredits.Add(credit);

            // Stage - Laser Shot Destruction / Stage - Barrier - Destroyed
            credit = new AudioCredits.AudioCredit();
            credit.title = "Power Failure";
            credit.artist = "Mike Koenig";
            credit.collection = "SoundBible.com";
            credit.source = "SoundBible.com";
            credit.link1 = "https://soundbible.com/1610-Power-Failure.html";
            credit.link2 = "";
            credit.copyright =
                "\"Power Failure\"" +
                "\nMike Koenig" +
                "\nLicensed under Creative Commons: By Attribution 3.0 License" +
                "\nhttps://creativecommons.org/licenses/by/3.0/";

            sfxCredits.audioCredits.Add(credit);
        }

        // Loads the font credits.
        private void LoadFontCredits()
        {
            AudioCredits.AudioCredit credit;

            credit = new AudioCredits.AudioCredit();
            credit.title = "Font Title";
            credit.artist = "Font Artist";
            credit.collection = "Font Collection";
            credit.source = "Font Source";
            credit.link1 = "Font Link 1 Debug";
            credit.link2 = "Font Link 2 Debug";
            credit.copyright = "Font Copyright";
            
            fontsCredits.audioCredits.Add(credit);

            // TODO: implement imported fonts.
        }

        // Enables all the credit buttons.
        public void EnableAllCreditButtons()
        {
            // Disables all the credit buttons.
            bgmButton.interactable = true;
            sfxButton.interactable = true;
            fontsButton.interactable = true;
        }

        // Disables all the credit buttons.
        public void DisableAllCreditButtons()
        {
            // Disables all the credit buttons.
            bgmButton.interactable = false;
            sfxButton.interactable = false;
            fontsButton.interactable = false;
        }

        // Shows the BGM credits.
        public void ShowBackgroundMusicCredits()
        {
            // Saves the current credit index, switches over, and then sets the new credit index.
            SaveCurrentCreditIndex();
            creditsInterface.audioCredits = bgmCredits;
            creditsInterface.SetCreditIndex(bgmCreditsIndex);

            // Change button settings.
            EnableAllCreditButtons();
            bgmButton.interactable = false;
        }

        // Shows the SFX credits.
        public void ShowSoundEffectCredits()
        {
            // Saves the old credits to know what button to enable.
            AudioCredits oldCredits = creditsInterface.audioCredits;

            // Saves the current credit index, switches over, and then sets the new credit index.
            SaveCurrentCreditIndex();
            creditsInterface.audioCredits = sfxCredits;
            creditsInterface.SetCreditIndex(sfxCreditsIndex);

            // Change button settings.
            EnableAllCreditButtons();
            sfxButton.interactable = false;
        }

        // Shows the font credits.
        public void ShowFontCredits()
        {
            // Saves the current credit index, switches over, and then sets the new credit index.
            SaveCurrentCreditIndex();
            creditsInterface.audioCredits = fontsCredits;
            creditsInterface.SetCreditIndex(fontsCreditsIndex);

            // Change button settings.
            EnableAllCreditButtons();
            fontsButton.interactable = false;
        }

        // Gets the current credits.
        public AudioCredits GetCurrentCredits()
        {
            return creditsInterface.audioCredits;
        }

        // Returns 'true' if the BGM credits are the current credits.
        public bool IsCurrentCreditsBgmCredits()
        {
            return GetCurrentCredits() == bgmCredits;
        }

        // Returns 'true' if the SFX credits are the current credits.
        public bool IsCurrentCreditsSfxCredits()
        {
            return GetCurrentCredits() == sfxCredits;
        }

        // Returns 'true' if the font credits are the current credits.
        public bool IsCurrentCreditsFontCredits()
        {
            return GetCurrentCredits() == fontsCredits;
        }

        // Saves the current credit index of the applicable window.
        public void SaveCurrentCreditIndex()
        {
            // Only one of these should be active at a time.
            if (IsCurrentCreditsBgmCredits())
            {
                bgmCreditsIndex = creditsInterface.GetCurrentCreditIndex();
            }
            else if (IsCurrentCreditsSfxCredits())
            {
                sfxCreditsIndex = creditsInterface.GetCurrentCreditIndex();
            }
            else if (IsCurrentCreditsFontCredits())
            {
                fontsCreditsIndex = creditsInterface.GetCurrentCreditIndex();
            }
        }
    }
}