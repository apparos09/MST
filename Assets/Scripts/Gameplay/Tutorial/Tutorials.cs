using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using util;

namespace RM_MST
{
    // The tutorial script.
    public class Tutorials : MonoBehaviour
    {
        [System.Serializable]
        public class TutorialsData
        {
            public bool clearedIntroTutorial;
            public bool clearedFirstStageTutorial;

            public bool clearedHiddenMultiplesTutorial;
            public bool clearedBarriersTutorial;
            public bool clearedSurfaceTutorial;

            public bool clearedPuzzleButtonsTutorial;
            public bool clearedPuzzleSwapTutorial;
            public bool clearedPuzzleSlideTutorial;
            public bool clearedPuzzlePathTutorial;

            public bool clearedFirstWinTutorial;
            public bool clearedMixStageTutorial;

            public bool clearedLengthImperial;
            public bool clearedWeightImperial;
            public bool clearedTimeTutorial;

            public bool clearedLengthMetricTutorial;
            public bool clearedWeightMetricTutorial;
            public bool clearedCapacityTutorial;
        }

        // The tutorial types 
        // TODO: remove.
        // public enum tutorialType
        // {
        //     none, intro, stage, firstWin, mixStage, weightImperial, lengthImperial, time, lengthMetric, weightMetric, capacity
        // };


        // The tutorial type count.
        public const int TUTORIAL_TYPE_COUNT = 7;

        // The singleton instance.
        private static Tutorials instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The game manager.
        public GameplayManager gameManager;

        // The tutorials UI.
        public TutorialUI tutorialsUI;

        // // If 'true', the tutorials object constantly checks for starting tutorials.
        // [Tooltip("Constant check for tutorial start.")]
        // public bool constantTutorialStartCheck = true;


        [Header("Tutorials")]

        public bool clearedIntroTutorial;
        public bool clearedFirstStageTutorial;

        public bool clearedHiddenMultiplesTutorial;
        public bool clearedBarrierTutorial;
        public bool clearedSurfaceTutorial;

        public bool clearedPuzzleButtonsTutorial;
        public bool clearedPuzzleSwapTutorial;
        public bool clearedPuzzleSlideTutorial;
        public bool clearedPuzzlePathTutorial;

        public bool clearedFirstWinTutorial;
        public bool clearedMixStageTutorial;

        public bool clearedLengthImperialTutorial;
        public bool clearedWeightImperialTutorial;
        public bool clearedTimeTutorial;

        public bool clearedLengthMetricTutorial;
        public bool clearedWeightMetricTutorial;
        public bool clearedCapacityTutorial;

        // Constructor
        private Tutorials()
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
        void Start()
        {
            // Gets the game manager object.
            if (gameManager == null)
            {
                // Checks for the user interfaces to attach.
                if (WorldManager.Instantiated)
                {
                    gameManager = WorldManager.Instance;
                }
                else if (StageManager.Instantiated)
                {
                    gameManager = StageManager.Instance;
                }
                else
                {
                    // Tries to find the object.
                    gameManager = FindObjectOfType<GameplayManager>();

                    // Not set, so state a warning.
                    if(gameManager == null)
                        Debug.LogWarning("Game manager could not be found.");
                }
            }

            // Gets the tutorials object.
            if (tutorialsUI == null)
                tutorialsUI = TutorialUI.Instance;

            // Don't destroy this game object.
            DontDestroyOnLoad(gameObject);
        }


        // This function is called when the object is enabled and active
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // This function is called when the behaviour becomes disabled or inactive
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // Gets the instance.
        public static Tutorials Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<Tutorials>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Tutorial (singleton)");
                        instance = go.AddComponent<Tutorials>();
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

        // Called when the scene is loaded.
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // If the game manager is not set, set it.
            if (gameManager == null)
            {
                // Try to find the game manager.
                gameManager = FindObjectOfType<GameplayManager>();
            }

            // Try to get the tutorials UI again.
            if(tutorialsUI == null)
                tutorialsUI = TutorialUI.Instance;
        }

