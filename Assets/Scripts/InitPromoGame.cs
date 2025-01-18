
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RM_MST
{
    // Initializes the promo game.
    public class InitPromoGame : MonoBehaviour
    {
        // Becomes 'true' when the game has been initialized.
        [HideInInspector]
        public bool initializedGame = false;


        void Awake()
        {
            // Unity Initialization
            Application.targetFrameRate = 30; // 30 FPS
            Application.runInBackground = false; // Don't run in the background.

            // Use the tutorial by default.
            GameSettings.Instance.UseTutorial = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            // The game has been initialized.
            initializedGame = true;

            // Enabling saving/loading for the game if this is not a WebGL build.
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // WebGL, so disable saving/loading.
                SaveSystem.Instance.allowSaveLoad = false;
            }
            else
            {
                // Non-WebGL, so enable saving/loading.
                SaveSystem.Instance.allowSaveLoad = true;
            }

            // Removes TMP_Text marking since this is the promo build, as the LOL SDK won't be initialized.
            TMP_TextTranslator.markIfFailed = false;

            // Aloow the player to select the game mode.
            GameSettings.Instance.allowPlayerSelectMode = true;
        }

        // Update is called once per frame
        void Update()
        {
            // If the game has been initialized, load the scene.
            if(initializedGame)
            {
                SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
            }
        }
    }
}