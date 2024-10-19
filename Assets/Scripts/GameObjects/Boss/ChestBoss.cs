using System.Collections;
using GameObjects.Road;
using UnityEngine;

namespace GameObjects.Boss
{
    public class ChestBoss : Boss
    {
        protected static readonly int AttackMode = Animator.StringToHash("HitPlayer");

        public override void PlayerArrived(BossRoad bossRoadInGame)
        {
            base.PlayerArrived(bossRoadInGame);
            GameManager.playerManager.TargetToATransform(this, false);
            
        }

        private IEnumerator StartBossMatch()
        {
            animator.SetBool(AttackMode, true);

            GameManager.playingState.isGameWon = true;

            yield return new WaitForSeconds(0.5f); // wait for punch to play
            
            GameManager.playingState.isGameFinished = true;

            BossRoad.GameFinished();
        }
        public override void TakeDamage(int memberDamageAmount)
        {
            Debug.Log("damage");
            health -= memberDamageAmount;
            StartCoroutine(StartBossMatch());
        }
    }
}