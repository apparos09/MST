using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static RM_MST.PuzzleManager;

namespace RM_MST
{
    // The puzzle UI.
    public class PuzzleUI : MonoBehaviour
    {
        // the instance of the class.
        private static PuzzleUI instance;

        // Gets set to 'true' when the singleton has been instanced.
        // This isn't needed, but it helps with the clarity.
        private static bool instanced = false;

        // The stage UI.
        public StageUI stageUI;

        // The puzzle manager.
        public PuzzleManager puzzleManager;

        [Header("Unit Buttons")]
        // The units buttons.
        public RectTransform unitButtonsParent;

        // The default position of the unit buttons object.
        private Vector3 unitButtonsDefaultPos;

        // The unit buttons hide object (used to hide the buttons off-screen).
        public RectTransform unitButtonsHiddenParent;

        [Header("Puzzle")]

        // The conversion displays.
        public GameObject conversionDisplaysParent;

        // Conversion Display 0
        public PuzzleConversionDisplay conversionDisplay0;

        // Conversion Display 1
        public PuzzleConversionDisplay conversionDisplay1;

        // Conversion Display 2
        public PuzzleConversionDisplay conversionDisplay2;

        // Conversion Display 3
        public PuzzleConversionDisplay conversionDisplay3;

        // Conversion Display 4
        public PuzzleConversionDisplay conversionDisplay4;

        // Conversion Display 5
        public PuzzleConversionDisplay conversionDisplay5;

        // Conversion Display 6
        public PuzzleConversionDisplay conversionDisplay6;

        // The puzzle window.
        public GameObject puzzleWindow;

        // The raw camera image.
        public RawImage cameraRawImage;

        // An object used to cover the puzzle window when the player shouldn't be able to use it.
        public GameObject puzzleWindowCover;

        // Gets set to 'true' when late start has been called.
        private bool calledLateStart = false;

        // Constructor
        private PuzzleUI()
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

                // Saves the default position.
                unitButtonsDefaultPos = unitButtonsParent.anchoredPosition;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Sets the stage UI.
            if (stageUI == null)
                stageUI = StageUI.Instance;

            // Sets the puzzle manager.
            if (puzzleManager == null)
                puzzleManager = PuzzleManager.Instance;
        }

        // Called on the first update frame of the puzzle UI.
        private void LateStart()
        {
            calledLateStart = true;     
        }

