using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // The async scene loader for the MST.
    public class MST_AsyncSceneLoader : util.AsyncSceneLoader
    {
        // The loading screen.
        public MST_LoadingScreen loadingScreen;

        // The load scene async is complete.
        public override void OnLoadSceneAsyncComplete()
        {
            base.OnLoadSceneAsyncComplete();
        
            // Plays the closing animation.
            loadingScreen.PlayLoadingScreenClosingAnimation();
        }
    }
}