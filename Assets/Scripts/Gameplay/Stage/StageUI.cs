using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using util;

namespace RM_MST
{
    // The stage UI.
    public class StageUI : GameplayUI
    {
        // The singleton instance.
        private static StageUI instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The stage manager.
        public StageManager stageManager;

        // The units table.
        public UnitsTable unitsTable;

        // The partner A icon.
        public CharacterIcon partnerAIcon;

        // The partner B icon.
        public CharacterIcon partnerBIcon;

        // The event text display.
        public EventTextDisplay eventTextDisplay;

        [Header("Text")]

        // The stage name text.
        public TMP_Text stageNameText;

        // The time text.
        public TMP_Text timeText;

        // The score text.
        public TMP_Text pointsText;

        [Header("Progress Bars")]

        // The points bar.
        public ProgressBar pointsBar;

        // The damage bar.
        public ProgressBar surfaceHealthBar;

        [Header("Mode Buttons")]

        // The buttons for the focus mode.
        public GameObject buttonsFocusMode;

        // The buttons for the rush mode.
        public GameObject buttonsRushMode;

        // The speed change button. This is only available in rush mode.
        public SpeedButton speedButton;


        [Header("PlayerUI")]

        // The conversion text.
        public TMP_Text conversionText;

        // The 7 units buttons.
        public UnitsButton unitsButton0;
        public UnitsButton unitsButton1;
        public UnitsButton unitsButton2;
        public UnitsButton unitsButton3;
        public UnitsButton unitsButton4;
        public UnitsButton unitsButton5;
        public UnitsButton unitsButton6;

        // Randomizes the unit button values if set to 'true'.
        private bool randomizeUnitButtons = true;

        [Header("Screen Effects")]

        // The stage screen effects.
        public ScreenEffects screenEffects;

        // Enables screen effects if true.
        private bool useScreenEffects = true;

        [Header("End Windows")]

        // The game win window.
        public StageEndUI stageWonWindow;

        // The game lost window.
        public StageEndUI stageLostWindow;
        // Constructor
        private StageUI()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected override void Awake()
        {
            base.Awake();

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

            // Gets the instance.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Updates the mode buttons.
            UpdateModeButtons();
        }

        // Late start is called on the first frame update.
        protected override void LateStart()
        {
            base.LateStart();

            // Set the name.
            stageNameText.text = stageManager.stageName;

            // Updates the mode buttons.
            UpdateModeButtons();

            // Updates all the UI.
            UpdateHUD();
        }

        // Gets the instance.
        public static StageUI Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<StageUI>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("Stage UI (singleton)");
                        instance = go.AddComponent<StageUI>();
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

        // Mode Buttons
        // Updates the mode buttons to show/hide the correct buttons based on the game mode.
        public void UpdateModeButtons()
        {
            // Checks the gameplay mode.
            switch(stageManager.gameplayMode)
            {
                case GameplayManager.gameMode.focus: // Focus
                    buttonsFocusMode.gameObject.SetActive(true);
                    buttonsRushMode.gameObject.SetActive(false);
                    break;

                case GameplayManager.gameMode.rush: // Rush
                    buttonsFocusMode.gameObject.SetActive(false);
                    buttonsRushMode.gameObject.SetActive(true);
                    break;
            }
        }

        // WINDOWS
        // Closes all the windows.
        public override void CloseAllWindows()
        {
            base.CloseAllWindows();

            stageWonWindow.gameObject.SetActive(false);
            stageLostWindow.gameObject.SetActive(false);
        }


        // UI

        // Updates the HUD. This is not counting the unit buttons.
        // TODO: rename this to be more clear?
        public void UpdateHUD()
        {
            UpdateTimeText();
            UpdatePointsText();
            UpdatePointsBar();
            UpdateSurfaceHealthBar();
            UpdateUnitsTable();
        }

        // Updates the time text.
        public void UpdateTimeText()
        {
            timeText.text = StringFormatter.FormatTime(stageManager.stageTime, false, true, false);
        }

