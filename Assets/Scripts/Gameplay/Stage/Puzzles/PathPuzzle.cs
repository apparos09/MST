using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

namespace RM_MST
{
    // Moves the puzzle symbols along a path.
    public class PathPuzzle : Puzzle
    {
        // The path puzzle piece.
        protected class PathPuzzlePiece
        {
            // The piece.
            public PuzzlePiece piece = null;

            // The t-value.
            public float t = 0.0F;

            // The start point.
            public GameObject startPoint = null;

            // The end point.
            public GameObject endPoint = null;

            // Returns 'true' if the puzzle piece is at the start point.
            public bool IsPuzzlePieceAtStartPoint()
            {
                return piece.transform.position == startPoint.transform.position;
            }

            // Returns 'true' if the puzzle piece is at the end point.
            public bool IsPuzzlePieceAtEndPoint()
            {
                return piece.transform.position == endPoint.transform.position;
            }

            // Destroys the saved puzzle piece.
            public void DestroyPuzzlePiece()
            {
                // Piece exists.
                if(piece != null)
                {
                    Destroy(piece.gameObject);
                }
            }

        }

        [Header("PathPuzzle")]

        // The points on the path.
        public List<GameObject> pathPoints = new List<GameObject>();

        // The list of path pieces
        protected List<PathPuzzlePiece> pathPieces = new List<PathPuzzlePiece>();

        // The parent used for the pieces.
        public GameObject pieceParent = null;

        // The speed of the path.
        public float speed = 0.25F;

        // The amount of space between the pieces (based on time-T)
        [Tooltip("The amount of space between the pieces based on time (t) for interpolation.")]
        public float pieceSpacingT = 0.75F;

        // If 'true', the path is being run.
        protected bool runPath = false;

        // Awake is called when the script instance is being loaded.
        protected override void Awake()
        {
            base.Awake();

            puzzleType = PuzzleManager.puzzleType.path;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // If the parent isn't set, make this object the parent.
            if (pieceParent == null)
                pieceParent = gameObject;
        }

        // Initializes the puzzle.
        public override void InitializePuzzle()
        {
            // Auto-sets the piece parent if it has not been set.
            if(pieceParent == null)
                pieceParent = gameObject;

            // Generates a group of pieces to be put on the path.
            List<PuzzlePiece> pieceGroup = GenerateConversionDisplayPuzzlePieces();

            // The spacing between pieces in terms of time (T)
            float pieceT = 0.0F;

            // Goes through the piece group.
            for(int i = 0; i < pieceGroup.Count; i++)
            {
                // Generates the path piece and sets the piece.
                PathPuzzlePiece pathPiece = GeneratePathPuzzlePiece();
                pathPiece.piece = pieceGroup[i];

                // Sets the parent of the piece and the local position.
                pathPiece.piece.transform.parent = pieceParent.transform;
                pathPiece.piece.transform.localPosition = Vector3.zero;

                // Adds the path piece to the list.
                pathPieces.Add(pathPiece);
            }

            // Goes through the list from the end to the start to determine where they should be placed.
            for(int i = pathPieces.Count - 1; i >= 0; i--)
            {
                // Calculating the T Value...
                // The final T value for the current piece, which may need to be corrected for the piece's starting point.
                float finalT;

                // If the piece T is greater than 1, calculate where the piece would be in its expected path.
                if(pieceT > 1.0F)
                {
                    finalT = (pieceT - Mathf.Floor(pieceT));
                }
                else // Use pieceT directly.
                {
                    finalT = pieceT;
                }

                // Set the t-value.
                pathPieces[i].t = finalT;


                // Calculating the Start and End Points...
                // The starting index.
                int startIndex, endIndex;

                // Uses modulus to see where the piece should be.
                startIndex = Mathf.FloorToInt(pieceT) % pathPoints.Count;

                

                // Does modulus to recalculate the start index in the proper bounds. Also does a bounds check.
                startIndex = Mathf.Clamp(startIndex, 0, pathPoints.Count - 1);

                // Calculates the endIndex.
                endIndex = startIndex + 1;

                // End bounds check.
                if (endIndex >= pathPoints.Count)
                    endIndex = 0;

                // Sets the start point and the end point for the path pieces.
                pathPieces[i].startPoint = pathPoints[startIndex];
                pathPieces[i].endPoint = pathPoints[endIndex];

                // Debug.Log("Start Index: " + startIndex.ToString() + " | End Index: " + endIndex.ToString() + " | T: " + finalT.ToString());

                // Runs the path for the pieces. Since this functions adds to the t-value, the t-value is pre-emptively reduced.
                pathPieces[i].t -= CalculateAdjustedSpeed();
                RunPath(pathPieces[i]); 

                // Increase piece T by the intended spacing.
                pieceT += pieceSpacingT;
            }


            base.InitializePuzzle();
        }

