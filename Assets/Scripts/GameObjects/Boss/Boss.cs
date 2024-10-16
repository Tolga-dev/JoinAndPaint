using System;
using System.Collections;
using GameObjects.Road;
using UnityEngine;

namespace GameObjects.Boss
{
    public class Boss : MonoBehaviour
    {
        private GameManager _gameManager;
        protected BossRoad BossRoad;
        public float speed;
            
        public virtual void PlayerArrived(BossRoad bossRoad)
        {
            BossRoad = bossRoad;
            _gameManager = bossRoad.gameManager;
            
            _gameManager.playerManager.TargetToATransform(transform);
            
            StartCoroutine(StartBossMatch());
        }

        private IEnumerator StartBossMatch()
        {
            yield return new WaitForSeconds(3);
            BossRoad.GameFinished();
        }
    }
}