        // Updates the points text.
        public void UpdatePointsText()
        {
            pointsText.text = Mathf.Round(stageManager.player.GetPoints()).ToString();
        }

        // Updates the points bar.
        public void UpdatePointsBar()
        {
            // Get the points percentage and clamp it.
            float percent = stageManager.player.GetPoints() / stageManager.pointsGoal;
            percent = Mathf.Clamp01(percent);
            pointsBar.SetValueAsPercentage(percent);
        }

        // Updates the surface health bar.
        public void UpdateSurfaceHealthBar()
        {
            // Get the percent and update the damage bar.
            float percent = stageManager.stageSurface.health / stageManager.stageSurface.maxHealth;
            percent = Mathf.Clamp01(percent);
            surfaceHealthBar.SetValueAsPercentage(percent);
        }

        // PARTNER ANIMATIONS //
        // Plays partner animation.
        public void PlayPartnersAnimation(CharacterIcon.charIconAnim anim)
        {
            PlayPartnerAAnimation(anim);
            PlayPartnerBAnimation(anim);
        }

        // Plays Parter A Animation
        public void PlayPartnerAAnimation(CharacterIcon.charIconAnim anim)
        {
            partnerAIcon.PlayAnimation(anim);
        }

        // Plays Parter B Animation
        public void PlayPartnerBAnimation(CharacterIcon.charIconAnim anim)
        {
            partnerBIcon.PlayAnimation(anim);
        }


        // GAMEPLAY
        // Updates the units table with the provied conversion. By default it's set to 'none'.
        public void UpdateUnitsTable(UnitsInfo.unitGroups group = UnitsInfo.unitGroups.none)
        {
            unitsTable.SetGroup(group);
        }

        // Generates a list of all the unit buttons.
        public List<UnitsButton> GenerateUnitsButtonsList()
        {
            // Returns the unit buttons list.
            List<UnitsButton> unitsButtons = new List<UnitsButton>
            {
                unitsButton0,
                unitsButton1,
                unitsButton2,
                unitsButton3,
                unitsButton4,
                unitsButton5,
                unitsButton6,
            };

            return unitsButtons;
        }

        // Generates a list of all active unit buttons.
        public List<UnitsButton> GenerateUnitsButtonsActiveList()
        {
            // The unit buttons list.
            List<UnitsButton> unitsButtonsAll = GenerateUnitsButtonsList();
            List<UnitsButton> unitsButtonsActive = new List<UnitsButton>();

            // Goes through all the unit buttons to get which ones are active.
            for(int i = 0; i < unitsButtonsAll.Count; i++)
            {
                // The units button is active, so add it to the list.
                if (unitsButtonsAll[i].gameObject.activeSelf)
                {
                    unitsButtonsActive.Add(unitsButtonsAll[i]);
                }
            }

            // Return the active list.
            return unitsButtonsActive;
        }

        // Set all the unit buttons to be active or inactive.
        public void SetAllUnitButtonsActive(bool active)
        {
            unitsButton0.gameObject.SetActive(active);
            unitsButton1.gameObject.SetActive(active);
            unitsButton2.gameObject.SetActive(active);
            unitsButton3.gameObject.SetActive(active);
            unitsButton4.gameObject.SetActive(active);
            unitsButton5.gameObject.SetActive(active);
            unitsButton6.gameObject.SetActive(active);
        }

        // Makes all unit buttons active.
        public void EnableAllUnitsButtons()
        {
            SetAllUnitButtonsActive(true);
        }

        // Makes all unit buttons inactive.
        public void DisableAllUnitsButtons()
        {
            SetAllUnitButtonsActive(false);
        }

