using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

namespace RM_MST
{
    // Swaps the puzzle pieces around the screen.
    public class SwapPuzzle : Puzzle
    {
        [Header("SwapPuzzle")]

        // The list of symbol positions.
        public List<GameObject> piecePositions;

        // The generated pieces.
        protected List<PuzzlePiece> genPieces = new List<PuzzlePiece>();

        // If 'true', pieces are duplicated to remaining spaces.
        private bool duplicatePieces = true;

        // Determines if symbols get swapped.
        // Becomes 'true' when the puzzle starts.
        protected bool swappingEnabled = false;

        // The timer used for swapping piece positions.
        private float swapTimer = 0.0F;

        // The maximum time it takes to swap piece positions.
        public const float SWAP_TIMER_MAX = 4.5F;

        // The starting index for swapping positions.
        // The symbol at this index is being moved forward.
        private int swapStartIndex = 0;

        // The progress bar that represents the swap timer.
        public ProgressBar swapTimerBar;

        // Awake is called when the script instance is being loaded.
        protected override void Awake()
        {
            base.Awake();
            puzzleType = PuzzleManager.puzzleType.swap;   
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Initializes the puzzle.
        public override void InitializePuzzle()
        {
            // Generate a list of the unit buttons.
            List<UnitsButton> unitsButtons = stageUI.GenerateUnitsButtonsActiveList();
            List<PuzzleConversionDisplay> displays = puzzleManager.puzzleUI.GenerateActiveConversionDisplayList();

            // There are no unit buttons.
            if (unitsButtons.Count < 0 || displays.Count < 0)
            {
                Debug.LogError("No active unit buttons were found! Puzzle will fail to load.");
            }

            // The display index.
            int displayIndex = 0;

            // The symbol index.
            int symbolIndex = -1;

            // The queue of instantiated symbols.
            Queue<PuzzlePiece>instSymbols = new Queue<PuzzlePiece>();

            // While there are positions to be filled.
            // TODO: loop around if the display index passes the list count.
            for (int i = 0; i < piecePositions.Count && displayIndex < displays.Count; i++)
            {
                // Instantiates the piece.
                PuzzlePiece piece = Instantiate(piecePrefab);

                // Set the conversion display.
                piece.SetPieceFromConversionDisplay(displays[displayIndex]);

                // Disable since it's not needed.
                piece.setDisplayInfoOnStart = false;

                // Add the piece to the symbol position via parenting.
                piece.transform.parent = piecePositions[i].transform;
                piece.transform.localPosition = Vector3.zero;

                // Add the piece to the gen list.
                genPieces.Add(piece);

                // Add a piece to the instantiated list.
                instSymbols.Enqueue(piece);

                // Increase the display index.
                displayIndex++;

                // Saves the symbol index.
                symbolIndex = i;
            }

            // Increases by 1 since the loop is over.
            symbolIndex++;

            // If there should be duplicate pieces.
            if(duplicatePieces)
            {
                // While there are displays left to fill.
                while (symbolIndex < piecePositions.Count)
                {
                    // Gets the original piece, removes it from the queue.
                    PuzzlePiece origPiece = instSymbols.Peek();
                    instSymbols.Dequeue();

                    // Copy the original piece and put it back in the queue.
                    PuzzlePiece copyPiece = Instantiate(origPiece);
                    instSymbols.Enqueue(origPiece);

                    // Change the parent of the copy piece, and set its local position to 0.
                    copyPiece.transform.parent = piecePositions[symbolIndex].transform;
                    copyPiece.transform.localPosition = Vector3.zero;

                    // Add the copied piece to the generated pieces list.
                    genPieces.Add(copyPiece);

                    // Increase the symbol index.
                    symbolIndex++;
                }
            }
            

            // Resets the swap timer.
            ResetPuzzleSwapTimer();

            // Reset the start swap index.
            swapStartIndex = 0;

            // Calls base function.
            base.InitializePuzzle();
        }

        // Initializes the puzzle for when a conversion question starts.
        public override void StartPuzzle()
        {
            // Resets the swap timer.
            ResetPuzzleSwapTimer();

            // Swapping is enabled.
            swappingEnabled = true;
        }

        // Stops the puzzle, which is called when a meteor is untargeted.
        public override void StopPuzzle()
        {
            // This is commented out so that the puzzle doesn't...
            // Constantly reset.
            // Resets the piece positions.
            // ResetPiecePositions();

            // Reset the timer.
            ResetPuzzleSwapTimer();

            // Swaping is disabled.
            swappingEnabled = false;
        }

        // Ends a puzzle when a meteor is untargeted.
        public override void EndPuzzle()
        {
            // Destroys all the generated pieces.
            foreach (PuzzlePiece piece in genPieces)
            {
                // Destroys the generated piece.
                if (piece != null)
                {
                    Destroy(piece.gameObject);
                }
            }

            // Clears out the list.
            genPieces.Clear();

            // Calls the base function.
            base.EndPuzzle();
        }

        // Swapping
        // Checks if swapping is enabled.
        public bool IsSwappingEnabled()
        {
            return swappingEnabled;
        }


        // Swaps the symbol positions.
        public void SwapPiecePositions()
        {
            // Clamps the sawp start index.
            swapStartIndex = Mathf.Clamp(swapStartIndex, 0, piecePositions.Count - 1);

            // Increases the index.
            swapStartIndex++;

            // Loops around back to the start.
            if(swapStartIndex >= piecePositions.Count)
                swapStartIndex = 0;

            // Set the pos index, and shifts it over by one.
            int posIndex = swapStartIndex;

            // Goes through all the pieces.
            for(int i = 0; i < genPieces.Count; i++)
            {
                // Grabs the new position.
                GameObject newPos = piecePositions[posIndex];

                // Grabs the piece from the piece list.
                PuzzlePiece piece = genPieces[i];

                // Changes the piece's parent and transform.
                piece.transform.parent = newPos.transform;
                piece.transform.localPosition = Vector3.zero;

                // Increases the index.
                posIndex++;

                // Loop around to 0 if it reaches the end. 
                if (posIndex >= piecePositions.Count)
                    posIndex = 0;
            }
        }

        // Resets the piece positions.
        public void ResetPiecePositions()
        {
            // Makes a queue out of the generated pieces and the piece positions.
            Queue<PuzzlePiece> pieceQueue = new Queue<PuzzlePiece>(genPieces);
            Queue<GameObject> posQueue = new Queue<GameObject>(piecePositions);

            // While there are pieces and positions.
            while (pieceQueue.Count > 0 && posQueue.Count > 0)
            {
                // Grabs the new position.
                GameObject newPos = posQueue.Peek();
                posQueue.Dequeue();

                // Grabs the piece, change it's parent, and reset it's local position.
                PuzzlePiece piece = pieceQueue.Dequeue();
                piece.transform.parent = newPos.transform;
                piece.transform.localPosition = Vector3.zero;
            }

            // Resets the swapping start index.
            swapStartIndex = 0;
        }

        // Resets the puzzle swap timer.
        public void ResetPuzzleSwapTimer()
        {
            swapTimer = SWAP_TIMER_MAX;
        }

        // Updates the swap timer progress bar.
        public void UpdateSwapTimerProgressBar()
        {
            // The percent.
            float percent;

            // Gets the swap max value.
            float swapTimerMax = SWAP_TIMER_MAX;

            // Checks that the swap timer max is not 0.
            if(swapTimerMax > 0)
            {
                percent = swapTimer / swapTimerMax;
            }
            else // max is 0.
            {
                percent = 0.0F;
            }

            // Clamp into [0.0, 1.0] bounds.
            percent = Mathf.Clamp01(percent);

            // Sets the value.
            swapTimerBar.SetValueAsPercentage(percent);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // If swapping is enabled, the puzzle has been initalized, and the game is being played.
            if(swappingEnabled && IsPuzzleInitializedAndIsGamePlaying())
            {
                // Reduce the timer.
                swapTimer -= Time.unscaledDeltaTime;
                
                // Bounds check.
                if(swapTimer < 0.0F)
                {
                    swapTimer = 0.0F;
                }

                // Time to swap positions.
                if(swapTimer <= 0.0F)
                {
                    // Swap the positions and reset the timer.
                    SwapPiecePositions();
                    ResetPuzzleSwapTimer();
                }

                // Updates the progress bar.
                UpdateSwapTimerProgressBar();
            }
        }
    }
}