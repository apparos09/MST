using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RM_MST
{
    // The stage manager.
    public class StageManager : GameplayManager
    {
        // the instance of the class.
        private static StageManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        [Header("StageManager")]
        // The stage user interface.
        public StageUI stageUI;

        // The audio for the stage.
        public StageAudio stageAudio;

        // The stage.
        public Stage stage;

        // The stage's name.
        public string stageName;

        // The stage's background number.
        [Tooltip("The stage's background number.")]
        public int bgdNumber = 0;

        // The stage's BGM number.
        [Tooltip("The stage's BGM number.")]
        public int bgmNumber = 0;

        // The stage index.
        public int stageIndex = -1;

        // The difficulty.
        // TODO: make private.
        public int difficulty = 0;

        // The base difficulty for the game.
        public int baseDifficulty = 0;

        // The maximum difficulty.
        public const int DIFFICULTY_MAX = 9;

        // Adjusts the difficulty dynamically by the number of losses the player has.
        private bool dynamicDifficulty = true;

        // If 'true', the game applies the difficulty changes.
        private bool applyDifficultyChanges = true;

        // The total number of losses.
        public int losses = 0;

        // The phase for the game.
        // TODO: make private.
        public int phase = 1;

        // The maximum phase value.
        public const int PHASE_MAX = 4;

        // Applies changes to the game difficulty based on the phase.
        private bool applyPhaseDifficultyChanges = false;

        // The score that must be met to win the game.
        public float pointsGoal = 1000.0F;

        // The number of consecutive correct answers given by the player.
        public int consecutiveSuccesses = 0;

        // The the threshold for the multipier effect threshold.
        public const int MULT_REVEAL_EFFECT_THRESHOLD = 3;

        // If 'true', the text reveal mechanic is applied as part of the difficulty.
        private bool applyMultReveals = true;

        [Header("StageManager/Objects, Stats")]

        // The player for the stage.
        public PlayerStage player;

        // The target for the meteor.
        public MeteorTarget meteorTarget;

        // If 'true', the player can only shoot when a meteor is being targeted exactly.
        private bool shootOnExactTargetOnly = true;

        // The stage surface.
        public StageSurface stageSurface;

        // The barriers for the stage.
        public List<Barrier> stageBarriers;

        // If 'true', a barrier is restored on a phase change.
        private bool restoreBarrierOnPhaseChange = true;

        // The timer for the stage.
        public float stageTime = 0.0F;

        // If 'true', the time display is optimized to only updated when it should be changed.
        // If 'false', the time display is updated every frame, even if the display string won't be changed.
        private bool optimizeTimeDisplayUpdate = true;

        // The normal game time scale.
        private float NORMAL_GAME_TIME_SCALE = 1.0F;

        // The fast game time scale.
        private float FAST_GAME_TIME_SCALE = 2.0F;

        // Times how long the game is going fast for.
        private float fastStageTime = 0.0F;

        // The slow game time scale.
        private float SLOW_GAME_TIME_SCALE = 0.5F;

        // Times how long the game is going slow for.
        private float slowStageTime = 0.0F;

        // If 'true', the audio speed is changed with the game speed.
        private bool adjustAudioSpeed = true;

        // The final score for the stage.
        public float stageFinalScore = 0.0F;

        // Shows if the stage is cleared.
        public bool cleared = false;

        // The world scene.
        public string worldScene = "WorldScene";

        // Gets set to 'true' when the game is running.
        private bool runningGame = false;

        [Header("StageManager/Conversions")]
        // The units used for the stage.
        public List<UnitsInfo.unitGroups> stageUnitGroups = new List<UnitsInfo.unitGroups>();

        // The conversions for the stage.
        public List<UnitsInfo.UnitsConversion> conversions = new List<UnitsInfo.UnitsConversion>();

        // The minimum units input value.
        public const float UNITS_INPUT_VALUE_MIN = 0.001F;

        // The maximum units input value.
        public const float UNITS_INPUT_VALUE_MAX = 100.0F;

        // Changes the max units input value by the game's difficulty.
        private bool setUnitsInputMaxByDifficulty = true;

        // If 'true', random inputs can be decimal values.
        private bool allowRandomInputDecimals = false; // Set to false from feedback.

        // The number of decimal places for the units.
        // TODO: maybe limit to 2 decimal places.
        public const int UNITS_DECIMAL_PLACES = 3;

        // If 'true', random inputs are limited to whole numbers for non-metric units.
        private bool limitRandomUnitInputs = true;

        // If 'true', fractions are used in the game.
        private bool useFractions = true;

        // The fraction display chance.
        public const float FRACTION_DISPLAY_CHANCE = 0.5F;

        [Header("Meteors")]
        // The meteor spawn rate. Changes with difficulty.
        private float meteorSpawnRate = 1.0F;

        // If 'true', the first meteor spawn is delayed when the stage starts.
        private bool delayFirstMeteorSpawn = true;

        // The timer used for spawning meteors.
        private float meteorSpawnTimer = 0.0F;

        // The meteor fall speed factor. Changes with difficulty.
        private float meteorSpeedMax = 1.0F;

        // The move distance from the point where it was hit (focus mode only). Changes based on answer time.
        private float meteorMoveDist = 3.0F;

        // The meteor prefabs.
        public List<Meteor> meteorPrefabs = new List<Meteor>();

        // TODO: The meteor pool. May not need this.
        // If you use this, make sure to account for the FindAndDestroyAllActiveMeteors function.
        // private List<Meteor> meteorPool = new List<Meteor>();

        // The total number of meteors that can be active at once.
        private int ACTIVE_METEORS_COUNT_MAX = 12;

        // The total number of meteors that can be used for focus mode.
        private int METEORS_FOCUS_MODE_MAX = 10;

        // If 'true', the closest meteor is constantly checked for potential retargeting.
        // This addresses an issue where a meteor behind another meteor gets targeted.
        private bool constClosestMeteorCheck = true;

        // The maximum distance for the warning sound to play for a meteor approaching the Earth's surface.
        protected const float METEOR_WARNING_SFX_MAX_DIST = 3.75F;

        // If 'true', the player can manually target meteors (focus mode only).
        private bool allowManualMeteorTargeting = true;

        // Plays the meteor warning sound effect when a meteor is too close to the surface.
        private bool useMeteorWarningSfx = true;

        [Header("Puzzles")]

        // The puzzle manager for the stage.
        public PuzzleManager puzzleManager;

        // The puzzle UI.
        public PuzzleUI puzzleUI;

        [Header("Combo")]
        // The combo for the stage.
        public int combo = 0;

        // The highest combo achieved.
        public int highestCombo = 0;

        // The timer for triggering a combo.
        private float comboTimer = 0;

        // The maximum time for the combo (in seconds).
        // Raised from 7 seconds to 16 seconds to account for multiplier fade-ins.
        private const float COMBO_TIMER_MAX = 16.0F;

        // The combo display.
        public ComboDisplay comboDisplay;

        // Constructor
        private StageManager()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        protected override void Awake()
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

                // Physics Ignores
                // Gets the laser shot and ignore laser shot layer.
                int layer1 = LayerMask.NameToLayer("Laser Shot");
                int layer2 = LayerMask.NameToLayer("Ignore Laser Shot");

                // If both layers have been found, set up the ignores.
                if (layer1 != -1 && layer2 != -1)
                {
                    Physics2D.IgnoreLayerCollision(layer1, layer2, true);
                }
            }
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            // Checks if the default mode should be kept by seeing if gameplay info exists.
            // This is used to avoid the mode being changed to the default in the debug scene.
            bool overwriteMode = !GameplayInfo.Instantiated;
            gameMode tempMode = gameplayMode;

            // Call base start.
            base.Start();

            // Gets the UI instance.
            if (stageUI == null)
                stageUI = StageUI.Instance;

            // Gets the audio instance.
            if (stageAudio == null)
                stageAudio = StageAudio.Instance;

            // Gets the puzle manager instance.
            if (puzzleManager == null)
                puzzleManager = PuzzleManager.Instance;

            // Gets the puzle UI instance.
            if (puzzleUI == null)
                puzzleUI = PuzzleUI.Instance;



            // Restore the intented mode.
            if(overwriteMode)
            {
                gameplayMode = tempMode;
            }

            // If the gameplay info has been instantiated.
            if (GameplayInfo.Instantiated)
            {
                // If the mode should be overwritten.
                if (overwriteMode)
                {
                    // Overwrite gameplay info and the default gameplay mode.
                    gameInfo.gameMode = tempMode;
                    gameplayMode = tempMode;
                }

                // Load the stage information.
                gameInfo.LoadStageInfo(this);
            }
            else
            {
                // If there are no stage units, generate the group list.
                if (stageUnitGroups.Count == 0)
                    stageUnitGroups = UnitsInfo.GenerateUnitGroupsList();
            }

            // If the barrier list is empty, find all the barriers.
            if (stageBarriers.Count == 0)
            {
                stageBarriers = new List<Barrier>(FindObjectsOfType<Barrier>());
            }

            // Just fill the stage name with elipses if there is no name.
            if (stageName == "")
                stageName = "...";

            // Sets the difficulty and the phase.
            SetDifficulty(difficulty, true);
            SetPhase(phase);

            // If the difficulty should be dynamically adjusted.
            if (dynamicDifficulty)
                AdjustDifficultyByLosses();

            // Sets the stage background and surface.
            // The background number is reused for the surface sprite set.
            stage.SetBackground(bgdNumber);
            stage.surface.SetSurfaceSpriteSet(bgdNumber);

            // Plays the background music using the provided BGM number.
            stageAudio.PlayStageBgm(bgmNumber);
        }

        // The function called after the start function.
        protected override void LateStart()
        {
            base.LateStart();

            // This is done here to make sure that the unit info object has been loaded.

            // Getting the conversions.
            conversions = new List<UnitsInfo.UnitsConversion>();
            List<UnitsInfo.unitGroups> usedGroups = new List<UnitsInfo.unitGroups>();


            // Goes through all the stage unit groups.
            foreach (UnitsInfo.unitGroups group in stageUnitGroups)
            {
                // If the group hasn't been used yet, get the group.
                if (!usedGroups.Contains(group))
                {
                    conversions.AddRange(UnitsInfo.Instance.GetGroupConversionListCopy(group));
                    usedGroups.Add(group);
                }
            }

            // Closes all the windows, and clears the buttons.
            stageUI.CloseAllWindows();
            stageUI.ClearConversionAndUnitsButtons();

            // Delays the first meteor to give time for the game to begin.
            if (delayFirstMeteorSpawn)
            {
                meteorSpawnTimer = 2.5F; // Starting wait time.
            }

            // The game is now running.
            runningGame = true;

            // Checks if tutorials are enabled.
            if (IsUsingTutorial())
            {
                // Checks that there is no tutorial running.
                if(!IsTutorialRunning())
                {
                    // If the first stage tutorial has not been cleared, load it.
                    if (!tutorials.clearedFirstStageTutorial)
                    {
                        tutorials.LoadFirstStageTutorial();
                    }
                }
            }

        }

        // Gets the instance.
        public static StageManager Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<StageManager>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("WorldManager (singleton)");
                        instance = go.AddComponent<StageManager>();
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

        // The stage start info.
        public void ApplyStageStartInfo(GameplayInfo.StageStartInfo stageStartInfo)
        {
            // If the information is valid.
            if (stageStartInfo.valid)
            {
                stageName = stageStartInfo.name;

                stageUnitGroups = stageStartInfo.stageUnitGroups;
                puzzleManager.pType = stageStartInfo.stagePuzzleType;

                bgdNumber = stageStartInfo.bgdNumber;
                bgmNumber = stageStartInfo.bgmNumber;

                difficulty = stageStartInfo.difficulty;
                losses = stageStartInfo.losses;
                stageIndex = stageStartInfo.index;
            }
            else
            {
                Debug.LogWarning("The stage info was not marked as valid.");
            }

            // If the stage units list is empty, generate a list of all types.
            if (stageUnitGroups.Count <= 0)
                stageUnitGroups = UnitsInfo.GenerateUnitGroupsList();

            // Sets the difficulty using the proper function.
            SetDifficulty(difficulty, true);
        }

        // Returns the game mode.
        public gameMode GetGameMode()
        {
            return gameplayMode;
        }

        // Returns 'true' if the game is in focus mode.
        public bool UsingFocusMode()
        {
            return gameplayMode == gameMode.focus;
        }

        // Returns 'true' if the game is in rush mode.
        public bool UsingRushMode()
        {
            return gameplayMode == gameMode.rush;
        }

        // Gets the difficulty.
        public int GetDifficulty()
        {
            return difficulty;
        }

        // Sets the difficulty for the game.
        public void SetDifficulty(int difficultyLevel, bool setBaseDifficulty)
        {
            // Sets the difficulty.
            difficulty = Mathf.Clamp(difficultyLevel, 1, DIFFICULTY_MAX);

            // If the base difficulty should be set, set it.
            if (setBaseDifficulty)
                baseDifficulty = difficulty;

            // If the difficulty changes should be applied.
            if (applyDifficultyChanges)
            {
                // Changes parameters based on the difficulty.
                // TODO: implement.
                switch (difficulty)
                {
                    default:
                    case 1:
                        meteorSpawnRate = 3.30F;
                        meteorSpeedMax = 0.10F;
                        pointsGoal = 450.0F;
                        break;

                    case 2:
                        meteorSpawnRate = 3.25F;
                        meteorSpeedMax = 0.15F;
                        pointsGoal = 500.0F;
                        break;

                    case 3:
                        meteorSpawnRate = 3.20F;
                        meteorSpeedMax = 0.20F;
                        pointsGoal = 550.0F;
                        break;

                    case 4:
                        meteorSpawnRate = 3.15F;
                        meteorSpeedMax = 0.25F;
                        pointsGoal = 600.0F;
                        break;

                    case 5:
                        meteorSpawnRate = 3.10F;
                        meteorSpeedMax = 0.30F;
                        pointsGoal = 650.0F;
                        break;

                    case 6:
                        meteorSpawnRate = 3.05F;
                        meteorSpeedMax = 0.35F;
                        pointsGoal = 700.0F;
                        break;

                    case 7:
                        meteorSpawnRate = 3.00F;
                        meteorSpeedMax = 0.40F;
                        pointsGoal = 750.0F;
                        break;

                    case 8:
                        meteorSpawnRate = 2.95F;
                        meteorSpeedMax = 0.45F;
                        pointsGoal = 800.0F;
                        break;

                    case 9:
                        meteorSpawnRate = 2.90F;
                        meteorSpeedMax = 0.50F;
                        pointsGoal = 850.0F;
                        break;
                }

                // Sets the unit buttons active/inactive by difficulty.
                stageUI.SetUnitButtonsActiveByDifficulty(difficulty);

                // Makes sure the conversion display count match the number of unit buttons.
                puzzleUI.RefreshConversionDisplays();
            }
        }

        // Returns the base difficulty.
        public float GetBaseDifficulty()
        {
            return baseDifficulty;
        }

        // Adjusts the difficulty by the amount of losses.
        public void AdjustDifficultyByLosses()
        {
            // For every 3 losses, lower the difficulty by 1.
            int quotient = losses / 3;

            // Set the difficulty with the base difficulty as a basis.
            SetDifficulty(baseDifficulty - quotient, false);
        }

        // Gets the phase.
        public int GetPhase()
        {
            return phase;
        }

        // Sets the phase
        public void SetPhase(int newPhase)
        {
            // Saves the old phase, and checks against the new phase.
            int oldPhase = phase;
            phase = Mathf.Clamp(newPhase, 1, PHASE_MAX);

            // The phase has changed.
            if (phase != oldPhase)
            {
                OnPhaseChanged();
            }
        }

        // Sets the game phase by the game progress.
        public void SetPhaseByPlayerPointsProgress()
        {
            // Gets the percent.
            float percent = GetPlayerPointsProgress();

            // Checks the percent.
            if (percent > 0.75F) // Phase 4
            {
                SetPhase(4);
            }
            else if (percent > 0.50F) // Phase 3
            {
                SetPhase(3);
            }
            else if (percent > 0.25F) // Phase 3
            {
                SetPhase(2);
            }
            else // Phase 1
            {
                SetPhase(1);
            }
        }

        // Called when the phase has changed.
        public void OnPhaseChanged()
        {
            // Applies phase difficultly changes.
            if (applyPhaseDifficultyChanges)
            {
                // Resets the difficulty (doesn't change base difficulty).
                SetDifficulty(difficulty, false);

                // Sets the phase.
                switch (phase)
                {
                    case 1:
                        // ...
                        // There are no changes here since the values have been reset back to their defaults...
                        // From the SetDifficulty function.
                        break;

                    case 2:
                        meteorSpawnRate -= 0.125F;
                        meteorSpeedMax += 0.25F;
                        break;

                    case 3:
                        meteorSpawnRate -= 0.25F;
                        meteorSpeedMax += 0.5F;
                        break;

                    case 4:
                        meteorSpawnRate -= 0.375F;
                        meteorSpeedMax += 0.75F;
                        break;
                }


                // The spawn rate is less than 0, so cap it. 
                if (meteorSpawnRate < 0)
                {
                    meteorSpawnRate = 0;
                }

                // Play the phase sound effect if the phase has changed.
                // Don't play it if it's phase 1.
                if (phase > 1)
                {
                    stageAudio.PlayPhaseSfx();
                }
            }

            // If a barrier should be restored on a phase change.
            if (restoreBarrierOnPhaseChange)
            {
                // Lists of dead barriers and damaged barriers.
                List<Barrier> deadBarriers = new List<Barrier>();
                List<Barrier> damagedBarriers = new List<Barrier>();

                // Goes through all barriers.
                for (int i = 0; i < stageBarriers.Count; i++)
                {
                    // Barrier does not exist.
                    if (stageBarriers[i] == null)
                        continue;

                    // If the barrier is dead, add it to the dead list.
                    if (stageBarriers[i].IsDead())
                    {
                        deadBarriers.Add(stageBarriers[i]);
                    }
                    // The barrier is damaged, so add it to the list.
                    else if (!stageBarriers[i].IsHealthAtMax())
                    {
                        damagedBarriers.Add(stageBarriers[i]);
                    }
                }

                // The barrier to be restored.
                Barrier barrier = null;

                // Gets a random barrier - prioritize dead barriers for restoration.
                // Dead
                if (deadBarriers.Count > 0)
                {
                    barrier = deadBarriers[Random.Range(0, deadBarriers.Count)];
                }
                // Damage
                else if (damagedBarriers.Count > 0)
                {
                    barrier = damagedBarriers[Random.Range(0, damagedBarriers.Count)];
                }

                // If the barrier exists, restore it.
                if (barrier != null)
                {
                    // Restore the barrier.
                    barrier.RestoreBarrier();
                }
            }


            // The phase has changed.
            stageUI.OnPhaseChanged();
        }


        // METEORS
        // Spawns a meteor.
        public Meteor SpawnMeteor()
        {
            // No meteor prefabs.
            if (meteorPrefabs.Count == 0)
            {
                Debug.LogError("No meteor prefabs available.");
                return null;
            }

            // Gets a copy of the meteors active list.
            List<Meteor> meteorsActive = Meteor.GetMeteorsActiveListCopy();

            // Maximum meteor count met.
            if (meteorsActive.Count >= ACTIVE_METEORS_COUNT_MAX)
            {
                // Debug.LogWarning("Maximum meteor count met.");
                return null;
            }

            // Generates a meteor.
            int randomIndex = Random.Range(0, meteorPrefabs.Count);
            Meteor meteor = Instantiate(meteorPrefabs[randomIndex]);

            // Generates a random conversion for the meteor, and generates alternate outputs.
            meteor.conversion = GenerateRandomConversionFromList();
            SetRandomConversionInputValue(meteor.conversion);
            meteor.GenerateAlternateOutputs();
            unitsInfo.gameObject.SetActive(true);

            // Set the parent.
            if (stage.meteorParent != null)
                meteor.transform.parent = stage.meteorParent.transform;

            // Generate the spawn point.
            meteor.spawnPoint = stage.GenerateMeteorSpawnPoint();

            // Called when the meteor has spawned.
            meteor.OnSpawn();

            // Add to the list.
            meteorsActive.Add(meteor);

            // Return the meteor.
            return meteor;
        }

        // Called when a meteor has been targeted.
        public void OnMeteorTargeted(Meteor meteor)
        {
            // Meteor is set.
            if (meteor != null)
            {
                // Conversion is set.
                if (meteor.conversion != null)
                {
                    stageUI.UpdateUnitsTable(meteor.conversion.group);
                }
            }

            // Updates the unit buttons with the provied meteor.
            stageUI.UpdateConversionAndUnitsButtons(meteor);

            // Call the puzzle manager's function.
            puzzleManager.OnMeteorTargeted(meteor);

        }

        // Called when a meteor is destroyed.
        public void OnMeteorKilled(Meteor meteor)
        {
            stageUI.OnMeteorKilled(meteor);
            puzzleManager.OnMeteorKilled(meteor);
        }

        // Called when a meteor fails to be destroyed.
        public void OnMeteorSurivived(Meteor meteor)
        {
            // Reset the combo.
            combo = 0;
            comboTimer = 0;
        }


        // Gets the closest meteor from the active list.
        public Meteor GetClosestMeteor()
        {
            // The meteor and the distance.
            Meteor meteor = null;
            float meteorDist = 0;

            // Gets a copy of the meteors active list.
            List<Meteor> meteorsActive = Meteor.GetMeteorsActiveListCopy();

            // Goes through all meteors from end to start.
            for (int i = meteorsActive.Count - 1; i >= 0; i--)
            {
                // If the index is null, remove it.
                // This isn't necessary anymore, but I don't feel like removing it.
                if (meteorsActive[i] == null)
                {
                    meteorsActive.RemoveAt(i);
                }
                // Don't target meteors that are dead.
                else if (meteorsActive[i].IsDead())
                {
                    meteorsActive.RemoveAt(i);
                }
                else // Check the position.
                {
                    // If there is no meteor, track it by default.
                    if (meteor == null)
                    {
                        // Get the meteor and the distance.
                        meteor = meteorsActive[i];

                        // Adjusts the distance to ignore the x and z components.
                        Vector3 meteorAdjustPos = meteor.transform.position;
                        meteorAdjustPos.x = stageSurface.transform.position.x;
                        meteorAdjustPos.z = stageSurface.transform.position.z;

                        // Save the distance.
                        meteorDist = Vector3.Distance(meteorAdjustPos, stageSurface.gameObject.transform.position);
                    }
                    else // Compare distances.
                    {
                        // Grabs the two meteors.
                        Meteor m1 = meteor;
                        Meteor m2 = meteorsActive[i];

                        // The adjusted positions for m2.
                        // They all have the same x and z pos as the surface for this comparison.
                        // M2
                        Vector3 m2AdjustPos = m2.transform.position;
                        m2AdjustPos.x = stageSurface.transform.position.x;
                        m2AdjustPos.z = stageSurface.transform.position.z;

                        // Gets the distances.
                        float m1Dist = meteorDist;
                        float m2Dist = Vector3.Distance(m2AdjustPos, stageSurface.gameObject.transform.position);

                        // Meteor 1 is Closer
                        if (m1Dist < m2Dist)
                        {
                            meteor = m1;
                            meteorDist = m1Dist;
                        }
                        // Meteor 2 is Closer
                        else if (m1Dist > m2Dist)
                        {
                            meteor = m2;
                            meteorDist = m2Dist;
                        }
                    }
                }
            }

            // Returns the meteor.
            return meteor;
        }

        // Returns a random meteor.
        public Meteor GetRandomMeteor()
        {
            // Gets a list of active meteors.
            List<Meteor> meteors = Meteor.GetMeteorsActiveListCopy();

            // The meteor to be returned.
            Meteor meteor;

            // If there are meteors, pick a random one.
            if(meteors.Count > 0)
            {
                int index = Random.Range(0, meteors.Count);
                meteor = meteors[index];
            }
            else // No meteors, so return none.
            {
                meteor = null;
            }

            return meteor;
        }

        // Returns 'true' if the player can manually target meteors.
        public bool IsManualMeteorTargetingEnabled()
        {
            return allowManualMeteorTargeting;
        }

        // Refreshes the meteors active list to remove null values.
        public void RefreshMeteorsActiveList()
        {
            Meteor.RefreshMeteorsActiveList();
        }

        // Destroys all the meteors in the list. Some meteors may not be in the list for some reason.
        public void KillAllMeteorsInList()
        {
            Meteor.KillAllMeteorsInActiveList();
        }

        // Finds and kills all meteors. This is more accurate than KillAllMeteorsActive().
        public void FindAndKillAllMeteors()
        {
            // Gets all the meteors.
            Meteor[] meteors = FindObjectsOfType<Meteor>(true);

            // Kills all the meteors.
            foreach (Meteor meteor in meteors)
            {
                meteor.Kill();
            }
        }

        // Generates a units conversion. This isn't needed anymore.
        public UnitsInfo.UnitsConversion GenerateRandomConversionClass()
        {
            // The conversion to be returned.
            UnitsInfo.UnitsConversion conversion;

            // The group the units are part of.
            UnitsInfo.unitGroups group = (UnitsInfo.unitGroups)Random.Range(0, UnitsInfo.UNIT_GROUPS_COUNT);

            // The group.
            switch (group)
            {
                default:
                case UnitsInfo.unitGroups.none:
                    conversion = null;
                    break;

                case UnitsInfo.unitGroups.lengthImperial: // Length
                case UnitsInfo.unitGroups.lengthMetric:
                    conversion = new UnitsInfo.LengthConversion();
                    break;

                case UnitsInfo.unitGroups.weightImperial: // Weight
                case UnitsInfo.unitGroups.weightMetric:
                    conversion = new UnitsInfo.WeightConversion();
                    break;

                case UnitsInfo.unitGroups.time: // Time
                    conversion = new UnitsInfo.TimeConversion();
                    break;

                case UnitsInfo.unitGroups.capacity: // Capacity
                    conversion = new UnitsInfo.CapacityConversion();
                    break;
            }

            // Conversion is unknown.
            if (conversion == null)
                return conversion;

            // Sets the group.
            conversion.group = group;

            // Returns the conversion.
            return conversion;
        }

        // Generates a random conversion from the list.
        public UnitsInfo.UnitsConversion GenerateRandomConversionFromList()
        {
            // Conversion object.
            UnitsInfo.UnitsConversion conversion;

            // Checks if there are conversions to pick from.
            if (conversions.Count > 0)
            {
                // The original conversion.
                // These conversions are copied as to prevent the original conversion objects from being altered.
                UnitsInfo.UnitsConversion origConvert = conversions[Random.Range(0, conversions.Count)];

                // Checks the conversion type.
                if (origConvert is UnitsInfo.WeightConversion) // Weight
                {
                    conversion = new UnitsInfo.WeightConversion((UnitsInfo.WeightConversion)origConvert);
                }
                else if (origConvert is UnitsInfo.LengthConversion) // Length
                {
                    conversion = new UnitsInfo.LengthConversion((UnitsInfo.LengthConversion)origConvert);
                }
                else if (origConvert is UnitsInfo.TimeConversion) // Time
                {
                    conversion = new UnitsInfo.TimeConversion((UnitsInfo.TimeConversion)origConvert);
                }
                else if (origConvert is UnitsInfo.CapacityConversion) // Capacity
                {
                    conversion = new UnitsInfo.CapacityConversion((UnitsInfo.CapacityConversion)origConvert);
                }
                else
                {
                    conversion = null;
                }


            }
            else // No conversions, so generate an empty conversion.
            {
                // conversion = GenerateRandomConversionClass();
                conversion = null;
            }

            // Returns the conversion.
            return conversion;
        }

        // Gets the minimum conversion input value.
        public float GetMinimumConversionInputValue()
        {
            return UNITS_INPUT_VALUE_MIN;
        }

        public float GetMaximumConversionInputValue()
        {
            // The value to be returned.
            float value;

            // If the units input max value should depend on the difficulty. 
            if(setUnitsInputMaxByDifficulty)
            {
                // Gets the difficulty.
                switch(GetDifficulty())
                {
                    default:
                        // Sets to the max value by default if one isn't set for this difficulty.
                        value = UNITS_INPUT_VALUE_MAX;
                        break;

                    case 1:
                        value = 10.0F;
                        break;

                    case 2:
                        value = 20.0F;
                        break;

                    case 3:
                        value = 40.0F;
                        break;

                    case 4:
                        value = 60.0F;
                        break;

                    case 5:
                        value = 80.0F;
                        break;

                    case 6: // Highest difficulty used by the game.
                        value = 100.0F;
                        break;

                    case 7:
                        value = 100.0F;
                        break;

                    case 8:
                        value = 100.0F;
                        break;

                    case 9:
                        value = 100.0F;
                        break;
                }
            }
            else
            {
                // Sets to the max value.
                value = UNITS_INPUT_VALUE_MAX;
            }

            return value;
        }

        // Generates a random input value for the conversion.
        public float SetRandomConversionInputValue(UnitsInfo.UnitsConversion conversion)
        {
            // The conversion object doesn't exist.
            if (conversion == null)
            {
                Debug.LogWarning("The conversion object is null. Returning 0.");
                return 0.0F;
            }

            // Generates the value, and gets the factor for the number of decimal places.
            // This now calls a function that changes the maximum value based on the game's difficulty.
            float value = Random.Range(UNITS_INPUT_VALUE_MIN, GetMaximumConversionInputValue());

            // Round the value, and cap it at 3 decimal places.
            value = util.CustomMath.Round(value, UNITS_DECIMAL_PLACES);

            // If random values cannot be decimals, round up to the nearest value.
            if (!allowRandomInputDecimals)
                value = Mathf.Ceil(value);

            // If random unit inputs should be limited.
            if (limitRandomUnitInputs)
            {
                // If these are not metric units, round up to a whole number.
                if (!UnitsInfo.IsMetricUnits(conversion.group))
                {
                    value = Mathf.Ceil(value);
                }
            }

            // Set and return the value.
            conversion.inputValue = value;
            return value;
        }

        // Tries to fix floating point imprecision through rounding and float-double conversion.
        // This is not guranteed to work, since the floating point error digit is not in a consistent place...
        // Within values that cause these errors to occur.
        public static float TryFixFloatingPointImprecision(float value, int decimalPlaces)
        {
            // Converts the value to a string.
            string valueStr = value.ToString();

            // The exponent multiplier.
            // If the decimal points provided are 0 or less, set to 1.
            int dpCorrected = (decimalPlaces > 0) ? decimalPlaces : 1;
            double expMult = Mathf.Pow(10.0F, dpCorrected);

            // If there is no decimal point, that means the provided float is a whole number.
            // As such, return the value as is.
            if (!valueStr.Contains("."))
            {
                return value;
            }

            // The whole part and the decimal part of the value.
            double wholePart, decimalPart;

            // Tries to get the whole part of the value.
            if (!double.TryParse(valueStr.Substring(0, valueStr.IndexOf(".")), out wholePart))
            {
                Debug.LogWarning("Conversion failure finding the whole portion of the value.");
            }

            // Tries to get the decimal part of the value. Includes 0 and the decimal point.
            if (!double.TryParse("0." + valueStr.Substring(valueStr.IndexOf(".") + 1), out decimalPart))
            {
                Debug.LogWarning("Conversion failure finding the decimal portion of the value.");
            }

            // Round the decimal part to 5 decimal places (not using Mathf.Pow since it uses floats).
            double decimalRounded = decimalPart * expMult;

            // Round the value using both the float conversion and the rounding function.
            // Floor is used so that it doesn't effect the value.
            decimalRounded = (Mathf.Floor((float)decimalRounded));

            // Return to original value by converting back from decimals.
            decimalRounded = decimalRounded / expMult;

            // Add the decimal rounded and the whole part together.
            double valueRounded = wholePart + decimalRounded;

            // Convert the rounded value to get the result.
            float result = (float)valueRounded;

            // Returns the result.
            return result;
        }

        // Gets the meteor spawn rate, modified by the phase.
        public float GetModifiedMeteorSpawnRate()
        {
            // The modifier value.
            float mod;

            // Checks the speed for the spawn rate modifier.
            switch (phase)
            {
                default:
                case 1:
                    mod = 1.0F;
                    break;

                case 2:
                    mod = 0.95F;
                    break;

                case 3:
                    mod = 0.90F;
                    break;

                case 4:
                    mod = 0.85F;
                    break;

            }

            // Get the result.
            float result = meteorSpawnRate * mod;

            // Return the result.
            return result;

        }

        // Gets the meteor speed, modified by the phase.
        public float GetModifiedMeteorSpeedMax()
        {
            // The modifier value.
            float mod;

            // Checks the phase to see what speed to use.
            switch (phase)
            {
                default:
                case 1:
                    mod = 1.0F;
                    break;

                case 2:
                    mod = 1.10F;
                    break;

                case 3:
                    mod = 1.20F;
                    break;

                case 4:
                    mod = 1.30F;
                    break;

            }

            // Generate the result.
            float result = meteorSpeedMax * mod;

            // Return the result.
            return result;
        }

        // Gets the modified meteor movement distance from its hit position.
        // This is for focus mode only.
        public float GetModifiedMeteorMoveDistance(float answerTime)
        {
            // The difficulty mode.
            float diffMod;

            // Gets the difficulty to check what the difficulty mod should be.
            switch (GetDifficulty())
            {
                default:
                case 1:
                case 2:
                    diffMod = 1.0F;
                    break;

                case 3:
                case 4:
                    diffMod = 1.25F;
                    break;

                case 5:
                case 6:
                    diffMod = 1.50F;
                    break;

                case 7:
                case 8:
                case 9:
                    diffMod = 1.75F;
                    break;
            }


            // Ignore negative values.
            if (answerTime <= 0.0F)
                answerTime = 0.0F;

            // The time based modifier.
            float timeMod;

            // Checks what to set the modifier to based on the answer time.
            if (answerTime >= 10.0F) // 10 seconds or more.
            {
                timeMod = 2.00F;
            }
            else if (answerTime >= 8.0F) // 8 seconds or more.
            {
                timeMod = 1.80F;
            }
            else if(answerTime >= 6.0F) // 6 seconds or more.
            {
                timeMod = 1.60F;
            }
            else if(answerTime >= 4.0F) // 4 seconds or more.
            {
                timeMod = 1.40F;
            }
            else if(answerTime >= 2.0F) // 2 seconds or more.
            {
                timeMod = 1.20F;
            }
            else // Less than 2 seconds.
            {
                timeMod = 1.0F;
            }


            // Generate the result.
            float result = meteorMoveDist * diffMod * timeMod;

            // Return the result.
            return result;
        }

        // Gets the modified meteor move distance by auto-calculating the answer time using the meteor target script.
        public float GetModifiedMeteorMoveDistance()
        {
            float answerTime = stageTime - meteorTarget.GetStageTimeOfTargeting();
            return GetModifiedMeteorMoveDistance(answerTime);
        }

        // Laser Shot

        // Returns 'true' if the player can shoot.
        public bool CanPlayerShootLaserShot()
        {
            // The result to be returned.
            bool result;
            
            // First check if a meteor is being targeted and if the player is not stunned.
            result = meteorTarget.IsMeteorTargeted() && !player.IsPlayerStunned();

            // Meteor is being targeted.
            if(result)
            {
                // If the meteor isn't being targeted exactly, and the player shouldn't be able to shoot under said conditions...
                // Don't allow them to fire.
                if (CanShootOnExactTargetOnly() && !meteorTarget.IsMeteorTargetedExactly())
                {
                    result = false;
                }
            }

            // Returns the result.
            return result;
        }

        // If 'true', the player can only shoot a laser shot when a meteor is being targeted exactly.
        public bool CanShootOnExactTargetOnly()
        {
            return shootOnExactTargetOnly;
        }


        // SPEED
        // Gets the game speed.
        public float GetGameSpeed()
        {
            // Returns the time scale.
            return GetGameTimeScale();
        }

        // Sets the game speed.
        public void SetGameSpeed(float timeScale)
        {
            // Sets the time scale.
            SetGameTimeScale(timeScale);

            // If the audio speed should be adjusted.
            if (adjustAudioSpeed)
            {
                // The new pitch.
                float newPitch;

                // Checks the game speed to know what the audio pitch should be.
                if (timeScale > 1.0F) // Fast
                {
                    newPitch = 1.25F;
                }
                else if (timeScale < 1.0F) // Slow
                {
                    newPitch = 0.75F;
                }
                else // Normal
                {
                    newPitch = 1.0F;
                }

                // Change the BGM and the World SFX Pitches
                stageAudio.bgmSource.pitch = newPitch;
                stageAudio.sfxWorldSource.pitch = newPitch;
            }
        }

        // Resets the game's speed.
        public void ResetGameSpeed()
        {
            SetGameSpeed(NORMAL_GAME_TIME_SCALE);
        }

        // Returns 'true' if the time scale is normal.
        public bool IsNormalSpeed()
        {
            return GetGameSpeed() == NORMAL_GAME_TIME_SCALE;
        }

        // Sets the game to normal speed.
        public void SetToNormalSpeed()
        {
            // Call the reset function.
            SetGameSpeed(NORMAL_GAME_TIME_SCALE);
        }

        // Returns 'true' if the game is at a fast speed.
        public bool IsFastSpeed()
        {
            return GetGameSpeed() == FAST_GAME_TIME_SCALE;
        }

        // Sets the game to fast speed.
        public void SetToFastSpeed()
        {
            SetGameSpeed(FAST_GAME_TIME_SCALE);
        }

        // Toggles fast speed.
        public void ToggleFastSpeed()
        {
            // If the game time scale is normal, set to fast.
            // If the game time scale is fast,set it to normal.
            if (IsGameTimeScaleNormal())
            {
                SetToFastSpeed();
            }
            else
            {
                SetToNormalSpeed();
            }
        }

        // Returns 'true' if the game is at a slow speed.
        public bool IsSlowSpeed()
        {
            return GetGameSpeed() == SLOW_GAME_TIME_SCALE;
        }

        // Sets the game to slow speed.
        public void SetToSlowSpeed()
        {
            SetGameSpeed(SLOW_GAME_TIME_SCALE);
        }

        // Toggles slow speed.
        public void ToggleSlowSpeed()
        {
            // If the game time scale is normal, set to slow.
            // If the game time scale is fast,set it to normal.
            if (IsGameTimeScaleNormal())
            {
                SetToSlowSpeed();
            }
            else
            {
                SetToNormalSpeed();
            }
        }

        // Resets the trackers for how long the game runs at certain speeds.
        public void ResetGameSpeedTimeTrackers()
        {
            fastStageTime = 0;
            slowStageTime = 0;
        }

        // UNIT OPERATIONS
        // Generates the conversion question for the player.
        public string GenerateConversionQuestion(Meteor meteor)
        {
            // The result.
            string result = string.Empty;

            // If 'true', fractions can be tried.
            bool tryFractions = UnitsInfo.IsMetricUnits(meteor.conversion.group) &&
                meteor.conversion.inputValue < 1.0F && meteor.conversion.inputValue >= 0.0F;

            // If fractions can be used, try to generate a fraction.
            if (useFractions && tryFractions)
            {
                // Generates a random value.
                float randValue = Random.Range(0.0F, 1.0F);

                // Fraction display change.
                if (randValue <= FRACTION_DISPLAY_CHANCE)
                {
                    // Variables to be used to set up the string.
                    float inputValue = meteor.conversion.inputValue;
                    string inputValueString = inputValue.ToString();
                    int decimalPlaces = 0;

                    // If there is a decimal place, use it to get the amount of decimal places.
                    if (inputValueString.Contains("."))
                    {
                        // Calculates the number of decimal places to know what to display.
                        decimalPlaces = inputValueString.Length - (inputValueString.IndexOf(".") + 1);
                    }

                    // There are decimal places, generate the result string.
                    if (decimalPlaces > 0)
                    {
                        float mult = Mathf.Pow(10, decimalPlaces);
                        result = (inputValue * mult).ToString() + "/" + mult.ToString() + " " +
                            meteor.conversion.GetInputSymbol() + " = ?";
                    }
                }

            }

            // Result wasn't set, so use the default format.
            if (result == "")
                result = meteor.conversion.inputValue.ToString() + " " + meteor.conversion.GetInputSymbol() + " = ?";

            return result;
        }

        // Calculates the points to be given for destroying the provided meteor.
        public float CalculatePoints(Meteor meteor)
        {
            // The points to be returned.
            float points = 0;

            // Base amount, combo bonus, and difficulty bonus.
            points += 15;
            points += 10 * combo;
            points += 5 * difficulty;

            // Returns the points.
            return points;
        }

        // Returns 'true' if the points goal has been reached.
        public bool IsPointsGoalReached(float points)
        {
            return points >= pointsGoal;
        }

        // Checks if the provided player has reached the points goal.
        public bool IsPointsGoalReached(PlayerStage playerStage)
        {
            return IsPointsGoalReached(playerStage.GetPoints());
        }

        // Gets the progress of the player's points towards the goal.
        public float GetPlayerPointsProgress()
        {
            // Calculates the percent and returns it.
            float percent = player.GetPoints() / pointsGoal;
            return percent;
        }

        // Gets the player's points.
        public float GetPlayerPoints()
        {
            return player.GetPoints();
        }

        // Sets the player's points.
        public void SetPlayerPoints(float points)
        {
            player.SetPoints(points);
            player.OnPointsChanged();
        }

        // Called when the player's points have changed.
        public void OnPlayerPointsChanged()
        {
            // Updates the points bar.
            stageUI.UpdatePointsText();
            stageUI.UpdatePointsBar();

            // If the points goal has been reached, trigger the stage win.
            if (IsPointsGoalReached(player.GetPoints()))
            {
                OnStageWon();
            }
            else // Change phase.
            {
                SetPhaseByPlayerPointsProgress();
            }
        }

        // Calculates the final stage score and returns it.
        public float CalculateStageFinalScore()
        {
            // Base score.
            float score = player.GetPoints();

            // Difficulty bonus.
            score += 50.0F * difficulty;

            // Highest combo bonus.
            score += 100.0F * highestCombo;

            // The speed change time bound.
            const float SPEED_CHANGE_TIME_BOUND = 60.0F;
            const float SPEED_CHANGE_POINTS_MAX = 150.0F;

            // If the slow stage time is less than 1 minute.
            if (slowStageTime < SPEED_CHANGE_TIME_BOUND)
            {
                float percent = Mathf.Clamp(slowStageTime, 0.0F, SPEED_CHANGE_TIME_BOUND);
                percent = 1.0F - (percent / SPEED_CHANGE_TIME_BOUND); // Reverses the percent.
                score += Mathf.Ceil(SPEED_CHANGE_POINTS_MAX * percent);
            }

            // If the fast stage time is less than 1 minute.
            if (fastStageTime < SPEED_CHANGE_TIME_BOUND)
            {
                float percent = Mathf.Clamp(fastStageTime, 0.0F, SPEED_CHANGE_TIME_BOUND);
                percent = 1.0F - (percent / SPEED_CHANGE_TIME_BOUND); // Reverses the percent.
                score += Mathf.Ceil(SPEED_CHANGE_POINTS_MAX * percent);
            }

            // Bounds heck.
            if (score < 0)
                score = 0;

            // Returns the score.
            return score;
        }

        // Calculates and set the stage final score.
        public void CalculateAndSetStageFinalScore()
        {
            // Sets the final score.
            stageFinalScore = CalculateStageFinalScore();

            // The score can't be negative.
            if (stageFinalScore < 0)
                stageFinalScore = 0;
        }

        // SUCCESSES
        // Gets the number of consecutive successes.
        public int GetConsecutiveSuccessesCount()
        {
            return consecutiveSuccesses;
        }

        // Adds to the consecutive number of successful meteor destroys.
        public void IncreaseConsecutiveSuccessesCount()
        {
            consecutiveSuccesses++;
        }

        // Resets the number of consecutive successes.
        public void ResetConsecutiveSuccessesCount()
        {
            consecutiveSuccesses = 0;
        }

        // COMBO
        // Gets the combo count.
        public int GetComboCount()
        {
            return combo;
        }

        // Increaes the combo.
        public void IncreaseCombo()
        {
            // Increase the combo and reset the timer.
            combo++;
            comboTimer = COMBO_TIMER_MAX;

            // If this is the new highest combo, set it.
            if (combo > highestCombo)
                highestCombo = combo;
        }

        // Displays the combo.
        public void DisplayCombo(Vector3 position)
        {
            comboDisplay.PlayComboAnimationAtPosition(position);
        }

        // Resets the combo.
        public void ResetCombo(bool resetHighestCombo)
        {
            combo = 0;
            comboTimer = 0;

            // If the highest combo should be reset, reset it.
            if (resetHighestCombo)
                highestCombo = 0;
        }

        // BARRIER, SURFACE
        // Called when a barrier has been damaged.
        public void OnBarrierDamaged()
        {
            stageUI.OnBarrierDamaged();

            // If tutorials are being used.
            if(IsUsingTutorial())
            {
                // If there is no tutorial running.
                if (!IsTutorialRunning())
                {
                    // If the barrier tutorial has not been triggered yet, trigger it.
                    if (!tutorials.clearedBarrierTutorial)
                    {
                        tutorials.LoadBarrierTutorial();
                    }
                }
            }
        }

        // Restores all barriers.
        public void RestoreAllBarriers()
        {
            // Goes through all the barriers and restores them.
            foreach (Barrier barrier in stageBarriers)
            {
                // Restore the barrier.
                if (barrier != null)
                    barrier.RestoreBarrier();
            }
        }

        // Called when the surface has been damaged.
        public void OnSurfaceDamaged()
        {
            stageUI.OnSurfaceDamaged();

            // If tutorials are being used.
            if (IsUsingTutorial())
            {
                // If there is no tutorial running.
                if (!IsTutorialRunning())
                {
                    // If the surface tutorial has not been triggered yet, trigger it.
                    if (!tutorials.clearedSurfaceTutorial)
                    {
                        tutorials.LoadSurfaceTutorial();
                    }
                }
            }
        }

        // ENDING
        // Called when the stage has ended.
        public void OnStageEnd()
        {
            // Change settings.
            runningGame = false;
            SetToNormalSpeed();
            PauseGame();

            // Mutes the stage world audio so that animations don't play sounds.
            stageAudio.sfxWorldSource.mute = true;

            // Kill all the meteors.
            FindAndKillAllMeteors();

            // Resets the barriers. This is done here so that the sound effects don't play.
            RestoreAllBarriers();

            // Calculate the final score, and add it to the game score.
            CalculateAndSetStageFinalScore();

            // If the warning sound effect is being played, stop playing it.
            if(useMeteorWarningSfx)
            {
                // If the warning is playing, stop it.
                if (stageAudio.IsWarningSfxPlaying())
                {
                    stageAudio.StopWarningSfx();
                }
            }

            // Puzzle manager stage end call.
            puzzleManager.OnStageEnd();

            // UI stage end.
            stageUI.OnStageEnd();
        }

        // Called when the game has been won.
        public void OnStageWon()
        {
            // On stage end.
            OnStageEnd();

            // The stage has been cleared.
            cleared = true;

            // gameScore += stageFinalScore; // Not needed.

            // Stage won.
            stageUI.OnStageWon();

            // Plays the stage complete music.
            stageAudio.PlayStageCompleteMusic(true);
        }

        // Called when the game has been lost.
        public void OnStageLost()
        {
            // On stage end.
            OnStageEnd();

            // The stage has not been cleared.
            cleared = false;

            // Time and score is reset in the ResetStage() function.

            // Add to the losses count, and adjusts the difficulty.
            losses++;
            AdjustDifficultyByLosses();

            // Stage lost UI.
            stageUI.OnStageLost();

            // Plays the stage complete music.
            stageAudio.PlayStageCompleteMusic(false);
        }

        // Called to restart the stage.
        public void ResetStage()
        {
            // Reset the player's points, kill all the meteors, and reset the game progress.
            player.SetPoints(0);
            KillAllMeteorsInList();
            SetPhaseByPlayerPointsProgress();

            // Barriers are now restored when the stage ends to stop the sound effects from playing.

            // Restores the surface to full health.
            stageSurface.RestoreSurface();

            // Resets the time, unpauses the game, and starts running the game again.
            stageTime = 0;
            stageFinalScore = 0;

            // Resets the successes count, combo, and game speed time trackers.
            ResetConsecutiveSuccessesCount();
            ResetCombo(true);
            ResetGameSpeedTimeTrackers();

            // Restarts the stage.
            stageUI.OnStageReset();

            // Play the stage music.
            stageAudio.PlayStageBgm(bgmNumber);

            // Unmute the SFX world audio.
            stageAudio.sfxWorldSource.mute = false;

            // Called to reset the puzzle.
            puzzleManager.OnStageReset();

            // Unpause game and start running.
            UnpauseGame();
            runningGame = true;
        }

        // Generates the stage data.
        public StageData GenerateStageData()
        {
            // The stage data.
            StageData data = new StageData();

            // Set values.
            data.stageName = stageName;
            data.stageTime = stageTime;
            data.stageScore = stageFinalScore;
            data.highestCombo = highestCombo;
            data.cleared = cleared;

            // Return the values.
            return data;
        }

        // Goes to the world.
        public void ToWorld()
        {
            // Saves the stage info and goes into the world.
            gameInfo.SaveStageInfo(this);
            LoadWorldScene();

            // Play loading animation?
        }

        // Goes to the world scene.
        public void LoadWorldScene()
        {
            UnpauseGame();

            LoadScene(worldScene);
        }

        // Returns 'true' if the game is playing.
        // This returns 'false' if the game is paused, the game is not running, a tutorial has frozen the game...
        // Or if the game is loading.
        // If 'ignoreAppWaiting' is false, the game registers as being played even if the window isn't in focus.
        public bool IsGamePlaying(bool ignoreAppWaiting = false)
        {
            // Checks if a loading animation is playing.
            bool loadingAnimPlaying = false;

            // Checks if the loading screen is being used. If it is, then the loading aniamtion might be playing.
            if (LoadingScreenCanvas.IsInstantiatedAndUsingLoadingScreen())
            {
                // Gets the instance.
                LoadingScreenCanvas lsc = LoadingScreenCanvas.Instance;

                // If the loading screen is beings shown, mark as true.
                loadingAnimPlaying = lsc.IsAnimationPlaying();
            }

            // Checks if the game is running, if the game is paused, if the tutorial is running...
            // And if the loading animation is playing.
            // It takes into account if the app is waiting or not based on this parameter.
            if (ignoreAppWaiting) // Ignore if the app is waiting or not.
            {
                return runningGame && !IsGamePaused() && !IsTutorialRunning() && !loadingAnimPlaying;
            }
            else // Register the game as paused if the app is waiting.
            {
                return runningGame && !IsGamePausedOrApplicationWaiting() && !IsTutorialRunning() && !loadingAnimPlaying;
            }

        }

        // Returns 'true' if the stage is running.
        public bool IsRunningGame()
        {
            return runningGame;
        }

        // Called to run the game mechanics.
        public void RunGame()
        {
            // Checks the gameplay mode.
            switch(gameplayMode)
            {
                // Focus Mode
                case gameMode.focus:

                    // If there are no meteors, spawn new ones.
                    if(Meteor.GetMeteorsActiveCount() <= 0)
                    {
                        // The meteor list.
                        List<Meteor> meteors = new List<Meteor>();

                        // Calculates the maximum amount of meteors to make.
                        int maxMeteors = Mathf.Min(ACTIVE_METEORS_COUNT_MAX, METEORS_FOCUS_MODE_MAX);

                        // If 0 or less, set to the active meteor max count.
                        if (maxMeteors <= 0)
                            maxMeteors = ACTIVE_METEORS_COUNT_MAX;

                        // While meteors are being spawned.
                        // This can work by checking the active list or the local meteor list.
                        while(meteors.Count < maxMeteors)
                        {
                            // Spawns a meteor and adds it the list.
                            meteors.Add(SpawnMeteor());
                        }

                        // Gets the minimum and maximum spawn positions for the meteors.
                        Vector3 spawnMin = stage.meteorSpawnPointMin.transform.position;
                        Vector3 spawnMax = stage.meteorSpawnPointMax.transform.position;

                        // Calculates the x-spacing for the meteors.
                        float xSpacing = (spawnMax.x - spawnMin.x) / meteors.Count;

                        // Goes through all the meteors to position them and turn gravity off.
                        for(int i = 0; i < meteors.Count; i++)
                        {
                            // Meteor rigidbody
                            meteors[i].rigidbody.velocity = Vector2.zero;
                            
                            // The gravity scale is not turned off so that the meteor moves onto screen...
                            // Before being stopped.
                            // meteors[i].rigidbody.gravityScale = 0;

                            // Calculates the new position.
                            Vector3 newPos = meteors[i].transform.position;
                            newPos.x = spawnMin.x  + xSpacing / 2.0F + xSpacing * i;
                            newPos.y = spawnMin.y;

                            // Sets the meteor's new position using the spawn point function.
                            meteors[i].SetSpawnPoint(newPos, true);
                        }
                    }

                    break;

                    // Rush Mode
                case gameMode.rush:
                    // Reduce the spawn timer.
                    meteorSpawnTimer -= Time.deltaTime;

                    // Cap timer.
                    if (meteorSpawnTimer <= 0)
                        meteorSpawnTimer = 0;


                    // Spawn a meteor.
                    if (meteorSpawnTimer <= 0)
                    {
                        SpawnMeteor();

                        // Sets the timer to the spawn rate.
                        meteorSpawnTimer = GetModifiedMeteorSpawnRate();
                    }
                    break;
            }

            // Gets set to 'true' if there is a new meteor being targeted.
            bool newTarget = false;

            // Checks the game mode to know how to handle meteor targeting.
            switch(gameplayMode)
            {
                case gameMode.focus:
                    
                    // The meteor has a target, so no new target is set.
                    if(meteorTarget.HasMeteor())
                    {
                        newTarget = false;
                    }
                    else // No target, so set a random target.
                    {
                        meteorTarget.SetTargetToRandomMeteor();
                        newTarget = true;
                    }

                    break;

                case gameMode.rush:
                    // If there is no meteor being target.
                    if (!meteorTarget.HasMeteor())
                    {
                        // Gets the closest meteor, and move towards it.
                        meteorTarget.SetTarget(GetClosestMeteor());
                        newTarget = true;
                    }
                    else // There's already a meteor being targeted.
                    {
                        // If the closest meteor should be constantly searched for...
                        // And there are meteors to be targeted.
                        if (constClosestMeteorCheck)
                        {
                            // If the active meteor count is greater than 0, then there are meteors to find.
                            if (Meteor.GetMeteorsActiveCount() > 0)
                            {
                                // Gets the closest meteor.
                                Meteor closestMeteor = GetClosestMeteor();

                                // If the targeted meteor is not the closest meteor...
                                // Target the closest meteor.
                                if (meteorTarget.GetMeteor() != closestMeteor)
                                {
                                    // Target the closest meteor.
                                    meteorTarget.SetTarget(closestMeteor);
                                    newTarget = true;
                                }
                            }

                        }
                    }
                    break;
            }


            // If a meteor is being targeted...
            if (meteorTarget.IsMeteorTargeted())
            {
                // Run the text reveal animation.
                if (applyMultReveals)
                {
                    // Apply the multiple reveals if the correct answer threshold has been met.
                    // Only do this if it's a new meteor target.
                    if (newTarget && consecutiveSuccesses >= MULT_REVEAL_EFFECT_THRESHOLD)
                    {
                        stageUI.StartUnitButtonMultipleReveals();

                        // If tutorials are being used.
                        if (IsUsingTutorial())
                        {
                            // If a tutorial is not running.
                            if (!IsTutorialRunning())
                            {
                                // Run the mults hidden tutorial if it hasn't been run yet.
                                if (!tutorials.clearedHiddenMultiplesTutorial)
                                {
                                    tutorials.LoadHiddenMultiplesTutorial();
                                }
                            }
                        }

                    }
                }

                // Checks if the warning sound effect should be used.
                if (useMeteorWarningSfx)
                {
                    // Gets the meteor Y and the surface Y.
                    float meteorY = meteorTarget.GetMeteor().transform.position.y;
                    float surfaceY = stage.surface.transform.position.y;

                    // Calculates the distance.
                    float dist = Mathf.Abs(meteorY - surfaceY);

                    // If the meteor is too close to the surface, play the warning sound.
                    if(dist <= METEOR_WARNING_SFX_MAX_DIST)
                    {
                        // Checks the gameplay mode.
                        switch(gameplayMode)
                        {
                            default:
                            case gameMode.focus: // Focus
                                
                                // The warning sound is NOT used.
                                // It only gets triggered with the targeted meteor, and ended up...
                                // Causing a looped sound when it shouldn't. Using one shot...
                                // Still triggered the loop somehow, and overlayed another play of the sound.
                                // So this is not being used.
                                // stageAudio.PlayWarningSfxOneShot();
                                break;

                            case gameMode.rush: // Rush
                                // play the warning sound if it isn't already playing.
                                if (!stageAudio.IsWarningSfxPlaying())
                                {
                                    stageAudio.PlayWarningSfx();
                                }
                                break;
                        }

                    }
                    else
                    {
                        // Stop the warning sound.
                        if(stageAudio.IsWarningSfxPlaying())
                        {
                            stageAudio.StopWarningSfx();
                        }
                    }
                }
            }
            else
            {
                // If the metoer warning sound effect is being used.
                if(useMeteorWarningSfx)
                {
                    // If the warning sound is playing, stop it.
                    if(stageAudio.IsWarningSfxPlaying())
                    {
                        stageAudio.StopWarningSfx();
                    }
                }
            }

            // TODO: check points and damage for a game win?
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // If the game is playing (ignores application waiting), update the timer.
            if (IsGamePlaying(true))
            {
                // Gets the stage before it's updated.
                float oldStageTime = stageTime;

                // Updates the stage time.
                stageTime += Time.unscaledDeltaTime;

                // If the time display's update is optimized.
                if (optimizeTimeDisplayUpdate)
                {
                    // If the stage time rounded down is 1 greater than the old stage time rounded down...
                    // That means a whole second has passed. 
                    // If so, update the time display text.
                    if (Mathf.Floor(stageTime) > Mathf.Floor(oldStageTime))
                    {
                        stageUI.UpdateTimeText();
                    }
                }
                else // No optimization, so update every frame.
                {
                    stageUI.UpdateTimeText();
                }
            }

            // Runs updates if the game is playing (takes into account if the application is paused).
            if (IsGamePlaying())
            {
                // Run the game.
                RunGame();

                // If the combo timer is greater than 0, and a meteor is targeted.
                // If the player has fired a laser shot, don't run the timer.
                if (comboTimer > 0.0F && meteorTarget.GetMeteor() != null && player.laserShotActive == null)
                {
                    // Reduce the timer.
                    comboTimer -= Time.unscaledDeltaTime; // Uses unscaled delta time.

                    // Reset the combo if the timer has run out.
                    if (comboTimer <= 0.0F)
                    {
                        ResetCombo(false);
                    }
                }

                // Fast and Slow Speed Time - times how long both are in effect.
                float gameSpeed = GetGameSpeed();
                if (gameSpeed > 1.0F) // Fast
                {
                    fastStageTime += Time.unscaledDeltaTime;
                }

                if (gameSpeed < 1.0F) // Slow
                {
                    slowStageTime += Time.unscaledDeltaTime;
                }

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