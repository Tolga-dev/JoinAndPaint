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
        
        public void SetTarget(Transform target, CinemachineVirtualCamera cam)
        {
            cam.Follow = target;
            cam.LookAt = target;
        }
        
    }
}