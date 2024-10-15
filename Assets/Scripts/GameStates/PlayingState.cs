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
        public GameObject clickAvoid;

        public override void Enter()
        {
        }

        public override void Update()
        {
            GameManager.playerManager.UpdatePlayer();
        }

        public void ClickAvoid(bool b)
        {
            GameManager.playingState.clickAvoid.SetActive(b);
        }
    }
}