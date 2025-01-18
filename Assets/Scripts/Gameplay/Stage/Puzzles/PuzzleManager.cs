using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // The manager for the puzzles.
    public class PuzzleManager : MonoBehaviour
    {
        // The puzzle types.
        public enum puzzleType { unknown, buttons, swap, slide, path }

        // the instance of the class.
        private static PuzzleManager instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The stage manager.
        public StageManager stageManager;

        // The stage UI.
        public StageUI stageUI;

        // The puzzle UI.
        public PuzzleUI puzzleUI;

        // Gets set to 'true' if the puzzle manager has checked for a tutorial.
        protected bool checkedForTutorial = false;

        // Gets set to 'true' when late start has been called.
        private bool calledLateStart = false;

        [Header("Puzzles")]

        // The type of puzzle being generated.
        public puzzleType pType = puzzleType.unknown;

        // The puzzle being used.
        public Puzzle puzzle;

        // The parent object for the puzzle.
        public GameObject puzzleParent;

        // The puzzle camera.
        public Camera puzzleCamera;

        [Header("Puzzles/Prefabs")]

        // The swap puzzle prefab.
        public SwapPuzzle swapPuzzlePrefab;

        // The slide puzzle prefab.
        public SlidePuzzle slidePuzzlePrefab;

        // The path puzzle prefab.
        public PathPuzzle pathPuzzlePrefab;

        // TODO: puzzle prefabs.

        // Constructor
        private PuzzleManager()
        {
            // ...
        }

        // Awake is called when the script is being loaded
        void Awake()
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
            // Gets the stage manager.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Gets the stage UI.
            if (stageUI == null)
                stageUI = StageUI.Instance;

            // Gets the puzzle UI.
            if (puzzleUI == null)
                puzzleUI = PuzzleUI.Instance;
        }

        // Called on the first update frame of the puzzle UI.
        private void LateStart()
        {
            calledLateStart = true;

            // The puzzle cover window is on by default.
            puzzleUI.puzzleWindowCover.SetActive(true);

            // Generates the puzzle.
            GeneratePuzzle();
        }

        // Gets the instance.
        public static PuzzleManager Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<PuzzleManager>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("PuzzleManager (singleton)");
                        instance = go.AddComponent<PuzzleManager>();
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

        // Returns the puzzle type.
        public puzzleType GetPuzzleType()
        {
            return pType;
        }

        // Generates the puzzle with the set type.
        public void GeneratePuzzle()
        {
            // If the puzzle is set.
            if (puzzle != null)
            {
                // Destroy the object.
                Destroy(puzzle.gameObject);

                // Clears the displays.
                puzzleUI.ClearConversionDisplays();
            }

            // Generate the puzzle.
            switch (pType)
            {
                // The none and buttons puzzle type produce the same result.
                case puzzleType.unknown:
                case puzzleType.buttons:
                    // No changes.
                    break;

                case puzzleType.swap:
                    if(swapPuzzlePrefab != null)
                        puzzle = Instantiate(swapPuzzlePrefab);
                    break;

                case puzzleType.slide:
                    if (slidePuzzlePrefab != null)
                        puzzle = Instantiate(slidePuzzlePrefab);
                    break;

                case puzzleType.path:
                    if (pathPuzzlePrefab != null)
                        puzzle = Instantiate(pathPuzzlePrefab);
                    break;
            }

            // The puzzle type is not set, meaning nothing was instantiated.
            if(puzzle == null)
            {
                // The puzzle tpye is not set to none or buttons.
                if(pType != puzzleType.unknown && pType != puzzleType.buttons)
                {
                    Debug.LogError("Puzzle was not instantiated. The puzzleType was reset to 'none'.");
                    pType = puzzleType.unknown;
                }                
            }
            else
            {
                // Set the puzzle parent and reset the local position.
                puzzle.transform.parent = puzzleParent.transform;
                puzzle.transform.localPosition = Vector3.zero;

                // Initialize the puzzle.
                // Since the puzzle's been initialized, it shouldn't initialize on start.
                puzzle.SetInstances();
                puzzle.InitializePuzzle();
                puzzle.initializeOnStart = false;
            }

            // Check for tutorials now that a puzzle has been generated.
            checkedForTutorial = false;

            // The puzzle has been generated.
            puzzleUI.OnPuzzleGenerated();

        }

        // Generates the puzzle with the provided type.
        public void GeneratePuzzle(puzzleType newType)
        {
            // Set the type.
            pType = newType;

            // Generate the puzzle.
            GeneratePuzzle();
        }
        
        // Tries to load a puzzle tutorial.
        public bool TryLoadPuzzleTutorial()
        {
            // Gets set to 'true' if a tutorial has been run. False if no tutorial is being run.
            // False by default.
            bool result = false;

            // TODO: implement.
            // If there is no tutorial running...
            if(!stageManager.tutorials.IsTutorialRunning())
            {
                // Checks the puzzle type.
                switch(pType)
                {
                    // Do nothing.
                    default:
                    case puzzleType.unknown:
                        result = false;
                        break;

                    case puzzleType.buttons:
                        
                        // Load the tutorial if it hasn't been used yet.
                        if(!stageManager.tutorials.clearedPuzzleButtonsTutorial)
                        {
                            stageManager.tutorials.LoadPuzzleButtonsTutorial();
                            result = true;
                        }

                        break;

                    case puzzleType.swap:

                        // Load the tutorial if it hasn't been used yet.
                        if (!stageManager.tutorials.clearedPuzzleSwapTutorial)
                        {
                            stageManager.tutorials.LoadPuzzleSwapTutorial();
                            result = true;
                        }

                        break;

                    case puzzleType.slide:

                        // Load the tutorial if it hasn't been used yet.
                        if (!stageManager.tutorials.clearedPuzzleSlideTutorial)
                        {
                            stageManager.tutorials.LoadPuzzleSlideTutorial();
                            result = true;
                        }

                        break;

                    case puzzleType.path:

                        // Load the tutorial if it hasn't been used yet.
                        if (!stageManager.tutorials.clearedPuzzlePathTutorial)
                        {
                            stageManager.tutorials.LoadPuzzlePathTutorial();
                            result = true;
                        }

                        break;
                }
            }

            return result;
        }

        // Checks if the puzzle is interactable.
        public bool IsPuzzleInteractable()
        {
            // If it's a buttons puzzle, check if the units buttons are interactable.
            if(pType == puzzleType.unknown ||  pType == puzzleType.buttons)
            {
                return stageUI.IsAllActiveUnitButtonsInteractable();
            }
            else // Check if the puzzle cover is active.
            {
                return puzzleCamera.gameObject.activeSelf;
            }
        }

        // Called when a meteor has been targeted.
        public void OnMeteorTargeted(Meteor meteor)
        {
            // Refreshes the conversion displays.
            puzzleUI.RefreshConversionDisplays();

            // Starts the puzzle.
            if(puzzle != null)
            {
                puzzle.StartPuzzle();
            }
        }

        // Called when a meteor has been killed.
        public void OnMeteorKilled(Meteor meteor)
        {
            // If there is no meteor targeted, or if the destroyed meteor is the meteor that was destroyed...
            // ...Call 'stop' for the puzzle.
            if(!stageManager.meteorTarget.IsMeteorTargeted() || stageManager.meteorTarget.IsMeteorTargeted(meteor))
            {
                // There is a puzzle, so tell it to stop.
                if(puzzle != null)
                {
                    puzzle.StopPuzzle();
                }
            }
        }

        // Called when the stage has ended.
        public void OnStageEnd()
        {
            // If there is a puzzle, end it and put up the puzzle over.
            if(puzzle != null)
            {
                puzzle.EndPuzzle();
                puzzleUI.puzzleWindowCover.SetActive(true);
            }
        }

        // Called when the stage is being reset.
        public void OnStageReset()
        {
            // Generates the puzzle again.
            GeneratePuzzle();
        }

        // Updates the puzzle inout.
        public void UpdatePuzzleInput()
        {
            // If the puzzle window cover is active, the player's inputs are blocked.
            // As such, do nothing.
            if (puzzleUI.puzzleWindowCover.activeSelf)
                return;

            // The position of the pointer (mouse/touch).
            Vector2 pointerPos = new Vector2();

            // Gets set to 'true' if the pointer position is set.
            bool pointerPosSet = false;

            // The mouse button is down, so track it.
            if (Input.GetMouseButtonDown(0))
            {
                // Gets the mouse position.
                pointerPos = Input.mousePosition;
                pointerPosSet = true;
            }
            else // No mouse down, so check for touches.
            {
                // If there are touches, get the position of the first touch.
                if (Input.touches.GetLength(0) > 0)
                {
                    // If the touch is in the "began" phase, register it. This means the touch just happened.
                    // This is the touch equivalent of 'GetMouseButtonDown()'.
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        pointerPos = Input.touches[0].position;
                        pointerPosSet = true;
                    }

                }
            }


            // If the pointer position has been set, and the pointer is over a UI element.
            if (pointerPosSet && EventSystem.current.IsPointerOverGameObject())
            {
                // Gets the raycast results.
                List<RaycastResult> raycastResults = MouseTouchInput.GetMouseUIRaycastResults();

                // Gets set to 'true' if the player is in the puzzle window.
                bool inPuzzleWindow = false;

                // The camera raw image.
                RawImage cameraRawImage = puzzleUI.cameraRawImage;

                // Goes through the raycast results.
                foreach (RaycastResult raycastResult in raycastResults)
                {
                    // If the player is interacting with the camera image, register the movement.
                    if(raycastResult.gameObject == cameraRawImage.gameObject)
                    {
                        inPuzzleWindow = true;
                    }
                }

                // TODO: implement touches from touch pad.

                // The player is in the puzzle window.
                if(inPuzzleWindow)
                {
                    // 1. Calculate the Pointer Position in the Camera Image

                    // The lower and upper bounds.
                    Vector2 camRawImageLower = puzzleUI.CalculateCameraRawImageLowerBounds();
                    Vector2 camRawImageUpper = puzzleUI.CalculateCameraRawImageUpperBounds(); ;

                    // Get the percentage positions of the pointer over the camera image.
                    // Treat this as the viewport position of the camera.
                    Vector3 mousePosPercents = new Vector3();

                    mousePosPercents.x = Mathf.InverseLerp(camRawImageLower.x, camRawImageUpper.x, pointerPos.x);
                    mousePosPercents.y = Mathf.InverseLerp(camRawImageLower.y, camRawImageUpper.y, pointerPos.y);
                    mousePosPercents.z = 1.0F;

                    // 2. Calculate the World Position in the Camera Source

                    // Set the viewport position.
                    Vector3 puzzleCamViewportPos = new Vector3();
                    puzzleCamViewportPos.x = mousePosPercents.x;
                    puzzleCamViewportPos.y = mousePosPercents.y;

                    // Calculate the viewport position as a world pos and as a ray.
                    Vector3 puzzleCamPointerPos = puzzleCamera.ViewportToWorldPoint(puzzleCamViewportPos);
                    puzzleCamPointerPos.z = 0; // Makes sure it's at 0 so that the sprite isn't hidden.

                    // 3. Cast the Ray
                    // The ray origin.
                    Vector2 rayOrigin2D = new Vector2(puzzleCamPointerPos.x, puzzleCamPointerPos.y);

                    // the max distance is the far clip plane minus the near clip plane.
                    float maxDist = puzzleCamera.farClipPlane - puzzleCamera.nearClipPlane;

                    // 2D ray cast.
                    RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin2D, Vector3.forward, maxDist);
                    bool rayHit = hitInfo.collider != null;


                    // 4. Try to Select the Piece
                    // The piece that has hit.
                    PuzzlePiece hitPiece;

                    // If the collider piece is not equal to null.
                    if (hitInfo.collider != null)
                    {
                        // Tries to get the hit piece component.
                        if (hitInfo.collider.gameObject.TryGetComponent(out hitPiece))
                        {
                            // Select the hit piece.
                            hitPiece.OnSelect();
                        }
                    }

                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Calls LateStart.
            if (!calledLateStart)
                LateStart();

            // If the game is playing.
            if (stageManager.IsGamePlaying())
            {
                // Gets set to 'true' if the buttons are interactive.
                bool buttonsInter = stageManager.stageUI.IsAllActiveUnitButtonsInteractable();

                // This isn't efficent, but this is the best you can do without reworking the other systems greatly.

                // If the puzzle cover's active should be changed, change it.
                // If the buttons are interactable, the cover should be inactive, and vice-versa.
                if (puzzleUI.puzzleWindowCover.activeSelf == buttonsInter)
                {
                    puzzleUI.puzzleWindowCover.SetActive(!buttonsInter);
                }

                // Updates the player's puzzle inputs.
                UpdatePuzzleInput();
            }

            // If a tutorial has not been checked for yet.
            if(!checkedForTutorial)
            {
                // If the tutorials are being used.
                if (stageManager.IsUsingTutorial())
                {
                    // If there is no tutorial running, and the first stage tutorial has been cleared.
                    if (!stageManager.IsTutorialRunning() && stageManager.tutorials.clearedFirstStageTutorial)
                    {
                        // Tries to load a puzzle tutorial.
                        TryLoadPuzzleTutorial();

                        // A tutorial has been checked for.
                        checkedForTutorial = true;
                    }
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