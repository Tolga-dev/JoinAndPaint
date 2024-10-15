using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    [Serializable]
    public class CameraController
    {
        public GameManager gameManager;
        // cams
        [Header("Game Cams")]
        public CinemachineVirtualCamera playerCam;
        public CinemachineVirtualCamera menuStateCam;
        public CinemachineVirtualCamera winCam;

        private CinemachineVirtualCamera _activeCam;
        public CinemachineBrain cinemaMachineBrain;
        
        private void SwitchCam(CinemachineVirtualCamera newActiveCam)
        {
            if (_activeCam == newActiveCam) return;

            playerCam.Priority = 0;
            menuStateCam.Priority = 0;
            winCam.Priority = 0;

            newActiveCam.Priority = 10;
            _activeCam = newActiveCam;
        }

        public void SwitchToPlayerCam()
        {
            SwitchCam(playerCam);
        }

        public void SwitchToMenuStateCam()
        {
            SwitchCam(menuStateCam);
        }

        public void SwitchToWinCam()
        {
            SwitchCam(winCam);
        }
        
        public void OnCameraSwitch(ICinemachineCamera toCam, ICinemachineCamera fromCam)
        {
            var firstCam = (CinemachineVirtualCamera)fromCam;
            var secondCam = (CinemachineVirtualCamera)toCam;
            if (firstCam == winCam && secondCam == menuStateCam)
            {
                gameManager.menuState.SetNewMotivationString(null);

                gameManager.playingState.ClickAvoid(true);
                gameManager.serviceManager.adsManager.PlaySceneTransitionAds();
                
                gameManager.StartCoroutine(WaitForCameraBlendToFinish());
            }
        }

        private IEnumerator WaitForCameraBlendToFinish()
        {
            while (cinemaMachineBrain.IsBlending)
            {
                yield return null;  // Wait until the next frame
            }
            gameManager.StartCoroutine(gameManager.saveManager.Save());
            
            gameManager.playingState.ClickAvoid(false);
            gameManager.menuState.SetNewMotivationString("Tap To Play!");
            Debug.Log("Arrived at menu state!");
        }

    }
}