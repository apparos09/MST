using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The game UI.
    public class GameplayUI : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        // Gets set to 'true' when late start is called.
        protected bool calledLateStart = false;

        [Header("Windows/Menus")]
        // The window panel.
        public Image windowPanel;

        // The settings UI.
        // TODO: add quit button.
        public GameSettingsUI gameSettingsUI;

        [Header("Tutorial")]

        // The tutorial UI.
        public TutorialUI tutorialUI;

        // Awake is called when the script is being loaded
        protected virtual void Awake()
        {
            // ...
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // If the game manager isn't set, try to find it.
            if(gameManager == null)
                gameManager = FindObjectOfType<GameplayManager>();

            // If the tutorial UI is not set, set it.
            if (tutorialUI == null)
                tutorialUI = TutorialUI.Instance;

            // Closes all windows by default.
            CloseAllWindows();
        }

        // Called on the first update frame.
        protected virtual void LateStart()
        {
            calledLateStart = true;
        }

        // TUTORIAL //
        // Start tutorial
        public void StartTutorial(List<Page> pages)
        {
            // Loads the pages, sets the index to 0, and closes the textbox.
            if(tutorialUI.textBox.IsVisible())
            {
                // Close the textbox.
                tutorialUI.textBox.Close();
            }

            // Load the pages.
            tutorialUI.LoadPages(ref pages, true);
            tutorialUI.textBox.CurrentPageIndex = 0;
            tutorialUI.textBox.Open();
        }

        // On Tutorial Start
        public virtual void OnTutorialStart()
        {
            // ...
        }

        // On Tutorial End
        public virtual void OnTutorialEnd()
        {
            // ...
        }

        // Checks if the tutorial text box is open.
        public bool IsTutorialTextBoxOpen()
        {
            // Checks if it's visible normally, and in the hierachy.
            return tutorialUI.textBox.IsVisible() && tutorialUI.textBox.IsVisibleInHierachy();
        }

        // Returns 'true' if the tutorial can be started.
        public bool IsTutorialAvailable()
        {
            return !IsTutorialTextBoxOpen();
        }

        // Checks if the tutorial is running.
        public bool IsTutorialRunning()
        {
            // Checks if the tutorial is instantiated.
            if (Tutorials.Instantiated)
            {
                // Checks if the tutorial UI is not set to null.
                if (tutorialUI != null)
                {
                    // The tutorials object has a dedicated function for seeing if it's running...
                    // You don't know why you did a seperate set up for this, but you're leaving it as is.
                    return IsTutorialTextBoxOpen();
                }
                else // No tutorial UI, so the tutorial cannot run.
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
                
        }

        // Adds the tutorial text box open/close callbacks.
        public void AddTutorialTextBoxCallbacks(GameplayManager manager)
        {
            tutorialUI.textBox.OnTextBoxOpenedAddCallback(manager.OnTutorialStart);
            tutorialUI.textBox.OnTextBoxClosedAddCallback(manager.OnTutorialEnd);
        }

        // Removes the tutorial text box open/close callbacks.
        public void RemoveTutorialTextBoxCallbacks(GameplayManager manager)
        {
            tutorialUI.textBox.OnTextBoxOpenedRemoveCallback(manager.OnTutorialStart);
            tutorialUI.textBox.OnTextBoxClosedRemoveCallback(manager.OnTutorialEnd);
        }

        // WINDOWS //
        // Checks if a window is open.
        public virtual bool IsWindowOpen()
        {
            // Only checks the settings window here.
            bool open = gameSettingsUI.gameObject.activeSelf;

            return open;
        }

        // Closes all the windows.
        public virtual void CloseAllWindows()
        {
            // Settings
            gameSettingsUI.gameObject.SetActive(false);
            
            // On Window Closed
            OnWindowClosed();
        }

        // Opens the provided window.
        public virtual void OpenWindow(GameObject window)
        {
            OpenWindow(window, true);
        }

        // Opens the provided window. The 'panelActive' value determines if the background panel is on or not. 
        public virtual void OpenWindow(GameObject window, bool panelActive)
        {
            CloseAllWindows();
            window.gameObject.SetActive(true);
            OnWindowOpened(window, panelActive);
        }

        // Called when a window is opened.
        public virtual void OnWindowOpened(GameObject window)
        {
            OnWindowOpened(window, true);
        }

        // Called when a window is opened.
        public virtual void OnWindowOpened(GameObject window, bool panelActive)
        {
            // Pause the game.
            gameManager.PauseGame();

            // Enables the menu panel to block the UI under it.
            if (windowPanel != null)
            {
                windowPanel.gameObject.SetActive(panelActive);
            }

            // // If the tutorial text box is open.
            // if (IsTutorialTextBoxOpen() && tutorialUI.backgroundPanel != null)
            // {
            //     // Turns off the tutorial panel so that they aren't overlayed.
            //     tutorialUI.backgroundPanel.gameObject.SetActive(false);
            // 
            // }
        }

        // Called when a window is closed.
        public virtual void OnWindowClosed()
        {
            // Unpause the game.
            gameManager.UnpauseGame();

            // Disables the tutorial panel.
            if (windowPanel != null)
                windowPanel.gameObject.SetActive(false);

            // // If the tutorial text box is open.
            // if (IsTutorialTextBoxOpen())
            // {
            //     // Turns on the tutorial panel since the menu panel isn't showing now.
            //     if(tutorialUI.backgroundPanel != null)
            //         tutorialUI.backgroundPanel.gameObject.SetActive(true);
            // }
        }

        // SCENES
        // Goes to the title scene.
        public virtual void ToTitle()
        {
            gameManager.ToTitle();
        }

        // Goes to the title scene.
        public virtual void ToResults()
        {
            gameManager.ToResults();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Call late start.
            if (!calledLateStart)
                LateStart();
        }
    }
}