        // Initializes the puzzle for when a conversion question starts.
        public override void StartPuzzle()
        {
            runPath = true;
        }

        // Stops the puzzle, which is called when a meteor is untargeted.
        public override void StopPuzzle()
        {
            runPath = false;
        }

        // Ends a puzzle when a meteor is untargeted.
        public override void EndPuzzle()
        {
            // While there are path pieces.
            for(int i = 0; i < pathPieces.Count; i++)
            {
                // Destroy the puzzle piece.
                pathPieces[i].DestroyPuzzlePiece();
            }

            // Clear the list.
            pathPieces.Clear();

            base.EndPuzzle();
        }

        // Generates a path puzzle piece with default values.
        protected PathPuzzlePiece GeneratePathPuzzlePiece(PuzzlePiece piece, GameObject startPoint, GameObject endPoint)
        {
            // Generates the object.
            PathPuzzlePiece pathPiece = new PathPuzzlePiece();

            // Sets the values.
            pathPiece.piece = piece;
            pathPiece.t = 0.0F;
            pathPiece.startPoint = startPoint;
            pathPiece.endPoint = endPoint;

            return pathPiece;
        }

        // Generates a path puzzle piece with null values.
        protected PathPuzzlePiece GeneratePathPuzzlePiece()
        {
            return GeneratePathPuzzlePiece(null, null, null);
        }

        // Calculates the adjusted speed (speed times unscaled delta time).
        public float CalculateAdjustedSpeed()
        {
            return speed * Time.unscaledDeltaTime;
        }

        // Runs interpolation for the path piece. Returns 'true' if the piece has reached the end of the path.
        protected void RunPath(PathPuzzlePiece pathPiece)
        {
            // The four points.
            GameObject p0, p1, p2, p3;

            // P1 and P2
            // The start and end point.
            p1 = pathPiece.startPoint;
            p2 = pathPiece.endPoint;

            // Gets the index of p1 and p2. 
            int p1Index = pathPoints.IndexOf(p1);
            int p2Index = pathPoints.IndexOf(p2);


            // P0
            // Calculates the p0 index and object.
            int p0Index = p1Index - 1;
            
            // Loop around if it went negative.
            if(p0Index < 0)
                p0Index = pathPoints.Count - 1;
            
            // Gets p0 from the lst.
            p0 = pathPoints[p0Index];


            // P3
            // Calculates the p3 index and object.
            int p3Index = p2Index + 1;

            // Loop around if it went over the limit.
            if (p3Index >= pathPoints.Count)
                p3Index = 0;

            // Gets P3.
            p3 = pathPoints[p3Index];


            // NEW POSITION
            // Multiplies the value of t, and bounds checks it.
            pathPiece.t += CalculateAdjustedSpeed();
            pathPiece.t = Mathf.Clamp01(pathPiece.t);

            // Calculates the new position.
            // TODO: replace with fixed speed calculation.
            Vector3 newPos = Interpolation.CatmullRom(
                p0.transform.position, 
                p1.transform.position, 
                p2.transform.position, 
                p3.transform.position,
                pathPiece.t);

            // Set the new position.
            pathPiece.piece.transform.position = newPos;

            // At the end of the path, so change the start and end points.
            if(pathPiece.t >= 1.0F)
            {
                pathPiece.startPoint = p2;
                pathPiece.endPoint = p3;
                pathPiece.t = 0.0F;
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // If the path should be run, the puzzle's been initialized, and the game is being played.
            if(runPath && IsPuzzleInitializedAndIsGamePlaying())
            {
                // Update all the path pieces.
                foreach(PathPuzzlePiece pathPiece in pathPieces)
                {
                    // Runs the interpolation.
                    RunPath(pathPiece);
                }
            }
        }
    }
}