        // Gets the instance.
        public static PuzzleUI Instance
        {
            get
            {
                // Checks if the instance exists.
                if (instance == null)
                {
                    // Tries to find the instance.
                    instance = FindObjectOfType<PuzzleUI>(true);


                    // The instance doesn't already exist.
                    if (instance == null)
                    {
                        // Generate the instance.
                        GameObject go = new GameObject("PuzzleUI (singleton)");
                        instance = go.AddComponent<PuzzleUI>();
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

        // Resets the positions of the unit buttons parent object.
        public void ResetUnitButtonsParentPosition()
        {
            unitButtonsParent.anchoredPosition = unitButtonsDefaultPos;
        }

        // Move the unit buttons parent to the hidden position.
        public void MoveUnitButtonsParentToHiddenPosition()
        {
            unitButtonsParent.anchoredPosition = unitButtonsHiddenParent.anchoredPosition;
        }

        // Calculates the lower bounds (in pixels) of the raw camera image.
        public Vector2 CalculateCameraRawImageLowerBounds()
        {
            // The lower bounds.
            Vector2 camRawImageLower;

            // The xy local scale.
            Vector2 camRawImageScaleXY = new Vector2();
            camRawImageScaleXY.x = cameraRawImage.transform.localScale.x;
            camRawImageScaleXY.y = cameraRawImage.transform.localScale.y;

            // The camera image's anchor point, and the rect size.
            Vector2 camImageAncPos = cameraRawImage.rectTransform.anchoredPosition;
            Vector2 camRectSize = cameraRawImage.rectTransform.sizeDelta;

            // Lower Bounds
            cameraRawImage.rectTransform.anchoredPosition = camImageAncPos - (camRectSize * camRawImageScaleXY) / 2;
            camRawImageLower = cameraRawImage.transform.position;

            // Reset the anchor position.
            cameraRawImage.rectTransform.anchoredPosition = camImageAncPos;

            // Returns the lower bounds.
            return camRawImageLower;
        }

        // Calculates the upper bounds (in pixels) of the raw camera image.
        public Vector2 CalculateCameraRawImageUpperBounds()
        {
            // The upper bounds.
            Vector2 camRawImageUpper;

            // The xy local scale.
            Vector2 camRawImageScaleXY = new Vector2();
            camRawImageScaleXY.x = cameraRawImage.transform.localScale.x;
            camRawImageScaleXY.y = cameraRawImage.transform.localScale.y;

            // The camera image's anchor point, and the rect size.
            Vector2 camImageAncPos = cameraRawImage.rectTransform.anchoredPosition;
            Vector2 camRectSize = cameraRawImage.rectTransform.sizeDelta;

            // Upper Bounds
            cameraRawImage.rectTransform.anchoredPosition = camImageAncPos + (camRectSize * camRawImageScaleXY) / 2;
            camRawImageUpper = cameraRawImage.transform.position;

            // Reset the anchor position.
            cameraRawImage.rectTransform.anchoredPosition = camImageAncPos;

            // Returns the upper bounds.
            return camRawImageUpper;
        }

        // Generates a list of conversion displays.
        public List<PuzzleConversionDisplay> GenerateConversionDisplayList()
        {
            // Generates a list of all the displays.
            List<PuzzleConversionDisplay> displays = new List<PuzzleConversionDisplay>()
            {
                conversionDisplay0,
                conversionDisplay1,
                conversionDisplay2,
                conversionDisplay3,
                conversionDisplay4,
                conversionDisplay5,
                conversionDisplay6
            };

            return displays;
        }

        // Generates a list of active conversion displays.
        public List<PuzzleConversionDisplay> GenerateActiveConversionDisplayList()
        {
            // The conversion display lists.
            List<PuzzleConversionDisplay> displayListAll = GenerateConversionDisplayList();
            List<PuzzleConversionDisplay> displayListActive = new List<PuzzleConversionDisplay>();

            // Goes through all the displays to get which ones are active.
            for (int i = 0; i < displayListAll.Count; i++)
            {
                // The display is active, so add it to the list.
                if (displayListAll[i].gameObject.activeSelf)
                {
                    displayListActive.Add(displayListAll[i]);
                }
            }

            // Return the active list.
            return displayListActive;
        }

        // Refreshes the conversion displays. This overwrites any unit buttons that are saved to the displays.
        public void RefreshConversionDisplays()
        {
            // Generates the displays list, and the active units buttons list.
            List<PuzzleConversionDisplay> displays = GenerateConversionDisplayList();
            List<UnitsButton> unitsButtons = stageUI.GenerateUnitsButtonsActiveList();

            // The units buttons index.
            int unitsButtonsIndex = 0;

            // Goes through all the displays and sets them all to specifc units butotns.
            for(int i = 0; i < displays.Count; i++)
            {
                // Turn the display on.
                displays[i].gameObject.SetActive(true);

                // The index is valid.
                if(unitsButtonsIndex < unitsButtons.Count)
                {
                    // Sets the units button for this display.
                    displays[i].SetInfoFromUnitsButton(unitsButtons[unitsButtonsIndex]);

                    // If there units button is not active, hide this display.
                    if(!unitsButtons[unitsButtonsIndex].gameObject.activeSelf)
                    {
                        displays[i].gameObject.SetActive(false);
                    }
                }
                else // Index is invalid.
                {
                    // Clear the display nad turn it off.
                    displays[i].Clear();
                    displays[i].gameObject.SetActive(false);
                }

                // Increase the index.
                unitsButtonsIndex++;
            }
        }

        // Clears the conversion displays.
        public void ClearConversionDisplays()
        {
            conversionDisplay0.Clear();
            conversionDisplay1.Clear();
            conversionDisplay2.Clear();
            conversionDisplay3.Clear();
            conversionDisplay4.Clear();
            conversionDisplay5.Clear();
            conversionDisplay6.Clear();
        }

        // Called when a puzzle has been generated.
        public void OnPuzzleGenerated()
        {
            // Generate the puzzle.
            switch (puzzleManager.pType)
            {
                // Show the unit buttons, and hide the puzzle UI.
                case puzzleType.unknown:
                case puzzleType.buttons:
                    
                    // Units Buttons
                    ResetUnitButtonsParentPosition();

                    // Puzzle
                    puzzleWindow.SetActive(false);
                    conversionDisplaysParent.gameObject.SetActive(false);

                    break;

                    // Hide the unit buttons and show the puzzle UI.
                case puzzleType.swap:
                case puzzleType.slide:
                case puzzleType.path:

                    // Units Buttons
                    MoveUnitButtonsParentToHiddenPosition();

                    // Puzzle
                    puzzleWindow.gameObject.SetActive(true);
                    conversionDisplaysParent.gameObject.SetActive(true);
                    break;
            }

            // Refreshes the conversion displays.
            RefreshConversionDisplays();
        }

        // Update is called once per frame
        void Update()
        {
            // Calls LateStart.
            if (!calledLateStart)
                LateStart();
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