        // Returns 'true' if all active unit buttons are interactable.
        public bool IsAllActiveUnitButtonsInteractable()
        {
            // Gets a list of all active buttons.
            List<UnitsButton> activeButtons = GenerateUnitsButtonsActiveList();

            // Checks if all are interactable.
            bool allInteract = true;

            // There are active buttons.
            if(activeButtons.Count > 0)
            {
                // Goes through all units buttons.
                foreach (UnitsButton unitsButton in activeButtons)
                {
                    // If one button is unintactable, then none of them are.
                    if(!unitsButton.button.interactable)
                    {
                        allInteract = false;
                        break;
                    }
                }
            }
            else // No buttons in list, so none are interactable.
            {
                allInteract = false;
            }

            // Returns result.
            return allInteract;
        }

        // Updates the units buttons.
        public void UpdateConversionAndUnitsButtons(Meteor meteor)
        {
            // Clear all the buttons.
            ClearConversionAndUnitsButtons();

            // No meteor, so return null.
            if (meteor == null)
                return;

            // No conversion, so return null.
            if (meteor.conversion == null)
                return;

            // Conversion
            conversionText.text = stageManager.GenerateConversionQuestion(meteor);

            // If there is a slash, then this is a fraction.
            bool isFraction = conversionText.text.Contains("/");

            // Buttons
            // Makes a list of the unit buttons.
            List<UnitsButton> unitsButtons = new List<UnitsButton>()
            {
                unitsButton0,
                unitsButton1,
                unitsButton2, 
                unitsButton3, 
                unitsButton4, 
                unitsButton5, 
                unitsButton6
            };

            // A stack of the button inactive indexes.
            Stack<int> buttonInactiveIndexes = new Stack<int>();

            // Remove inactive buttons. There should always be at least one button.
            for(int i = 0; i < unitsButtons.Count; i++)
            {
                // If the button is not active and enabled, remove it.
                if(!unitsButtons[i].isActiveAndEnabled)
                {
                    // Add to the stack.
                    buttonInactiveIndexes.Push(i);
                }
            }

            // Don't remove any if all buttons would be removed.
            if(buttonInactiveIndexes.Count != unitsButtons.Count)
            {
                // Remove the relevant indexes.
                while(buttonInactiveIndexes.Count > 0 && unitsButtons.Count > 0)
                {
                    // Gets the index.
                    int index = buttonInactiveIndexes.Pop();

                    // Valid index check.
                    if(index >= 0 && index < unitsButtons.Count)
                        unitsButtons.RemoveAt(index);
                }
            }

            // If there are no buttons left, add in button 4 (the middle button).
            if (unitsButtons.Count == 0)
                unitsButtons.Add(unitsButton4);

            // If the unit buttons should be randomized.
            if (randomizeUnitButtons)
            {
                // The random button list.
                List<UnitsButton> randomButtonList = new List<UnitsButton>();

                // While there are values.
                while(unitsButtons.Count > 0)
                {
                    // Grabs the index, and gets the button.
                    int randIndex = Random.Range(0, unitsButtons.Count);
                    UnitsButton unitsButton = unitsButtons[randIndex];

                    // Adds the button to the random list, and removes it from the normal list.
                    randomButtonList.Add(unitsButton);
                    unitsButtons.Remove(unitsButton);
                }

                // Replace the list with the randomized buttons.
                unitsButtons = new List<UnitsButton>(randomButtonList);
            }


            // Gets set to 'true' if the right value is found.
            float trueOutputValue = meteor.GetConvertedValue();
            bool foundRightValue = false;

            // Calculates the true multiple for the true output value.
            float trueMult;

            // If the input value is 0, have the conversion be 1.0.
            if (meteor.conversion.inputValue == 0)
            {
                trueMult = 1;
            }
            else // The input isn't 0, so calculate the conversion.
            {
                trueMult = trueOutputValue / meteor.conversion.inputValue;
            }



            // Goes through all buttons, getting nad setting values.
            for (int i = 0; i < unitsButtons.Count && i < meteor.possibleOutputs.Length; i++)
            {
                // Used to calculate the conversion multiple for the possible output.
                float multiple;

                // If the possible output multiplier is 0, try calculating it.
                // If it isn't 0, take it directly.
                if(meteor.possibleOutputMults[i] == 0)
                {
                    // If the input value is 0, have the conversion be 1.0.
                    if (meteor.conversion.inputValue == 0)
                    {
                        multiple = 1;
                    }
                    else // The input isn't 0, so calculate the conversion.
                    {
                        multiple = meteor.possibleOutputs[i] / meteor.conversion.inputValue;
                    }
                }
                else
                {
                    // Gets the multipler from the meteor.
                    multiple = meteor.possibleOutputMults[i];
                }

                // Checks if the output if correct.
                bool correct = meteor.possibleOutputs[i] == trueOutputValue;

                // If this is the correct value, use the true mult instead.
                // This is to make sure the proper mult is displayed.
                if(correct)
                {
                    multiple = trueMult;
                }

                unitsButtons[i].SetMeasurementValueAndSymbol(meteor.possibleOutputs[i], multiple, 
                    meteor.conversion.GetOutputSymbol(), isFraction);              

                // If this is the true output value, set that it's been found.
                if (correct)
                {
                    foundRightValue = true;
                    unitsButtons[i].correctValue = true;
                }
            }

            // If the right value has not been found, set it to a random button.
            if(!foundRightValue)
            {
                // Gets random indexes for the output and the buttons.
                int outputIndex = Random.Range(0, meteor.possibleOutputs.Length);
                int buttonIndex;

                // If the indexes match, replace it on that button.
                // If they don't match, choose a random button.
                if(outputIndex >= 0 && outputIndex < unitsButtons.Count)
                    buttonIndex = outputIndex;
                else
                    buttonIndex = Random.Range(0, unitsButtons.Count);

                // Replaces the value at the provided index, and marks that button as having the true value.
                meteor.possibleOutputs[outputIndex] = trueOutputValue;
                
                // Updates the unit buttons with the true measurement and conversion mult.
                unitsButtons[buttonIndex].SetMeasurementValueAndSymbol(
                    trueOutputValue, trueMult, meteor.conversion.GetOutputSymbol(), isFraction);

                // Sets that this unit button is the correct value.
                unitsButtons[buttonIndex].correctValue = true;
            }
        }

