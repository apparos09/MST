using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace RM_MST
{
    // Slides puzzle symbols across the screen.
    public class SlidePuzzle : Puzzle
    {
        [Header("SlidePuzzle")]

        // Lines for the pieces to travel across.
        public List<SlidePuzzleSlider> sliders = new List<SlidePuzzleSlider>();

        // The base pieces that are used for the slide puzzle.
        [HideInInspector]
        public List<PuzzlePiece> basePieces = new List<PuzzlePiece>();

        // The piece spawn time.
        public float pieceSpawnTimeMax = 2.25F;

        // The speed of the slide puzzle.
        public float speed = 1.75F;

        // Gets set to 'true' when sliders should be run.
        protected bool runSliders = false;

        // Awake is called when the script instance is being loaded.
        protected override void Awake()
        {
            base.Awake();
            puzzleType = PuzzleManager.puzzleType.slide;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // There are no lines, so try to find them.
            if(sliders.Count <= 0)
            {
                // Tries to get the components in the children and set it to the lines list.
                GetComponentsInChildren(sliders);
            }
        }

        // Initializes the puzzle.
        public override void InitializePuzzle()
        {
            // Generates the base pieces from the list.
            basePieces = GenerateConversionDisplayPuzzlePieces();

            // Turns off all the base pieces.
            foreach(PuzzlePiece piece in basePieces)
            {
                piece.gameObject.SetActive(false);
            }

            // The base piece index.
            int basePieceIndex = 0;

            // Goes through each slide.
            foreach(SlidePuzzleSlider slide in sliders)
            {
                // Set the slide puzzle.
                slide.slidePuzzle = this;

                // Sets the index.
                slide.basePiecesIndex = basePieceIndex;

                // Saves this as the starting index for the slider.
                int startPieceIndex = basePieceIndex;

                // Increases the index.
                basePieceIndex++;

                // If the count has been reached or surpassed, loop around.
                if(basePieceIndex >= basePieces.Count)
                {
                    basePieceIndex = 0;
                }

                // Clears the piece pool.
                slide.piecePool.Clear();

                // While the piece pool does not match the number of basee pieces.
                while(slide.piecePool.Count < basePieces.Count)
                {
                    // Instantiates the piece from the base piece list.
                    PuzzlePiece piece = Instantiate(basePieces[startPieceIndex]);

                    // Set the parent and the local position.
                    piece.transform.parent = slide.transform;
                    piece.transform.localPosition = Vector3.zero;

                    // Add the piece to the pool from the starting index.
                    slide.piecePool.Enqueue(piece);
                    startPieceIndex++;

                    // Loop around.
                    if (startPieceIndex >= basePieces.Count)
                        startPieceIndex = 0;
                }
            }

            // Puzzle initialzied.
            base.InitializePuzzle();
        }

        // Initializes the puzzle for when a conversion question starts.
        public override void StartPuzzle()
        {
            runSliders = true;
        }

        // Stops the puzzle, which is called when a meteor is untargeted.
        public override void StopPuzzle()
        {
            runSliders = false;
        }

        // Ends a puzzle when a meteor is untargeted.
        public override void EndPuzzle()
        {
            // Goes through all the sliders and destroys their pieces.
            foreach(SlidePuzzleSlider slider in sliders)
            {
                slider.DestroyAllPuzzlePieces();
            }

            base.EndPuzzle();
        }

        // Returns 'true' if the puzzle sliders are running.
        public bool GetSlidersRunning()
        {
            return runSliders && IsPuzzleInitializedAndIsGamePlaying();
        }

        // Calculates and returns the max speed of the sliders adjusted by unscaled delta time.
        public float CalculateAdjustedSliderSpeed()
        {
            return speed * Time.unscaledDeltaTime;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
    }
}