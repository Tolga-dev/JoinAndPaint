using System;
using GameStates.Base;
using UnityEngine;

namespace GameStates
{
    [Serializable]
    public class PlayingState : GameState
    {
        public int score;
        public Transform playerInitialPosition;

        public override void Enter()
        {
            GameManager.cameraController.SwitchToPlayerCam();
        }

        public override void Update()
        {
            GameManager.playerManager.UpdatePlayer();
        }

        public void ClickAvoid(bool b)
        {
            GameManager.menuState.clickAvoid.SetActive(b);
        }
    }
}