        // Clears the units buttons.
        public void ClearConversionAndUnitsButtons(bool refreshConvesionDisplays)
        {
            // Conversion
            conversionText.text = "-";

            // Buttons
            unitsButton1.ClearButton();
            unitsButton2.ClearButton();
            unitsButton3.ClearButton();
            unitsButton4.ClearButton();
            unitsButton5.ClearButton();
            unitsButton6.ClearButton();
            unitsButton0.ClearButton();

            // Refreshes the conversion displays to make sure they match the units buttons.
            if(refreshConvesionDisplays)
            {
                stageManager.puzzleManager.puzzleUI.RefreshConversionDisplays();
            }
        }

        // Clears the units buttons.
        public void ClearConversionAndUnitsButtons()
        {
            ClearConversionAndUnitsButtons(true);
        }

        // Sets all unit buttons to be interactable.
        public void SetUnitButtonsInteractable(bool interactable)
        {
            unitsButton1.button.interactable = interactable;
            unitsButton2.button.interactable = interactable;
            unitsButton3.button.interactable = interactable;
            unitsButton4.button.interactable = interactable;
            unitsButton5.button.interactable = interactable;
            unitsButton6.button.interactable = interactable;
            unitsButton0.button.interactable = interactable;
        }

        // Set the unit buttons interactable to true.
        public void MakeUnitButtonsInteractable()
        {
            SetUnitButtonsInteractable(true);
        }

        // Set the unit buttons interactable to false.
        public void MakeUnitButtonsUninteractable()
        {
            SetUnitButtonsInteractable(false);
        }

