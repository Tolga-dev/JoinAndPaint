using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("winCam")] 
        public CinemachineVirtualCamera bossCam;

        private CinemachineVirtualCamera _activeCam;
        public CinemachineBrain cinemaMachineBrain;
        
        private void SwitchCam(CinemachineVirtualCamera newActiveCam)
        {
            if (_activeCam == newActiveCam) return;

            playerCam.Priority = 0;
            menuStateCam.Priority = 0;
            bossCam.Priority = 0;

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

        public void SwitchToBossCam()
        {
            SwitchCam(bossCam);
        }
        
        public void SetTarget(Transform target, CinemachineVirtualCamera cam)
        {
            cam.Follow = target;
            cam.LookAt = target;
        }

    }
}