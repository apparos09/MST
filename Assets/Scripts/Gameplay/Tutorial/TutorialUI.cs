using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The UI for the tutorial.
    public class TutorialUI : MonoBehaviour
    {
        // The singleton instance.
        private static TutorialUI instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The game UI.
        public GameplayUI gameUI;

        // The tutorials object.
        public Tutorials tutorials;

        // The background panel used to block other buttons.
        public Image backgroundPanel;

        // The tutorial text box.
        public TutorialTextBox textBox;


        [Header("Characters")]
        
        // The border colours do not change for now. Will probably be kept this way.

        // The alpha 0 sprite. Used to hide the diagram if there's no image.
        [Tooltip("Used to mkae an empty character image.")]
        public Sprite alpha0Sprite;

        // Partner A's sprite.
        public Sprite partnerASprite;

        // The border colour for partner A.
        private Color partnerABorderColor = Color.blue;

        // Partner B's sprite.
        public Sprite partnerBSprite;

        // The border colour for partner B.
        private Color partnerBBorderColor = Color.red;

        // Changes the border color.
        private bool changeBorderColor = false;

        // Constructor
        private TutorialUI()
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
                textBox.OnTextBoxOpenedAddCallback(OnTextBoxOpened);
                textBox.OnTextBoxClosedAddCallback(OnTextBoxClosed);
                textBox.OnTextBoxFinishedAddCallback(OnTextBoxFinished);

                instanced = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Gets the UI instance if it's not set.
            if (gameUI == null)
            {
                // Checks for the user interfaces to attach.
                if(WorldUI.Instantiated)
                {
                    gameUI = WorldUI.Instance;
                }
                else if(StageUI.Instantiated)
                {
                    gameUI = StageUI.Instance;
                }
                else
                {
                    Debug.LogWarning("Game UI could not be found.");
                }
                    
            }

            // Gets the tutorials object.
            if (tutorials == null)
                tutorials = Tutorials.Instance;

            // If the text box is open, close it.
            if(textBox.IsVisible())
            {
                textBox.Close();
            }
        }

        // Gets the instance.
        public static TutorialUI Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<TutorialUI>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Tutorial UI (singleton)");
                        instance = go.AddComponent<TutorialUI>();
                    }

                }

                // Return the instance.
                return instance;
            }
        }

        // Returns 'true' if the object has been initialized.
        public static bool Instantiated
        {
            get
            {
                return instanced;
            }
        }

        // Is the tutorial active?
        public bool IsTutorialRunning()
        {
            // If the textbox is isible, then the tutorial is active.
            return textBox.IsVisible();
        }

        // Starts a tutorial.
        public void StartTutorial()
        {
            textBox.SetPage(0);
            OpenTextBox();
        }

        // Restarts the tutorial.
        public void RestartTutorial()
        {
            // Gets the pages from the text box.
            List<Page> pages = textBox.pages;

            // Ends the tutorial, sets the textbox pages, and starts the tutorial again.
            EndTutorial();
            textBox.pages = pages;
            StartTutorial();
        }

        // Ends the tutorial.
        public void EndTutorial()
        {
            // If the tutorial is running, end it.
            if(IsTutorialRunning())
            {
                // Sets to the last page and closes the text box.
                textBox.SetPage(textBox.GetPageCount() - 1);
                CloseTextBox();
            }
        }

        // Called when a tutorial is started.
        public void OnTutorialStart()
        {
            // If there is a background panel, turn it on.
            if(backgroundPanel != null)
                backgroundPanel.gameObject.SetActive(true);
        }

        // Called when a tutorail ends.
        public void OnTutorialEnd()
        {
            // If there is no background panel, turn it off.
            if (backgroundPanel != null)
                backgroundPanel.gameObject.SetActive(false);
        }

        // TEXT BOX
        // Loads pages for the textbox.
        public void LoadPages(ref List<Page> pages, bool clearPages)
        {
            // If the pages should be cleared.
            if (clearPages)
                textBox.ClearPages();

            // Adds pages to the end of the text box.
            textBox.pages.AddRange(pages);

        }

        // Opens Text Box
        public void OpenTextBox()
        {
            textBox.Open();
        }

        // Closes the Text Box
        public void CloseTextBox()
        {
            textBox.Close();
        }

        // Text box operations.
        // Called when the text box is opened.
        private void OnTextBoxOpened()
        {
            // These should be handled by the pages.
            // Hides the diagram by default.
            // HideDiagram();

            // The tutorial has started.
            tutorials.OnTutorialStart();
        }

        // Called when the text box is closed.
        private void OnTextBoxClosed()
        {
            // NOTE: this may not be needed.
            // // The tutorial has ended (at least for now), so allow the game to move again.
            // tutorials.OnTutorialEnd();
        }

        // Called when the text box is finished.
        private void OnTextBoxFinished()
        {
            // Remove all the pages.
            textBox.ClearPages();

            // These should be handled by the pages.
            // // Clear the diagram and hides it.
            // ClearDiagram();
            // HideDiagram();

            // The tutorial has ended.
            tutorials.OnTutorialEnd();
        }

        // CHARACTERS
        // Sets the character to partner A.
        public void SetCharacterToPartnerA()
        {
            textBox.SetCharacterImage(partnerASprite);

            // Changes the border color.
            if (changeBorderColor)
            {
                textBox.SetBorderColor(partnerABorderColor);
            }
        }

        // Sets the character to partner B.
        public void SetCharacterToPartnerB()
        {
            textBox.SetCharacterImage(partnerBSprite);

            // Changes the border color.
            if (changeBorderColor)
            {
                textBox.SetBorderColor(partnerBBorderColor);
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