        // Sets the unit buttons active by difficulty.
        public void SetUnitButtonsActiveByDifficulty(int difficulty)
        {
            // Goes by the difficulty.
            switch(difficulty)
            {
                // Easy - 3 Buttons
                case 1:
                case 2:
                    unitsButton0.gameObject.SetActive(false);
                    unitsButton1.gameObject.SetActive(false);
                    unitsButton2.gameObject.SetActive(true);
                    unitsButton3.gameObject.SetActive(true);
                    unitsButton4.gameObject.SetActive(true);
                    unitsButton5.gameObject.SetActive(false);
                    unitsButton6.gameObject.SetActive(false);
                    break;
                    
                    // Medium - 5 Buttons
                case 3:
                case 4:
                case 5:
                case 6:
                    unitsButton0.gameObject.SetActive(false);
                    unitsButton1.gameObject.SetActive(true);
                    unitsButton2.gameObject.SetActive(true);
                    unitsButton3.gameObject.SetActive(true);
                    unitsButton4.gameObject.SetActive(true);
                    unitsButton5.gameObject.SetActive(true);
                    unitsButton6.gameObject.SetActive(false);
                    break;
                    
                    // Hard - All Buttons
                default:
                case 7:
                case 8:
                case 9:
                    unitsButton0.gameObject.SetActive(true);
                    unitsButton1.gameObject.SetActive(true);
                    unitsButton2.gameObject.SetActive(true);
                    unitsButton3.gameObject.SetActive(true);
                    unitsButton4.gameObject.SetActive(true);
                    unitsButton5.gameObject.SetActive(true);
                    unitsButton6.gameObject.SetActive(true);
                    break;
            }
        }

        // Sets the unit butotns active by difficulty.
        public void SetUnitButtonsActiveByDifficulty()
        {
            SetUnitButtonsActiveByDifficulty(stageManager.GetDifficulty());
        }

        // SPEED
        // Sets the game to fast.
        public void SetToFastSpeed()
        {
            stageManager.SetToFastSpeed();
        }

        // Toggles fast speed.
        public void ToggleFastSpeed()
        {
            stageManager.ToggleFastSpeed();
        }

        // Sets the game to slow.
        public void SetToSlowSpeed()
        {
            stageManager.SetToSlowSpeed();
        }

        // Toggles slow speed.
        public void ToggleSlowSpeed()
        {
            stageManager.ToggleSlowSpeed();
        }


        // LASER

        // Shoots the laser with the value from the provided value.
        public void ShootLaserShot(float value)
        {
            // If the player can shoot a laser shot, allow them to shoot.
            if(stageManager.CanPlayerShootLaserShot())
            {
                stageManager.player.ShootLaserShot(value);
            }
        }

        // Shoots the laser with the value from the provided units button.
        public void ShootLaserShot(UnitsButton unitsButton)
        {
            // The laser shot.
            LaserShot laserShot = null;

            // If the button is automatically correct, pull the exact value.
            if(unitsButton.correctValue)
            {
                // Gets the meteor.
                Meteor meteor = stageManager.meteorTarget.GetMeteor();

                // If the player can shoot a laser shot, allow them to shoot.
                if (stageManager.CanPlayerShootLaserShot())
                {
                    // Use meteor's value if true. Use button's value if false.
                    if (meteor != null)
                        laserShot = stageManager.player.ShootLaserShot(meteor.GetConvertedValue(), unitsButton.laserColor);
                    else
                        laserShot = stageManager.player.ShootLaserShot(unitsButton.GetMeasurementValue(), unitsButton.laserColor);
                }

            }
            else // Button is not automatically correct.
            {
                // If the player can shoot, allow them to shoot.
                if (stageManager.CanPlayerShootLaserShot())
                {
                    laserShot = stageManager.player.ShootLaserShot(unitsButton.GetMeasurementValue(), unitsButton.laserColor);
                }
            }

            // If a laser shot was fired, save the units button to it.
            if(laserShot != null)
            {
                laserShot.unitsButton = unitsButton;
            }
            
        }


