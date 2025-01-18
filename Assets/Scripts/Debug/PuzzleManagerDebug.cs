using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using util;

namespace RM_MST
{
    // Used to manage debug operations to test for puzzles.
    public class PuzzleManagerDebug : MonoBehaviour
    {
        [Header("UI")]

        // The test button.
        public Button testButton;

        [Header("Other")]
        // The puzzle camera.
        public Camera puzzleCam;

        // The camera image.
        public RawImage cameraImage;

        // The screen marker.
        public Image screenMarker;

        // The pointer in the puzzle camera.
        public GameObject puzzlePointer;

        // The mouse touch input object.
        public MouseTouchInput mouseTouchInput;

        // Start is called before the first frame update
        void Start()
        {
            // Debug.Log("Camera Raw Image Pos: " + cameraImage.transform.position.ToString());
            // // Debug.Log("Camera Raw Image Rect Anchored Pos: " + cameraImage.rectTransform.anchoredPosition.ToString());
            // // Debug.Log("Camera Raw Image VP Pos" + Camera.main.ScreenToViewportPoint(cameraImage.rectTransform.anchoredPosition));
            // 
            // // Test to Calculate the Corners
            // Vector2 arCurr = cameraImage.rectTransform.anchoredPosition;
            // Vector2 arTemp = cameraImage.rectTransform.anchoredPosition;
            // arTemp -= cameraImage.rectTransform.sizeDelta / 2;
            // cameraImage.rectTransform.anchoredPosition = arTemp;
            // 
            // Debug.Log("Camera Raw Image Pos (BL Corner): " + cameraImage.transform.position.ToString());
            // cameraImage.rectTransform.anchoredPosition = arCurr;
            // 
            // Debug.Log("Screen Marker Pos: " + screenMarker.transform.position.ToString());
        }

        // Called when a UI button is pressed.
        public void OnUIButtonPressed()
        {
            Debug.Log("Button Pressed!");
        }

        // Update is called once per frame
        void Update()
        {
            // If the pointer is over a UI element.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Gets the raycast results.
                List<RaycastResult> raycastResults = MouseTouchInput.GetMouseUIRaycastResults();

                // Print then names of the objects.
                for (int i = 0; i < raycastResults.Count; i++)
                {
                    // Debug.Log(i.ToString() + " | " + raycastResults[i].gameObject.name);

                    // This is the camera raw image.
                    if (raycastResults[i].gameObject == cameraImage.gameObject)
                    {
                        // 1. Calculate the position in the Camera Image

                        // Gets the mouse position.
                        Vector2 mousePos = Input.mousePosition;

                        // The lower and upper bounds.
                        Vector2 camRawImageLower, camRawImageUpper;

                        Vector2 camRawImageScaleXY = new Vector2();
                        camRawImageScaleXY.x = cameraImage.transform.localScale.x;
                        camRawImageScaleXY.y = cameraImage.transform.localScale.y;

                        // The camera image's anchor point, and the rect size.
                        Vector2 camImageAncPos = cameraImage.rectTransform.anchoredPosition;
                        Vector2 camRectSize = cameraImage.rectTransform.sizeDelta;

                        // Lower Bounds
                        cameraImage.rectTransform.anchoredPosition = camImageAncPos - (camRectSize * camRawImageScaleXY) / 2;
                        camRawImageLower = cameraImage.transform.position;

                        // Upper Bounds
                        cameraImage.rectTransform.anchoredPosition = camImageAncPos + (camRectSize * camRawImageScaleXY) / 2;
                        camRawImageUpper = cameraImage.transform.position;

                        // Reset the anchor position.
                        cameraImage.rectTransform.anchoredPosition = camImageAncPos;

                        // Get the percentage positions of the pointer over the camera image.
                        // Treat this as the viewport position of the camera.
                        Vector3 mousePosPercents = new Vector3();

                        mousePosPercents.x = Mathf.InverseLerp(camRawImageLower.x, camRawImageUpper.x, mousePos.x);
                        mousePosPercents.y = Mathf.InverseLerp(camRawImageLower.y, camRawImageUpper.y, mousePos.y);
                        mousePosPercents.z = 1.0F;

                        // Debug.Log("Mouse Pos Over Cam Raw Image (Percent): " + mousePosPercents.ToString());


                        // 2. Calculate the World Position in the Camera

                        // Set the viewport position.
                        Vector3 puzzleCamViewportPos = new Vector3();
                        puzzleCamViewportPos.x = mousePosPercents.x;
                        puzzleCamViewportPos.y = mousePosPercents.y;

                        // Calculate the viewport position as a world pos and as a ray.
                        Vector3 puzzleCamPointerPos = puzzleCam.ViewportToWorldPoint(puzzleCamViewportPos);
                        puzzleCamPointerPos.z = 0; // Makes sure it's at 0 so that the sprite isn't hidden.

                        // The rya to be casted.
                        Ray puzzleCamRay = puzzleCam.ViewportPointToRay(puzzleCamViewportPos);
                        Vector2 rayOrigin2D = new Vector2(puzzleCamPointerPos.x, puzzleCamPointerPos.y);

                        // the max distance is the far clip plane minus the near clip plane.
                        float maxDist = puzzleCam.farClipPlane - puzzleCam.nearClipPlane;

                        // Casts the ray.
                        bool rayHit;

                        // Old calculation (3D cast) - doesn't work.
                        // RaycastHit hitInfo3D;
                        // rayHit = Physics.Raycast(puzzleCamRay, out hitInfo3D, maxDist);

                        // 2D cast.
                        RaycastHit2D hitInfo2D = Physics2D.Raycast(rayOrigin2D, Vector3.forward, maxDist);
                        rayHit = hitInfo2D.collider != null;

                        // 3. Move the Pointer Marker and Check Ray Hit
                        puzzlePointer.transform.position = puzzleCamPointerPos;

                        // Print the name of the object that got hit.
                        if(rayHit)
                        {
                            Debug.Log("Ray Hit Object: " + hitInfo2D.collider.gameObject);

                            testButton.onClick.Invoke();
                        }

                        //// Checks if the mouse button is down.
                        //if(Input.GetMouseButtonDown(0))
                        //{
                        //    Debug.Log("Mouse Button 0 Down!");
                        //}

                        //// There are touches, so check for the position. This is the screen positon.
                        //if(mouseTouchInput.currentTouches.Count > 0)
                        //{
                        //    Debug.Log("Touch Position: " + mouseTouchInput.currentTouches[0].position);
                        //}
                    }
                }


            }
        }
    }
}