        // Checks if a tutorial is running.
        public bool IsTutorialRunning()
        {
            return tutorialsUI.IsTutorialRunning();
        }

        // Starts the tutorial.
        public void StartTutorial()
        {
            tutorialsUI.StartTutorial();
        }

        // Restarts the tutorial.
        public void RestartTutorial()
        {
            tutorialsUI.RestartTutorial();
        }

        // Ends the tutorial.
        public void EndTutorial()
        {
            tutorialsUI.EndTutorial();
        }

        // Called when a tutorial is started.
        public void OnTutorialStart()
        {
            // UI start function.
            tutorialsUI.OnTutorialStart();

            // Freeze the game.
            Time.timeScale = 0.0F;
        }

        // Called when a tutorial ends.
        public void OnTutorialEnd()
        {
            // UI end function.
            tutorialsUI.OnTutorialEnd();

            // Unfreeze the game if the game is not paused.
            if (!gameManager.IsGamePaused())
            {
                // If the game manager is set, check it for the time scale.
                // If it's not set, use 1.0F.
                if(gameManager != null)
                {
                    Time.timeScale = gameManager.GetGameTimeScale();
                }
                else
                {
                    Time.timeScale = 1.0F;
                }
                
            }
                
            // Ignore the current input for this frame in case the player is holding the space bar.
            // gameManager.player.IgnoreInputs(1);
        }
        

        // TUTORIAL DATA
        // Generates the tutorials data.
        public TutorialsData GenerateTutorialsData()
        {
            TutorialsData data = new TutorialsData();
            
            data.clearedIntroTutorial = clearedIntroTutorial;
            data.clearedFirstStageTutorial = clearedFirstStageTutorial;

            data.clearedHiddenMultiplesTutorial = clearedHiddenMultiplesTutorial;
            data.clearedBarriersTutorial = clearedBarrierTutorial;
            data.clearedSurfaceTutorial = clearedSurfaceTutorial;

            data.clearedPuzzleButtonsTutorial = clearedPuzzleButtonsTutorial;
            data.clearedPuzzleSwapTutorial = clearedPuzzleSwapTutorial;
            data.clearedPuzzleSlideTutorial = clearedPuzzleSlideTutorial;
            data.clearedPuzzlePathTutorial = clearedPuzzlePathTutorial;

            data.clearedFirstWinTutorial = clearedFirstWinTutorial;
            data.clearedMixStageTutorial = clearedMixStageTutorial;

            data.clearedWeightImperial = clearedWeightImperialTutorial;
            data.clearedLengthImperial = clearedLengthImperialTutorial;
            data.clearedTimeTutorial = clearedTimeTutorial;

            data.clearedLengthMetricTutorial = clearedLengthMetricTutorial;
            data.clearedWeightMetricTutorial = clearedWeightMetricTutorial;
            data.clearedCapacityTutorial = clearedCapacityTutorial;

            return data;
        }

        // Sets the tutorials data.
        public void LoadTutorialsData(TutorialsData data)
        {
            clearedIntroTutorial = data.clearedIntroTutorial;
            clearedFirstStageTutorial = data.clearedFirstStageTutorial;

            clearedHiddenMultiplesTutorial = data.clearedHiddenMultiplesTutorial;
            clearedBarrierTutorial = data.clearedBarriersTutorial;
            clearedSurfaceTutorial = data.clearedSurfaceTutorial;

            clearedPuzzleButtonsTutorial = data.clearedPuzzleButtonsTutorial;
            clearedPuzzleSwapTutorial = data.clearedPuzzleSwapTutorial;
            clearedPuzzleSlideTutorial = data.clearedPuzzleSlideTutorial;
            clearedPuzzlePathTutorial = data.clearedPuzzlePathTutorial;

            clearedFirstWinTutorial = data.clearedFirstWinTutorial;
            clearedMixStageTutorial = data.clearedMixStageTutorial;

            clearedWeightImperialTutorial = data.clearedWeightImperial;
            clearedLengthImperialTutorial = data.clearedLengthImperial;
            clearedTimeTutorial = data.clearedTimeTutorial;

            clearedLengthMetricTutorial = data.clearedLengthMetricTutorial;
            clearedWeightMetricTutorial = data.clearedWeightMetricTutorial;
            clearedCapacityTutorial = data.clearedCapacityTutorial;
        }