        // EVENTS
        // Start the unit button multiplier reveals.
        public void StartUnitButtonMultipleReveals()
        {
            unitsButton0.StartMultipleReveal();
            unitsButton1.StartMultipleReveal();
            unitsButton2.StartMultipleReveal();
            unitsButton3.StartMultipleReveal();
            unitsButton4.StartMultipleReveal();
            unitsButton5.StartMultipleReveal();
            unitsButton6.StartMultipleReveal();
        }

        // Ends the unit button multiple reveals.
        public void EndUnitButtonMultipleReveals()
        {
            unitsButton0.EndMultipleReveal();
            unitsButton1.EndMultipleReveal();
            unitsButton2.EndMultipleReveal();
            unitsButton3.EndMultipleReveal();
            unitsButton4.EndMultipleReveal();
            unitsButton5.EndMultipleReveal();
            unitsButton6.EndMultipleReveal();
        }

        // Ends the unit button multiplier reveals.

        // Called when the phase has changed.
        public void OnPhaseChanged()
        {
            // TODO: this animation seems to getting overwritten by the happy animation.
            // TODO: play sound effect.
            
            // Angry animation.
            // PlayPartnersAnimation(CharacterIcon.charIconAnim.angry);
        }

        // Called when a meteor has been killed.
        public void OnMeteorKilled(Meteor meteor)
        {
            // Make the unit buttons uninteractable and clears them since the meteor has been killed.
            MakeUnitButtonsUninteractable();
            // ClearConversionAndUnitsButtons();
        }

        // Called when a barrier has been damaged.
        public void OnBarrierDamaged()
        {
            PlayPartnersAnimation(CharacterIcon.charIconAnim.shocked);
        }

        // Called when the surface has been damaged.
        public void OnSurfaceDamaged()
        {
            // Play the partner animation.
            PlayPartnersAnimation(CharacterIcon.charIconAnim.sad);

            // Play a red edge glow animation.
            if(UseScreenEffects)
                screenEffects.PlayEdgeGlowRedAnimation();
        }

        // SCREEN EFFECTS
        // Returns 'true' if screen effects should be used.
        public bool UseScreenEffects
        {
            get
            {
                return useScreenEffects;
            }
        }

        // STAGE END
        // Updates the HUD one last time after the stage ends.
        public void OnStageEnd()
        {
            // Updates the HUD.
            UpdateHUD();

            // Play the offline animation.
            PlayPartnersAnimation(CharacterIcon.charIconAnim.offline);

            // Refreshes the speed button's icon since the game speed is back to normal.
            speedButton.RefreshButtonIcon();

            // Make sure the multipliers are all available.
            EndUnitButtonMultipleReveals();
        }

        // Called when the stage has been won.
        public void OnStageWon()
        {
            CloseAllWindows();
            stageWonWindow.gameObject.SetActive(true);

            // Set the time and score text.
            stageWonWindow.SetTimeText(stageManager.stageTime);
            stageWonWindow.SetScoreText(stageManager.stageFinalScore);

            // The offline animation is played instead.
            // Play happy animation.
            // PlayPartnersAnimation(CharacterIcon.charIconAnim.happy);
        }

        // Called when the stage has been lost.
        public void OnStageLost()
        {
            CloseAllWindows();
            stageLostWindow.gameObject.SetActive(true);

            // Set the time and score text.
            stageLostWindow.SetTimeText(stageManager.stageTime);
            stageLostWindow.SetScoreText(stageManager.stageFinalScore);

            // The offline aniamtion is played instead.
            // // Play sad animation.
            // PlayPartnersAnimation(CharacterIcon.charIconAnim.sad);
        }

        // Called to restart the stage.
        public void RestartStage()
        {
            stageManager.ResetStage();
        }

        // Called when the start has been restarted.
        public void OnStageReset()
        {
            CloseAllWindows();
            ClearConversionAndUnitsButtons();

            // Switch to neutral expressions.
            PlayPartnersAnimation(CharacterIcon.charIconAnim.neutral);
        }

        // Goes to the world.
        public void ToWorld()
        {
            stageManager.ToWorld();
        }


        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
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