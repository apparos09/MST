using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The puzzle.
    public abstract class Puzzle : MonoBehaviour
    {
        // Gets set to 'true' when a puzzle is initialized.
        protected bool puzzleInitialized = false;

        // The stage manager.
        public StageManager stageManager;

        // The stage UI.
        public StageUI stageUI;

        // The puzzle manager.
        public PuzzleManager puzzleManager;

        // The puzzle UI.
        public PuzzleUI puzzleUI;

        // The type of the puzzle.
        protected PuzzleManager.puzzleType puzzleType = PuzzleManager.puzzleType.unknown;

        // The puzzle piece prefabs.
        public PuzzlePiece piecePrefab;

        // If set to 'true', initialize is called onStart for the puzzle.
        public bool initializeOnStart = true;

        // Awake is called when the script instance is being loaded.
        protected virtual void Awake()
        {
            // ...
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // Sets the instances needed for the puzzle to function.
            SetInstances();

            // Initializes the puzzle.
            if(initializeOnStart)
                InitializePuzzle();
        }

        // Sets the instances.
        public void SetInstances()
        {
            // Gets the stage manager.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Gets the stage UI.
            if (stageUI == null)
                stageUI = StageUI.Instance;

            // Gets the puzzle manager.
            if (puzzleManager == null)
                puzzleManager = PuzzleManager.Instance;

            // Gets the puzzle UI.
            if (puzzleUI == null)
                puzzleUI = PuzzleUI.Instance;
        }

        // Returns this puzzle's type.
        public PuzzleManager.puzzleType GetPuzzleType()
        {
            return puzzleType;
        }

        // Generates a list of puzzle pieces that all correspond to a unit button.
        // If 'activeOnly' is true, only active conversion displays are consulted.
        public List<PuzzlePiece> GenerateConversionDisplayPuzzlePieces(bool activeOnly = true)
        {
            // The list of conversion displays.
            List<PuzzleConversionDisplay> displays;

            // Checks if only active unit buttons and conversion displays should be used.
            if(activeOnly)
            {
                displays = puzzleManager.puzzleUI.GenerateActiveConversionDisplayList(); 
            }
            else
            {
                displays = puzzleManager.puzzleUI.GenerateConversionDisplayList();
            }

            // There are no unit buttons.
            if (displays.Count < 0)
            {
                Debug.LogWarning("No displays could be found.");
            }

            // The pieces being generated.
            List<PuzzlePiece> pieces = new List<PuzzlePiece>();

            // Goes through all the displays and generates a piece for each one.
            for (int i = 0; i < displays.Count; i++)
            {
                // Instantiates the piece.
                PuzzlePiece piece = Instantiate(piecePrefab);

                // Set the conversion display for this piece.
                piece.SetPieceFromConversionDisplay(displays[i]);

                // Disable since it's not needed.
                piece.setDisplayInfoOnStart = false;

                // Make the piece a child of the puzzle, and reset its position.
                piece.transform.parent = gameObject.transform;
                piece.transform.localPosition = Vector3.zero;

                // Add the piece to the pieces list.
                pieces.Add(piece);
            }

            // Returns the pieces list.
            return pieces;
        }

        // Returns true if the puzzle has been initialized.
        public bool IsPuzzleInitialized()
        {
            return puzzleInitialized;
        }

        // Returns true if the puzzle has been initalized and the game is playing.
        public bool IsPuzzleInitializedAndIsGamePlaying()
        {
            return IsPuzzleInitialized() && stageManager.IsGamePlaying();
        }

        // Initializes the puzzle.
        public virtual void InitializePuzzle()
        {
            puzzleInitialized = true;
        }

        // Starts the puzzle.
        public abstract void StartPuzzle();

        // Stops the puzzle, which is called when a meteor is untargeted.
        public abstract void StopPuzzle();

        // Ends a puzzle when the game finishes.
        public virtual void EndPuzzle()
        {
            puzzleInitialized = false;
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        // This function is called when the MonoBehaviour will be destroyed.
        protected virtual void OnDestroy()
        {
            // Makes sure the puzzle is ended.
            EndPuzzle();

            // The puzzle is not initalized if it is being destroyed.
            puzzleInitialized = false;
        }
    }
}