        // Tutorial Loader

        // Loads the tutorial
        private void LoadTutorial(ref List<Page> pages, bool startTutorial = true)
        {
            // The gameplay manager isn't set, try to find it.
            if (gameManager == null)
                gameManager = FindObjectOfType<GameplayManager>();

            // Loads pages for the tutorial.
            if(gameManager != null && startTutorial) // If the game manager is set, start the tutorial.
            {
                gameManager.StartTutorial(pages);
            }
            else // No game manager, so just load the pages.
            {
                tutorialsUI.LoadPages(ref pages, false);
            }
        }

        // // Loads the tutorial of the provided type.
        // public void LoadTutorial(tutorialType tutorial)
        // {
        //     // ...
        // }


        // Load the tutorial (template)
        private void LoadTutorialTemplate(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new Page("Insert text here.")
            };

            // Change the display image when certain pages are opened using callbacks.

            // Loads the tutorial.
            LoadTutorial(ref pages, startTutorial);
        }

        // Load test tutorial
        public void LoadTutorialTest(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a test."),
                new MST_Page("This is only a test.")
            };

            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);
            pages[1].OnPageOpenedAddCallback(tutorialsUI.textBox.HideCharacterImage);


            // Change the display image when certain pages are opened using callbacks.

            // Loads the tutorial.
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the intro tutorial.
        public void LoadIntroTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("Welcome to the Meteor Strike Team (MST)! We track down meteors and destroy them before they hit the Earth's surface, which we do by converting larger measurement units into smaller measurement units. This is the world area, which is where you select stages, view unit information, change the game settings, and save your game. I'm Reteor...", "trl_intro_00"),
                new MST_Page("And I'm Astrite! When you select a stage, the units you'll be working with will be displayed. Once this information is given, it's added to the 'units info menu', which can be viewed using the 'units info button'. With all that explained, please select the available stage to start destroying meteors!", "trl_intro_01"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[1].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);

            // If the world manager UI is instantiated.
            if(WorldUI.Instantiated)
            {
                // Disable the units info button since there's nothing to display yet.
                WorldUI worldUI = WorldUI.Instance;
                pages[0].OnPageOpenedAddCallback(worldUI.MakeUnitsInfoButtonUninteractable);
            }

            // Sets the bool and loads the tutorial.
            clearedIntroTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the stage tutorial.
        public void LoadFirstStageTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages;

            // Checks if the player select mode is enabled.
            if(GameSettings.Instance.allowPlayerSelectMode) // Change tutorial based on mode.
            {
                // Checks the gameplay mode.
                switch(gameManager.gameplayMode)
                {
                    // Since the mode select is for the non-LOL build, these versions are not included in the language file.
                    default:
                    case GameplayManager.gameMode.focus: // Focus

                        // Create the tutorial.
                        pages = new List<Page>
                        {
                            // Load the pages.
                            new Page("Welcome to the stage area (focus mode), which is where you'll shoot down meteors. When a meteor is targeted, you are presented with a conversion to solve. If the conversion is successful, you get points, and once you get enough points, the stage is completed. But if the Earth's surface takes too much damage, the stage is lost."),
                            new Page("Meteors are automatically targeted, but you can also manually target non-moving meteors by clicking on/touching them. When a meteor is targeted, you are presented with different conversion outputs and the multipliers that were used to get them. Once you select an output (the method of doing so varies by stage), a laser shot is fired at the targeted meteor."),
                            new Page("If you've selected the correct output, the targeted meteor is destroyed, the screen flashes green, and \"Correct!\" text appears at the bottom of the stage window. If you've selected an incorrect output, the targeted meteor moves closer to the Earth's surface, you're briefly stunned, the screen flashes blue, and \"Incorrect!\" text appears at the bottom of the stage window. Notably, once a meteor appears, it only moves when hit by a laser shot that fails to destroy it."),
                            new Page("On the left is the points bar, the surface's health bar, and the calculator. On the right is the settings button, the world button, and the units conversion table. The 'units conversion table' shows all the unit conversions for the current units group. If you have trouble finding the correct output, try using the calculator to solve the equation. With all that said, time to start the stage!"),
                        };

                        break;

                    case GameplayManager.gameMode.rush: // Rush

                        // Create the tutorial.
                        pages = new List<Page>
                        {
                            // Load the pages.
                            new Page("Welcome to the stage area (rush mode), which is where you'll shoot down meteors. The meteor closest to the Earth's surface is automatically targeted, so all you must do is solve the conversions presented to you. You get points for performing successful conversions, and once you get enough points, the stage is completed. But if the Earth's surface takes too much damage, the stage is lost."),
                            new Page("When a meteor is targeted, you are presented with different conversion outputs and the multipliers that were used to get them. Once you select an output (the method of doing so varies by stage), a laser shot is fired at the targeted meteor."),
                            new Page("If you've selected the correct output, the targeted meteor is destroyed, the rest of the meteors are pushed back, the screen flashes green, and \"Correct!\" text appears at the bottom of the stage window. If you've selected an incorrect output, only the targeted meteor is knocked back, the targeted meteor is not destroyed, the screen flashes blue, and \"Incorrect!\" text appears at the bottom of the stage window. You're also briefly stunned if you choose a wrong output."),
                            new Page("On the left is the points bar, the surface's health bar, and the calculator. On the right is the settings button, the world button, the speed button, and the units conversion table. The 'speed button' can be used to slow the game down, and the 'units conversion table' shows all the unit conversions for the current units group. If you have trouble finding the correct output, try using the calculator to solve the equation. With all that said, time to start the stage!"),
                        };

                        break;
                }
            }
            else // Use LOL tutorial, which is the focus tutorial, but without mentioning the mode itself.
            {
                // Create the tutorial.
                pages = new List<Page>
                {
                    // Load the pages.
                    new MST_Page("Welcome to the stage area, which is where you'll shoot down meteors. When a meteor is targeted, you are presented with a conversion to solve. If the conversion is successful, you get points, and once you get enough points, the stage is completed. But if the Earth's surface takes too much damage, the stage is lost.", "trl_firstStage_00"),
                    new MST_Page("Meteors are automatically targeted, but you can also manually target non-moving meteors by clicking on/touching them. When a meteor is targeted, you are presented with different conversion outputs and the multipliers that were used to get them. Once you select an output (the method of doing so varies by stage), a laser shot is fired at the targeted meteor.", "trl_firstStage_01"),
                    new MST_Page("If you've selected the correct output, the targeted meteor is destroyed, the screen flashes green, and \"Correct!\" text appears at the bottom of the stage window. If you've selected an incorrect output, the targeted meteor moves closer to the Earth's surface, you're briefly stunned, the screen flashes blue, and \"Incorrect!\" text appears at the bottom of the stage window. Notably, once a meteor appears, it only moves when hit by a laser shot that fails to destroy it.", "trl_firstStage_02"),
                    new MST_Page("On the left is the points bar, the surface's health bar, and the calculator. On the right is the settings button, the world button, and the units conversion table. The 'units conversion table' shows all the unit conversions for the current units group. If you have trouble finding the correct output, try using the calculator to solve the equation. With all that said, time to start the stage!", "trl_firstStage_03"),
                };
            }

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[1].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[2].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[3].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);


            // Sets the bool and loads the tutorial.
            clearedFirstStageTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the hidden mutliples tutorial.
        public void LoadHiddenMultiplesTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("The multipliers have become hidden! If you perform enough conversions correctly in a row, the conversion multipliers will disappear. They will become visible again in a short while, but waiting for this to happen makes the stage take longer. If you get a conversion wrong, the multipliers will start showing up instantly like before, unless you get enough consecutive correct answers again.", "trl_hiddenMultiples_00")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedHiddenMultiplesTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the barriers tutorial.
        public void LoadBarrierTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("A barrier has been hit! When a meteor hits a barrier, the meteor is destroyed, the barrier takes damage, and the screen flashes orange. If a barrier takes too much damage, it'll be destroyed, leaving an opening for meteors to hit the Earth's surface and damage it. Once you get enough points, a barrier is restored.", "trl_barrier_00")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedBarrierTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the surface tutorial.
        public void LoadSurfaceTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("The Earth's surface has taken damage! When the surface takes damage, the screen flashes red. Remember: if the surface takes too much damage, the stage is lost.", "trl_surface_00")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedSurfaceTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }


        // Puzzles
        // Loads the puzzle - unit buttons tutorial.
        public void LoadPuzzleButtonsTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a units buttons puzzle! Submit an output by selecting the button it's attached to.", "trl_pzl_buttons")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedPuzzleButtonsTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the puzzle - swap tutorial.
        public void LoadPuzzleSwapTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a swap puzzle! Symbols in the puzzle area switch places when the time bar in the puzzle area fully depletes. Select an output using one of its corresponding symbols in the puzzle area.", "trl_pzl_swap")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedPuzzleSwapTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the puzzle - slide tutorial.
        public void LoadPuzzleSlideTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a slide puzzle! Symbols slide across the puzzle area while a meteor is targeted. Select an output using one of its corresponding symbols in the puzzle area.", "trl_pzl_slide")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedPuzzleSlideTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the puzzle - path tutorial.
        public void LoadPuzzlePathTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a path puzzle! Symbols move along the path in the puzzle area while a meteor is targeted. Select an output using its corresponding symbol in the puzzle area.", "trl_pzl_path")
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedPuzzlePathTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }


        // First Win and Mix
        // Loads the first win tutorial.
        public void LoadFirstWinTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("You've completed the first stage, which has unlocked even more stages! The more stages you beat, the more stages you'll unlock. Of the stages available, you can clear them in any order, but you must beat all the stages to complete the game.", "trl_firstWin_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedFirstWinTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the mix stage tutorial.
        public void LoadMixStageTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("This is a mix stage. Mix stages have you deal with multiple unit groups at once. You'll only deal with a mix stage after you've experienced all relevant unit groups.", "trl_mixStage_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);


            // Sets the bool and loads the tutorial.
            clearedMixStageTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }


        // Measurement Groups
        // Loads the length (imperial) tutorial.
        public void LoadLengthImperialTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure how long something is.", "trl_lengthImperial_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedLengthImperialTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the weight (imperial) tutorial.
        public void LoadWeightImperialTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure how heavy something is.", "trl_weightImperial_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);


            // If the world manager UI is instantiated.
            if (WorldUI.Instantiated)
            {
                // This is the first stage of the game.
                // As such, it makes sure to turn on the units button now that this tutorial has been given.
                WorldUI worldUI = WorldUI.Instance;
                pages[0].OnPageClosedAddCallback(worldUI.MakeUnitsInfoButtonInteractable);
            }


            // Sets the bool and loads the tutorial.
            clearedWeightImperialTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the time tutorial.
        public void LoadTimeTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure lengths of time.", "trl_time_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedTimeTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the length metric tutorial.
        public void LoadLengthMetricTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure how long something is.", "trl_lengthMetric_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedLengthMetricTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the weight metric tutorial.
        public void LoadWeightMetricTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure how heavy something is.", "trl_weightMetric_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerA);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedWeightMetricTutorial = true;
            LoadTutorial(ref pages, startTutorial);
        }

        // Loads the weight metric tutorial.
        public void LoadCapacityTutorial(bool startTutorial = true)
        {
            // Create the pages list.
            List<Page> pages = new List<Page>
            {
                // Load the pages.
                new MST_Page("These units are used to measure how much liquid a container can hold.", "trl_capacity_00"),
            };

            // Change the display image when certain pages are opened using callbacks.
            pages[0].OnPageOpenedAddCallback(tutorialsUI.SetCharacterToPartnerB);
            pages[0].OnPageOpenedAddCallback(tutorialsUI.textBox.ShowCharacterImage);

            // Sets the bool and loads the tutorial.
            clearedCapacityTutorial = true;
            LoadTutorial(ref pages, startTutorial);
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